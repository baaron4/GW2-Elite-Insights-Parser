using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GW2EIParserCommons;

namespace GW2EIParser.Setting
{
    public partial class SettingsForm : Form
    {
        public event EventHandler SettingsClosedEvent;
        public event EventHandler SettingsLoadedEvent;
        public event EventHandler WatchDirectoryUpdatedEvent;

        private readonly ProgramHelper _programHelper;

        private ProgramSettings Settings => _programHelper.Settings;

        public SettingsForm(ProgramHelper programHelper)
        {
            _programHelper = programHelper;
            InitializeComponent();
        }

        private void SettingsFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
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
            PanelHtml.Enabled = Settings.SaveOutHTML;
            PanelJson.Enabled = Settings.SaveOutJSON;
            PanelXML.Enabled = Settings.SaveOutXML;
            GroupRawSettings.Enabled = Settings.SaveOutJSON || Settings.SaveOutXML;
            TxtHtmlExternalScriptsPath.Enabled = Settings.HtmlExternalScripts;
            LblHtmlExternalScriptsPath.Enabled = Settings.HtmlExternalScripts;
            TxtHtmlExternalScriptsCdn.Enabled = Settings.HtmlExternalScripts;
            LblHtmlExternalScriptsCdn.Enabled = Settings.HtmlExternalScripts;
            BtnHtmlExternalScriptPathSelect.Enabled = Settings.HtmlExternalScripts;
        }

