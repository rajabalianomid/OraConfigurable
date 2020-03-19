using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Commands
{
    public class BaseCommane
    {
        public BaseCommane(string message = "Bad request") { this.Message = message; }
        public string Message { get; set; }
    }
}
