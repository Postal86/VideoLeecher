using System;
using System.IO;
using System.Xml.Linq;
using VideoLeecher.core.Models;
using VideoLeecher.services.Interfaces;
using VideoLeecher.shared.Extensions;
using VideoLeecher.shared.IO;
using VideoLeecher.shared.Reflection;


namespace VideoLeecher.services.Services
{
    internal class RuntimeDataService : IRuntimeDataService
    {
        #region კონსტანტები 

        private const string RUNTIMEDATA_FILE = "runtime.xml";

        private const string RUNTIMEDATA_EL = "RuntimeData";
        private const string RUNTIMEDATA_VERSION_ATTR = "Version";

        private const string AUTH_EL = "Authorization";
        private const string AUTH_ACCESSTOKEN_EL = "AccessToken";

        private const string APP_EL = "Application";

        #endregion კონსტანტები


        #region ველები

        private IFolderService _folderService;

        private RuntimeData _runtimeData;

        private Version _tileVersion;

        private readonly object _commandLockObject;


        #endregion ველები

        #region კონსტრუქტორი

        public RuntimeDataService(IFolderService folderService)
        {
            _folderService = folderService;
            _tileVersion = AssemblyUtils.Get.GetAssemblyVersion().Trim();
            _commandLockObject = new object();
        }


        #endregion კონსტრუქტორი



        #region თვისებები 

        public RuntimeData RuntimeData
        {
            get
            {
                if (_runtimeData == null)
                {
                    _runtimeData = Load();
                }

                return _runtimeData;
            }

        }


        #endregion თვისებები

        #region მეთოდები 

        public void Save()
        {
            lock (_commandLockObject)
            {
                RuntimeData runtimeData = RuntimeData;

                XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", null));

                XElement runtimeDataEl = new XElement(RUNTIMEDATA_EL);
                runtimeDataEl.Add(new XAttribute(RUNTIMEDATA_VERSION_ATTR, _tileVersion));
                doc.Add(runtimeDataEl);

                if (!string.IsNullOrWhiteSpace(runtimeData.AccessToken))
                {
                    XElement authEl = new XElement(AUTH_EL);
                    runtimeDataEl.Add(authEl);

                    XElement accessTokenEl = new XElement(AUTH_ACCESSTOKEN_EL);
                    accessTokenEl.SetValue(runtimeData.AccessToken);
                    authEl.Add(accessTokenEl);
                }

                if (runtimeData.MainWindowInfo != null)
                {
                    XElement mainWindowInfoEl = runtimeData.MainWindowInfo.GetXML();

                    if (mainWindowInfoEl.HasElements)
                    {
                        XElement applicationEl = new XElement(APP_EL);
                        applicationEl.Add(mainWindowInfoEl);
                        runtimeDataEl.Add(applicationEl);
                    }

                }

                string appDataFolder = _folderService.GetAppDataFolder();

                FileSystem.CreateDirectory(appDataFolder);

                string configFile = Path.Combine(appDataFolder, RUNTIMEDATA_FILE);

                doc.Save(configFile);
            }

        }

        private RuntimeData Load()
        {
            lock (_commandLockObject)
            {
                string configFile = Path.Combine(_folderService.GetAppDataFolder(), RUNTIMEDATA_FILE);

                RuntimeData runtimeData = new RuntimeData()
                {
                    Version = _tileVersion
                };

                if (File.Exists(configFile))
                {
                    XDocument doc = XDocument.Load(configFile);

                    XElement runtimeDataEl = doc.Root;

                    if (runtimeDataEl != null)
                    {
                        XAttribute rtVersionAttr = runtimeDataEl.Attribute(RUNTIMEDATA_VERSION_ATTR);

                        if (rtVersionAttr != null && Version.TryParse(rtVersionAttr.Value, out Version rtVersion))
                        {
                            runtimeData.Version = rtVersion;
                        }
                        else
                        {
                            runtimeData.Version = new Version(1, 0);
                        }

                        XElement authEl = runtimeDataEl.Element(AUTH_ACCESSTOKEN_EL);

                        if (authEl != null)
                        {
                            XElement accessTokenEl = authEl.Element(AUTH_ACCESSTOKEN_EL);

                            if (accessTokenEl != null)
                            {
                                try
                                {
                                    runtimeData.AccessToken = accessTokenEl.GetValueAsString();
                                }
                                catch
                                {
                                    //  Value from config file could not be loaded, use default value
                                }
                            }

                        }

                        XElement applicationEl = runtimeDataEl.Element(APP_EL);

                        if (applicationEl != null)
                        {
                            XElement mainWindowInfoEl = applicationEl.Element(MainWindowInfo.MAINWINDOW_EL);

                            if (mainWindowInfoEl != null)
                            {

                                try
                                {
                                    runtimeData.MainWindowInfo = MainWindowInfo.GetFromXml(mainWindowInfoEl);
                                }
                                catch
                                {
                                    //  Value from config file could  not be  loaded, use default value
                                }

                            }

                        }
                    }
                }

                    return runtimeData;
                }

            }


            #endregion მეთოდები
        }

    }

