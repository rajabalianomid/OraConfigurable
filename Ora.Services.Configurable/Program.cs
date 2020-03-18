using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Ora.Common.Commands;
using Ora.Common.Services;
//using Shared.Logging;

namespace Ora.Services.Configurable
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build();
            var build = CreateHostBuilder(args).Build();
            build.RabbitMQ().SubscribeToCommand<CreateSetting>(build);
            build.RabbitMQ().SubscribeToCommand<RemoveSetting>(build);
            build.RabbitMQ().SubscribeToCommand<EditSetting>(build);
            build.TaskBuilder();
            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            return host;
        }
    }
}
