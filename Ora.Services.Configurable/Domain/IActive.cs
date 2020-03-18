using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ora.Services.Configurable.Domain
{
    public interface IActive
    {
        bool IsActive { get; set; }
    }
}
