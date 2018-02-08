using System;
using System.Collections.Generic;
using System.Linq;
using VideoLeecher.shared.Extensions;

namespace VideoLeecher.core.Models
{
    public class LeecherVideo
    {
        #region კონსტანტები

        private const string UNTITLED_BROADCAST = "Untitled Broadcast";
        private const string UNKNOWN_GAME = "Unknown";

        #endregion კონსტანტები

        #region ველები

        private string _channel;
        private string _title;
        private string _id;
        private string _game;

        private int _views;

        private TimeSpan _length;

        private IList<LeecherVideoQuality> _qualities;

        private DateTime _recordedDate;

        private Uri _thumbnail;
        private Uri _gameThumbnail;
        private Uri _url;


        #endregion ველები

        #region კონსტრუქტორები

        public LeecherVideo(string channel, string title, string id, string game, int views, TimeSpan length, IList<LeecherVideoQuality> qualities, DateTime recordedDate,
            Uri thumbnail, Uri gameThumbnail, Uri url)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentNullException(nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (qualities == null || qualities.Count == 0)
            {
                throw new ArgumentNullException(nameof(qualities));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                title = UNTITLED_BROADCAST;
            }

            _channel = channel;
            _title = title;
            _id = id;

            if (string.IsNullOrWhiteSpace(game))
            {
                _game = UNKNOWN_GAME;
            }
            else
            {
                _game = game;
            }

            _views = views;
            _length = length;
            _qualities = qualities;
            _recordedDate = recordedDate;
            _thumbnail = thumbnail ?? throw new ArgumentNullException(nameof(thumbnail));
            _gameThumbnail = gameThumbnail ?? throw new ArgumentNullException(nameof(gameThumbnail));
            _url = url ?? throw new ArgumentNullException(nameof(url));

        }

        #endregion კონსტრუქტორები

        #region თვისებები

        public string Channel
        {
            get
            {
                return _channel;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public string Game
        {
            get
            {
                return _game;
            }
        }

        public TimeSpan Length
        {
            get
            {
                return _length;
            }

        }

        public string LengthStr
        {
            get
            {
                return _length.ToDaylesString();
            }
        }

        public int Views
        {
            get
            {
                return _views;
            }
        }

        public  IList<LeecherVideoQuality>  Qualities
        {
            get
            {
                return _qualities;
            }
        }

        public string BestQuality
        {
            get
            {
                if (_qualities == null || _qualities.Count == 0)
                {
                    return LeecherVideoQuality.UNKNOWN;
                }

                return _qualities.First().ResFpsString;
            }
        }

        public  DateTime  RecordedDate
        {
            get
            {
                return _recordedDate;
            }
        }

        public Uri  Thumbnail
        {
            get
            {
                return _thumbnail;
            }
        }

        public Uri  GameThumbnail
        {
            get
            {
                return _gameThumbnail;
            }
        }

        public Uri  Url
        {
            get
            {
                return _url;
            }
        }


        #endregion თვისებები


    }
}
