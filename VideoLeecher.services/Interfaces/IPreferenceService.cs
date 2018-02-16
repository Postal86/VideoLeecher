using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
   public interface IPreferenceService
    {
        Preferences CurrentPreferences { get; }

        void Save(Preferences preferecnces);

        Preferences CreateDefault();
    }
}
