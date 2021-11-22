using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AviaExplorer.Services.Utils.Settings
{
    public class SettingsService : ISettingsService
    {
        #region Fields
        private readonly string _projectName = "Avia Explorer";
        private readonly string _supportNumber = "-";
        private readonly string _supportEmail = "-";
        private readonly string _defaultApiUrl = "http://map.aviasales.ru";
        private readonly string _defaultAppUpdateUrl = "";
        private readonly string _appCenterAndroidKey = "android=befa8f25-96c6-481a-85d7-0ef8eeb1f8a0;";
        private readonly string _appCenteriOSKey = "ios=c3f7f289-bfd0-485b-926e-12feb91751db;";
        #endregion

        #region Properties
        public string ProjectName =>
            GetValueOrDefault("ProjectName", _projectName);

        public string SupportNumber =>
            GetValueOrDefault("SupportNumber", _supportNumber);

        public string SupportEmail =>
            GetValueOrDefault("SupportEmail", _supportEmail);

        public string DefaultApiUrl =>
            GetValueOrDefault("DefaultApiUrl", _defaultApiUrl);

        public string DefaultAppUpdateUrl =>
            GetValueOrDefault("DefaultAppUpdateUrl", _defaultAppUpdateUrl);

        public string AppCenterAndroidKey =>
            GetValueOrDefault("AppCenterAndroidKey", _appCenterAndroidKey);

        public string AppCenteriOSKey =>
            GetValueOrDefault("AppCenteriOSKey", _appCenteriOSKey);
        #endregion

        #region Methods
        public bool ContainsKey(string key) =>
            Application.Current.Properties.ContainsKey(key);

        public Task AddOrUpdateValueAsync<T>(string key, T value, bool serialize = false)
        {
            if (serialize)
                Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
            else Application.Current.Properties[key] = value;

            try
            {
                return Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                return Task.CompletedTask;
            }
        }

        public Task AddOrUpdateValuesAsync<T>(Dictionary<string, T> keyValues, bool serialize = false)
        {
            if (keyValues == null) return Task.CompletedTask;

            foreach (var item in keyValues)
            {
                var key = item.Key;
                var value = item.Value;

                if (serialize)
                    Application.Current.Properties[key] = JsonConvert.SerializeObject(value);
                else Application.Current.Properties[key] = value;
            }

            try
            {
                return Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to save or update values within this dict" + keyValues,
                    " Message: " + ex.Message);
                return Task.CompletedTask;
            }
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default, bool deserialize = false)
        {
            T value = defaultValue;
            if (Application.Current.Properties.ContainsKey(key))
            {
                var data = Application.Current.Properties[key];
                if (deserialize) value = JsonConvert.DeserializeObject<T>((string)data);
                else value = (T)data;
            }
            return value;
        }

        public T GetApplicationResourceOrDefault<T>(string key, T defaultValue = default)
        {
            T value = defaultValue;
            if (Application.Current.Resources.ContainsKey(key))
                value = (T)Application.Current.Resources[key];
            return null != value ? value : defaultValue;
        }

        public List<T> GetValuesOrDefaults<T>(string[] keys, T defaultValue = default, bool deserialize = false)
        {
            var list = new List<T>();
            for (var i = 0; i < keys.Length; i++)
                list.Add(GetValueOrDefault(keys[i], defaultValue, deserialize));
            return list;
        }

        public Task DetermineAndSetDefaultsAsync()
        {
            if (!GetValueOrDefault<bool>("DefaultsSet"))
                return SetDefaultsAsync();
            return Task.CompletedTask;
        }

        private Task SetDefaultsAsync()
        {
            Application.Current.Properties.Clear();
            return AddOrUpdateValuesAsync(new Dictionary<string, string>
            {
                { nameof(ProjectName), _projectName },
                { nameof(SupportNumber), _supportNumber },
                { nameof(SupportEmail), _supportEmail },
                { nameof(DefaultApiUrl), _defaultApiUrl },
                { nameof(DefaultAppUpdateUrl), _defaultAppUpdateUrl },
                { nameof(AppCenterAndroidKey), _appCenterAndroidKey },
                { nameof(AppCenteriOSKey), _appCenteriOSKey },
            }).ContinueWith(t => AddOrUpdateValueAsync("DefaultsSet", true),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task RemoveValueAsync(string key)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                Application.Current.Properties.Remove(key);

                try
                {
                    return Application.Current.SavePropertiesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to save: " + key, " Message: " + ex.Message);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }

        public Task RemoveValuesAsync(string[] keys)
        {
            if (keys == null) return Task.CompletedTask;

            foreach (var item in keys)
            {
                if (Application.Current.Properties.ContainsKey(item))
                    Application.Current.Properties.Remove(item);
            }

            try
            {
                return Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unable to save or update values within this list " + keys,
                    " Message: " + ex.Message);
                return Task.CompletedTask;
            }
        }
        #endregion
    }
}