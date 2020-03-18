using Ora.Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ora.Services.Configurable.Domain
{
    public abstract class BaseMongoEntity : BaseEntity, IActive
    {
        public bool IsActive { get; set; }
    }
}
