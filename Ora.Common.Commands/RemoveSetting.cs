using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Commands
{
    public class RemoveSetting : ICommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ApplicationName { get; set; }
    }
}
