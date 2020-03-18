using System;
using System.Collections.Generic;
using System.Text;

namespace Ora.Services.Configurable.Configuration
{
    public class OraConfigurationDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string SettingsAllCacheKey => "Ora.setting.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SettingsPrefixCacheKey => "Ora.setting.";
    }
}
