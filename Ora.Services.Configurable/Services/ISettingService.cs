using Ora.Common.Services;
using Ora.Services.Configurable.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ora.Services.Configurable.Services
{
    /// <summary>
    /// Setting service interface
    /// </summary>
    public partial interface ISettingService
    {
        Task InsertSetting(Setting setting, bool clearCache = true);
        Task UpdateSetting(Setting setting, bool clearCache = true);
        Task ResolveSetting();
        void DeleteSetting(Setting setting);
        Task<Setting> GetSettingById(int settingId);
        Task<Setting> GetSetting(string key, string applicationName = null, bool loadSharedValueIfNotFound = false);
        T GetSettingByKey<T>(string key, T defaultValue = default(T), string applicationName = null, bool loadSharedValueIfNotFound = false);
        Task SetSetting<T>(string key, T value, string applicationName = null, bool clearCache = true);
        IList<Setting> GetAllSettings(string applicationName = null, string name = null, bool cache = false);
        bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, string applicationName = null) where T : ISettings, new();
        T LoadSetting<T>(string applicationName = null) where T : ISettings, new();
        ISettings LoadSetting(Type type, string applicationName = null);
        Task SaveSetting<T>(T settings, string applicationName = null) where T : ISettings, new();
        Task SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, string applicationName = null, bool clearCache = true) where T : ISettings, new();
        Task SaveSettingOverridablePerStore<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore, string applicationName = null, bool clearCache = true) where T : ISettings, new();
        void DeleteSetting<T>() where T : ISettings, new();
        void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, string applicationName = null) where T : ISettings, new();
        void ClearCache();
        string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector) where TSettings : ISettings, new();
    }
}
