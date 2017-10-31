using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posttrack.BLL.Tests
{
    /// <summary>
    ///     TaskScheduker for executing tasks on the same thread that calls RunTasksUntilIdle() or RunPendingTasks()
    /// </summary>
    public class DeterministicTaskScheduler : TaskScheduler
    {
        private readonly List<Task> scheduledTasks = new List<Task>();

        /// <summary>
        ///     Executes the scheduled Tasks synchronously on the current thread.
        ///     If those tasks schedule new tasks they will also be executed
        ///     until no pending tasks are left.
        /// </summary>
        public void RunTasksUntilIdle()
        {
            while (scheduledTasks.Any())
            {
                RunPendingTasks();
            }
        }

        /// <summary>
        ///     Executes the scheduled Tasks synchronously on the current thread.
        ///     If those tasks schedule new tasks they will only be executed
        ///     with the next call to RunTasksUntilIdle() or RunPendingTasks().
        /// </summary>
        public void RunPendingTasks()
        {
            foreach (var task in scheduledTasks.ToArray())
            {
                TryExecuteTask(task);
                scheduledTasks.Remove(task);
            }
        }

        #region TaskScheduler methods

        protected override void QueueTask(Task task)
        {
            scheduledTasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            scheduledTasks.Add(task);
            return false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return scheduledTasks;
        }

        public override int MaximumConcurrencyLevel
        {
            get { return 1; }
        }

        #endregion
    }
}