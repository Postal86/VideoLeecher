using System;
using System.Threading;
using System.Threading.Tasks;

namespace VideoLeecher.core.Models
{
   public  class DownloadTask
   {
        public DownloadTask(Task  task, Task continueTask, CancellationTokenSource cancellationTokenSource)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));
            ContinueTask = continueTask ?? throw new ArgumentNullException(nameof(continueTask));
            CancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        public Task Task { get; private set; }

        public Task ContinueTask { get; private set; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }

   }
}
