using System;
using System.IO;
using System.Xml.Linq;
using VideoLeecher.core.Enums;
using VideoLeecher.core.Events;
using VideoLeecher.core.Models;
using VideoLeecher.services.Interfaces;
using VideoLeecher.shared.Events;
using VideoLeecher.shared.Extensions;
using VideoLeecher.shared.IO;
using VideoLeecher.shared.Reflection;
    
namespace VideoLeecher.services.Services
{
    internal class PreferencesService : IPreferenceService
    {
        #region კონსტანტები

        private const string CONFIG_FILE = "config.xml";

        private const string PREFERENCES_EL = "Preferences";
        private const string PREFERENCES_VERSION_ATTR = "Version";

        private const string APP_EL = "Application";
        private const string APP_CHECKFORUPDATES_EL = "Version";
        private const string APP_SHOWDONATIONBUTTON_EL = "ShowDonationButton";

        private const string SEARCH_EL = "Search";
        private const string SEARCH_CHANNELNAME_EL = "ChannelName";
        private const string SEARCH_VIDEOTYPE_EL = "VideoType";
        private const string SEARCH_LOADLIMITTYPE_EL = "LoadLimitType";
        private const string SEARCH_LOADLASTDAYS_EL = "LoadLastDays";
        private const string SEARCH_LOADLASTVODS_EL = "LoadLastVods";
        private const string SEARCH_SEARCHONSTARTUP_EL = "SearchOnStartup";

        private const string DOWNLOAD_EL = "Download";
        private const string DOWNLOAD_TEMPFOLDER_EL = "TempFolder";
        private const string DOWNLOAD_FOLDER_EL = "Folder";
        private const string DOWNLOAD_FILENAME_EL = "FileName";
        private const string DOWNLOAD_REMOVECOMPLETED_EL = "RemoveCompleted";

        #endregion კონსტანტები

        #region ველები 

        private IFolderService _folderService;
        private IEventAggregator _eventAggregator;

        private Preferences _currentPreferences;
        private Version _tileVersion;

        private readonly object _commandLockObject;

        #endregion ველები

        #region კონსტრუქტორი

        public  PreferencesService(IFolderService  folderService, IEventAggregator  eventAggregator)
        {
            _folderService = folderService;
            _eventAggregator = eventAggregator;

            _tileVersion = AssemblyUtils.Get.GetAssemblyVersion().Trim();
            _commandLockObject = new object();


        }

        #endregion კონსტრუქტორი

        #region თვისებები 

        public Preferences  CurrentPreferences
        {
            get 
            {
                if (_currentPreferences == null)
                {
                    _currentPreferences = Load();
                }

                return _currentPreferences;
            }
        }

        #endregion თვისებები

        #region მეთოდები 

        public bool IsChannelInFavourites(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                return false;
            }

