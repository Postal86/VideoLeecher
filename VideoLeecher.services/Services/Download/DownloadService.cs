using System;
using System.Collections.Concurrent;
using System.Linq;
using VideoLeecher.services.Interfaces;


namespace VideoLeecher.services.Services.Download
{
    internal class DownloadService : IDownloadServices
    {

        #region ველები

        private ConcurrentDictionary<Guid, Download> _downloads;

        #endregion ველები 

        #region კონსტრუქტორები

        public DownloadService()
        {
            _downloads = new ConcurrentDictionary<Guid, Download>();
            
        }


        #endregion კონსტრუქტორები

        #region ველები

        public int Rate
        {
            get { return _downloads.Values.Sum(d => d.Rate); }
        }



        #endregion ველები


    }
}