using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ora.Web.Models
{
    public class ConfigModel
    {
        public enum ConfigType
        {
            String,
            Integer,
            Boolean,
            Double
        }
        public ConfigModel()
        {
            Type = ConfigType.String.ToString();
        }
        public int Id { get; set; }
        public string IdSt
        {
            get
            {
                return Id.ToString();
            }
            set
            {
                Id = int.Parse(value);
            }
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string ApplicationName { get; set; }
    }
}
