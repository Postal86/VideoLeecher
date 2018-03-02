using System;
using System.Collections.Generic;
using VideoLeecher.core.Enums;

namespace VideoLeecher.services.Services.Download
{
    public class Download
    {
        #region ველები

        private Guid _id;

        private IList<DownloadFileInfo> _fileInfolist;

        private DownloadState _state;

        private int _priority;

        #endregion ველები

        #region   კონსტრუქტორი

        public Download(Guid id, IList<DownloadFileInfo> fileInfoList, int priority)
        {
            if (id == Guid.Empty)
            {
                throw  new ArgumentException("ცარიელი Guid არ არის მისაღები", nameof(id));
            }

            if (fileInfoList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoList));
            }

            if (fileInfoList.Count == 0)
            {
                throw  new ArgumentException("ლისტის  კონტეინერში ერთი ელემენტი მაინც უნდა იყოს", nameof(fileInfoList));
            }

            CheckPriority(priority);

            _id = id;
            _fileInfolist = fileInfoList;
            _state = DownloadState.Queued;
            _priority = priority;
        }



        #endregion კონსტრუქტორი


    }
}
