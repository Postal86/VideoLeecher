namespace VideoLeecher.services.Services.Download
{
    public class DownloadFileInfo
    {
        #region ველები

        private string _url;
        private string _localFile;

        #endregion ველები

        #region კონსტრუქტორები

        public DownloadFileInfo(string url, string localFile)
        {
            _url = url;
            _localFile = localFile;
        }



        #endregion კონსტრუქტორები

        #region თვისებები

        public string Url => _url;

        public string LocalFile => _localFile;

        #endregion თვისებები


    }
}