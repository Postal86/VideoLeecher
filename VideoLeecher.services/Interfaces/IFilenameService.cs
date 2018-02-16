using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
    public interface IFilenameService
    {
        string SubstituteWildcards(string filename, LeecherVideo  video);

        string SubstituteInvalidChars(string filename,  string replaceStr);
    }
}
