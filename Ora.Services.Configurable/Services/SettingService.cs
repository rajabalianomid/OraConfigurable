using Ora.Common.Caching;
using Ora.Common.Services;
using Ora.Services.Configurable.Configuration;
using Ora.Services.Configurable.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Ora.Common;
using System.Reflection;
using Ora.Services.Configurable.Data;

namespace Ora.Services.Configurable.Services
{
    public class SettingService : ISettingService
    {
        #region Fields

        private readonly IMongoRepository<Setting> _settingRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public SettingService(IMongoRepository<Setting> settingRepository,
            IStaticCacheManager cacheManager)
        {
            _settingRepository = settingRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// Setting (for caching)
        /// </summary>
        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public bool IsActive { get; set; }
            public string ApplicationName { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Settings</returns>
        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached(string applicationName)
        {
            //cache
            return _cacheManager.Get(applicationName, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = _settingRepository.Table.OrderBy(o => o.Name).ThenBy(o => o.Id).Select(s => s);
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.ApplicationName.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        Type = s.Type,
                        ApplicationName = s.ApplicationName,
                        IsActive = s.IsActive
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="applicationName">ApplicationName identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        protected virtual async Task SetSetting(Type type, string key, object value, string applicationName = null, bool clearCache = true)
        {
            if (applicationName == null)
                throw new ArgumentNullException(nameof(applicationName));
            applicationName = applicationName.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached(applicationName);
            var settingForCaching = allSettings.ContainsKey(applicationName) ?
                allSettings[applicationName].FirstOrDefault(x => x.Name == key) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = await GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                await UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    ApplicationName = applicationName,
                    IsActive = true,
                    Type = type.GetType().ToString()
                };
                await InsertSetting(setting, clearCache);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual async Task InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            var x = GetAllSettingsCached(setting.ApplicationName);

            setting.CreateUTC = DateTime.UtcNow;
            setting.ChangeUTC = DateTime.UtcNow;
            await _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPrefix(setting.ApplicationName);

        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual async Task UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            setting.ChangeUTC = DateTime.UtcNow;
            await _settingRepository.Update(setting);

            //cache
            if (clearCache)
                _cacheManager.RemoveByPrefix(setting.ApplicationName);

        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _settingRepository.Delete(setting);

            //cache
            _cacheManager.RemoveByPrefix(setting.ApplicationName);

        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual async Task<Setting> GetSettingById(int settingId)
        {
            if (settingId == 0)
                return null;

            return await _settingRepository.GetById(settingId);
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="applicationName">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting</returns>
        public virtual async Task<Setting> GetSetting(string key, string applicationName = null, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(applicationName))
                return null;

            var settings = GetAllSettingsCached(applicationName);
            applicationName = applicationName.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(applicationName))
                return null;

            var settingsByKey = settings[applicationName];
            var setting = settingsByKey.FirstOrDefault(x => x.Name == key);

            //load shared value?
            if (setting == null && key != null && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.Name == null);

            return setting != null ? await GetSettingById(setting.Id) : null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="applicationName">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), string applicationName = null, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(applicationName))
                return defaultValue;

            var settings = GetAllSettingsCached(applicationName);
            applicationName = applicationName.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(applicationName))
                return defaultValue;

            var settingsByKey = settings[applicationName];
            var setting = settingsByKey.FirstOrDefault(x => x.Name == key);

            //load shared value?
            if (setting == null && key != null && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.Name == null);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="applicationName">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual async Task SetSetting<T>(string key, T value, string applicationName = null, bool clearCache = true)
        {
            await SetSetting(typeof(T), key, value, applicationName, clearCache);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Settings</returns>
        public virtual IList<Setting> GetAllSettings(string applicationName, string name = null, bool cache = false)
        {
            applicationName = applicationName.Trim().ToLowerInvariant();
            if (applicationName != null && (cache))
            {
                IEnumerable<SettingForCaching> settingForCachings = null;
                var all = GetAllSettingsCached(applicationName);
                if (applicationName == null)
                    settingForCachings = all.SelectMany(s => s.Value);
                else if (all.Any() && all.ContainsKey(applicationName))
                    settingForCachings = all[applicationName];

                return settingForCachings == null ? new List<Setting>() : settingForCachings.Select(s => new Setting
                {
                    ApplicationName = s.ApplicationName,
                    Id = s.Id,
                    IsActive = s.IsActive,
                    Name = s.Name,
                    Type = s.Type,
                    Value = s.Value
                }).Where(w => string.IsNullOrEmpty(name) || w.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            var query = _settingRepository.Table.OrderBy(o => o.Name).ThenBy(t => t.ApplicationName).AsQueryable();

            if (applicationName != null)
                query = query.Where(w => w.ApplicationName == applicationName && (string.IsNullOrEmpty(name) || w.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)));

            var settings = query.ToList();
            return settings;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="applicationName">Store identifier</param>
        /// <returns>true -setting exists; false - does not exist</returns>
        public virtual bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, string applicationName = null) where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = GetSettingByKey<string>(key, applicationName: applicationName);
            return setting != null;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="applicationName">Store identifier for which settings should be loaded</param>
        public virtual T LoadSetting<T>(string applicationName = null) where T : ISettings, new()
        {
            return (T)LoadSetting(typeof(T), applicationName);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="applicationName">Store identifier for which settings should be loaded</param>
        public virtual ISettings LoadSetting(Type type, string applicationName = null)
        {
            var settings = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, applicationName: applicationName, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="applicationName">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual async Task SaveSetting<T>(T settings, string applicationName = null) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    await SetSetting(prop.PropertyType, key, value, applicationName, false);
                else
                    await SetSetting(key, string.Empty, applicationName, false);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="applicationName">Application Name</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual async Task SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, string applicationName = null, bool clearCache = true) where T : ISettings, new()
        {
            if (!(keySelector.Body is MemberExpression member))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                await SetSetting(key, value, applicationName, clearCache);
            else
                await SetSetting(key, string.Empty, applicationName, clearCache);
        }

        /// <summary>
        /// Save settings object (per store). If the setting is not overridden per store then it'll be delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForStore">A value indicating whether to setting is overridden in some store</param>
        /// <param name="applicationName">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual async Task SaveSettingOverridablePerStore<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore, string applicationName = null, bool clearCache = true) where T : ISettings, new()
        {
            if (overrideForStore || applicationName == null)
                await SaveSetting(settings, keySelector, applicationName, clearCache);
            else if (applicationName != null)
                DeleteSetting(settings, keySelector, applicationName);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public virtual void DeleteSetting<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = GetAllSettings(null);
            foreach (var prop in typeof(T).GetProperties())
            {
                var key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            settingsToDelete.ForEach(f => DeleteSetting(f));
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="applicationName">Store ID</param>
        public virtual void DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, string applicationName = null) where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = GetAllSettingsCached(applicationName);
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[applicationName].FirstOrDefault(x => x.Name == key) : null;
            if (settingForCaching == null)
                return;

            //update
            var setting = GetSettingById(settingForCaching.Id).Result;
            DeleteSetting(setting);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.Clear();
        }

        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="TSettings">Type of settings</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public virtual string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector) where TSettings : ISettings, new()
        {
            if (!(keySelector.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = $"{typeof(TSettings).Name}.{propInfo.Name}";

            return key;
        }

        public async Task ResolveSetting()
        {
            await Task.Run(() =>
            {
                if (_settingRepository.TableAll.Any())
                {
                    var result = _settingRepository.TableAll.GroupBy(g => new Setting { ApplicationName = g.ApplicationName, Name = g.Name })
                    .Where(w => w.Count() > 2)
                    .Select(s => s.Key)
                    .ToList();
                    foreach (var item in result)
                    {
                        var found = _settingRepository.TableAll.Where(w => w.Name == item.Name && w.ApplicationName == item.ApplicationName).ToList();
                        var forremoves = found.OrderBy(o => o.ChangeUTC).Take(found.Count - 1);
                        foreach (var remove in forremoves)
                        {
                            DeleteSetting(remove);
                        }
                    }
                    _cacheManager.Clear();
                }
            });
        }

        #endregion

        bool DbIsAvailable => _settingRepository.DbIsAvailable;
    }
}
