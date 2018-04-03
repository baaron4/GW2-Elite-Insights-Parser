using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            //checkBox1.Checked = settingArray[0];
            //checkBox2.Checked = settingArray[1];
            //checkBox3.Checked = settingArray[2];
            //checkBox4.Checked = settingArray[3];
            //checkBox5.Checked = settingArray[4];
            //checkBox6.Checked = settingArray[5];
            //checkBox8.Checked = settingArray[6];
            //checkBox7.Checked = settingArray[7];
        }
    }
}
