using System;
using System.Linq;


namespace VideoLeecher.shared.Events
{
  public  class PubSubEvent<TPayload> : EventBase
  {
        #region მეთოდები

        public virtual bool Contains(Action<TPayload> subscriber)
        {
            IEventSubscription eventSubscription;

            lock(Subscriptions)
            {
                eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            }

            return eventSubscription != null;
        }

        public virtual void Publish(TPayload  payload)
        {
            base.InternalPublish(payload);
        }

        public  SubscriptionToken Subscribe(Action<TPayload> action)
        {
            return Subscribe(action, ThreadOption.PublisherThread);
        }

        public SubscriptionToken  Subscribe(Action<TPayload> action, ThreadOption threadOption)
        {
            return Subscribe(action, threadOption);
        }

        public SubscriptionToken Subscribe(Action<TPayload> action, ThreadOption  threadOpion, bool keepSubsciberReferenceAlive)
        {
            return Subscribe(action, ThreadOption.PublisherThread,  keepSubsciberReferenceAlive);
        }

        public virtual SubscriptionToken  Subscribe(Action<TPayload> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<TPayload>  filter)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            IDelegateReference filterReference;

            if (filter != null)
            {
                filterReference = new DelegateReference(filter, keepSubscriberReferenceAlive);
            }
            else
            {
                filterReference = new DelegateReference(new Predicate<TPayload>(delegate { return true; }), true);
            }

            EventSubscription<TPayload> subscription;

            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                    break;

                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundEventSubscription<TPayload>(actionReference, filterReference);
                    break;

                case ThreadOption.UIThread:
                    if (SynchronizationContext == null)
                        throw new InvalidOperationException("To use  the  UIThread  option for subscribing,  the EventAggregator  must be constructed  on  the  UI  thread");
                    subscription = new DispatchEventSubscription<TPayload>(actionReference, filterReference, SynchronizationContext);
                    break;

                default:
                    subscription = new EventSubscription<TPayload>(actionReference, filterReference);
                    break;

            }
            return base.InternalSubscribe(subscription);
        }

        public virtual void Unsubscribe(Action<TPayload> subscriber)
        {
            lock(Subscriptions)
            {
                IEventSubscription eventSubscription = Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);

                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }


        #endregion მეთოდები

    }
}
