using System;
using VideoLeecher.core.Models;


namespace VideoLeecher.services.Interfaces
{
    public interface IProcessingService
    {
        string FFMPEGExe { get; }

        void ConcatParts(Action<string> log, Action<string> setStatus, Action<double> setProgress, VodPlaylist  vodPlaylist,  string  concatFile);

        void ConvertVideo(Action<string> log, Action<string> setStatus, Action<double> setProgress, Action<bool> setIsIndeterminate, string sourceFile, string outputFile,  CropInfo cropInfo);
    }
}
