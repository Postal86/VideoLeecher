using System;

namespace VideoLeecher.services.Services.Download
{
    public class DownloadEventArgs : EventArgs
    {
        #region ველები

        private Download _download;


        #endregion ველები

        #region კონსტრუქტორები

        public DownloadEventArgs(Download download)
        {
            _download = download ?? throw new ArgumentNullException(nameof(download));
        }

        #endregion კონსტრუქტორები

        #region თვისებები

        public Download Download => _download;

        #endregion თვისებები

    }
}