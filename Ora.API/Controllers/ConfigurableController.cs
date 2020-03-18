using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ora.API.Models;
using Ora.Common.Commands;
using RawRabbit;

namespace Ora.API.Controllers
{
    [Route("[controller]")]
    public class ConfigurableController : BaseController
    {
        IOptionsMonitor<EndpointSetting> _endpointSettings;
        public ConfigurableController(IBusClient busClient) : base(busClient)
        {
        }
        [HttpGet("")]
        public async Task<IActionResult> Get(string name)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add(ApplicationName, name);
            SetEndpoint(_endpointSettings.CurrentValue.Config);
            return Json(await MakeRequestAsync<List<ConfigModel>>(HTTPMethod.Get, route: EndpointMethod.Config, parameters));
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]CreateSetting command)
        {
            await BusClient.PublishAsync(command);
            return Accepted();

        }

        [HttpDelete("")]
        public async Task<IActionResult> Delete([FromBody]RemoveSetting command)
        {
            await BusClient.PublishAsync(command);
            return Accepted();
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody]EditSetting command)
        {
            await BusClient.PublishAsync(command);
            return Accepted();
        }
    }
}