using Microsoft.Win32;
namespace EFTMap
{
    internal static class Config
    {
        public static readonly IConfigOption<int> MapIndex = new SelectedMapIndex();
        public static readonly IConfigOption<bool> AutoRemove = new AutoRemoveOption();
        public static readonly IConfigOption<bool> AutoSelect = new AutoSelectOption();
        public static readonly IConfigOption<Point> Location = new WindowLocation();

        public class SelectedMapIndex : ConfigOptionBase<int>
        {
            public override string Key => "SelectedIndex";
            public override int Default => 0;
        }

        public class AutoRemoveOption : ConfigOptionBase<bool>
        {
            public override string Key => "AutoRemove";
            public override bool Default => false;
        }

        public class AutoSelectOption : ConfigOptionBase<bool>
        {
            public override string Key => "AutoSelect";
            public override bool Default => false;
        }

        public class WindowLocation : IConfigOption<Point>
        {
            private const string XKey = "LocationX";
            private const string YKey = "LocationY";

            public string Key => "Location";
            public Point Default => new(100, 50);

            public Point Get()
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(ConfigOptionBase<Point>.RegistryPath);
                if (key != null)
                {
                    int x = (int)(key.GetValue(XKey) ?? Default.X);
                    int y = (int)(key.GetValue(YKey) ?? Default.Y);
                    return new Point(x, y);
                }
                return Default;
            }

            public void Save(Point value)
            {
                using RegistryKey key = Registry.CurrentUser.CreateSubKey(ConfigOptionBase<Point>.RegistryPath)!;
                key.SetValue(XKey, value.X, RegistryValueKind.DWord);
                key.SetValue(YKey, value.Y, RegistryValueKind.DWord);
            }
        }
    }
}