        private void SetValues()
        {

            ChkDefaultOutputLoc.Checked = Settings.SaveAtOut;
            TxtCustomSaveLocation.Text = Settings.OutLocation;
            NumericCustomTooShort.Value = Settings.CustomTooShort;
            NumericMemoryLimit.Value = Settings.MemoryLimit;
            ChkOutputHtml.Checked = Settings.SaveOutHTML;
            ChkOutputCsv.Checked = Settings.SaveOutCSV;
            ChkPhaseParsing.Checked = Settings.ParsePhases;
            ChkSingleThreaded.Checked = Settings.SingleThreaded;
            RadioThemeLight.Checked = Settings.LightTheme;
            RadioThemeDark.Checked = !Settings.LightTheme;
            PictureTheme.Image = Settings.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
            ChkCombatReplay.Checked = Settings.ParseCombatReplay;
            ChkOutputJson.Checked = Settings.SaveOutJSON;
            ChkIndentJSON.Checked = Settings.IndentJSON;
            ChkOutputXml.Checked = Settings.SaveOutXML;
            ChkIndentXML.Checked = Settings.IndentXML;
            ChkUploadDPSReports.Checked = Settings.UploadToDPSReports;
            ChkUploadWingman.Checked = Settings.UploadToWingman;
            TxtDPSReportUserToken.Text = Settings.DPSReportUserToken;
            ChkUploadWebhook.Checked = Settings.SendEmbedToWebhook;
            ChkUploadSimpleMessageWebhook.Checked = Settings.SendSimpleMessageToWebhook;
            TxtUploadWebhookUrl.Text = Settings.WebhookURL;
            ChkSkipFailedTries.Checked = Settings.SkipFailedTries;
            ChkAutoAdd.Checked = Properties.Settings.Default.AutoAdd;
            ChkAutoParse.Checked = Properties.Settings.Default.AutoParse;
            ChkAddPoVProf.Checked = Settings.AddPoVProf;
            ChkCompressRaw.Checked = Settings.CompressRaw;
            ChkAddDuration.Checked = Settings.AddDuration;
            ChkAnonymous.Checked = Settings.Anonymous;
            ChkSaveOutTrace.Checked = Settings.SaveOutTrace;
            ChkDamageMods.Checked = Settings.ComputeDamageModifiers;
            ChkMultiLogs.Checked = Settings.ParseMultipleLogs;
            ChkRawTimelineArrays.Checked = Settings.RawTimelineArrays;
            ChkDetailledWvW.Checked = Settings.DetailledWvW;
            ChkHtmlExternalScripts.Checked = Settings.HtmlExternalScripts;
            ChkHtmlCompressJson.Checked = Settings.HtmlCompressJson;
            TxtHtmlExternalScriptsPath.Text = Settings.HtmlExternalScriptsPath;
            TxtHtmlExternalScriptsCdn.Text = Settings.HtmlExternalScriptsCdn;

            SetUIEnable();
        }

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            SetValues();
        }

        private void ChkDefaultOutputLocationCheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveAtOut = ChkDefaultOutputLoc.Checked;
            Properties.Settings.Default.SaveAtOut = Settings.SaveAtOut;
        }

        private void BtnCustomSaveLocationSelectClick(object sender, EventArgs e)
        {
            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (!string.IsNullOrWhiteSpace(Settings.OutLocation) && Directory.Exists(Settings.OutLocation))
                    {
                        fbd.ShowNewFolderButton = true;
                        fbd.SelectedPath = Settings.OutLocation;
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
            Settings.OutLocation = TxtCustomSaveLocation.Text.Trim();
            Properties.Settings.Default.OutLocation = Settings.OutLocation;
        }

        private void NumericCustomTooShortValueChanged(object sender, EventArgs e)
        {
            Settings.CustomTooShort = (long)NumericCustomTooShort.Value;
            Properties.Settings.Default.CustomTooShort = Settings.CustomTooShort;
        }

        private void NumericMemoryLimitValueChanged(object sender, EventArgs e)
        {
            Settings.MemoryLimit = (int)NumericMemoryLimit.Value;
            Properties.Settings.Default.MemoryLimit = Settings.MemoryLimit;
        }

        private void TxtWebhookURLChanged(object sender, EventArgs e)
        {
            Settings.WebhookURL = TxtUploadWebhookUrl.Text;
            Properties.Settings.Default.WebhookURL = Settings.WebhookURL;
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
            Settings.SaveOutHTML = ChkOutputHtml.Checked;
            Properties.Settings.Default.SaveOutHTML = Settings.SaveOutHTML;
            PanelHtml.Enabled = Settings.SaveOutHTML;
        }

        private void ChkOutputCsvCheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveOutCSV = ChkOutputCsv.Checked;
            Properties.Settings.Default.SaveOutCSV = Settings.SaveOutCSV;
        }

        private void ChkSingleThreadedCheckedChanged(object sender, EventArgs e)
        {
            Settings.SingleThreaded = ChkSingleThreaded.Checked;
            Properties.Settings.Default.SingleThreaded = Settings.SingleThreaded;
        }

        private void ChkPhaseParsingCheckedChanged(object sender, EventArgs e)
        {
            Settings.ParsePhases = ChkPhaseParsing.Checked;
            Properties.Settings.Default.ParsePhases = Settings.ParsePhases;
        }

        private void ChkCombatReplayCheckedChanged(object sender, EventArgs e)
        {
            Settings.ParseCombatReplay = ChkCombatReplay.Checked;
            Properties.Settings.Default.ParseCombatReplay = Settings.ParseCombatReplay;
        }

        private void ChkUploadDPSReportsCheckedChanged(object sender, EventArgs e)
        {
            Settings.UploadToDPSReports = ChkUploadDPSReports.Checked;
            Properties.Settings.Default.UploadToDPSReports = Settings.UploadToDPSReports;
            SetUIEnable();
        }

        private void ChkDPSReportUserTokenTextChanged(object sender, EventArgs e)
        {
            Settings.DPSReportUserToken = TxtDPSReportUserToken.Text;
            Properties.Settings.Default.DPSReportUserToken = Settings.DPSReportUserToken;
        }

        private void ChkUploadWingmanCheckedChanged(object sender, EventArgs e)
        {
            Settings.UploadToWingman = ChkUploadWingman.Checked;
            Properties.Settings.Default.UploadToWingman = Settings.UploadToWingman;
            SetUIEnable();
        }

        private void ChkUploadWebhookCheckedChanged(object sender, EventArgs e)
        {
            Settings.SendEmbedToWebhook = ChkUploadWebhook.Checked;
            Properties.Settings.Default.SendEmbedToWebhook = Settings.SendEmbedToWebhook;
        }

        private void ChkUploadSimpleMessageWebhookCheckedChanged(object sender, EventArgs e)
        {
            Settings.SendSimpleMessageToWebhook = ChkUploadSimpleMessageWebhook.Checked;
            Properties.Settings.Default.SendSimpleMessageToWebhook = Settings.SendSimpleMessageToWebhook;
        }

        private void ChkSkipFailedTriesCheckedChanged(object sender, EventArgs e)
        {
            Settings.SkipFailedTries = ChkSkipFailedTries.Checked;
            Properties.Settings.Default.SkipFailedTries = Settings.SkipFailedTries;
        }

        private void ChkOutputJSONCheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveOutJSON = ChkOutputJson.Checked;
            Properties.Settings.Default.SaveOutJSON = Settings.SaveOutJSON;
            SetUIEnable();
        }

        private void ChkOutputXMLCheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveOutXML = ChkOutputXml.Checked;
            Properties.Settings.Default.SaveOutXML = Settings.SaveOutXML;
            SetUIEnable();
        }

        private void ChkIndentJSONCheckedChanged(object sender, EventArgs e)
        {
            Settings.IndentJSON = ChkIndentJSON.Checked;
            Properties.Settings.Default.IndentJSON = Settings.IndentJSON;
        }

        private void ChkIndentXMLCheckedChanged(object sender, EventArgs e)
        {
            Settings.IndentXML = ChkIndentXML.Checked;
            Properties.Settings.Default.IndentXML = Settings.IndentXML;
        }

        private void ChkHtmlExternalScriptsCheckedChanged(object sender, EventArgs e)
        {
            Settings.HtmlExternalScripts = ChkHtmlExternalScripts.Checked;
            Properties.Settings.Default.HtmlExternalScripts = Settings.HtmlExternalScripts;
            LblHtmlExternalScriptsPath.Enabled = ChkHtmlExternalScripts.Checked;
            TxtHtmlExternalScriptsPath.Enabled = ChkHtmlExternalScripts.Checked;
            LblHtmlExternalScriptsCdn.Enabled = ChkHtmlExternalScripts.Checked;
            TxtHtmlExternalScriptsCdn.Enabled = ChkHtmlExternalScripts.Checked;
            BtnHtmlExternalScriptPathSelect.Enabled = ChkHtmlExternalScripts.Checked;
        }

        private void ChkHtmlCompressCheckedChanged(object sender, EventArgs e)
        {
            Settings.HtmlCompressJson = ChkHtmlCompressJson.Checked;
            Properties.Settings.Default.HtmlCompressJson = Settings.HtmlCompressJson;
        }

        private void RadioThemeLightCheckedChanged(object sender, EventArgs e)
        {
            Settings.LightTheme = RadioThemeLight.Checked;
            Properties.Settings.Default.LightTheme = Settings.LightTheme;
            PictureTheme.Image = Settings.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void RadioThemeDarkCheckedChanged(object sender, EventArgs e)
        {
            Settings.LightTheme = RadioThemeLight.Checked;
            Properties.Settings.Default.LightTheme = Settings.LightTheme;
            PictureTheme.Image = Settings.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ChkAutoAddCheckedChanged(object sender, EventArgs e)
        {
            if (ChkAutoAdd.Checked && !Properties.Settings.Default.AutoAdd)
            {
                string path = Properties.Settings.Default.AutoAddPath;
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
                        Properties.Settings.Default.AutoAddPath = fbd.SelectedPath;
                    }
                    else
                    {
                        ChkAutoAdd.Checked = false;
                    }
                }
            }
            Properties.Settings.Default.AutoAdd = ChkAutoAdd.Checked;
            WatchDirectoryUpdatedEvent(this, null);
        }

        private void ChkAutoParseCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoParse = ChkAutoParse.Checked;
        }

        private void ChkAddPoVProfCheckedChanged(object sender, EventArgs e)
        {
            Settings.AddPoVProf = ChkAddPoVProf.Checked;
            Properties.Settings.Default.AddPoVProf = Settings.AddPoVProf;
        }

        private void ChkCompressRawCheckedChanged(object sender, EventArgs e)
        {
            Settings.CompressRaw = ChkCompressRaw.Checked;
            Properties.Settings.Default.CompressRaw = Settings.CompressRaw;
        }

        private void ChkAddDurationCheckedChanged(object sender, EventArgs e)
        {
            Settings.AddDuration = ChkAddDuration.Checked;
            Properties.Settings.Default.AddDuration = Settings.AddDuration;
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
            Settings.Anonymous = ChkAnonymous.Checked;
            Properties.Settings.Default.Anonymous = Settings.Anonymous;
        }

        private void ChkDetailledWvWCheckedChange(object sender, EventArgs e)
        {
            Settings.DetailledWvW = ChkDetailledWvW.Checked;
            Properties.Settings.Default.DetailledWvW = Settings.DetailledWvW;
        }

        private void ChkSaveOutTraceCheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveOutTrace = ChkSaveOutTrace.Checked;
            Properties.Settings.Default.SaveOutTrace = Settings.SaveOutTrace;
        }

        private void ChkComputeDamageModsCheckedChanged(object sender, EventArgs e)
        {
            Settings.ComputeDamageModifiers = ChkDamageMods.Checked;
            Properties.Settings.Default.ComputeDamageModifiers = Settings.ComputeDamageModifiers;
        }

        private void ChkMultiLogsCheckedChanged(object sender, EventArgs e)
        {
            Settings.ParseMultipleLogs = ChkMultiLogs.Checked;
            Properties.Settings.Default.ParseMultipleLogs = Settings.ParseMultipleLogs;
            SetUIEnable();
        }

        private void ChkRawTimelineArraysCheckedChanged(object sender, EventArgs e)
        {
            Settings.RawTimelineArrays = ChkRawTimelineArrays.Checked;
            Properties.Settings.Default.RawTimelineArrays = Settings.RawTimelineArrays;
        }

        private void TxtHtmlExternalScriptsPathTextChanged(object sender, EventArgs e)
        {
            Settings.HtmlExternalScriptsPath = TxtHtmlExternalScriptsPath.Text.Trim();
            Properties.Settings.Default.HtmlExternalScriptsPath = Settings.HtmlExternalScriptsPath;
        }

        private void TxtHtmlExternalScriptCdnUrlTextChanged(object sender, EventArgs e)
        {
            Settings.HtmlExternalScriptsCdn = TxtHtmlExternalScriptsCdn.Text.Trim();
            Properties.Settings.Default.HtmlExternalScriptsCdn = Settings.HtmlExternalScriptsCdn;
        }

        private void BtnHtmlExternalScriptPathSelectClick(object sender, EventArgs e)
        {
            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (!string.IsNullOrWhiteSpace(Settings.HtmlExternalScriptsPath) && Directory.Exists(Settings.HtmlExternalScriptsPath))
                    {
                        fbd.ShowNewFolderButton = true;
                        fbd.SelectedPath = Settings.HtmlExternalScriptsPath;
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
}
