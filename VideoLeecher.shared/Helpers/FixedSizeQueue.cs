using System.Collections.Concurrent;


namespace VideoLeecher.shared.Helpers
{
    public class FixedSizeQueue<T>
    {

        #region ველები

        private readonly object _queueLockObject;

        private ConcurrentQueue<T> _queue;

        private int _size;

        #endregion  ველები

        #region კონსტრუქტორები

        public FixedSizeQueue(int size)
        {
            _queueLockObject = new object();
            _queue = new ConcurrentQueue<T>();

            if (size < 1)
            {
                size = 1;
            }

            _size = size;
        }

        #endregion კონსტრუქტორები


        #region თვისებები

        public int Size
        {
            get
            {
                return _size;
            }
        }

        #endregion თვისებები

        #region მეთოდები

        public void Enqueue(T obj)
        {
            lock (_queueLockObject)
            {
                _queue.Enqueue(obj);

                while (_queue.Count > _size)
                {
                    _queue.TryDequeue(out  T outObj);
                }
            }
        }


        #endregion მეთოდები


    }
}
