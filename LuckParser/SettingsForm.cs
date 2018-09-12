using LuckParser.Controllers;
using System;
using System.Windows.Forms;

namespace LuckParser
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void PlayerDpsPlotCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DPSGraphTotals = chkPlayerDpsPlot.Checked;
            //mainfrm.settingArray[0] = checkBox1.Checked;
        }

        private void UniversalBoonsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerBoonsUniversal = chkUniversalBoons.Checked;
            //mainfrm.settingArray[3] = checkBox4.Checked;
        }

        private void ImportantProfessionSpecificBoonsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerBoonsImpProf = chkImpProfSpecBoons.Checked;
           // mainfrm.settingArray[4] = checkBox5.Checked;
        }
        private void AllProfessionSpecificBoonsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerBoonsAllProf = chkAllProfSpecBoons.Checked;
            //mainfrm.settingArray[5] = checkBox6.Checked;
        }
        private void RotationCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerRot = chkRotation.Checked;
            chkSkillIcons.Enabled = Properties.Settings.Default.PlayerRot;
        }
        
        private void SkillIconsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerRotIcons = chkSkillIcons.Checked;
            // mainfrm.settingArray[6] = checkBox7.Checked;
        }

        private void SettingsFormFormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void SettingsFormLoad(object sender, EventArgs e)
        {
            chkPlayerDpsPlot.Checked = Properties.Settings.Default.DPSGraphTotals;
            chkShowCl.Checked = Properties.Settings.Default.ClDPSGraphTotals;
            chkUniversalBoons.Checked = Properties.Settings.Default.PlayerBoonsUniversal;
            chkImpProfSpecBoons.Checked = Properties.Settings.Default.PlayerBoonsImpProf;
            chkAllProfSpecBoons.Checked = Properties.Settings.Default.PlayerBoonsAllProf;
            chkRotation.Checked = Properties.Settings.Default.PlayerRot;
            chkSkillIcons.Checked = Properties.Settings.Default.PlayerRotIcons;
            chkSkillIcons.Enabled = Properties.Settings.Default.PlayerRot;
            chkDefaultOutputLoc.Checked =Properties.Settings.Default.SaveAtOut;
            chkEventListDbg.Checked = Properties.Settings.Default.EventList ;
            txtCustomSaveLoc.Text = Properties.Settings.Default.OutLocation;
            checkBossSummary.Checked = Properties.Settings.Default.BossSummary;
            chkShowAutos.Checked = Properties.Settings.Default.ShowAutos;
            chkShowAutos.Enabled = Properties.Settings.Default.SimpleRotation;
            chkLargeSkillIcons.Checked = Properties.Settings.Default.LargeRotIcons;
            chkLargeSkillIcons.Enabled = Properties.Settings.Default.SimpleRotation;
            chkOutputHtml.Checked = Properties.Settings.Default.SaveOutHTML;
            chkOutputCsv.Checked = Properties.Settings.Default.SaveOutCSV;  
            chkShowEstimates.Checked = Properties.Settings.Default.ShowEstimates;
            chkPhaseParsing.Checked = Properties.Settings.Default.ParsePhases;
            chkShow10s.Checked = Properties.Settings.Default.Show10s;
            chkShow30s.Checked = Properties.Settings.Default.Show30s;
            chkOneAtATime.Checked = Properties.Settings.Default.ParseOneAtATime;
            radioThemeLight.Checked = Properties.Settings.Default.LightTheme;
            radioThemeDark.Checked = !Properties.Settings.Default.LightTheme;
            imgTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
            chkSimpleRotationTab.Checked = Properties.Settings.Default.SimpleRotation;
            chkCombatReplay.Checked = Properties.Settings.Default.ParseCombatReplay;
            chkOutputJson.Checked = Properties.Settings.Default.SaveOutJSON;
            chkIndentJSON.Checked = Properties.Settings.Default.IndentJSON;
            UploadDPSReports_checkbox.Checked = Properties.Settings.Default.UploadToDPSReports;
            UploadDRRH_check.Checked = Properties.Settings.Default.UploadToDPSReportsRH;
            UploadRaidar_check.Checked = Properties.Settings.Default.UploadToRaidar;
            chkB_SkipFailedTrys.Checked = Properties.Settings.Default.SkipFailedTrys;

            chkHtmlExperimental.Checked = Properties.Settings.Default.NewHtmlMode;
            toolTip1.SetToolTip(chkHtmlExperimental, "Alternative method to build the HTML page.\nThe page is much smaller, and some static CSS and JS scripts are written in an external file.");

            chkHtmlExternalScripts.Checked = Properties.Settings.Default.NewHtmlExternalScripts;
            chkHtmlExternalScripts.Enabled = Properties.Settings.Default.NewHtmlMode;
            toolTip1.SetToolTip(chkHtmlExternalScripts, "Writes static css and js scripts in own files, which are shared between all logs. Log file size decreases, but the script files have to be kept along with the html.");

            panelHtml.Enabled = Properties.Settings.Default.SaveOutHTML;
            panelJson.Enabled = Properties.Settings.Default.SaveOutJSON;
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

        private void EventListDebugCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EventList = chkEventListDbg.Checked;
        }

        private void CustomSaveLocationTextChanged(object sender, EventArgs e)
        {

        }

        private void BossSummaryCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BossSummary = checkBossSummary.Checked;
        }

        private void SimpleRotationTabCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SimpleRotation = chkSimpleRotationTab.Checked;
            chkShowAutos.Enabled = Properties.Settings.Default.SimpleRotation;
            chkLargeSkillIcons.Enabled = Properties.Settings.Default.SimpleRotation;
        }

        private void ShowAutosCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowAutos = chkShowAutos.Checked;
        }

        private void LargeSkillIconsCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LargeRotIcons = chkLargeSkillIcons.Checked;
        }

        private void ResetSkillListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController tempcontroller = new GW2APIController();
            tempcontroller.WriteSkillListToFile();
        }

        private void RetrySkillListClick(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController tempcontroller = new GW2APIController();
            tempcontroller.RetryWriteSkillListtoFile();
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

        private void ShowEstimatesCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowEstimates = chkShowEstimates.Checked;
        }

        private void ChkOneAtATimeCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParseOneAtATime = chkOneAtATime.Checked;
        }

        private void PhaseParsingCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParsePhases = chkPhaseParsing.Checked;
        }

        private void Show10sCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Show10s = chkShow10s.Checked;
        }

        private void Show30sCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Show30s = chkShow30s.Checked;
        }

        private void ChkCombatReplayCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParseCombatReplay = chkCombatReplay.Checked;
        }

        private void ShowClCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ClDPSGraphTotals = chkShowCl.Checked;
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

        private void chkB_SkipFailedTrys_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SkipFailedTrys = chkB_SkipFailedTrys.Checked;
        }
        private void OutputJSONCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutJSON = chkOutputJson.Checked;
            panelJson.Enabled = Properties.Settings.Default.SaveOutJSON;
        }

        private void chkIndentJSONCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.IndentJSON = chkIndentJSON.Checked;
        }

        private void chkHtmlExperimental_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NewHtmlMode = chkHtmlExperimental.Checked;
            chkHtmlExternalScripts.Enabled = Properties.Settings.Default.NewHtmlMode;
        }

        private void chkHtmlExternalScripts_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NewHtmlExternalScripts = chkHtmlExternalScripts.Checked;
        }

        private void radioThemeLight_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LightTheme = radioThemeLight.Checked;
            imgTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void radioThemeDark_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LightTheme = radioThemeLight.Checked;
            imgTheme.Image = Properties.Settings.Default.LightTheme ? Properties.Resources.theme_cosmo : Properties.Resources.theme_slate;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
