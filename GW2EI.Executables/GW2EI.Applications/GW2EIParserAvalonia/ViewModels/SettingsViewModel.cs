using System;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIParserCommons;
using GW2EIParserCommons.Properties;

namespace GW2EIParserAvalonia.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ProgramSettings _settings;

    public SettingsViewModel(ProgramSettings settings)
    {
        _settings = settings;
    }

    // General
    // - Log
    [ObservableProperty]
    private bool computePhases;
    [ObservableProperty]
    private bool computeDamage;
    [ObservableProperty]
    private bool computeBuff;
    [ObservableProperty]
    private bool computeCast;
    [ObservableProperty]
    private bool computeDamageModifiers;
    [ObservableProperty]
    private bool computeCombatReplay;
    [ObservableProperty]
    private bool computeMechanics;
    [ObservableProperty]
    private bool parseExtensions;
    [ObservableProperty]
    private bool detailledWvW;
    // - Parsing
    [ObservableProperty]
    private bool singleThreaded;
    [ObservableProperty]
    private bool parseMultipleLogs;
    [ObservableProperty]
    private bool skipFailedTries;
    [ObservableProperty]
    private bool autoAdd;
    [ObservableProperty]
    private string autoAddPath = string.Empty;
    [ObservableProperty]
    private bool autoParse;
    [ObservableProperty]
    private long customTooShort;
    [ObservableProperty]
    private int customTooBig;
    [ObservableProperty]
    private int memoryLimit;
    // - Output
    [ObservableProperty]
    private bool anonymous;
    [ObservableProperty]
    private bool saveAtOut;
    [ObservableProperty]
    private string outLocation = string.Empty;
    [ObservableProperty]
    private bool addDuration;
    [ObservableProperty]
    private bool addPoVProf;
    [ObservableProperty]
    private bool saveOutTrace;
    [ObservableProperty]
    private bool applicationTraces;

    // HTML
    [ObservableProperty]
    private bool saveOutHTML;
    [ObservableProperty]
    private bool lightTheme;
    [ObservableProperty]
    private bool darkTheme;
    [ObservableProperty]
    private bool htmlExternalScripts;
    [ObservableProperty]
    private string htmlExternalScriptsPath = string.Empty;
    [ObservableProperty]
    private string htmlExternalScriptsCdn = string.Empty;
    [ObservableProperty]
    private bool htmlCompressJson;

    // CSV
    [ObservableProperty]
    private bool saveOutCSV;

    // JSON
    [ObservableProperty]
    private bool saveOutJSON;
    [ObservableProperty]
    private bool indentJSON;
    [ObservableProperty]
    private bool rawTimelineArrays;
    [ObservableProperty]
    private bool compressRaw;

    // Upload / Webhook
    [ObservableProperty]
    private bool uploadToDPSReports;
    [ObservableProperty]
    private string dPSReportUserToken = string.Empty;
    [ObservableProperty]
    private bool uploadToWingman;
    [ObservableProperty]
    private bool sendEmbedToWebhook;
    [ObservableProperty]
    private string webhookURL = string.Empty;
    [ObservableProperty]
    private bool sendSimpleMessageToWebhook; 
    [ObservableProperty]
    private bool autoDiscordBatch;

    // GUI
    [ObservableProperty]
    private long populateHourLimit;
    
    // Updater
    [ObservableProperty]
    private bool updateAvailable;

    private bool _loading = false;

    //
    public event EventHandler? AutoAddFolderRequested;

    public void LoadFromSettings()
    {
        _loading = true;
        // General
        // - Log
        ComputePhases = _settings.ComputePhases;
        ComputeDamage = _settings.ComputeDamage;
        ComputeBuff = _settings.ComputeBuff;
        ComputeCast = _settings.ComputeCast;
        ComputeDamageModifiers = _settings.ComputeDamageModifiers;
        ComputeCombatReplay = _settings.ComputeCombatReplay;
        ComputeMechanics = _settings.ComputeMechanics;
        ParseExtensions = _settings.ParseExtensions;
        DetailledWvW = _settings.DetailledWvW;

        // - Parsing
        SingleThreaded = _settings.SingleThreaded;
        ParseMultipleLogs = _settings.ParseMultipleLogs;
        SkipFailedTries = _settings.SkipFailedTries;
        AutoAdd = Settings.Default.AutoAdd;
        AutoAddPath = Settings.Default.AutoAddPath ?? string.Empty;
        AutoParse = Settings.Default.AutoParse;
        CustomTooShort = _settings.CustomTooShort;
        CustomTooBig = _settings.CustomTooBig;
        MemoryLimit = _settings.MemoryLimit;

        // - Output
        Anonymous = _settings.Anonymous;
        SaveAtOut = _settings.SaveAtOut;
        OutLocation = _settings.OutLocation ?? string.Empty;
        AddDuration = _settings.AddDuration;
        AddPoVProf = _settings.AddPoVProf;
        SaveOutTrace = _settings.SaveOutTrace;
        ApplicationTraces = Settings.Default.ApplicationTraces;

        // HTML
        SaveOutHTML = _settings.SaveOutHTML;
        LightTheme = _settings.LightTheme;
        DarkTheme = !_settings.LightTheme;
        HtmlExternalScripts = _settings.HtmlExternalScripts;
        HtmlExternalScriptsPath = _settings.HtmlExternalScriptsPath ?? string.Empty;
        HtmlExternalScriptsCdn = _settings.HtmlExternalScriptsCdn ?? string.Empty;
        HtmlCompressJson = _settings.HtmlCompressJson;

        // CSV
        SaveOutCSV = _settings.SaveOutCSV;

        // JSON
        SaveOutJSON = _settings.SaveOutJSON;
        IndentJSON = _settings.IndentJSON;
        RawTimelineArrays = _settings.RawTimelineArrays;
        CompressRaw = _settings.CompressRaw;

        // Upload
        UploadToDPSReports = _settings.UploadToDPSReports;
        DPSReportUserToken = _settings.DPSReportUserToken ?? string.Empty;
        UploadToWingman = _settings.UploadToWingman;
        SendEmbedToWebhook = _settings.SendEmbedToWebhook;
        WebhookURL = _settings.WebhookURL ?? string.Empty;
        SendSimpleMessageToWebhook = _settings.SendSimpleMessageToWebhook;
        AutoDiscordBatch = Settings.Default.AutoDiscordBatch;

        // GUI
        PopulateHourLimit = Settings.Default.PopulateHourLimit;

        // Updater
        UpdateAvailable = Settings.Default.UpdateAvailable;

        _loading = false;
    }

    private void ApplyToDefault()
    {
        // General
        // - Log
        Settings.Default.ParsePhases = ComputePhases;
        Settings.Default.ComputeDamage = ComputeDamage;
        Settings.Default.ComputeBuff = ComputeBuff;
        Settings.Default.ComputeCast = ComputeCast;
        Settings.Default.ComputeDamageModifiers = ComputeDamageModifiers;
        Settings.Default.ParseCombatReplay = ComputeCombatReplay;
        Settings.Default.ComputeMechanics = ComputeMechanics;
        Settings.Default.ParseExtensions = ParseExtensions;
        Settings.Default.DetailledWvW = DetailledWvW;

        // - Parsing
        Settings.Default.SingleThreaded = SingleThreaded;
        Settings.Default.ParseMultipleLogs = ParseMultipleLogs;
        Settings.Default.SkipFailedTries = SkipFailedTries;
        Settings.Default.AutoAdd = AutoAdd;
        Settings.Default.AutoAddPath = AutoAddPath;
        Settings.Default.AutoParse = AutoParse;
        Settings.Default.CustomTooShort = CustomTooShort;
        Settings.Default.CustomTooBig = CustomTooBig;
        Settings.Default.MemoryLimit = MemoryLimit;

        // - Output
        Settings.Default.Anonymous = Anonymous;
        Settings.Default.SaveAtOut = SaveAtOut;
        Settings.Default.OutLocation = OutLocation;
        Settings.Default.AddDuration = AddDuration;
        Settings.Default.AddPoVProf = AddPoVProf;
        Settings.Default.SaveOutTrace = SaveOutTrace;
        Settings.Default.ApplicationTraces = ApplicationTraces;

        // HTML
        Settings.Default.SaveOutHTML = SaveOutHTML;
        Settings.Default.LightTheme = LightTheme;
        Settings.Default.HtmlExternalScripts = HtmlExternalScripts;
        Settings.Default.HtmlExternalScriptsPath = HtmlExternalScriptsPath;
        Settings.Default.HtmlExternalScriptsCdn = HtmlExternalScriptsCdn;
        Settings.Default.HtmlCompressJson = HtmlCompressJson;

        // CSV
        Settings.Default.SaveOutCSV = SaveOutCSV;

        // JSON
        Settings.Default.SaveOutJSON = SaveOutJSON;
        Settings.Default.IndentJSON = IndentJSON;
        Settings.Default.RawTimelineArrays = RawTimelineArrays;
        Settings.Default.CompressRaw = CompressRaw;

        // Upload
        Settings.Default.UploadToDPSReports = UploadToDPSReports;
        Settings.Default.DPSReportUserToken = DPSReportUserToken;
        Settings.Default.UploadToWingman = UploadToWingman;
        Settings.Default.SendEmbedToWebhook = SendEmbedToWebhook;
        Settings.Default.WebhookURL = WebhookURL;
        Settings.Default.SendSimpleMessageToWebhook = SendSimpleMessageToWebhook;
        Settings.Default.AutoDiscordBatch = AutoDiscordBatch;

        // GUI
        Settings.Default.PopulateHourLimit = PopulateHourLimit;

        // Updater
        Settings.Default.UpdateAvailable = UpdateAvailable;
    }

    public void ApplyToSettings()
    {
        if (_loading)
        {
            return;
        }
        // General
        // - Log
        _settings.ComputePhases = ComputePhases;
        _settings.ComputeDamage = ComputeDamage;
        _settings.ComputeBuff = ComputeBuff;
        _settings.ComputeCast = ComputeCast;
        _settings.ComputeDamageModifiers = ComputeDamageModifiers;
        _settings.ComputeCombatReplay = ComputeCombatReplay;
        _settings.ComputeMechanics = ComputeMechanics;
        _settings.ParseExtensions = ParseExtensions;
        _settings.DetailledWvW = DetailledWvW;

        // - Parsing
        _settings.SingleThreaded = SingleThreaded;
        _settings.ParseMultipleLogs = ParseMultipleLogs;
        _settings.SkipFailedTries = SkipFailedTries;
        _settings.CustomTooShort = CustomTooShort;
        _settings.CustomTooBig = CustomTooBig;
        _settings.MemoryLimit = MemoryLimit;

        // - Output
        _settings.Anonymous = Anonymous;
        _settings.SaveAtOut = SaveAtOut;
        _settings.OutLocation = OutLocation;
        _settings.AddDuration = AddDuration;
        _settings.AddPoVProf = AddPoVProf;
        _settings.SaveOutTrace = SaveOutTrace;

        // HTML
        _settings.SaveOutHTML = SaveOutHTML;
        _settings.LightTheme = LightTheme;
        _settings.HtmlExternalScripts = HtmlExternalScripts;
        _settings.HtmlExternalScriptsPath = HtmlExternalScriptsPath;
        _settings.HtmlExternalScriptsCdn = HtmlExternalScriptsCdn;
        _settings.HtmlCompressJson = HtmlCompressJson;

        // CSV
        _settings.SaveOutCSV = SaveOutCSV;

        // JSON
        _settings.SaveOutJSON = SaveOutJSON;
        _settings.IndentJSON = IndentJSON;
        _settings.RawTimelineArrays = RawTimelineArrays;
        _settings.CompressRaw = CompressRaw;

        // Upload
        _settings.UploadToDPSReports = UploadToDPSReports;
        _settings.DPSReportUserToken = DPSReportUserToken;
        _settings.UploadToWingman = UploadToWingman;
        _settings.SendEmbedToWebhook = SendEmbedToWebhook;
        _settings.WebhookURL = WebhookURL;
        _settings.SendSimpleMessageToWebhook = SendSimpleMessageToWebhook;

        ApplyToDefault();

    }

    partial void OnAutoAddChanged(bool value)
    {
        if (_loading)
        {
            return;
        }
        if (value)
        {
            AutoAddFolderRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ApplyLoadedSettings(string path)
    {
        CustomSettingsManager.ReadConfig(path);
        CustomSettingsManager.GetProgramSettings(_settings);
        LoadFromSettings();
    }
}
