using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Common.Commands
{
    public class CommandNamespaceAttribute : Attribute
    {
        public string Namespace { get; }

        public CommandNamespaceAttribute(string _namespace)
        {
            Namespace = _namespace?.ToLowerInvariant();
        }
    }
}
