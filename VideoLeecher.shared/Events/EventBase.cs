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

        protected virtual void InternalPublish(params object[] arguments)
        {
            IList<Action<object[]>> executionStrategies = PruneAndReturnStrategies();

            foreach (var executionStrategy in executionStrategies)
            {
                executionStrategy(arguments);
            }
        }

        protected virtual SubscriptionToken  InternalSubscribe(IEventSubscription eventSubscription)
        {
            if (eventSubscription == null)
            {
                throw new ArgumentNullException(nameof(eventSubscription));
            }

            eventSubscription.SubscriptionToken = new SubscriptionToken(Unsubscribe);

            lock(Subscriptions)
            {
                Subscriptions.Add(eventSubscription);
            }

            return eventSubscription.SubscriptionToken;
        }

        private IList<Action<object[]>> PruneAndReturnStrategies()
        {
            IList<Action<object[]>> returnList = new List<Action<object[]>>();

            lock(Subscriptions)
            {
                for(int i = Subscriptions.Count - 1; i >= 0; i--)
                {
                    Action<object[]> listItem = _subscription[i].GetExecutionStrategy();

                    if (listItem == null)
                    {
                        _subscription.RemoveAt(i);
                    }
                    else
                    {
                        returnList.Add(listItem);
                    }
                }

            }
            return returnList;
        }


        #endregion მეთოდები


    }
}
