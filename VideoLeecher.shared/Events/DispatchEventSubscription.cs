using System;
using System.Threading;

namespace VideoLeecher.shared.Events
{
   public class DispatchEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        #region ველები


        private readonly SynchronizationContext _syncContext;

        #endregion ველები

        #region კონსტრუქტორები

        public DispatchEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, SynchronizationContext syncContext)
            : base(actionReference, filterReference)
        {
            _syncContext = syncContext;
        }

        #endregion კონსტრუქტორები

        #region მეთოდები

        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            _syncContext.Post((o) => action((TPayload)o), argument);
        }


        #endregion მეთოდები

    }
}
