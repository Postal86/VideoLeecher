﻿using System;
using System.IO;
using VideoLeecher.core.Enums;
using VideoLeecher.shared.Helpers;
using VideoLeecher.shared.IO;
using VideoLeecher.shared.Notification;


namespace VideoLeecher.core.Models
{
   public class Preferences : BindableBase
    {
        #region ველები 

        private Version _version;

        private bool _appCheckForUpdates;

        private bool _appShowDonationButton;

        private RangeObservableCollection<string> _searchFavouriteChannels;

        private string _searchChannelName;

        private VideoType _searchVideoType;

        private LoadLimitType _searchLoadLimitType;

        private int _searchLoadLastDays;

        private int _searchLoadLastVods;

        private bool _searchOnStartup;

        private string _downloadTempFolder;

        private string _downloadFolder;

        private string _downloadFileName;

        private bool _downloadSubfoldersForFav;

        private bool _downloadRemoveCompleted;

        private bool _miscUserExternalPlayer;

        private string _miscExternalPlayer;

        #endregion ველები

        #region  თვისებები

        public  Version Version
        {
            get
            {
                return _version; 
            }
            set
            {
                SetProperty(ref _version, value);
            }
        }

        
        public bool AppCheckForUpdates
        {
            get
            {
                return _appCheckForUpdates;
            }

            set
            {
                SetProperty(ref _appCheckForUpdates, value);
            }


        }

        public bool AppShowDonationButton
        {
            get
            {
                return _appShowDonationButton;
            }
            set
            {
                SetProperty(ref _appShowDonationButton, value);
            }
        }


        public bool MiscUseExternalPlayer
        {
            get
            {
                return _miscUserExternalPlayer;
            }
            set
            {
                SetProperty(ref _miscUserExternalPlayer, value);
            }

        }

        public string MiscExternalPlayer
        {
            get
            {
                return _miscExternalPlayer;
            }

            set
            {
                SetProperty(ref  _miscExternalPlayer, value);
            }
        }

        public RangeObservableCollection<string> SearchFavouriteChannels
        {
            get
            {
                if (_searchFavouriteChannels == null)
                {
                    _searchFavouriteChannels = new RangeObservableCollection<string>();
                }

                return _searchFavouriteChannels;
            }

        }


        public string SearchChannelName
        {
            get
            {
                return _searchChannelName;
            }
            set
            {
                SetProperty(ref _searchChannelName, value);
            }
        }

        public VideoType SearchVideoType
        {
            get
            {
                return _searchVideoType;
            }
            set
            {
                SetProperty(ref _searchVideoType, value);
            }
        }

        public LoadLimitType  SearchLoadLimitType
        {
            get
            {
                return _searchLoadLimitType;
            }
            set
            {
                SetProperty(ref _searchLoadLimitType, value);
            }
        }


        public int SearchLoadLastDays
        {
            get
            {
                return _searchLoadLastDays;
            }
            set
            {
                SetProperty(ref _searchLoadLastDays, value);
            }
        }


        public int  SearchLoadLastVods
        {
            get
            {
                return _searchLoadLastVods;
            }
            set => SetProperty(ref _searchLoadLastVods, value);
        }

        public bool SearchOnStartup
        {
            get
            {
                return _searchOnStartup;
            }
            set => SetProperty(ref _searchOnStartup, value);

        }

        public string  DownloadTempFolder
        {
            get
            {
                return _downloadTempFolder;
            }
            set => SetProperty(ref _downloadTempFolder, value);
        }

        public string DownloadFolder
        {

            get {

                return _downloadFolder;
                    
            }

            set => SetProperty(ref _downloadFolder,  value);

        }

        public string DownloadFileName
        {
            get
            {
                return _downloadFileName;
            }
            set => SetProperty(ref _downloadFileName, value); 
        }





        public bool  DownloadRemoveCompleted
        {

            get
            {
                return _downloadRemoveCompleted;
            }

            set => SetProperty(ref _downloadRemoveCompleted, value);

        }

        #endregion თვისებები

        #region მეთოდები 

        public override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = nameof(SearchChannelName);

            if (string.IsNullOrWhiteSpace(propertyName) ||  propertyName == currentProperty)
            {
                if (_searchOnStartup &&  string.IsNullOrWhiteSpace(_searchChannelName))
                {
                    AddError(currentProperty, "If 'Search on Startup' is enabled , you  need to specify a default  channel  name!");
                }
            }

            currentProperty = nameof(SearchLoadLastVods);

            if (string.IsNullOrWhiteSpace(propertyName) ||  propertyName == currentProperty)
            {
                if (_searchLoadLimitType == LoadLimitType.LastVods &&  (_searchLoadLastVods <  1 ||  _searchLoadLastVods >  999))
                {
                    AddError(currentProperty, "Value  has to be  between 1 and 999!");
                }
            }

            currentProperty = nameof(DownloadTempFolder);

            if (string.IsNullOrWhiteSpace(propertyName) ||  propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(DownloadTempFolder))
                {
                    AddError(currentProperty, "Please  specify  a temporary  download  folder!");
                }
                else if (!FileSystem.HasWritePermission(_downloadTempFolder))
                {
                    AddError(currentProperty, "You  do not have  write  permissions  on the specified  ");
                }


            }

            currentProperty = nameof(DownloadFolder); 


            if (string.IsNullOrWhiteSpace(propertyName) ||  propertyName == currentProperty)
            {

                if (string.IsNullOrWhiteSpace(_downloadFolder))
                {
                    AddError(currentProperty, "Please  specify  a  default  download  folder!");
                }
                else  if (!FileSystem.HasWritePermission(_downloadFolder))
                {
                    AddError(currentProperty, "You do not  have  write permissions on  the  specified  folder!  Please  choose  a different  one!");
                }

            }

            currentProperty = nameof(DownloadFileName);

            if (string.IsNullOrWhiteSpace(propertyName) ||  propertyName == currentProperty)
            {
                if (string.IsNullOrWhiteSpace(_downloadFileName))
                {
                    AddError(currentProperty, "Filename specify  a  default  download  filename!");
                }
                else if (!_downloadFileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                {
                    AddError(currentProperty, "Filename must end  with '.mp4'!");
                }
                else if (FileSystem.FilenameContainsInvalidChars(_downloadFileName))
                {
                    AddError(currentProperty, "Filename contains  invalid  characters!");
                }
            }
        }


        public  Preferences Clone()
        {
            Preferences clone = new Preferences()
            {

                Version = Version,
                AppCheckForUpdates = AppCheckForUpdates,
                AppShowDonationButton = AppShowDonationButton,
                SearchChannelName = SearchChannelName,
                SearchVideoType = SearchVideoType,
                SearchLoadLimitType = SearchLoadLimitType,
                SearchLoadLastDays = SearchLoadLastDays,
                SearchLoadLastVods = SearchLoadLastVods,
                SearchOnStartup = SearchOnStartup,
                DownloadTempFolder = DownloadTempFolder,
                DownloadFolder = DownloadFolder,
                DownloadFileName = DownloadFileName,
                DownloadRemoveCompleted = DownloadRemoveCompleted
            };

            return clone;
        }

       #endregion  მეთოდები
    }
}
