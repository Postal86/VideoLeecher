using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VideoLeecher.shared.Events
{
    public abstract class EventBase 
    {
        #region თვისებები

        private readonly List<IEventSubscription> _subscription = new List<IEventSubscription>();

        #endregion თვისებები 

        #region თვისებები

        public SynchronizationContext SynchronizationContext { get; set; }


        protected  ICollection<IEventSubscription> Subscriptions
        {
            get
            {
                return _subscription;
            }
        }

        #endregion თვისებები

        #region მეთოდები

        public virtual bool Contains(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                return Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token) != null;
            }
        }

        public virtual void Unsubscribe(SubscriptionToken token)
        {
            lock (Subscriptions)
            {
                IEventSubscription subscription = Subscriptions.FirstOrDefault(evt => evt.SubscriptionToken == token);

                if (subscription != null)
                {
                    Subscriptions.Remove(subscription);
                }
            }

        }





        #endregion მეთოდები


    }
}
