using System;


namespace VideoLeecher.core.Models
{
    public  class  VodPlaylistPart
    {
        #region კონსტრუქტორები

        public  VodPlaylistPart(double  length, string remoteFile, string localFile)
        {
            if (string.IsNullOrWhiteSpace(remoteFile))
            {
                throw new ArgumentNullException(nameof(remoteFile));
            }

            if (string.IsNullOrWhiteSpace(localFile))
            {
                throw new ArgumentNullException(nameof(localFile));
            }

            Length = length;
            RemoteFile = remoteFile;
            LocalFile = localFile;

        }

        #endregion კონსტრუქტორები

        #region თვისებები 

        public string RemoteFile { get;  }

        public string LocalFile { get; }

        public double Length { get; }



        #endregion თვისებები







    }



}