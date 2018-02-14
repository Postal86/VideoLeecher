using System;
using System.Globalization;

namespace VideoLeecher.core.Models
{
    public class VodPlaylistPartExt : IVodPlaylistPartExt
    {
        #region ველები

        private int _index;
        private string _output;
        private string _urlPrefix;
        private string _localFile;
        private string _downloadUrl;
        private double _length;


        #endregion ველები 


        #region კონსტრუქტორები

        public IVodPlaylistPartExt(int index, string extinf, string remoteFile, string urlPrefix, string localFile)
        {
            if (string.IsNullOrWhiteSpace(extinf))
            {
                throw new ArgumentNullException(nameof(extinf));
            }

            if (string.IsNullOrWhiteSpace(remoteFile))
            {
                throw new ArgumentNullException(nameof(remoteFile));
            }

            if (string.IsNullOrWhiteSpace(urlPrefix))
            {
                throw new ArgumentNullException(nameof(urlPrefix));
            }

            if (string.IsNullOrWhiteSpace(localFile))
            {
                throw new ArgumentNullException(nameof(localFile));
            }

            _index = index;
            _downloadUrl = urlPrefix + remoteFile;
            _length = Math.Max(double.Parse(extinf.Substring(extinf.LastIndexOf(":") + 1).TrimEnd(','), NumberStyles.Any, CultureInfo.InvariantCulture), 0);
            _urlPrefix = urlPrefix;
            _localFile = localFile;

            _output = extinf + "\n" + localFile;
        }

      #endregion კონსტრუქტორები

    }
}
