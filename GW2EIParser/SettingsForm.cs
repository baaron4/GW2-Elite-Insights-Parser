using System.Text;
using GW2EIParserCommons;
using GW2EIParserCommons.Properties;

namespace GW2EIParser.SettingsForm;

public partial class SettingsForm : Form
{
    public event EventHandler SettingsClosedEvent;
    public event EventHandler SettingsLoadedEvent;
    public event EventHandler WatchDirectoryUpdatedEvent;

    private readonly ProgramHelper _programHelper;

    private ProgramSettings _programSettings => _programHelper.Settings;

    public SettingsForm(ProgramHelper programHelper)
    {
        _programHelper = programHelper;
        InitializeComponent();
    }

    private void SettingsFormFormClosing(object sender, FormClosingEventArgs e)
    {
        Settings.Default.Save();
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            SettingsClosedEvent(this, null);
        }
    }

    public void ConditionalSettingDisable(bool busy)
    {
        ChkSingleThreaded.Enabled = !busy;
        ChkMultiLogs.Enabled = !busy;
        ChkUploadDPSReports.Enabled = !busy;
        ChkUploadWingman.Enabled = !busy;
        TxtDPSReportUserToken.Enabled = !busy;
        BtnResetSkillList.Enabled = !busy;
        BtnResetSpecList.Enabled = !busy;
        BtnResetTraitList.Enabled = !busy;
        BtnLoadSettings.Enabled = !busy;
        GroupWebhookSettings.Enabled = !busy;
        TxtCustomSaveLocation.Enabled = !busy;
        BtnCustomSaveLocSelect.Enabled = !busy;
        TxtHtmlExternalScriptsPath.Enabled = !busy;
        TxtHtmlExternalScriptsCdn.Enabled = !busy;
        BtnHtmlExternalScriptPathSelect.Enabled = !busy;
        NumericMemoryLimit.Enabled = !busy;
    }

    private void SetUIEnable()
    {
        PanelHtml.Enabled = _programSettings.SaveOutHTML;
        PanelJson.Enabled = _programSettings.SaveOutJSON;
        GroupRawSettings.Enabled = _programSettings.SaveOutJSON;
        TxtHtmlExternalScriptsPath.Enabled = _programSettings.HtmlExternalScripts;
        LblHtmlExternalScriptsPath.Enabled = _programSettings.HtmlExternalScripts;
        TxtHtmlExternalScriptsCdn.Enabled = _programSettings.HtmlExternalScripts;
        LblHtmlExternalScriptsCdn.Enabled = _programSettings.HtmlExternalScripts;
        BtnHtmlExternalScriptPathSelect.Enabled = _programSettings.HtmlExternalScripts;
    }

    private void SetValues()
    {

        ChkDefaultOutputLoc.Checked = _programSettings.SaveAtOut;
        TxtCustomSaveLocation.Text = _programSettings.OutLocation;
        NumericCustomTooShort.Value = _programSettings.CustomTooShort;
        NumericMemoryLimit.Value = _programSettings.MemoryLimit;
        ChkOutputHtml.Checked = _programSettings.SaveOutHTML;
        ChkOutputCsv.Checked = _programSettings.SaveOutCSV;
        ChkPhaseParsing.Checked = _programSettings.ParsePhases;
        ChkSingleThreaded.Checked = _programSettings.SingleThreaded;
        RadioThemeLight.Checked = _programSettings.LightTheme;
        RadioThemeDark.Checked = !_programSettings.LightTheme;
        PictureTheme.Image = _programSettings.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        ChkCombatReplay.Checked = _programSettings.ParseCombatReplay;
        ChkOutputJson.Checked = _programSettings.SaveOutJSON;
        ChkIndentJSON.Checked = _programSettings.IndentJSON;
        ChkUploadDPSReports.Checked = _programSettings.UploadToDPSReports;
        ChkUploadWingman.Checked = _programSettings.UploadToWingman;
        TxtDPSReportUserToken.Text = _programSettings.DPSReportUserToken;
        ChkUploadWebhook.Checked = _programSettings.SendEmbedToWebhook;
        ChkUploadSimpleMessageWebhook.Checked = _programSettings.SendSimpleMessageToWebhook;
        TxtUploadWebhookUrl.Text = _programSettings.WebhookURL;
        ChkSkipFailedTries.Checked = _programSettings.SkipFailedTries;
        ChkAutoAdd.Checked = Settings.Default.AutoAdd;
        ChkAutoParse.Checked = Settings.Default.AutoParse;
        ChkAddPoVProf.Checked = _programSettings.AddPoVProf;
        ChkCompressRaw.Checked = _programSettings.CompressRaw;
        ChkAddDuration.Checked = _programSettings.AddDuration;
        ChkAnonymous.Checked = _programSettings.Anonymous;
        ChkSaveOutTrace.Checked = _programSettings.SaveOutTrace;
        ChkDamageMods.Checked = _programSettings.ComputeDamageModifiers;
        ChkMultiLogs.Checked = _programSettings.ParseMultipleLogs;
        ChkRawTimelineArrays.Checked = _programSettings.RawTimelineArrays;
        ChkDetailledWvW.Checked = _programSettings.DetailledWvW;
        ChkHtmlExternalScripts.Checked = _programSettings.HtmlExternalScripts;
        ChkHtmlCompressJson.Checked = _programSettings.HtmlCompressJson;
        TxtHtmlExternalScriptsPath.Text = _programSettings.HtmlExternalScriptsPath;
        TxtHtmlExternalScriptsCdn.Text = _programSettings.HtmlExternalScriptsCdn;

        SetUIEnable();
    }

    private void SettingsFormLoad(object sender, EventArgs e)
    {
        SetValues();
    }

    private void ChkDefaultOutputLocationCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SaveAtOut = ChkDefaultOutputLoc.Checked;
        Settings.Default.SaveAtOut = _programSettings.SaveAtOut;
    }

    private void BtnCustomSaveLocationSelectClick(object sender, EventArgs e)
    {
        try
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(_programSettings.OutLocation) && Directory.Exists(_programSettings.OutLocation))
                {
                    fbd.ShowNewFolderButton = true;
                    fbd.SelectedPath = _programSettings.OutLocation;
                }
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && Directory.Exists(fbd.SelectedPath))
                {
                    TxtCustomSaveLocation.Text = fbd.SelectedPath;
                }
            }
        }
        catch { }
    }

    private void TxtCustomSaveLocationTextChanged(object sender, EventArgs e)
    {
        _programSettings.OutLocation = TxtCustomSaveLocation.Text.Trim();
        Settings.Default.OutLocation = _programSettings.OutLocation;
    }

    private void NumericCustomTooShortValueChanged(object sender, EventArgs e)
    {
        _programSettings.CustomTooShort = (long)NumericCustomTooShort.Value;
        Settings.Default.CustomTooShort = _programSettings.CustomTooShort;
    }

    private void NumericMemoryLimitValueChanged(object sender, EventArgs e)
    {
        _programSettings.MemoryLimit = (int)NumericMemoryLimit.Value;
        Settings.Default.MemoryLimit = _programSettings.MemoryLimit;
    }

    private void TxtWebhookURLChanged(object sender, EventArgs e)
    {
        _programSettings.WebhookURL = TxtUploadWebhookUrl.Text;
        Settings.Default.WebhookURL = _programSettings.WebhookURL;
    }

    private void BtnResetSkillListClick(object sender, EventArgs e)
    {
        //Update skill list
        ProgramHelper.APIController.WriteAPISkillsToFile(ProgramHelper.SkillAPICacheLocation);
        MessageBox.Show("Skill List has been redone");
    }

    private void BtnResetTraitListClick(object sender, EventArgs e)
    {
        //Update skill list
        ProgramHelper.APIController.WriteAPITraitsToFile(ProgramHelper.TraitAPICacheLocation);
        MessageBox.Show("Trait List has been redone");
    }

    private void BtnResetSpecListClick(object sender, EventArgs e)
    {
        //Update skill list
        ProgramHelper.APIController.WriteAPISpecsToFile(ProgramHelper.SpecAPICacheLocation);
        MessageBox.Show("Spec List has been redone");
    }

    private void ChkOuputHTMLCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SaveOutHTML = ChkOutputHtml.Checked;
        Settings.Default.SaveOutHTML = _programSettings.SaveOutHTML;
        PanelHtml.Enabled = _programSettings.SaveOutHTML;
    }

    private void ChkOutputCsvCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SaveOutCSV = ChkOutputCsv.Checked;
        Settings.Default.SaveOutCSV = _programSettings.SaveOutCSV;
    }

    private void ChkSingleThreadedCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SingleThreaded = ChkSingleThreaded.Checked;
        Settings.Default.SingleThreaded = _programSettings.SingleThreaded;
    }

    private void ChkPhaseParsingCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.ParsePhases = ChkPhaseParsing.Checked;
        Settings.Default.ParsePhases = _programSettings.ParsePhases;
    }

    private void ChkCombatReplayCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.ParseCombatReplay = ChkCombatReplay.Checked;
        Settings.Default.ParseCombatReplay = _programSettings.ParseCombatReplay;
    }

    private void ChkUploadDPSReportsCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.UploadToDPSReports = ChkUploadDPSReports.Checked;
        Settings.Default.UploadToDPSReports = _programSettings.UploadToDPSReports;
        SetUIEnable();
    }

    private void ChkDPSReportUserTokenTextChanged(object sender, EventArgs e)
    {
        _programSettings.DPSReportUserToken = TxtDPSReportUserToken.Text;
        Settings.Default.DPSReportUserToken = _programSettings.DPSReportUserToken;
    }

    private void ChkUploadWingmanCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.UploadToWingman = ChkUploadWingman.Checked;
        Settings.Default.UploadToWingman = _programSettings.UploadToWingman;
        SetUIEnable();
    }

    private void ChkUploadWebhookCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SendEmbedToWebhook = ChkUploadWebhook.Checked;
        Settings.Default.SendEmbedToWebhook = _programSettings.SendEmbedToWebhook;
    }

    private void ChkUploadSimpleMessageWebhookCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SendSimpleMessageToWebhook = ChkUploadSimpleMessageWebhook.Checked;
        Settings.Default.SendSimpleMessageToWebhook = _programSettings.SendSimpleMessageToWebhook;
    }

    private void ChkSkipFailedTriesCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SkipFailedTries = ChkSkipFailedTries.Checked;
        Settings.Default.SkipFailedTries = _programSettings.SkipFailedTries;
    }

    private void ChkOutputJSONCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SaveOutJSON = ChkOutputJson.Checked;
        Settings.Default.SaveOutJSON = _programSettings.SaveOutJSON;
        SetUIEnable();
    }

    private void ChkIndentJSONCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.IndentJSON = ChkIndentJSON.Checked;
        Settings.Default.IndentJSON = _programSettings.IndentJSON;
    }

    private void ChkHtmlExternalScriptsCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.HtmlExternalScripts = ChkHtmlExternalScripts.Checked;
        Settings.Default.HtmlExternalScripts = _programSettings.HtmlExternalScripts;
        LblHtmlExternalScriptsPath.Enabled = ChkHtmlExternalScripts.Checked;
        TxtHtmlExternalScriptsPath.Enabled = ChkHtmlExternalScripts.Checked;
        LblHtmlExternalScriptsCdn.Enabled = ChkHtmlExternalScripts.Checked;
        TxtHtmlExternalScriptsCdn.Enabled = ChkHtmlExternalScripts.Checked;
        BtnHtmlExternalScriptPathSelect.Enabled = ChkHtmlExternalScripts.Checked;
    }

    private void ChkHtmlCompressCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.HtmlCompressJson = ChkHtmlCompressJson.Checked;
        Settings.Default.HtmlCompressJson = _programSettings.HtmlCompressJson;
    }

    private void RadioThemeLightCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.LightTheme = RadioThemeLight.Checked;
        Settings.Default.LightTheme = _programSettings.LightTheme;
        PictureTheme.Image = _programSettings.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
    }

    private void RadioThemeDarkCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.LightTheme = RadioThemeLight.Checked;
        Settings.Default.LightTheme = _programSettings.LightTheme;
        PictureTheme.Image = _programSettings.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
    }

    private void BtnCloseClick(object sender, EventArgs e)
    {
        Close();
    }

    private void ChkAutoAddCheckedChanged(object sender, EventArgs e)
    {
        if (ChkAutoAdd.Checked && !Settings.Default.AutoAdd)
        {
            string path = Settings.Default.AutoAddPath;
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                try
                {
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Guild Wars 2/addons/arcdps/arcdps.cbtlogs");
                }
                catch (PlatformNotSupportedException)
                {
                    path = null;
                }
            }
            if (!Directory.Exists(path))
            {
                path = null;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = false;
                fbd.SelectedPath = path;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Settings.Default.AutoAddPath = fbd.SelectedPath;
                }
                else
                {
                    ChkAutoAdd.Checked = false;
                }
            }
        }
        Settings.Default.AutoAdd = ChkAutoAdd.Checked;
        WatchDirectoryUpdatedEvent(this, null);
    }

    private void ChkAutoParseCheckedChanged(object sender, EventArgs e)
    {
        Settings.Default.AutoParse = ChkAutoParse.Checked;
    }

    private void ChkAddPoVProfCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.AddPoVProf = ChkAddPoVProf.Checked;
        Settings.Default.AddPoVProf = _programSettings.AddPoVProf;
    }

    private void ChkCompressRawCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.CompressRaw = ChkCompressRaw.Checked;
        Settings.Default.CompressRaw = _programSettings.CompressRaw;
    }

    private void ChkAddDurationCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.AddDuration = ChkAddDuration.Checked;
        Settings.Default.AddDuration = _programSettings.AddDuration;
    }

    private void BtnDumpSettingsClicked(object sender, EventArgs e)
    {
        string dump = CustomSettingsManager.DumpSettings();
        using (var saveFile = new SaveFileDialog())
        {
            saveFile.Filter = "Conf file|*.conf";
            saveFile.Title = "Save a Configuration file";
            DialogResult result = saveFile.ShowDialog();
            if (saveFile.FileName.Length > 0)
            {
                var fs = (FileStream)saveFile.OpenFile();
                byte[] settings = new UTF8Encoding(true).GetBytes(dump);
                fs.Write(settings, 0, settings.Length);
                fs.Close();
            }
        }
    }

    private void BtnLoadSettingsClicked(object sender, EventArgs e)
    {
        using (var loadFile = new OpenFileDialog())
        {
            loadFile.Filter = "Conf file|*.conf";
            loadFile.Title = "Load a Configuration file";
            DialogResult result = loadFile.ShowDialog();
            if (loadFile.FileName.Length > 0)
            {
                CustomSettingsManager.ReadConfig(loadFile.FileName);
                _programHelper.ApplySettings(CustomSettingsManager.GetProgramSettings());
                SetValues();
                SettingsLoadedEvent(this, null);
            }
        }
    }

    private void ChkAnonymousCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.Anonymous = ChkAnonymous.Checked;
        Settings.Default.Anonymous = _programSettings.Anonymous;
    }

    private void ChkDetailledWvWCheckedChange(object sender, EventArgs e)
    {
        _programSettings.DetailledWvW = ChkDetailledWvW.Checked;
        Settings.Default.DetailledWvW = _programSettings.DetailledWvW;
    }

    private void ChkSaveOutTraceCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.SaveOutTrace = ChkSaveOutTrace.Checked;
        Settings.Default.SaveOutTrace = _programSettings.SaveOutTrace;
    }

    private void ChkComputeDamageModsCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.ComputeDamageModifiers = ChkDamageMods.Checked;
        Settings.Default.ComputeDamageModifiers = _programSettings.ComputeDamageModifiers;
    }

    private void ChkMultiLogsCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.ParseMultipleLogs = ChkMultiLogs.Checked;
        Settings.Default.ParseMultipleLogs = _programSettings.ParseMultipleLogs;
        SetUIEnable();
    }

    private void ChkRawTimelineArraysCheckedChanged(object sender, EventArgs e)
    {
        _programSettings.RawTimelineArrays = ChkRawTimelineArrays.Checked;
        Settings.Default.RawTimelineArrays = _programSettings.RawTimelineArrays;
    }

    private void TxtHtmlExternalScriptsPathTextChanged(object sender, EventArgs e)
    {
        _programSettings.HtmlExternalScriptsPath = TxtHtmlExternalScriptsPath.Text.Trim();
        Settings.Default.HtmlExternalScriptsPath = _programSettings.HtmlExternalScriptsPath;
    }

    private void TxtHtmlExternalScriptCdnUrlTextChanged(object sender, EventArgs e)
    {
        _programSettings.HtmlExternalScriptsCdn = TxtHtmlExternalScriptsCdn.Text.Trim();
        Settings.Default.HtmlExternalScriptsCdn = _programSettings.HtmlExternalScriptsCdn;
    }

    private void BtnHtmlExternalScriptPathSelectClick(object sender, EventArgs e)
    {
        try
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(_programSettings.HtmlExternalScriptsPath) && Directory.Exists(_programSettings.HtmlExternalScriptsPath))
                {
                    fbd.ShowNewFolderButton = true;
                    fbd.SelectedPath = _programSettings.HtmlExternalScriptsPath;
                }
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && Directory.Exists(fbd.SelectedPath))
                {
                    TxtHtmlExternalScriptsPath.Text = fbd.SelectedPath;
                }
            }
        }
        catch { }
    }
}
