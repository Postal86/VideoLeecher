using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace VideoLeecher.shared.Helpers
{
   public  class RangeObservableCollection<T> :  ObservableCollection<T>
   {
        public RangeObservableCollection() 
         : base()
        {
        }

        public RangeObservableCollection(ICollection<T> collection)
            : base(collection)
        {
        }


        public  RangeObservableCollection(IList<T> list)
            : base(list)
        {

        }

        public void AddRange(ICollection<T> range)
        {
            foreach (var item in range)
            {
                Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(ICollection<T> range)
        {
            Items.Clear();
            AddRange(range);
        }

   }

}