            if (CurrentPreferences.SearchChannelName.Equals(channel, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string  existingEntry =  CurrentPreferences.SearchF

        }




        public void Save(Preferences preferences)
        {

            lock (_commandLockObject)
            {
                XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

                XElement preferenceEl = new XElement(PREFERENCES_EL);
                preferenceEl.Add(new XAttribute(PREFERENCES_VERSION_ATTR, _tileVersion));
                doc.Add(preferenceEl);

                XElement appEl = new XElement(APP_EL);
                preferenceEl.Add(appEl);

                XElement searchEl = new XElement(SEARCH_EL);
                preferenceEl.Add(searchEl);

                XElement downloadEl = new XElement(DOWNLOAD_EL);
                preferenceEl.Add(downloadEl);

                // Application 
                XElement appCheckForUpdateEl = new XElement(APP_CHECKFORUPDATES_EL);
                appCheckForUpdateEl.SetValue(preferences.AppCheckForUpdates);
                appEl.Add(appCheckForUpdateEl);

                XElement appShowDonationButtonEl = new XElement(APP_SHOWDONATIONBUTTON_EL);
                appShowDonationButtonEl.SetValue(preferences.AppShowDonationButton);
                appEl.Add(appShowDonationButtonEl);

                //  Search
                if (!string.IsNullOrWhiteSpace(preferences.SearchChannelName))
                {
                    XElement searchChannelNameEl = new XElement(SEARCH_CHANNELNAME_EL);
                    searchChannelNameEl.SetValue(preferences.SearchChannelName);
                    searchEl.Add(searchChannelNameEl);
                }

                XElement searchVideoTypeEl = new XElement(SEARCH_VIDEOTYPE_EL);
                searchVideoTypeEl.SetValue(preferences.SearchVideoType);
                searchEl.Add(searchVideoTypeEl);

                XElement searchLoadLimitTypeEl = new XElement(SEARCH_LOADLIMITTYPE_EL);
                searchLoadLimitTypeEl.SetValue(preferences.SearchLoadLastDays);
                searchEl.Add(searchLoadLimitTypeEl);

                XElement searchLoadLastDaysEl = new XElement(SEARCH_LOADLASTDAYS_EL);
                searchLoadLastDaysEl.SetValue(preferences.SearchLoadLastDays);
                searchEl.Add(searchLoadLastDaysEl);

                XElement searchOnStartupEl = new XElement(SEARCH_SEARCHONSTARTUP_EL);
                searchOnStartupEl.SetValue(preferences.SearchOnStartup);
                searchEl.Add(searchOnStartupEl);

                //  Download
                if (!string.IsNullOrWhiteSpace(preferences.DownloadTempFolder))
                {
                    XElement downloadTempFolderEl = new XElement(DOWNLOAD_TEMPFOLDER_EL);
                    downloadTempFolderEl.SetValue(preferences.DownloadTempFolder);
                    downloadEl.Add(downloadTempFolderEl);
                }

                if (!string.IsNullOrWhiteSpace(preferences.DownloadFolder))
                {
                    XElement downloadFolderEl = new XElement(DOWNLOAD_FOLDER_EL);
                    downloadFolderEl.SetValue(preferences.DownloadFolder);
                    downloadEl.Add(downloadFolderEl);
                }

                if (!string.IsNullOrWhiteSpace(preferences.DownloadFileName))
                {
                    XElement downloadFileNameEl = new XElement(DOWNLOAD_FILENAME_EL);
                    downloadFileNameEl.SetValue(preferences.DownloadFileName);
                    downloadEl.Add(downloadFileNameEl);
                }

                XElement downloadRemoveCompletedEl = new XElement(DOWNLOAD_REMOVECOMPLETED_EL);
                downloadRemoveCompletedEl.SetValue(preferences.DownloadRemoveCompleted);
                downloadEl.Add(downloadRemoveCompletedEl);

                string appDataFolder = _folderService.GetAppDataFolder();

                FileSystem.CreateDirectory(appDataFolder);

                string configFile = Path.Combine(appDataFolder, CONFIG_FILE);

                doc.Save(configFile);

                _currentPreferences = preferences;

                _eventAggregator.GetEvent<PreferencesSavedEvent>().Publish();
                    
            }

        }


        private Preferences  Load()
        {
            lock (_commandLockObject)
            {
                string configFile = Path.Combine(_folderService.GetAppDataFolder(), CONFIG_FILE);

                Preferences preferences = CreateDefault();

                if (File.Exists(configFile))
                {
                    XDocument doc = XDocument.Load(configFile);

                    XElement preferencesEl = doc.Root;

                    if (preferencesEl != null)
                    {
                        XAttribute prefVersionAttr = preferencesEl.Attribute(PREFERENCES_VERSION_ATTR);

                        if (prefVersionAttr !=  null &&  Version.TryParse(prefVersionAttr.Value, out Version prefVersion))
                        {
                            preferences.Version = prefVersion;
                        }
                        else
                        {
                            preferences.Version = new Version(1, 0);
                        }

                        XElement appEl = preferencesEl.Element(APP_EL);

                        if (appEl != null)
                        {
                            XElement appCheckForUpdateEL = appEl.Element(APP_CHECKFORUPDATES_EL);

                            if (appCheckForUpdateEL != null)
                            {
                                try
                                {
                                    preferences.AppCheckForUpdates = appCheckForUpdateEL.GetValueAsBool();
                                }
                                catch
                                {
                                    //  Value  from config  file could not  be loaded, use  default value
                                }
                            }

                            XElement appShowDonationButtonEl = appEl.Element(APP_SHOWDONATIONBUTTON_EL);

                            if (appShowDonationButtonEl != null)
                            {
                                try
                                {
                                    preferences.AppShowDonationButton = appShowDonationButtonEl.GetValueAsBool();
                                }
                                catch
                                {
                                    //  Value  from config  file could not  be loaded, use  default value
                                }
                            }
                        }

                        XElement searchEl = preferencesEl.Element(SEARCH_EL);

                        if (searchEl != null)
                        {
                            XElement searchChannelNameEl = searchEl.Element(SEARCH_CHANNELNAME_EL);

                            if (searchChannelNameEl != null)
                            {
                                try
                                {
                                    preferences.SearchChannelName = searchChannelNameEl.GetValueAsString();
                                }
                                catch
                                {
                                    //  Value  from config  file could not  be loaded, use  default value
                                }
                            }

                            XElement searchVideoTypeEl = searchEl.Element(SEARCH_VIDEOTYPE_EL);

                            if (searchVideoTypeEl != null)
                            {
                                try
                                {
                                    preferences.SearchVideoType = searchVideoTypeEl.GetValueAsEnum<VideoType>();
                                }
                                catch
                                {
                                    // Value  from config  file could not  be loaded, use  default value
                                }
                            }

                            XElement searchLoadLastDaysEl = searchEl.Element(SEARCH_LOADLASTDAYS_EL);

                            if (searchLoadLastDaysEl != null)
                            {
                                try
                                {
                                    preferences.SearchLoadLastDays = searchLoadLastDaysEl.GetValueAsInt();
                                }
                                catch
                                {
                                    // Value from config file could not be loaded, use default value
                                }
                            }

                            XElement searchLoadLastVodsEl = searchEl.Element(SEARCH_LOADLASTVODS_EL);

                            if (searchLoadLastVodsEl != null)
                            {
                                try
                                {
                                    preferences.SearchLoadLastVods = searchLoadLastVodsEl.GetValueAsInt();
                                }
                                catch
                                {
                                    // Value from config  file  could not  loaded,  use  default value
                                }

                            }

                            XElement searchOnStartupEl = searchEl.Element(SEARCH_SEARCHONSTARTUP_EL);

                            if (searchOnStartupEl != null)
                            {
                                try
                                {
                                    preferences.SearchOnStartup = searchOnStartupEl.GetValueAsBool();
                                        
                                }
                                catch
                                {
                                    //  Value from config file could not be loaded, use default value
                                }

                            }
                        }
                    }

                    XElement downloadEl = preferencesEl.Element(DOWNLOAD_EL);

                    if (downloadEl != null)
                    {
                        XElement downloadTempFolderEl = downloadEl.Element(DOWNLOAD_TEMPFOLDER_EL);

                        if (downloadTempFolderEl != null)
                        {
                            try
                            {
                                preferences.DownloadTempFolder = downloadTempFolderEl.GetValueAsString();
                            }
                            catch
                            {
                                //  Value from config  file  could not be  loaded, use default value
                            }

                        }

                        XElement downloadFolderEl = downloadEl.Element(DOWNLOAD_FOLDER_EL);

                        if (downloadFolderEl != null)
                        {
                            try
                            {
                                preferences.DownloadFolder = downloadFolderEl.GetValueAsString();
                            }
                            catch
                            {
                                // Value from config  file  could  not be loaded,  use  default  value
                            }
                        }

                        XElement downloadFileNameEl = downloadEl.Element(DOWNLOAD_FILENAME_EL);

                        if (downloadFileNameEl  != null)
                        {
                            try
                            {
                                preferences.DownloadFileName = downloadFileNameEl.GetValueAsString();
                            }
                            catch
                            {
                                //  Value  from config  file could  not be loaded,  use  default value
                            }
                        }

                        XElement downloadRemoveCompletedEl = downloadEl.Element(DOWNLOAD_REMOVECOMPLETED_EL);

                        if (downloadRemoveCompletedEl != null)
                        {
                            try
                            {
                                preferences.DownloadRemoveCompleted = downloadRemoveCompletedEl.GetValueAsBool();
                            }
                            catch
                            {
                                 //  Value  from config  file could not  be  loaded,  use  default value
                            }
                        }
                    }

                }


                return preferences;
            }

        }

        public Preferences  CreateDefault()
        {
            Preferences preferences = new Preferences()
            {
                Version = _tileVersion,
                AppCheckForUpdates = true,
                AppShowDonationButton = true,
                SearchChannelName = null,
                SearchVideoType = VideoType.Broadcast,
                SearchLoadLimitType = LoadLimitType.Timespan,
                SearchLoadLastDays = 10,
                SearchLoadLastVods = 10,
                SearchOnStartup = false,
                DownloadTempFolder = _folderService.GetTempFolder(),
                DownloadFolder = _folderService.GetDownloadFolder(),
                DownloadFileName = FilenameWildcards.DATE + "_" + FilenameWildcards.ID + "_" + FilenameWildcards.GAME + ".mp4",

            };
            return preferences;
        }



        #endregion მეთოდები


    }
}
