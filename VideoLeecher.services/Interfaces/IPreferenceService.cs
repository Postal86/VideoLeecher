using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
   public interface IPreferenceService
    {
        Preferences CurrentPreferences { get; }

        bool IsChannelInFavourites(string channel);

        void Save(Preferences preferecnces);

        Preferences CreateDefault();
    }
}
