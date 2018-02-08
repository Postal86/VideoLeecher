using System;
using System.Collections.Generic;


namespace VideoLeecher.shared.Events
{
    public class EmptyPresentationEvent : EventBase
    {
        #region ველები

        private readonly PubSubEvent<object> _innerEvent;
        private readonly Dictionary<Action, Action<object>> _subscriberActions;

        #endregion ველები


        #region კონსტრუქტორები

        public EmptyPresentationEvent()
        {
            _innerEvent = new PubSubEvent<object>();
            _subscriberActions = new Dictionary<Action, Action<object>>();
        }

        #endregion კონსტრუქტორები

        #region მეთოდები

        public void Publish()
        {
            _innerEvent.Publish(null);
        }

        public void Subscribe(Action  action)
        {
            Subscribe(action, false);
        }

        public void Subscribe(Action action, bool keepSubscriberReferenceAlive)
        {
            Subscribe(action, ThreadOption.PublisherThread, keepSubscriberReferenceAlive);
        }

        public  void Subscribe(Action action, ThreadOption  threadOption)
        {
            Subscribe(action, threadOption, false);
        }

        public void Subscribe(Action action, ThreadOption threadOption,  bool keepSubscriberReferenceAlive)
        {
            Action<object> wrapperAction = o => action();
            _subscriberActions.Add(action, wrapperAction);
            _innerEvent.Subscribe(wrapperAction, threadOption , keepSubscriberReferenceAlive );
        }

        public void Unsubscribe(Action action)
        {
            if (!_subscriberActions.ContainsKey(action))
            {
                return;
            }

            Action<object> wrapperActionToUnsubscribe = _subscriberActions[action];
            _innerEvent.Unsubscribe(wrapperActionToUnsubscribe);
            _subscriberActions.Remove(action);

        }


        #endregion მეთოდები

    }
}
