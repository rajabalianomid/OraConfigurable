using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Moq;
using Ora.Common.Caching;
using Ora.Common.Mongo;
using Ora.Services.Configurable;
using Ora.Services.Configurable.Data;
using Ora.Services.Configurable.Domain;
using Ora.Services.Configurable.Repository;
using Ora.Services.Configurable.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ora.Common.Commands.Test.Service
{
    public class ConfigServiceTest
    {
        [Fact]
        public async Task activity_service_add_async_should_succeed()
        {
            var cashManagerMock = new Mock<IStaticCacheManager>();
            var _runner = MongoDbRunner.Start();
            var _client = new MongoClient(_runner.ConnectionString);
            var _fakeDb = _client.GetDatabase("IntegrationTestDb");
            var mongoRepository = new MongoRepository<Setting>(_fakeDb);
            var activityService = new SettingService(mongoRepository, cashManagerMock.Object);
            await activityService.InsertSetting(new Setting
            {
                ApplicationName = "Ora",
                ChangeUTC = DateTime.UtcNow,
                CreateUTC = DateTime.UtcNow,
                Id = 1,
                IsActive = true,
                Name = "Omid",
                Type = "String",
                Value = "1"
            });
            Assert.True(mongoRepository.Table.FirstOrDefault().IsActive);
        }
    }
}
