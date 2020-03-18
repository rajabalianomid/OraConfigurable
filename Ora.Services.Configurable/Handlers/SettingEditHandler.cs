using AutoMapper;
using MongoDB.Driver;
using Ora.Common.Commands;
using Ora.Common.CommandsHandler;
using Ora.Services.Configurable.Domain;
using Ora.Services.Configurable.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ora.Services.Configurable.Handlers
{
    public class SettingEditHandler : ICommandHandler<EditSetting>
    {
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;

        public SettingEditHandler(ISettingService settingService, IMapper mapper)
        {
            _settingService = settingService;
            _mapper = mapper;
        }

        public async Task HandleAsync(EditSetting command)
        {
            var setting = _mapper.Map<EditSetting, Setting>(command);
            setting.IsActive = true;
            await _settingService.UpdateSetting(setting);
        }
    }
}
