using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GW2EIGW2API;

namespace GW2EIParser.Setting
{
    public partial class SettingsForm : Form
    {
        public event EventHandler SettingsClosedEvent;
        public event EventHandler WatchDirectoryUpdatedEvent;

        public SettingsForm()
        {
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
            ChkMultiThreaded.Enabled = !busy;
            ChkMultiLogs.Enabled = !busy;
            ChkUploadDPSReports.Enabled = !busy;
            ChkUploadDRRH.Enabled = !busy;
            TxtDPSReportUserToken.Enabled = !busy;
            BtnResetSkillList.Enabled = !busy;
            BtnResetSpecList.Enabled = !busy;
            BtnResetTraitList.Enabled = !busy;
            BtnLoadSettings.Enabled = !busy;
            GroupWebhookSettings.Enabled = !busy;
        }

        private void SetUIEnable()
        {
            ChkUploadRaidar.Enabled = false;
            PanelHtml.Enabled = Properties.Settings.Default.SaveOutHTML;
            PanelJson.Enabled = Properties.Settings.Default.SaveOutJSON;
            PanelXML.Enabled = Properties.Settings.Default.SaveOutXML;
            GroupRawSettings.Enabled = Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML;
        }

        private void SetValues()
        {

            ChkDefaultOutputLoc.Checked = Properties.Settings.Default.SaveAtOut;
            TxtCustomSaveLoc.Text = Properties.Settings.Default.OutLocation;
            NumericCustomTooShort.Value = Properties.Settings.Default.CustomTooShort;
            ChkOutputHtml.Checked = Properties.Settings.Default.SaveOutHTML;
            ChkOutputCsv.Checked = Properties.Settings.Default.SaveOutCSV;
            ChkPhaseParsing.Checked = Properties.Settings.Default.ParsePhases;
            ChkMultiThreaded.Checked = Properties.Settings.Default.MultiThreaded;
            RadioThemeLight.Checked = Properties.Settings.Default.LightTheme;
            RadioThemeDark.Checked = !Properties.Settings.Default.LightTheme;
            PictureTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
            ChkCombatReplay.Checked = Properties.Settings.Default.ParseCombatReplay;
            ChkOutputJson.Checked = Properties.Settings.Default.SaveOutJSON;
            ChkIndentJSON.Checked = Properties.Settings.Default.IndentJSON;
            ChkOutputXml.Checked = Properties.Settings.Default.SaveOutXML;
            ChkIndentXML.Checked = Properties.Settings.Default.IndentXML;
            ChkUploadDPSReports.Checked = Properties.Settings.Default.UploadToDPSReports;
            TxtDPSReportUserToken.Text = Properties.Settings.Default.DPSReportUserToken;
            ChkUploadDRRH.Checked = Properties.Settings.Default.UploadToDPSReportsRH;
            ChkUploadRaidar.Checked = Properties.Settings.Default.UploadToRaidar;
            ChkUploadWebhook.Checked = Properties.Settings.Default.SendEmbedToWebhook;
            ChkUploadSimpleMessageWebhook.Checked = Properties.Settings.Default.SendSimpleMessageToWebhook;
            TxtUploadWebhookUrl.Text = Properties.Settings.Default.WebhookURL;
            ChkSkipFailedTries.Checked = Properties.Settings.Default.SkipFailedTries;
            ChkAutoAdd.Checked = Properties.Settings.Default.AutoAdd;
            ChkAutoParse.Checked = Properties.Settings.Default.AutoParse;
            ChkAddPoVProf.Checked = Properties.Settings.Default.AddPoVProf;
            ChkCompressRaw.Checked = Properties.Settings.Default.CompressRaw;
            ChkAddDuration.Checked = Properties.Settings.Default.AddDuration;
            ChkAnonymous.Checked = Properties.Settings.Default.Anonymous;
            ChkSaveOutTrace.Checked = Properties.Settings.Default.SaveOutTrace;
            ChkDamageMods.Checked = Properties.Settings.Default.ComputeDamageModifiers;
            ChkMultiLogs.Checked = Properties.Settings.Default.ParseMultipleLogs;
            ChkRawTimelineArrays.Checked = Properties.Settings.Default.RawTimelineArrays;

            ChkHtmlExternalScripts.Checked = Properties.Settings.Default.HtmlExternalScripts;

            SetUIEnable();
        }

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            SetValues();
        }

        private void ChkDefaultOutputLocationCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveAtOut = ChkDefaultOutputLoc.Checked;
        }

        private void BtnFolderSelectClick(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    TxtCustomSaveLoc.Text = fbd.SelectedPath;
                }
            }
        }

        private void TxtCustomSaveLocationTextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OutLocation = TxtCustomSaveLoc.Text;
        }

        private void NumericCustomTooShortValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CustomTooShort = (long)NumericCustomTooShort.Value;
        }

        private void TxtWebhookURLChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WebhookURL = TxtUploadWebhookUrl.Text;
        }

        private void BtnResetSkillListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController.WriteAPISkillsToFile(ProgramHelper.SkillAPICacheLocation);
            MessageBox.Show("Skill List has been redone");
        }

        private void BtnResetTraitListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController.WriteAPITraitsToFile(ProgramHelper.TraitAPICacheLocation);
            MessageBox.Show("Trait List has been redone");
        }

        private void BtnResetSpecListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController.WriteAPISpecsToFile(ProgramHelper.SpecAPICacheLocation);
            MessageBox.Show("Spec List has been redone");
        }

        private void ChkOuputHTMLCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutHTML = ChkOutputHtml.Checked;
            PanelHtml.Enabled = Properties.Settings.Default.SaveOutHTML;
        }

        private void ChkOutputCsvCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutCSV = ChkOutputCsv.Checked;
        }

        private void ChkMultiThreadedCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MultiThreaded = ChkMultiThreaded.Checked;
        }

        private void ChkPhaseParsingCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParsePhases = ChkPhaseParsing.Checked;
        }

        private void ChkCombatReplayCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParseCombatReplay = ChkCombatReplay.Checked;
        }

        private void ChkUploadDPSReportsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UploadToDPSReports = ChkUploadDPSReports.Checked;
            SetUIEnable();
        }

        private void ChkDPSReportUserTokenTextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DPSReportUserToken = TxtDPSReportUserToken.Text;
        }

        private void ChkUploadRaidarCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UploadToRaidar = ChkUploadRaidar.Checked;
        }

        private void ChkUploadWebhookCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendEmbedToWebhook = ChkUploadWebhook.Checked;
        }

        private void ChkUploadSimpleMessageWebhookCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SendSimpleMessageToWebhook = ChkUploadSimpleMessageWebhook.Checked;
        }

        private void ChkUploadDRRHCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UploadToDPSReportsRH = ChkUploadDRRH.Checked;
        }

        private void ChkSkipFailedTriesCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SkipFailedTries = ChkSkipFailedTries.Checked;
        }
        private void ChkOutputJSONCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutJSON = ChkOutputJson.Checked;
            SetUIEnable();
        }

        private void ChkOutputXMLCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutXML = ChkOutputXml.Checked;
            SetUIEnable();
        }

        private void ChkIndentJSONCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IndentJSON = ChkIndentJSON.Checked;
        }

        private void ChkIndentXMLCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IndentXML = ChkIndentXML.Checked;
        }

        private void ChkHtmlExternalScriptsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HtmlExternalScripts = ChkHtmlExternalScripts.Checked;
        }

        private void RadioThemeLightCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LightTheme = RadioThemeLight.Checked;
            PictureTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void RadioThemeDarkCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LightTheme = RadioThemeLight.Checked;
            PictureTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
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
            Properties.Settings.Default.AddPoVProf = ChkAddPoVProf.Checked;
        }
        private void ChkCompressRawCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CompressRaw = ChkCompressRaw.Checked;
        }

        private void ChkAddDurationCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AddDuration = ChkAddDuration.Checked;
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
                    SetValues();
                }
            }
        }

        private void ChkAnonymousCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Anonymous = ChkAnonymous.Checked;
        }

        private void ChkSaveOutTraceCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutTrace = ChkSaveOutTrace.Checked;
        }

        private void ChkComputeDamageModsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ComputeDamageModifiers = ChkDamageMods.Checked;
        }

        private void ChkMultiLogsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParseMultipleLogs = ChkMultiLogs.Checked;
            SetUIEnable();
        }

        private void ChkRawTimelineArraysCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RawTimelineArrays = ChkRawTimelineArrays.Checked;
        }
    }
}
