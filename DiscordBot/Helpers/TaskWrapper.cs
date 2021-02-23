using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiscordBot.Helpers
{
    public class TaskWrapper
    {
        #region Fields

        private readonly Func<Task> taskToRun;

        #endregion

        #region Constructors

        public TaskWrapper(Func<Task> taskToRun)
        {
            this.taskToRun = taskToRun ?? throw new ArgumentNullException(nameof(taskToRun));
        }

        #endregion

        #region Instance Methods

        public async Task Start()
        {
            await taskToRun.Invoke()
                           .ContinueWith(t =>
                                         {
                                             Debug.WriteLine($"{t.Exception}");
                                         },
                                         TaskContinuationOptions.OnlyOnFaulted);
        }

        #endregion
    }
}