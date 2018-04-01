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
        private bool[] settingArray;
        private Form1 mainfrm;
        public SettingsForm(bool[] setArray,Form1 mnfrm)
        {
            InitializeComponent();
            settingArray = setArray;
            mainfrm = mnfrm;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
           mainfrm.settingArray[0] = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[1] = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[2] = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[3] = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[4] = checkBox5.Checked;
        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[5] = checkBox6.Checked;
        }
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[7] = checkBox8.Checked;
        }
        
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            mainfrm.settingArray[6] = checkBox7.Checked;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = settingArray[0];
            checkBox2.Checked = settingArray[1];
            checkBox3.Checked = settingArray[2];
            checkBox4.Checked = settingArray[3];
            checkBox5.Checked = settingArray[4];
            checkBox6.Checked = settingArray[5];
            checkBox8.Checked = settingArray[6];
            checkBox7.Checked = settingArray[7];
        }
    }
}
