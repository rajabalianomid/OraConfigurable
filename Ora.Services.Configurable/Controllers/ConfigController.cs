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
            try
            {
                return Json(_settingService.GetAllSettings(model.ApplicationName, name: model.Name, cache: true));
            }
            catch (Exception)
            {
                return Json(new BaseCommane());
            }

        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]CreateSetting command)
        {
            var result = new BaseCommane();
            try
            {
                if (ModelState.IsValid)
                {
                    var setting = _mapper.Map<CreateSetting, Setting>(command);
                    setting.IsActive = true;
                    await _settingService.InsertSetting(setting);
                    result.Message = "Success";
                }
            }
            catch (Exception) { }
            return Json(result);
        }

        [HttpDelete("")]
        public async Task<IActionResult> Delete([FromBody]RemoveSetting command)
        {
            var result = new BaseCommane();
            try
            {
                var setting = _mapper.Map<RemoveSetting, Setting>(command);
                setting.IsActive = false;
                await _settingService.UpdateSetting(setting);
                result.Message = "Success";
            }
            catch (Exception) { }
            return Json(result);
        }

        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody]EditSetting command)
        {
            var result = new BaseCommane();
            try
            {
                if (ModelState.IsValid)
                {
                    var setting = _mapper.Map<EditSetting, Setting>(command);
                    setting.IsActive = true;
                    await _settingService.UpdateSetting(setting);
                    result.Message = "Success";
                }
            }
            catch (Exception) { }
            return Json(result);
        }
    }
}