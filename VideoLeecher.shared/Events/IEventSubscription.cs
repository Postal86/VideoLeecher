using System;


namespace VideoLeecher.shared.Events
{
    public interface IEventSubscription
    {
        #region თვისებები

        SubscriptionToken SubscriptionToken { get; set; }

        #endregion თვისებები

        #region მეთოდები

        Action<object[]> GetExecutionStrategy();

        #endregion მეთოდები


    }
}
