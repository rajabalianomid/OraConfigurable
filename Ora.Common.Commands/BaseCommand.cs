using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Commands
{
    public class BaseCommand
    {
        public BaseCommand(string message = "Bad request") { this.Message = message; }
        public string Message { get; set; }
    }
}
