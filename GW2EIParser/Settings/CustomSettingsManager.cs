using System;
using System.Configuration;
using System.IO;

namespace GW2EIParser
{
    internal static class CustomSettingsManager
    {
        public static void ReadConfig(string filename)
        {
            var f = new FileInfo(filename);
            if (!f.Exists)
            {
                Console.WriteLine("Warning: settings file \"" + filename + "\" not found");
                return;
            }
            if (f.Extension == ".conf")
            {
                ReadConfFile(filename);
                return;
            }
            else
            {
                // old code, left for backwards compatibility
                Console.WriteLine("Warning: using of xml structure custom settings is deprecated. Please use a *.conf file instead.");
                ReadXmlFile(filename);
            }
        }

        private static void ReadXmlFile(string filename)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", filename);
        }

        private static void ReadConfFile(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    ProcessSettingsLine(line.Trim());
                }
            }
        }

        public static string DumpSettings()
        {
            string res = "";
            foreach (SettingsProperty key in Properties.Settings.Default.Properties)
            {
                res += key.Name + "=" + Properties.Settings.Default[key.Name] + "\n";
            }
            return res;
        }

        private static void ProcessSettingsLine(string line)
        {
            if (line.StartsWith("#"))
            {
                return; // commented out line
            }

            int equalsPos = line.IndexOf("=");
            if (equalsPos <= 0)
            {
                Console.WriteLine("Warning: invalid setting line \"" + line + "\"");
                return;
            }
            string name = line.Substring(0, equalsPos).Trim();
            string value = line.Substring(equalsPos + 1).Trim();
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Trim('\"');
            }
            SettingsProperty prop = Properties.Settings.Default.Properties[name];
            if (prop == null)
            {
                Console.WriteLine("Warning: Ignoring unknown setting \"" + name + "\"");
                return;
            }
            Type type = prop.PropertyType;
            if (type == typeof(bool))
            {
                if (bool.TryParse(value, out bool boolValue))
                {
                    Properties.Settings.Default[name] = boolValue;
                }
                else if (value == "0" || value == "1")
                {
                    Properties.Settings.Default[name] = value == "1";
                }
                else
                {
                    Console.WriteLine("Warning: Ignoring unreadable boolean value \"" + value + "\" for setting \"" + name + "\"");
                }
            }
            else if (type == typeof(string))
            {
                Properties.Settings.Default[name] = value;
            }
            else if (type == typeof(long))
            {
                if (long.TryParse(value, out long res))
                {
                    Properties.Settings.Default[name] = res;
                }
                else
                {
                    Console.WriteLine("Warning: Setting \"" + name + "\" expected type \"" + type.Name);
                    Properties.Settings.Default[name] = -1;
                }
            }
            else
            {
                Console.WriteLine("Warning: Setting \"" + name + "\" of type \"" + type.Name + "\" cannot be processed.");
            }
        }
    }
}
