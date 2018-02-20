﻿using VideoLeecher.core.Models;

namespace VideoLeecher.services.Interfaces
{
    public interface IFilenameService
    {
        string SubstituteWildcards(string filename, TwitchVideo  video);

        string SubstituteInvalidChars(string filename,  string replaceStr);
    }
}
