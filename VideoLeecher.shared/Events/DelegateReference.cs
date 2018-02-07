using System;
using System.Reflection;

namespace VideoLeecher.shared.Events
{
    public class DelegateReference : IDelegateReference
    {
        #region ველები

        private readonly Delegate _delegate;
        private readonly Type _delegateType;
        private readonly MethodInfo _method;
        private readonly WeakReference _weakReference;


        #endregion ველები


        #region კონსტრუქტორები

        public  DelegateReference(Delegate @delegate, bool keepReferenceAlive)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            if (keepReferenceAlive)
            {
                _delegate = @delegate;
            }
            else
            {
                _weakReference = new WeakReference(@delegate.Target);
                _method = @delegate.GetMethodInfo();
                _delegateType = @delegate.GetType();
            }
        }

        #endregion კონსტრუქტორები

        #region  თვისებები

        public Delegate Target
        {

         get
            {
                if (_delegate != null)
                {
                    return _delegate;
                }
                else
                {
                    return TryGetDelegate();
                }
            }
        }

        #endregion თვისებები

        #region მეთოდები

        private Delegate TryGetDelegate()
        {
            if (_method.IsStatic)
            {
                return _method.CreateDelegate(_delegateType, null);
            }

            object target = _weakReference.Target;

            if (target != null)
            {
                return _method.CreateDelegate(_delegateType, target);
            }
            return null;
        }

        #endregion მეთოდები
    }
}
