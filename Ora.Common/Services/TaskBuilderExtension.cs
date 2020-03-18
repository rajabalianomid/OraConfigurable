using Microsoft.Extensions.Hosting;
using Ora.Common.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Services
{
    public static class TaskBuilderExtension
    {
        public static void TaskBuilder(this IHost host)
        {
            var handler = (ITaskManager)host.Services.GetService(typeof(ITaskManager));

            handler.Initialize();
            handler.Start();
        }
    }
}
