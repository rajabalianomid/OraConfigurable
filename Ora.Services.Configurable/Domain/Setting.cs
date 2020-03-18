using Ora.Common.Commands;
using Ora.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Services.Configurable.Domain
{
    public class Setting : BaseMongoEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ApplicationName { get; set; }
        public DateTime CreateUTC { get; set; }
        public DateTime ChangeUTC { get; set; }
    }
}
