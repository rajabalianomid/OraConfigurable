using AutoMapper;
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
    public class SettingRemoveHandler : ICommandHandler<RemoveSetting>
    {
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;

        public SettingRemoveHandler(ISettingService settingService, IMapper mapper)
        {
            _settingService = settingService;
            _mapper = mapper;
        }

        public async Task HandleAsync(RemoveSetting command)
        {
            var setting = _mapper.Map<RemoveSetting, Setting>(command);
            setting.IsActive = false;
            await _settingService.UpdateSetting(setting);
        }
    }
}
