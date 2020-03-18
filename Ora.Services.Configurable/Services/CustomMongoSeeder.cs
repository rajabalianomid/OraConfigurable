using MongoDB.Driver;
using Ora.Common.Mongo;
using Ora.Services.Configurable.Data;
using Ora.Services.Configurable.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ora.Services.Configurable.Services
{
    internal class CustomMongoSeeder : MongoSeeder
    {
        private readonly IMongoRepository<Setting> _settingRepository;

        public CustomMongoSeeder(IMongoDatabase database, IMongoRepository<Setting> settingRepository) : base(database)
        {
            _settingRepository = settingRepository;
        }

        protected override async Task CustomSeedAsync()
        {
            var settings = new List<Setting>
            {
                //new Setting {  ApplicationName = "SERVICE-A", IsActive = true,  Name = "SiteName", Type = "String", Value = "voidu.com"}
                //...
            };
            await Task.WhenAll(settings.Select(s => _settingRepository.Insert(s)));
        }
    }
}
