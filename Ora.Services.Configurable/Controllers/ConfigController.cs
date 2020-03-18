using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Ora.Common.Commands;
using Ora.Services.Configurable.Domain;
using Ora.Services.Configurable.Services;

namespace Ora.Services.Configurable.Controllers
{
    [Route("[controller]")]
    public class ConfigController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        public ConfigController(ISettingService settingService, IMapper mapper)
        {
            _settingService = settingService;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Get(CreateSetting model)
        {
            return Json(_settingService.GetAllSettings(model.ApplicationName, name: model.Name, cache: true));
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]CreateSetting command)
        {
            if (ModelState.IsValid)
            {
                var setting = _mapper.Map<CreateSetting, Setting>(command);
                setting.IsActive = true;
                await _settingService.InsertSetting(setting);
                return Accepted();

            }
            return Content("Invalid parameter");
        }

        [HttpDelete("")]
        public async Task<IActionResult> Delete([FromBody]RemoveSetting command)
        {
            var setting = _mapper.Map<RemoveSetting, Setting>(command);
            setting.IsActive = false;
            await _settingService.UpdateSetting(setting);
            return Accepted();
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody]EditSetting command)
        {
            if (ModelState.IsValid)
            {
                var setting = _mapper.Map<EditSetting, Setting>(command);
                setting.IsActive = true;
                await _settingService.UpdateSetting(setting);
                return Accepted();
            }
            return Content("Invalid parameter");
        }
    }
}