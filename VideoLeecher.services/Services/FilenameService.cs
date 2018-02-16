﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using VideoLeecher.core.Models;
using VideoLeecher.services.Interfaces;


namespace VideoLeecher.services.Services
{
    internal class FilenameService : IFilenameService
    {
        #region მეთოდები 

        public  string SubstituteWildcards(string filename, LeecherVideo  video)
        {
            if (video == null)
            {
                throw new ArgumentNullException(nameof(video));
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                return filename;
            }

            string result = filename;

            DateTime recorded = video.RecordedDate;


            LeecherVideoQuality quality = video.Qualities.First();

            result = result.Replace(FilenameWildcards.CHANNEL, video.Channel);
            result = result.Replace(FilenameWildcards.GAME, video.Game);
            result = result.Replace(FilenameWildcards.DATE, recorded.ToString("yyyyMMdd"));
            result = result.Replace(FilenameWildcards.TIME, recorded.ToString("hhmmsstt", CultureInfo.InvariantCulture));
            result = result.Replace(FilenameWildcards.TIME24, recorded.ToString("HHmmss", CultureInfo.InvariantCulture));
            result = result.Replace(FilenameWildcards.ID, video.Id);
            result = result.Replace(FilenameWildcards.TITLE, video.Title);
            result = result.Replace(FilenameWildcards.RES, !string.IsNullOrWhiteSpace(quality.Resolution) ? quality.Resolution : LeecherVideoQuality.UNKNOWN);
            result = result.Replace(FilenameWildcards.FPS, quality.Fps.HasValue ? quality.Fps.ToString() : LeecherVideoQuality.UNKNOWN);

            result = SubstituteInvalidChars(result, "_");

            return result;
        }

        public string SubstituteInvalidChars(string filename, string replaceStr)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return filename;
            }

            if (string.IsNullOrEmpty(replaceStr))
            {
                throw new ArgumentNullException(nameof(replaceStr));
            }

            foreach (char c in  Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c.ToString(), replaceStr);
            }

            return filename;

        }


       #endregion მეთოდები
    }
}
