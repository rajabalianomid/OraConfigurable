using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Tasks
{
    public static class Extensions
    {
        public static void AddTasks<T>(this IServiceCollection services, IConfiguration configuration) where T : class, ITaskManager, new()
        {
            services.Configure<TaskOptions>(configuration.GetSection("tasks"));

            services.AddSingleton<ITaskManager, T>(s =>
             {
                 var options = s.GetService<IOptions<TaskOptions>>();
                 var task = new T();
                 task.Interval = options.Value.Interval;
                 task.ServiceProvider = s;
                 task.Initialize();
                 task.Start();
                 return task;
             });
        }
    }
}
