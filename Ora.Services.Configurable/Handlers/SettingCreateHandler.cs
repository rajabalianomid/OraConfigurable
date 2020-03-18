using AutoMapper;
using Microsoft.Extensions.Logging;
using Ora.Common.Commands;
using Ora.Common.CommandsHandler;
using Ora.Services.Configurable.Domain;
using Ora.Services.Configurable.Services;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ora.Services.Configurable.Handlers
{
    public class SettingCreateHandler : ICommandHandler<CreateSetting>
    {
        private readonly IMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly IBusClient _busClient;
        public SettingCreateHandler(ISettingService settingService, IMapper mapper, IBusClient busClient)
        {
            _settingService = settingService;
            _mapper = mapper;
            _busClient = busClient;
        }

        public async Task HandleAsync(CreateSetting command)
        {
            var setting = _mapper.Map<CreateSetting, Setting>(command);
            setting.IsActive = true;
            await _settingService.InsertSetting(setting);
        }
    }
}
