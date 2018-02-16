namespace VideoLeecher.core.Models
{
   public  class VodPlaylistPart : IVodPlaylistPart
    {
        #region ველები 

        private int _index;
        private string _output;

        #endregion ველები


        #region კონსტრუქტორები

        public VodPlaylistPart(int index, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                _output = null;
            }
            else
            {
                _output = input;
            }

            _index = index;
        }

        #endregion კონსტრუქტორები

        #region თვისებები

        public  int Index
        {

            get
            {
                return _index;
            }

        }

        #endregion თვისებები


        #region მეთოდები 

        public string GetOutput()
        {
            return _output;
        }


        #endregion მეთოდები



    }
}
