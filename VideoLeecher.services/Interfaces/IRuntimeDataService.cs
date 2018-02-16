using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
    public interface IRuntimeDataService
    {
        RuntimeData RuntimeData { get; }

        void Save();
    }
}
