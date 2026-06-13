using GW2EIEvtcParser;

namespace GW2EIParserCommons;

public class ProgramSettings
{
    // Upload
    public bool SendEmbedToWebhook { get; set; } = false;
    public bool SendSimpleMessageToWebhook { get; set; } = false;
    public string WebhookURL { get; set; }
    public bool UploadToDPSReports { get; set; } = false;
    public string DPSReportUserToken { get; set; }
    public bool UploadToWingman { get; set; } = false;
    public bool UploadToMistWarrior { get; set; } = false;
    public string MistWarriorUserToken { get; set; }
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
    public string OutLocation { get; set; }
    // Output
    public bool AddDuration { get; set; } = false;
    public bool AddPoVProf { get; set; } = false;
    // HTML
    public bool LightTheme { get; set; } = false;
    public bool HtmlExternalScripts { get; set; } = false;
    public string HtmlExternalScriptsPath { get; set; }
    public string HtmlExternalScriptsCdn { get; set; }
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

    public int GetMaxParallelRunning()
    {
        int count;
        if (SendEmbedToWebhook || UploadToDPSReports || UploadToWingman || UploadToMistWarrior)
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
