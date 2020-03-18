using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ora.API.Models
{
    public class ConfigModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ApplicationName { get; set; }
    }
}
