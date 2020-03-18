using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Tasks
{
    public interface ITaskManager
    {
        IServiceProvider ServiceProvider { get; set; }
        int Interval { get; set; }
        void Initialize();
        void Start();
    }
}
