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
                throw  new ArgumentException("ლისტის  კონტეინერში მინიმუმ ერთი ელემენტი  უნდა იყოს", nameof(fileInfoList));
            }

            CheckPriority(priority);

            _id = id;
            _fileInfolist = fileInfoList;
            _state = DownloadState.Queued;
            _priority = priority;
        }



        #endregion კონსტრუქტორი


        #region თვისებები

        public int Priority
        {

            get => _priority;
            set {

                CheckPriority(value);
                _priority = value;
                }


        }


        public DownloadState State => _state;

        public int Rate { get; set; }


        #endregion თვისებები

        #region მეთოდები

        private void CheckPriority(int priority)
        {
            if (priority < 0)
            {
                throw new ArgumentException("უარყოფითი მნიშვნელობა  არ არის მისაღები.", nameof(priority));
            }
        }

        public void Start()
        {

        }

        public void Pause()
        {

        }

        public void Resume()
        {

        }

        public void Cancel()
        {

        }


        #endregion მეთოდები

        #region ივენთები

        public event EventHandler<DownloadEventArgs> StateChanged;

        private void FireStateChanged()
        {
            StateChanged?.Invoke(this, new DownloadEventArgs(this));
        }

        #endregion ივენთები

    }
}
