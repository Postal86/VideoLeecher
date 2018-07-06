using System;
using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
    public interface IFilenameService
    {
        string SubstituteWildcards(string filename, TwitchVideo  video,  TwitchVideoQuality quality = null, TimeSpan?  cropStart = null, TimeSpan?  cropEnd = null);

        string SubstituteInvalidChars(string filename,  string replaceStr);
    }
}
