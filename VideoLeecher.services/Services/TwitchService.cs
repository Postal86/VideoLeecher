using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VideoLeecher.core.Enums;
using VideoLeecher.core.Events;
using VideoLeecher.core.Models;
using VideoLeecher.services.Interfaces;
using VideoLeecher.shared.Events;
using VideoLeecher.shared.Extensions;
using VideoLeecher.shared.IO;
using VideoLeecher.shared.Notification;
using VideoLeecher.shared.Reflection;






namespace VideoLeecher.services.Services
{
    internal class TwitchService : BindableBase, ITwitchService, IDisposable
    {
        #region კონსტანტები 
        
        private const string KRAKEN_URL = "https://api.twitch.tv/kraken";
        private const string VIDEO_URL = "https://api.twitch.tv/kraken/videos/{0}";
        private const string GAMES_URL = "https://api.twitch.tv/kraken/games/top";
        private const string USERS_URL = "https://api.twitch.tv/kraken/users";
        private const string CHANNEL_URL = "https://api.twitch.tv/kraken/channels/{0}";
        private const string CHANNEL_VIDEOS_URL = "https://api.twitch.tv/kraken/channels/{0}/videos";
        private const string ACCESS_TOKEN_URL = "https://api.twitch.tv/api/vods/{0}/access_token";
        private const string ALL_PLAYLISTS_URL = "https://usher.twitch.tv/vod/{0}?nauthsig={1}&nauth={2}&allow_source=true&player=twitchweb&allow_spectre=true&allow_audio_only=true";
        private const string UNKNOWN_GAME_URL = "https://static-cdn.jtvnw.net/ttv-boxart/404_boxart.png";

        private const string TEMP_PREFIX = "TL_";
        private const string PLAYLIST_NAME = "vod.m3u8";
        private const string FFMPEG_EXE_X86 = "ffmpeg_x86.exe";
        private const string FFMPEG_EXE_X64 = "ffmpeg_x64.exe";

        private const int TIMER_INTERVAL = 2;
        private const int DOWNLOAD_RETRIES = 3;
        private const int DOWNLOAD_RETRY_TIME = 20;

        private const int TWITCH_MAX_LOAD_LIMIT = 100;

        private const string TWITCH_CLIENT_ID = "37v97169hnj8kaoq8fs3hzz8v6jezdj";
        private const string TWITCH_CLIENT_ID_HEADER = "Client-ID";
        private const string TWITCH_VS_ACCEPT = "application/vnd.twitchtv.v5+json";
        private const string TWITCH_VS_ACCEPT_HEADER = "Accept";
        private const string TWITCH_AUTHORIZATION_HEADER = "Authorization";

        #endregion კონსტანტები


        #region ველები 

        private bool disposedValue = false;

        private IPreferenceService  _preferencesService;
        private IRuntimeDataService _runtimeDataService;
        private IEventAggregator _eventAggregator;

        private Timer _downloadTimer;


        private ObservableCollection<TwitchVideo> _videos;
        private ObservableCollection<TwitchVideoDownload> _downloads;

        private ConcurrentDictionary<string, DownloadTask> _downloadTasks;
        private Dictionary<string, Uri> _gameThumbnails;
        private TwitchAuthInfo _twitchAuthInfo;

        private string _appDir;

        private object _changeDownloadLockObject;

        private volatile bool _paused;


        #endregion  ველები

        #region კონსტრუქტორი

        public TwitchService(
            IPreferenceService  preferenceService, 
            IRuntimeDataService runtimeDataService, 
            IEventAggregator  eventAggregator)
        {
            _preferencesService = preferenceService;
            _runtimeDataService = runtimeDataService;
            _eventAggregator = eventAggregator;

            _videos = new ObservableCollection<TwitchVideo>();
            _videos.CollectionChanged += Videos_CollectionChanged;

            _downloads = new ObservableCollection<TwitchVideoDownload>();
            _downloads.CollectionChanged += Downloads_CollectionChanged;

            _downloadTasks = new ConcurrentDictionary<string, DownloadTask>();

            _appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _changeDownloadLockObject = new object();

            _downloadTimer = new Timer(DownloadTimerCallback, null, 0,  TIMER_INTERVAL);

            _eventAggregator.GetEvent<RemoveDownloadEvent>().Subscribe(Remove, ThreadOption.UIThread);
        }

        #endregion  კონსტრუქტორი


        #region თვისებები

        public bool IsAuthorized
        {
            get
            {
                return _twitchAuthInfo != null;
            }
        }

        public ObservableCollection<TwitchVideo>  Videos
        {

            get
            {
                return _videos;
            }

            private set
            {

                if (_videos != null)
                {
                    _videos.CollectionChanged -= Videos_CollectionChanged;
                }

                SetProperty(ref _videos, value, nameof(Videos));

                if (_videos != null)
                {
                    _videos.CollectionChanged += Videos_CollectionChanged;
                }

                FireVideosCountChanged();
            }
        }

        #endregion თვისებები


        #region მეთოდები 

        private WebClient  CreateTwitchWebClient()
        {
            var wc = new WebClient();
            wc.Headers.Add(TWITCH_CLIENT_ID_HEADER, TWITCH_CLIENT_ID);
            wc.Headers.Add(TWITCH_VS_ACCEPT_HEADER, TWITCH_VS_ACCEPT);
            wc.Encoding = Encoding.UTF8;

            return wc;
        }


        private WebClient  CreateAuthorizedTwitchWebClient()
        {
            WebClient wc = CreateTwitchWebClient();

            if (IsAuthorized)
            {
                wc.Headers.Add(TWITCH_AUTHORIZATION_HEADER, "OAuth" + _twitchAuthInfo.AccessToken);
            }

            return wc;
        }

        public VodAuthInfo  RetrieveVodAuthInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (WebClient webClient = CreateAuthorizedTwitchWebClient())
            {
                string accessTokenStr = webClient.DownloadString(string.Format(ACCESS_TOKEN_URL, id));

                JObject accessTokenJson = JObject.Parse(accessTokenStr);

                string token = Uri.EscapeDataString(accessTokenJson.Value<string>("token"));
                string signature = accessTokenJson.Value<string>("sig");

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new ApplicationException("VOD access token is null!");
                }

                if (string.IsNullOrWhiteSpace(signature))
                {
                    throw new ApplicationException("VOD signature  is null!");
                }

                bool privileged = false;
                bool subOnly = false;

                JObject tokenJson = JObject.Parse(HttpUtility.UrlDecode(token));

                if (tokenJson == null)
                {
                    throw new ApplicationException("Decoded  VOD access  token  is null");
                }

                privileged = tokenJson.Value<bool>("privileged");

                if (privileged)
                {
                    subOnly = true;
                }



            }
        }

        #endregion მეთოდები




    }
}
