namespace VideoLeecher.shared.Events
{
    public interface IEventAggregator
    {
        #region მეთოდები

        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();

        #endregion მეთოდები
    }
}
