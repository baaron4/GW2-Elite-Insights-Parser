using LuckParser.Properties;

namespace LuckParser.Models.DataModels
{
    class SettingsContainer
    {
        public readonly bool ShowEstimates;
        public readonly bool ParsePhases;
        public readonly bool LightTheme;
        public readonly bool ParseCombatReplay;
        public readonly int PollingRate = 150;
        public readonly bool UploadToDPSReports;
        public readonly bool UploadToDPSReportsRH;
        public readonly bool UploadToRaidar;
        public readonly bool IndentJSON;
        public readonly bool IndentXML;

        public SettingsContainer(Settings settings)
        {
            ShowEstimates = settings.ShowEstimates;
            ParsePhases = settings.ParsePhases;
            LightTheme = settings.LightTheme;
            ParseCombatReplay = settings.ParseCombatReplay;
            UploadToDPSReports = settings.UploadToDPSReports;
            UploadToDPSReportsRH = settings.UploadToDPSReportsRH;
            UploadToRaidar = settings.UploadToRaidar;

            IndentJSON = settings.IndentJSON;
            IndentXML = settings.IndentXML;
        }
    }
}
