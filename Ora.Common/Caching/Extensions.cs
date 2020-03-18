using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Caching
{
    public static class Extensions
    {
        public static void AddInMemoryCash(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEasyCaching(options =>
            {
                //use memory cache
                options.UseInMemory(configuration, "default", "easycahing:inmemory");
            });
        }
    }
}
