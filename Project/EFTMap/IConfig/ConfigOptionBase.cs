using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTMap
{
    public abstract class ConfigOptionBase<T> : IConfigOption<T>
    {
        // 컴퓨터\HKEY_CURRENT_USER\SOFTWARE\Untitled\Map
        internal const string RegistryPath = @"Software\Untitled\Map";

        public abstract string Key { get; }
        public abstract T Default { get; }

        public virtual T Get()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            if (key != null)
            {
                object? value = key.GetValue(Key);
                if (value != null)
                {
                    try
                    {
                        if (typeof(T) == typeof(bool))
                            return (T)(object)((int)value != 0);
                        return (T)Convert.ChangeType(value, typeof(T))!;
                    }
                    catch { }
                }
            }
            return Default;
        }

        public virtual void Save(T value)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath)!;
            object regValue = value switch
            {
                bool b => b ? 1 : 0,
                _ => value!
            };

            RegistryValueKind kind = value switch
            {
                int => RegistryValueKind.DWord,
                bool => RegistryValueKind.DWord,
                string => RegistryValueKind.String,
                _ => RegistryValueKind.String
            };

            key.SetValue(Key, regValue, kind);
        }
    }



    public interface IConfigOption<T>
    {
        string Key { get; }
        T Default { get; }
        T Get();
        void Save(T value);
    }
}
