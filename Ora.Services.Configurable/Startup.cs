using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Ora.Common.Mongo;
using Ora.Services.Configurable.Data;
using Ora.Services.Configurable.Domain;
using Ora.Services.Configurable.Repository;
using Microsoft.AspNetCore.Builder;
using AutoMapper;
using Ora.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Autofac;
using Ora.Common.CommandsHandler;
using Ora.Services.Configurable.Handlers;
using Ora.Common.RabbitMq;
using Ora.Common.Commands;
using Ora.Services.Configurable.Services;
using Ora.Common.Caching;
using EasyCaching.Core;
using Ora.Common.Tasks;
using Ora.Services.Configurable.Tasks;

namespace Ora.Services.Configurable
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddAutoMapper(typeof(Startup));
            services.AddMongoDB(Configuration);
            services.AddTasks<TaskManager>(Configuration);
            services.AddInMemoryCash(Configuration);
            services.AddScoped<ICommandHandler<CreateSetting>, SettingCreateHandler>();
            services.AddScoped<ICommandHandler<RemoveSetting>, SettingRemoveHandler>();
            services.AddScoped<ICommandHandler<EditSetting>, SettingEditHandler>();
            services.AddScoped<IMongoRepository<Setting>, MongoRepository<Setting>>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IStaticCacheManager, MemoryCacheManager>();
            services.AddScoped<ILocker, MemoryCacheManager>();
            services.AddRabbitMq(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
