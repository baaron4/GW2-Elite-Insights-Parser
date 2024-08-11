using System;
using System.Configuration;
using System.IO;
using GW2EIParserCommons;

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

        /// <summary>
        /// Returns current program settings
        /// </summary>
        /// <returns></returns>
        public static ProgramSettings GetProgramSettings()
        {
            var settings = new ProgramSettings()
            {
                SendEmbedToWebhook = Properties.Settings.Default.SendEmbedToWebhook,
                SendSimpleMessageToWebhook = Properties.Settings.Default.SendSimpleMessageToWebhook,
                WebhookURL = Properties.Settings.Default.WebhookURL,
                UploadToDPSReports = Properties.Settings.Default.UploadToDPSReports,
                DPSReportUserToken = Properties.Settings.Default.DPSReportUserToken,
                UploadToWingman = Properties.Settings.Default.UploadToWingman,
                SaveOutCSV = Properties.Settings.Default.SaveOutCSV,
                SaveOutHTML = Properties.Settings.Default.SaveOutHTML,
                SaveOutXML = Properties.Settings.Default.SaveOutXML,
                SaveOutJSON = Properties.Settings.Default.SaveOutJSON,
                SaveOutTrace = Properties.Settings.Default.SaveOutTrace,
                ParseMultipleLogs = Properties.Settings.Default.ParseMultipleLogs,
                SingleThreaded = Properties.Settings.Default.SingleThreaded,
                Anonymous = Properties.Settings.Default.Anonymous,
                SkipFailedTries = Properties.Settings.Default.SkipFailedTries,
                CustomTooShort = Properties.Settings.Default.CustomTooShort,
                DetailledWvW = Properties.Settings.Default.DetailledWvW,
                ParsePhases = Properties.Settings.Default.ParsePhases,
                ParseCombatReplay = Properties.Settings.Default.ParseCombatReplay,
                ComputeDamageModifiers = Properties.Settings.Default.ComputeDamageModifiers,
                SaveAtOut = Properties.Settings.Default.SaveAtOut,
                OutLocation = Properties.Settings.Default.OutLocation,
                AddDuration = Properties.Settings.Default.AddDuration,
                AddPoVProf = Properties.Settings.Default.AddPoVProf,
                LightTheme = Properties.Settings.Default.LightTheme,
                HtmlExternalScripts = Properties.Settings.Default.HtmlExternalScripts,
                HtmlExternalScriptsPath = Properties.Settings.Default.HtmlExternalScriptsPath,
                HtmlExternalScriptsCdn = Properties.Settings.Default.HtmlExternalScriptsCdn,
                HtmlCompressJson = Properties.Settings.Default.HtmlCompressJson,
                RawTimelineArrays = Properties.Settings.Default.RawTimelineArrays,
                CompressRaw = Properties.Settings.Default.CompressRaw,
                IndentJSON = Properties.Settings.Default.IndentJSON,
                IndentXML = Properties.Settings.Default.IndentXML,
                MemoryLimit = Properties.Settings.Default.MemoryLimit,
            };
            return settings;
        }
    }
}
