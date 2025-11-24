using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OWON_GUI.Classes
{
    internal class CancellableTask
    {
        private readonly CancellationTokenSource _cts;
        public Task InnerTask { get; }

        public CancellableTask(Func<CancellationToken, Task> action, CancellationTokenSource? cts = null)
        {
            _cts = cts ?? new CancellationTokenSource();
            InnerTask = new Task(() => {
                
                action(_cts.Token).Wait();
                Debug.WriteLine("TASK FINITO");
                
                }, _cts.Token);
        }

        /// <summary>
        /// Requests cancellation of the associated Task.
        /// </summary>
        public void Cancel() => _cts.Cancel();
        public void Start() => InnerTask.Start();

        // la classe diventa automaticamente un Task
        public static implicit operator Task(CancellableTask ct)
            => ct.InnerTask;
    }
}
