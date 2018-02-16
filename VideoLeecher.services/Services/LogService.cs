using System;
using System.IO;
using VideoLeecher.services.Interfaces;
using VideoLeecher.shared.IO;

namespace VideoLeecher.services.Services
{
    internal class LogService : ILogService
    {
        #region კონსტანტები

        private const string LOGS_FOLDER_NAME = "logs";

        #endregion  კონსტანტები


        #region ველები


        private string _logDir;

        #endregion ველები

        #region კონსტრუქტორები

        public LogService(IFolderService  folderService)
        {

            if (folderService == null)
            {
                throw new ArgumentNullException(nameof(folderService));
            }

            _logDir = Path.Combine(folderService.GetAppDataFolder(), LOGS_FOLDER_NAME);
        }



        #endregion კონსტრუქტორები

        #region მეთოდები

        public string LogException(Exception ex)
        {
            try
            {
                FileSystem.CreateDirectory(_logDir);

                string logFile = Path.Combine(_logDir, DateTime.UtcNow.ToString("MMddyyyy_hhmmss_fff_tt") + "_error.log");

                File.WriteAllText(logFile, ex.ToString());

                return logFile;
            }
            catch
            {

            }
            return null;
        }

       #endregion მეთოდები
    }
}
