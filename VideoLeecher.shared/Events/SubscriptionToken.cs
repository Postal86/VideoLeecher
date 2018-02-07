using System;


namespace VideoLeecher.shared.Events
{
    public sealed class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        #region ველები

        private readonly Guid _token;
        private Action<SubscriptionToken> _unsubscribeAction;

        #endregion ველები

        #region კონსტრუქტორები

        public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;
            _token = Guid.NewGuid();
        }

        #endregion კონსტრუქტორები

        #region მეთოდები

        public void Dispose()
        {

            if (_unsubscribeAction != null)
            {
                _unsubscribeAction(this);
                _unsubscribeAction = null;
            }

            GC.SuppressFinalize(this); 
        }

        public bool Equals(SubscriptionToken other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(_token, other._token);
        }


        public override int GetHashCode()
        {
            return _token.GetHashCode();
        }

        #endregion მეთოდები
    }
}
