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
                else
                {
                    JObject chansubJson = tokenJson.Value<JObject>("chansub");

                    if (chansubJson == null)
                    {
                        throw new ApplicationException("Token  property  'chansub' is  null!");
                    }

                    JArray restrictedQualitiesJson = chansubJson.Value<JArray>("restricted_bitrates");

                    if (restrictedQualitiesJson == null)
                    {
                        throw new ApplicationException("Token  property 'chansub -> restricted_bitrates' is  null!");
                    }

                    if (restrictedQualitiesJson.Count > 0)
                    {
                        subOnly = true;
                    }

                }

                return new VodAuthInfo(token, signature, privileged, subOnly);

            }
        }

        public bool ChannelExists(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentNullException(nameof(channel));
            }

            return GetChannelIdByName(channel) != null;
        }

        public string GetChannelIdByName(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentNullException(nameof(channel));
            }

            using (WebClient webClient = CreateTwitchWebClient())
            {
                webClient.QueryString.Add("login",  channel);

                string result = webClient.DownloadString(USERS_URL);

                JObject searchResultJson = JObject.Parse(result);

                JArray usersJson = searchResultJson.Value<JArray>("users");

                if (usersJson !=  null && usersJson.HasValues)
                {
                    JToken userJson = usersJson.FirstOrDefault(); 

                    if (userJson != null)
                    {
                        string id = userJson.Value<string>("_id");

                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            using (WebClient webClientChannel = CreateTwitchWebClient())
                            {
                                try
                                {
                                    webClientChannel.DownloadString(string.Format(CHANNEL_URL, id));
                                }
                                catch (WebException)
                                {
                                    return null;
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }

                    }

                }

                return null;
            }
        }

        public bool Authorize(string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                using (WebClient webClient = CreateTwitchWebClient())
                {
                    webClient.Headers.Add(TWITCH_AUTHORIZATION_HEADER, "OAuth" + accessToken);

                    string result = webClient.DownloadString(KRAKEN_URL);

                    JObject verifyRequestJson = JObject.Parse(result);

                    if (verifyRequestJson != null)
                    {
                        JObject tokenJson = verifyRequestJson.Value<JObject>("token");

                        if (tokenJson != null)
                        {
                            bool valid = tokenJson.Value<bool>("valid");

                            if (valid)
                            {
                                string username = tokenJson.Value<string>("user_name");
                                string clientId = tokenJson.Value<string>("client_id");

                                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(clientId) &&
                                    clientId.Equals(TWITCH_CLIENT_ID, StringComparison.OrdinalIgnoreCase))
                                {
                                    _twitchAuthInfo = new TwitchAuthInfo(accessToken, username);
                                    FireIsAuthorizedChanged();
                                    return true;
                                }
                            }

                        }
                    }
                }
            }

            RevokeAuthorization();
            return false;
        }

        public void RevokeAuthorization()
        {
            _twitchAuthInfo = null;
            FireIsAuthorizedChanged();
        }

        public void Search(SearchParameters searchParams)
        {
            if (searchParams == null)
            {
                throw new ArgumentNullException(nameof(searchParams));
            }

            switch (searchParams.SearchType)
            {
                case SearchType.Channel:
                    SearchChannel(searchParams.Channel, searchParams.VideoType, searchParams.LoadLimitType, searchParams.LoadFrom.Value, searchParams.LoadTo.Value, searchParams.LoadLastVods);
                    break;
                case SearchType.Urls:
                    SearchUrls(searchParams.Urls);
                    break;
                case SearchType.Ids:
                    SearchIds(searchParams.Ids);
                    break;
            }
        }

        private void SearchChannel(string channel, VideoType videoType, LoadLimitType loadLimit, DateTime loadFrom,
            DateTime loadTo, int loadLastVods)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw  new ArgumentNullException(nameof(channel));
            }

            string channelId = GetChannelIdByName(channel);

            ObservableCollection<TwitchVideo>  videos = new ObservableCollection<TwitchVideo>();

            string broadcastTypeParam = null;

            if (videoType == VideoType.Broadcast)
            {
                broadcastTypeParam = "archive";
            }
            else if (videoType == VideoType.Highlight)
            {
                broadcastTypeParam = "highlight";
            }
            else if (videoType == VideoType.Upload)
            {
                broadcastTypeParam = "upload";
            }
            else
            {
                throw new ApplicationException("ამ ტიპის  ვიდეოს მხარდაჭერა არ არსებობს '" +  videoType.ToString() + "'");
            }

            string channelVideosUrls = string.Format(CHANNEL_VIDEOS_URL, channelId);

            DateTime fromDate = DateTime.Now;
            DateTime toDate = DateTime.Now;

            if (loadLimit == LoadLimitType.Timespan)
            {
                fromDate = loadFrom;
                toDate = loadTo;
            }

            int offset = 0;
            int total = 0;
            int sum = 0;

            bool stop = false;

            do
            {
                using (WebClient webClient = CreateTwitchWebClient())
                {
                    webClient.QueryString.Add("broadcast_type", broadcastTypeParam);
                    webClient.QueryString.Add("limit", TWITCH_MAX_LOAD_LIMIT.ToString());
                    webClient.QueryString.Add("offset", offset.ToString());

                    string result = webClient.DownloadString(channelVideosUrls);

                    JObject videosResponseJson = JObject.Parse(result);

                    if (videosResponseJson != null)
                    {
                        if (total == 0)
                        {
                            total = videosResponseJson.Value<int>("_total");
                        }

                        foreach (JObject videoJson in videosResponseJson.Value<JArray>("videos"))
                        {
                            sum++;

                            if (videoJson.Value<string>("_id").StartsWith("v"))
                            {
                                TwitchVideo video = ParseVideo(videoJson);

                                if (loadLimit == LoadLimitType.LastVods)
                                {
                                    videos.Add(video);

                                    if (sum >= loadLastVods)
                                    {
                                        stop = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    DateTime recordedDate = video.RecordedDate;

                                    if (recordedDate.Date >= fromDate.Date && recordedDate.Date <= toDate.Date)
                                    {
                                        videos.Add(video);
                                    }

                                    if (recordedDate.Date < fromDate.Date)
                                    {
                                        stop = true;
                                        break;
                                    }

                                }

                            }

                        }
                    }
                }

                offset += TWITCH_MAX_LOAD_LIMIT;
            } while (!stop && sum < total);

            Videos = videos;
        }

        private void SearchUrls(string urls)
        {
            if (string.IsNullOrWhiteSpace(urls))
            {
                throw new ArgumentNullException(nameof(urls));
            }

            ObservableCollection<TwitchVideo> videos = new ObservableCollection<TwitchVideo>();

            string[] urlArr = urls.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            if (urlArr.Length > 0)
            {
                HashSet<int>  addedIds = new HashSet<int>();

                foreach (string url in urlArr)
                {
                    int? id = GetVideoIdFromUrl(url);

                    if (id.HasValue && !addedIds.Contains(id.Value))
                    {
                        TwitchVideo video = GetTwitchVideoFromId(id.Value);

                        if (video != null)
                        {
                            videos.Add(video);
                            addedIds.Add(id.Value);
                        }
                    }
                }
            }
            Videos = videos;
        }

        private void SearchIds(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                throw new ArgumentNullException(nameof(ids));
            }

            ObservableCollection<TwitchVideo> videos = new ObservableCollection<TwitchVideo>();

            string[] idsArr = ids.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            if (idsArr.Length > 0)
            {
                HashSet<int>  addedIds = new HashSet<int>();

                foreach (string id in idsArr )
                {
                    if (int.TryParse(id, out int idInt) && !addedIds.Contains(idInt))
                    {
                        TwitchVideo video = GetTwitchVideoFromId(idInt);

                        if (video != null)
                        {
                            videos.Add(video);
                            addedIds.Add(idInt);
                        }
                    }
                }
            }

            Videos = videos;
        }

        private int? GetVideoIdFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri validUrl))
            {
                return null;
            }

            string[] segments = validUrl.Segments;

            if (segments.Length < 2)
            {
                return null;
            }

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Equals("videos/", StringComparison.OrdinalIgnoreCase))
                {
                    if (segments.Length > (i + 1))
                    {
                        string idStr = segments[i + 1];

                        if (!string.IsNullOrWhiteSpace(idStr))
                        {
                            idStr = idStr.Trim(new char[] {'/'});

                            if (int.TryParse(idStr, out int idInt) && idInt > 0)
                            {
                                return idInt;
                            }
                        }
                    }

                    break;
                }
            }

            return null;
        }

        private TwitchVideo GetTwitchVideoFromId(int id)
        {
            using (WebClient webClient = CreateTwitchWebClient())
            {

                try
                {
                    string result = webClient.DownloadString(string.Format(VIDEO_URL, id));

                    JObject videoJson = JObject.Parse(result);

                    if (videoJson != null)
                    {
                        return ParseVideo(videoJson);
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse resp && resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else
                    {
                        throw;
                    }

                }

            }

            return null;

        }

        public void Enqueue(DownloadParameters downloadParams)
        {
            if (_paused)
            {
                return;

            }

            lock (_changeDownloadLockObject)
            {
                _downloads.Add(new TwitchVideoDownload(downloadParams));
            }
        }

        private void DownloadTimerCallback(object state)
        {
            if (_paused)
            {
                return;
            }

            StartQueuedDownloadIfExists();
        }

        private void StartQueuedDownloadIfExists()
        {
            if (_paused)
            {
                return;
            }

            if (Monitor.TryEnter(_changeDownloadLockObject))
            {
                try
                {
                    if (!_downloads.Where(d => d.DownloadState == DownloadState.Downloading).Any())
                    {
                        TwitchVideoDownload  download = _downloads.Where(d => d.DownloadState == )


                    }


                }
                catch
                {

                }
            }
        }


        #endregion მეთოდები




    }
}
