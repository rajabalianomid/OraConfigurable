using AutoMapper;
using Ora.Common.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Services.Configurable.Domain
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<CreateSetting, Setting>();
            CreateMap<RemoveSetting, Setting>();
            CreateMap<EditSetting, Setting>();
        }
    }
}
