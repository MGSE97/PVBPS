using System;
using System.Linq;
using Microsoft.Win32;

namespace KeyLogger
{
    public static class RegistryManager
    {
        public static bool HasKey(string key, string name)
        {
            return Registry.GetValue(key, name, null) != null;
        }

        public static T ReadKey<T>(string key, string name)
        {
            if (HasKey(key, name))
                return (T) Registry.GetValue(key, name, default(T));
            return default;
        }

        public static void WriteKey<T>(string key, string name, T value)
        {
            Registry.SetValue(key, name, value);
        }

        public static void RemoveKey(string key, string name)
        {
            if (HasKey(key, name))
            {
                var parts = key.Split('\\');
                var subKey = string.Join('\\', parts.Skip(1));
                var handle = parts[0] switch
                {
                    "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                    "HKEY_CURRENT_USER" => Registry.CurrentUser,
                    _ => null
                };

                if (handle == null || string.IsNullOrWhiteSpace(subKey))
                    return;

                try
                {
                    var regKey = handle.OpenSubKey(subKey, true);
                    regKey?.DeleteValue(name, false);
                }
                finally
                {
                }
            }
        }
    }
}