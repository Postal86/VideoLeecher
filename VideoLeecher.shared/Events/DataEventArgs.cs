using System;


namespace VideoLeecher.shared.Events
{
   public class DataEventArgs<TData> : EventArgs
    {
        #region ველები

        private readonly TData _value;

        #endregion ველები

        #region კონსტრუქტორები

        public DataEventArgs(TData value)
        {
            _value = value;
        }

        #endregion კონსტრუქტორები


        #region თვისებები

        public  TData Value
        {
            get { return _value; }
        }

        #endregion თვისებები

    }
}
