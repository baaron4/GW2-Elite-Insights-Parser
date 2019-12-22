using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GW2EIParser.Controllers;

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

        private void SetValues()
        {

            chkDefaultOutputLoc.Checked = Properties.Settings.Default.SaveAtOut;
            txtCustomSaveLoc.Text = Properties.Settings.Default.OutLocation;
            chkOutputHtml.Checked = Properties.Settings.Default.SaveOutHTML;
            chkOutputCsv.Checked = Properties.Settings.Default.SaveOutCSV;
            chkPhaseParsing.Checked = Properties.Settings.Default.ParsePhases;
            chkOneAtATime.Checked = Properties.Settings.Default.ParseOneAtATime;
            radioThemeLight.Checked = Properties.Settings.Default.LightTheme;
            radioThemeDark.Checked = !Properties.Settings.Default.LightTheme;
            imgTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
            chkCombatReplay.Checked = Properties.Settings.Default.ParseCombatReplay;
            chkOutputJson.Checked = Properties.Settings.Default.SaveOutJSON;
            chkIndentJSON.Checked = Properties.Settings.Default.IndentJSON;
            chkOutputXml.Checked = Properties.Settings.Default.SaveOutXML;
            chkIndentXML.Checked = Properties.Settings.Default.IndentXML;
            UploadDPSReports_checkbox.Checked = Properties.Settings.Default.UploadToDPSReports;
            UploadDRRH_check.Checked = Properties.Settings.Default.UploadToDPSReportsRH;
            UploadRaidar_check.Checked = Properties.Settings.Default.UploadToRaidar;
            chkB_SkipFailedTries.Checked = Properties.Settings.Default.SkipFailedTries;
            chkAutoAdd.Checked = Properties.Settings.Default.AutoAdd;
            chkAutoParse.Checked = Properties.Settings.Default.AutoParse;
            chkAddPoVProf.Checked = Properties.Settings.Default.AddPoVProf;
            chkCompressRaw.Checked = Properties.Settings.Default.CompressRaw;
            chkAddDuration.Checked = Properties.Settings.Default.AddDuration;
            chkAnonymous.Checked = Properties.Settings.Default.Anonymous;
            chkDamageMods.Checked = Properties.Settings.Default.ComputeDamageMods;
            chkTimelineArrays.Checked = Properties.Settings.Default.AddTimelineArrays;

            chkHtmlExternalScripts.Checked = Properties.Settings.Default.HtmlExternalScripts;

            panelHtml.Enabled = Properties.Settings.Default.SaveOutHTML;
            panelJson.Enabled = Properties.Settings.Default.SaveOutJSON;
            panelXML.Enabled = Properties.Settings.Default.SaveOutXML;
            groupRawSettings.Enabled = Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML;
        }

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            SetValues();
            toolTip1.SetToolTip(chkHtmlExternalScripts, "Writes static css and js scripts in own files, which are shared between all logs. Log file size decreases, but the script files have to be kept along with the html.");
        }

        private void DefaultOutputLocationCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveAtOut = chkDefaultOutputLoc.Checked;
        }

        private void BtnFolderSelectClick(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //string[] files = Directory.GetFiles(fbd.SelectedPath);

                    // System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                    txtCustomSaveLoc.Text = fbd.SelectedPath;
                    Properties.Settings.Default.OutLocation = fbd.SelectedPath;
                }
            }
        }

        private void CustomSaveLocationTextChanged(object sender, EventArgs e)
        {

        }

        private void ResetSkillListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController.WriteSkillListToFile();
            MessageBox.Show("Skill List has been redone");
        }

        private void ResetTraitListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController.WriteTraitListToFile();
            MessageBox.Show("Trait List has been redone");
        }

        private void ResetSpecListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController.WriteSpecListToFile();
            MessageBox.Show("Spec List has been redone");
        }

        private void OuputCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutHTML = chkOutputHtml.Checked;
            panelHtml.Enabled = Properties.Settings.Default.SaveOutHTML;
        }

        private void OutputCsvCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutCSV = chkOutputCsv.Checked;
        }

        private void ChkOneAtATimeCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParseOneAtATime = chkOneAtATime.Checked;
        }

        private void PhaseParsingCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParsePhases = chkPhaseParsing.Checked;
        }

        private void ChkCombatReplayCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParseCombatReplay = chkCombatReplay.Checked;
        }

        private void UploadDPSReports_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UploadToDPSReports = UploadDPSReports_checkbox.Checked;
        }

        private void UploadRaidar_check_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UploadToRaidar = UploadRaidar_check.Checked;
        }

        private void UploadDRRH_check_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UploadToDPSReportsRH = UploadDRRH_check.Checked;
        }

        private void ChkB_SkipFailedTries_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SkipFailedTries = chkB_SkipFailedTries.Checked;
        }
        private void OutputJSONCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutJSON = chkOutputJson.Checked;
            panelJson.Enabled = Properties.Settings.Default.SaveOutJSON;
            groupRawSettings.Enabled = Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML;
        }

        private void OutputXMLCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutXML = chkOutputXml.Checked;
            panelXML.Enabled = Properties.Settings.Default.SaveOutXML;
            groupRawSettings.Enabled = Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML;
        }

        private void ChkIndentJSONCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IndentJSON = chkIndentJSON.Checked;
        }

        private void ChkIndentXMLCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IndentXML = chkIndentXML.Checked;
        }

        private void ChkHtmlExternalScripts_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HtmlExternalScripts = chkHtmlExternalScripts.Checked;
        }

        private void RadioThemeLight_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LightTheme = radioThemeLight.Checked;
            imgTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void RadioThemeDark_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LightTheme = radioThemeLight.Checked;
            imgTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void CmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChkAutoAdd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoAdd.Checked && !Properties.Settings.Default.AutoAdd)
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
                        chkAutoAdd.Checked = false;
                    }
                }
            }
            Properties.Settings.Default.AutoAdd = chkAutoAdd.Checked;
            WatchDirectoryUpdatedEvent(this, null);
        }

        private void ChkAutoParse_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoParse = chkAutoParse.Checked;
        }

        private void ChkAddPoVProf_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AddPoVProf = chkAddPoVProf.Checked;
        }
        private void ChkCompressRaw_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CompressRaw = chkCompressRaw.Checked;
        }

        private void ChkAddDuration_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AddDuration = chkAddDuration.Checked;
        }

        private void SettingsDump_Click(object sender, EventArgs e)
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

        private void SettingsLoad_Click(object sender, EventArgs e)
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

        private void ChkAnonymous_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Anonymous = chkAnonymous.Checked;
        }

        private void ChkComputeDamageMods_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ComputeDamageMods = chkDamageMods.Checked;
        }

        private void ChkTimelineArrays_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AddTimelineArrays = chkTimelineArrays.Checked;
        }
    }
}
