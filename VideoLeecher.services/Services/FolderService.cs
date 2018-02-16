using System;
using System.IO;
using VideoLeecher.services.Interfaces;
using VideoLeecher.shared.Reflection;


namespace VideoLeecher.services.Services
{
    internal class FolderService : IFolderService
    {
        #region ველები 

        private string appDataFolder;
        private string downloadsTempFolder;
        private string downloadsFolder;

        #endregion  ველები


        #region მეთოდები

        public  string GetAppDataFolder()
        {
            if (string.IsNullOrWhiteSpace(appDataFolder))
            {
                string productName = AssemblyUtils.Get.GetProductName();
                appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), productName);
            }

            return appDataFolder;
        }

        public  string GetTempFolder()
        {
            if (string.IsNullOrWhiteSpace(downloadsTempFolder))
            {
                downloadsTempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
            }

            return downloadsTempFolder;
        }

        public string GetDownloadFolder()
        {
            if (string.IsNullOrWhiteSpace(downloadsFolder))
            {
                downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            }

            return downloadsFolder;
        }

        #endregion მეთოდები;


    }
}
