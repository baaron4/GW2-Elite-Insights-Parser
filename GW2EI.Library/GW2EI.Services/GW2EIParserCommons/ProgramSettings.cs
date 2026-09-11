using GW2EIEvtcParser;
using GW2EIParserCommons.Properties;

namespace GW2EIParserCommons;

public class ProgramSettings
{
    // Upload
    public bool SendEmbedToWebhook { get; set; } = false;
    public bool SendSimpleMessageToWebhook { get; set; } = false;
    public string? WebhookURL { get; set; }
    public bool UploadToDPSReports { get; set; } = false;
    public string? DPSReportUserToken { get; set; }
    public bool UploadToWingman { get; set; } = false;
#if DEBUG
#pragma warning disable CA1822 // Mark members as static
    public bool UploadToWingmanInternal => false;
#pragma warning restore CA1822 // Mark members as static
#else
    public bool UploadToWingmanInternal => UploadToWingman;
#endif
    // Format
    public bool SaveOutCSV { get; set; } = false;
    public bool SaveOutHTML { get; set; } = true;
    public bool SaveOutJSON { get; set; } = false;
    public bool SaveOutTrace { get; set; } = false;
    // Multi threading
    public bool ParseMultipleLogs { get; set; } = false;
    public bool SingleThreaded { get; set; } = false;
    // Parsing
    public bool Anonymous { get; set; } = false;
    public bool SkipFailedTries { get; set; } = false;
    public long CustomTooShort { get; set; } = ParserHelper.MinimumInCombatDuration;
    public int CustomTooBig { get; set; } = 400;
    public bool DetailledWvW { get; set; } = false;
    public bool ComputePhases { get; set; } = true;
    public bool ComputeCombatReplay { get; set; } = true;
    public bool ComputeDamageModifiers { get; set; } = true;
    public bool ParseExtensions { get; set; } = true;
    public bool ComputeDamage { get; set; } = true;
    public bool ComputeBuff { get; set; } = true;
    public bool ComputeCast { get; set; } = true;
    public bool ComputeMechanics { get; set; } = true;
    // Save Location
    public bool SaveAtOut { get; set; }
    public string? OutLocation { get; set; }
    // Output
    public bool AddDuration { get; set; } = false;
    public bool AddPoVProf { get; set; } = false;
    // HTML
    public bool LightTheme { get; set; } = false;
    public bool HtmlExternalScripts { get; set; } = false;
    public string? HtmlExternalScriptsPath { get; set; }
    public string? HtmlExternalScriptsCdn { get; set; }
    public bool HtmlCompressJson { get; set; } = false;
    // JSON
    public bool RawTimelineArrays { get; set; } = true;
    public bool CompressRaw { get; set; } = false;
    public bool IndentJSON { get; set; } = false;
    // Other
    public int MemoryLimit { get; set; } = 0;
    public ProgramSettings()
    {

    }
    public ProgramSettings(Settings settings)
    {
        FromSettings(settings);
    }

    public void FromSettings(Settings settings)
    {
        SendEmbedToWebhook = settings.SendEmbedToWebhook;
        SendSimpleMessageToWebhook = settings.SendSimpleMessageToWebhook;
        WebhookURL = settings.WebhookURL;
        UploadToDPSReports = settings.UploadToDPSReports;
        DPSReportUserToken = settings.DPSReportUserToken;
        UploadToWingman = settings.UploadToWingman;
        SaveOutCSV = settings.SaveOutCSV;
        SaveOutHTML = settings.SaveOutHTML;
        SaveOutJSON = settings.SaveOutJSON;
        SaveOutTrace = settings.SaveOutTrace;
        ParseMultipleLogs = settings.ParseMultipleLogs;
        SingleThreaded = settings.SingleThreaded;
        Anonymous = settings.Anonymous;
        SkipFailedTries = settings.SkipFailedTries;
        CustomTooShort = settings.CustomTooShort;
        CustomTooBig = settings.CustomTooBig;
        DetailledWvW = settings.DetailledWvW;
        ComputePhases = settings.ParsePhases;
        ComputeCombatReplay = settings.ParseCombatReplay;
        ComputeDamageModifiers = settings.ComputeDamageModifiers;
        ParseExtensions = settings.ParseExtensions;
        ComputeBuff = settings.ComputeBuff;
        ComputeCast = settings.ComputeCast;
        ComputeDamage = settings.ComputeDamage;
        ComputeMechanics = settings.ComputeMechanics;
        SaveAtOut = settings.SaveAtOut;
        OutLocation = settings.OutLocation;
        AddDuration = settings.AddDuration;
        AddPoVProf = settings.AddPoVProf;
        LightTheme = settings.LightTheme;
        HtmlExternalScripts = settings.HtmlExternalScripts;
        HtmlExternalScriptsPath = settings.HtmlExternalScriptsPath;
        HtmlExternalScriptsCdn = settings.HtmlExternalScriptsCdn;
        HtmlCompressJson = settings.HtmlCompressJson;
        RawTimelineArrays = settings.RawTimelineArrays;
        CompressRaw = settings.CompressRaw;
        IndentJSON = settings.IndentJSON;
        MemoryLimit = settings.MemoryLimit;
    }
    

    public int GetMaxParallelRunning()
    {
        int count;
        if (SendEmbedToWebhook || UploadToDPSReports || UploadToWingmanInternal)
        {
            count = Math.Max(Environment.ProcessorCount / 2, 1);
        }
        else
        {
            count = Environment.ProcessorCount;
        }
        if (MemoryLimit >= 0)
        {
            return count - 1;
        }
        return count;
    }

    public bool HasFormat()
    {
        return SaveOutCSV || SaveOutHTML || SaveOutJSON;
    }

    public bool DoParseMultipleLogs()
    {
        if (ParseMultipleLogs)
        {
            if (!HasFormat() && UploadToDPSReports)
            {
                return false;
            }
            return true;
        }
        return false;
    }
}
