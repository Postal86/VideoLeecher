using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoLeecher.shared.Commands
{
    public abstract class DelegateCommandBase : ICommand
    {
        #region Fields

        private readonly HashSet<string> _propertiesToObserve;
        private INotifyPropertyChanged _inpc;

        protected readonly Func<object, Task> _executeMethod;
        protected Func<object, bool> _canExecuteMethod;

        #endregion Fields

        #region Constructors

        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null ||  canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod), "Neither the executeMethod nor the canExecuteMethod delegates can be null");

            }

            _propertiesToObserve = new HashSet<string>();
            _executeMethod = (arg) => { executeMethod(arg); return Task.Delay(0);  }
            _canExecuteMethod = canExecuteMethod;
        }

        protected DelegateCommandBase(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod), "Neither the executeMethod  nor the canExecuteMethod delegates can be  null");
            }

            _propertiesToObserve = new HashSet<string>();
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }



        #endregion Constructors



    }
}
