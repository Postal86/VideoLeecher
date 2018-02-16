using System.Collections.ObjectModel;
using System.ComponentModel;
using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
    public interface ILeecherService : INotifyPropertyChanged
    {
        #region თვისებები

        bool IsAuthorized { get; }

        ObservableCollection<LeecherVideo> Videos { get; }

        ObservableCollection<LeecherVideoDownload> Downloads { get; }

        #endregion თვისებები

        #region  მეთოდები 

        VodAuthInfo RetrieveVodAuthInfo(string id);

        bool ChannelExists(string channel);

        string GetChannelIdByName(string channel);

        bool Authorize(string accessToken);

        void RevokeAuthorization();

        void Search(SearchParameters searchParams);

        void Enqueue(DownloadParameters downloadParams);

        void Cancel(string id);

        void Retry(string id);

        void Remove(string id); 

        void Pause();

        bool Resume();

        bool CanShutdown();

        void Shutdown();

        bool IsFileNameUsed(string fullPath);


        #endregion მეთოდები


    }
}
