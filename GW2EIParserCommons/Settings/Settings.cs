namespace GW2EIParserCommons.Properties;

public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
{

    private static Settings defaultInstance = (Settings)Synchronized(new Settings());

    public static Settings Default
    {
        get
        {
            return defaultInstance;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool SaveAtOut
    {
        get
        {
            return (bool)this[nameof(SaveAtOut)];
        }
        set
        {
            this[nameof(SaveAtOut)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string OutLocation
    {
        get
        {
            return (string)this[nameof(OutLocation)];
        }
        set
        {
            this[nameof(OutLocation)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool SaveOutHTML
    {
        get
        {
            return (bool)this[nameof(SaveOutHTML)];
        }
        set
        {
            this[nameof(SaveOutHTML)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SaveOutCSV
    {
        get
        {
            return (bool)this[nameof(SaveOutCSV)];
        }
        set
        {
            this[nameof(SaveOutCSV)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SingleThreaded
    {
        get
        {
            return (bool)this[nameof(SingleThreaded)];
        }
        set
        {
            this[nameof(SingleThreaded)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool ParsePhases
    {
        get
        {
            return (bool)this[nameof(ParsePhases)];
        }
        set
        {
            this[nameof(ParsePhases)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool LightTheme
    {
        get
        {
            return (bool)this[nameof(LightTheme)];
        }
        set
        {
            this[nameof(LightTheme)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool ParseCombatReplay
    {
        get
        {
            return (bool)this[nameof(ParseCombatReplay)];
        }
        set
        {
            this[nameof(ParseCombatReplay)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool UploadToDPSReports
    {
        get
        {
            return (bool)this[nameof(UploadToDPSReports)];
        }
        set
        {
            this[nameof(UploadToDPSReports)] = value;
        }
    }


    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool UploadToWingman
    {
        get
        {
            return (bool)this[nameof(UploadToWingman)];
        }
        set
        {
            this[nameof(UploadToWingman)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string DPSReportUserToken
    {
        get
        {
            return (string)this[nameof(DPSReportUserToken)];
        }
        set
        {
            this[nameof(DPSReportUserToken)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SaveOutJSON
    {
        get
        {
            return (bool)this[nameof(SaveOutJSON)];
        }
        set
        {
            this[nameof(SaveOutJSON)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool IndentJSON
    {
        get
        {
            return (bool)this[nameof(IndentJSON)];
        }
        set
        {
            this[nameof(IndentJSON)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool HtmlExternalScripts
    {
        get
        {
            return (bool)this[nameof(HtmlExternalScripts)];
        }
        set
        {
            this[nameof(HtmlExternalScripts)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string HtmlExternalScriptsPath
    {
        get
        {
            return (string)this[nameof(HtmlExternalScriptsPath)];
        }
        set
        {
            this[nameof(HtmlExternalScriptsPath)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string HtmlExternalScriptsCdn
    {
        get
        {
            return (string)this[nameof(HtmlExternalScriptsCdn)];
        }
        set
        {
            this[nameof(HtmlExternalScriptsCdn)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SkipFailedTries
    {
        get
        {
            return (bool)this[nameof(SkipFailedTries)];
        }
        set
        {
            this[nameof(SkipFailedTries)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool AutoAdd
    {
        get
        {
            return (bool)this[nameof(AutoAdd)];
        }
        set
        {
            this[nameof(AutoAdd)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool AutoParse
    {
        get
        {
            return (bool)this[nameof(AutoParse)];
        }
        set
        {
            this[nameof(AutoParse)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string AutoAddPath
    {
        get
        {
            return (string)this[nameof(AutoAddPath)];
        }
        set
        {
            this[nameof(AutoAddPath)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool Outdated
    {
        get
        {
            return (bool)this[nameof(Outdated)];
        }
        set
        {
            this[nameof(Outdated)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool AddPoVProf
    {
        get
        {
            return (bool)this[nameof(AddPoVProf)];
        }
        set
        {
            this[nameof(AddPoVProf)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool AddDuration
    {
        get
        {
            return (bool)this[nameof(AddDuration)];
        }
        set
        {
            this[nameof(AddDuration)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool Anonymous
    {
        get
        {
            return (bool)this[nameof(Anonymous)];
        }
        set
        {
            this[nameof(Anonymous)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool SaveOutTrace
    {
        get
        {
            return (bool)this[nameof(SaveOutTrace)];
        }
        set
        {
            this[nameof(SaveOutTrace)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool CompressRaw
    {
        get
        {
            return (bool)this[nameof(CompressRaw)];
        }
        set
        {
            this[nameof(CompressRaw)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool ComputeDamageModifiers
    {
        get
        {
            return (bool)this[nameof(ComputeDamageModifiers)];
        }
        set
        {
            this[nameof(ComputeDamageModifiers)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool ParseMultipleLogs
    {
        get
        {
            return (bool)this[nameof(ParseMultipleLogs)];
        }
        set
        {
            this[nameof(ParseMultipleLogs)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool ApplicationTraces
    {
        get
        {
            return (bool)this[nameof(ApplicationTraces)];
        }
        set
        {
            this[nameof(ApplicationTraces)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool AutoDiscordBatch
    {
        get
        {
            return (bool)this[nameof(AutoDiscordBatch)];
        }
        set
        {
            this[nameof(AutoDiscordBatch)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool HtmlCompressJson
    {
        get
        {
            return (bool)this[nameof(HtmlCompressJson)];
        }
        set
        {
            this[nameof(HtmlCompressJson)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("True")]
    public bool RawTimelineArrays
    {
        get
        {
            return (bool)this[nameof(RawTimelineArrays)];
        }
        set
        {
            this[nameof(RawTimelineArrays)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("")]
    public string WebhookURL
    {
        get
        {
            return (string)this[nameof(WebhookURL)];
        }
        set
        {
            this[nameof(WebhookURL)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SendEmbedToWebhook
    {
        get
        {
            return (bool)this[nameof(SendEmbedToWebhook)];
        }
        set
        {
            this[nameof(SendEmbedToWebhook)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool SendSimpleMessageToWebhook
    {
        get
        {
            return (bool)this[nameof(SendSimpleMessageToWebhook)];
        }
        set
        {
            this[nameof(SendSimpleMessageToWebhook)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("2200")]
    public long CustomTooShort
    {
        get
        {
            return (long)this[nameof(CustomTooShort)];
        }
        set
        {
            this[nameof(CustomTooShort)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("0")]
    public long PopulateHourLimit
    {
        get
        {
            return (long)this[nameof(PopulateHourLimit)];
        }
        set
        {
            this[nameof(PopulateHourLimit)] = value;
        }
    }

    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("False")]
    public bool DetailledWvW
    {
        get
        {
            return (bool)this[nameof(DetailledWvW)];
        }
        set
        {
            this[nameof(DetailledWvW)] = value;
        }
    }
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("0")]
    public int MemoryLimit
    {
        get
        {
            return (int)this[nameof(MemoryLimit)];
        }
        set
        {
            this[nameof(MemoryLimit)] = value;
        }
    }
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("false")]
    public bool UpdateAvailable
    {
        get
        {
            return (bool)this[nameof(UpdateAvailable)];
        }
        set
        {
            this[nameof(UpdateAvailable)] = value;
        }
    }
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("0")]
    public long UpdateLastChecked
    {
        get
        {
            return (long)this[nameof(UpdateLastChecked)];
        }
        set
        {
            this[nameof(UpdateLastChecked)] = value;
        }
    }
}
