using Ora.Common.Tasks;
using Ora.Services.Configurable.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

namespace Ora.Services.Configurable.Tasks
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    public partial class TaskManager : ITaskManager
    {
        #region Fields

        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();
        public IServiceProvider ServiceProvider { get; set; }
        public int Interval { get; set; }

        #endregion


        #region Methods

        /// <summary>
        /// Initializes the task manager
        /// </summary>
        public void Initialize()
        {
            _taskThreads.Clear();

            var taskThread = new TaskThread(Run)
            {
                Seconds = Interval
            };

            taskThread.InitSeconds = Interval;

            _taskThreads.Add(taskThread);
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in _taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in _taskThreads)
            {
                taskThread.Dispose();
            }
        }

        public void Run()
        {
            var _settingService = ServiceProvider.GetService<ISettingService>();
            _settingService.ResolveSetting();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads => new ReadOnlyCollection<TaskThread>(_taskThreads);


        #endregion
    }
}
