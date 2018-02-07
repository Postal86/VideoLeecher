using System;
using System.Threading.Tasks;

namespace VideoLeecher.shared.Events
{
    public class BackgroundEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        #region კონსტრუქტორები

        public BackgroundEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
             : base(actionReference, filterReference)
        {

        }

        #endregion კონსტრუქტორები

        #region მეთოდები

        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            Task.Run(() => action(argument));
        }

        #endregion მეთოდები
    }
}
