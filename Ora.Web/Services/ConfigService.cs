using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ora.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Ora.Web.Services
{
    public class ConfigService : BaseServices
    {
        IOptionsMonitor<EndpointSetting> _endpointSettings;
        public ConfigService(IOptionsMonitor<EndpointSetting> endpointSettings)
        {
            _endpointSettings = endpointSettings;
        }
        public async Task<List<ConfigModel>> GetAllAsync(string applicationName = "General")
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ApplicationName", applicationName);
            SetEndpoint(_endpointSettings.CurrentValue.Config);
            return await MakeRequestAsync<List<ConfigModel>>(HTTPMethod.Get, route: EndpointMethod.Config, parameters);
        }
        public async Task<List<ConfigModel>> GetByNameAsync(string applicationName, string name)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ApplicationName", applicationName);
            parameters.Add("Name", name);
            SetEndpoint(_endpointSettings.CurrentValue.Config);
            return await MakeRequestAsync<List<ConfigModel>>(HTTPMethod.Get, route: EndpointMethod.Config, parameters);
        }
        public async Task Disable(ConfigModel configModel)
        {
            SetEndpoint(_endpointSettings.CurrentValue.Config);
            await MakeRequestAsync<List<ConfigModel>, ConfigModel>(HTTPMethod.Delete, configModel, EndpointMethod.Config);
        }
        public async Task Add(ConfigModel configModel)
        {
            SetEndpoint(_endpointSettings.CurrentValue.Config);
            await MakeRequestAsync<List<ConfigModel>, ConfigModel>(HTTPMethod.Post, configModel, EndpointMethod.Config);
        }
        public async Task Edit(ConfigModel configModel)
        {
            SetEndpoint(_endpointSettings.CurrentValue.Config);
            await MakeRequestAsync<List<ConfigModel>, ConfigModel>(HTTPMethod.Put, configModel, EndpointMethod.Config);
        }
    }
}
