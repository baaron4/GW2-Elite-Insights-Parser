using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuckParser.Controllers;

namespace LuckParser
{
    public partial class SettingsForm : Form
    {
       // private bool[] settingArray;
       // private Form1 mainfrm;
        public SettingsForm(/*bool[] setArray,Form1 mnfrm*/)
        {
            InitializeComponent();
            //settingArray = setArray;
           // mainfrm = mnfrm;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DPSGraphTotals = checkBox1.Checked;
            //mainfrm.settingArray[0] = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerGraphTotals = checkBox2.Checked;
            //mainfrm.settingArray[1] = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerGraphBoss = checkBox3.Checked;
           // mainfrm.settingArray[2] = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerBoonsUniversal = checkBox4.Checked;
            //mainfrm.settingArray[3] = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerBoonsImpProf = checkBox5.Checked;
           // mainfrm.settingArray[4] = checkBox5.Checked;
        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerBoonsAllProf = checkBox6.Checked;
            //mainfrm.settingArray[5] = checkBox6.Checked;
        }
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerRot = checkBox8.Checked;
           // mainfrm.settingArray[7] = checkBox8.Checked;
        }
        
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayerRotIcons = checkBox7.Checked;
            // mainfrm.settingArray[6] = checkBox7.Checked;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = Properties.Settings.Default.DPSGraphTotals;
            checkBox2.Checked = Properties.Settings.Default.PlayerGraphTotals;
            checkBox3.Checked = Properties.Settings.Default.PlayerGraphBoss;
            checkBox4.Checked = Properties.Settings.Default.PlayerBoonsUniversal;
            checkBox5.Checked = Properties.Settings.Default.PlayerBoonsImpProf;
            checkBox6.Checked = Properties.Settings.Default.PlayerBoonsAllProf;
            checkBox8.Checked = Properties.Settings.Default.PlayerRot;
            checkBox7.Checked = Properties.Settings.Default.PlayerRotIcons;
            checkOutputLoc.Checked =Properties.Settings.Default.SaveAtOut;
            checkBox9.Checked = Properties.Settings.Default.EventList ;
            outFolderTextBox.Text = Properties.Settings.Default.OutLocation;
            checkBossSummary.Checked = Properties.Settings.Default.BossSummary;
            checkBox10.Checked = Properties.Settings.Default.ShowAutos;
            checkBox11.Checked = Properties.Settings.Default.LargeRotIcons;
            saveoutHTML.Checked = Properties.Settings.Default.SaveOutHTML;
            saveoutCSV.Checked = Properties.Settings.Default.SaveOutCSV;
        }

        private void checkOutputLoc_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveAtOut = checkOutputLoc.Checked;
        }

        private void btnFolderSelect_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //string[] files = Directory.GetFiles(fbd.SelectedPath);

                    // System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                    outFolderTextBox.Text = fbd.SelectedPath;
                    Properties.Settings.Default.OutLocation = fbd.SelectedPath;
                }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EventList = checkBox9.Checked;
        }

        private void outFolderTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBossSummary_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BossSummary = checkBossSummary.Checked;
        }

        private void SimpleRotCheck_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SimpleRotation = SimpleRotCheck.Checked;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowAutos = SimpleRotCheck.Checked;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LargeRotIcons = SimpleRotCheck.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController tempcontroller = new GW2APIController();
            tempcontroller.WriteSkillListToFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Update skill list
            GW2APIController tempcontroller = new GW2APIController();
            tempcontroller.RetryWriteSkillListtoFile();
        }

        private void saveoutHTML_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutHTML = saveoutHTML.Checked;
        }

        private void saveoutCSV_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOutCSV = saveoutCSV.Checked;
        }
    }
}
