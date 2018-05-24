namespace LuckParser
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkOutputLoc = new System.Windows.Forms.CheckBox();
            this.outFolderTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFolderSelect = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBossSummary = new System.Windows.Forms.CheckBox();
            this.SimpleRotCheck = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.saveoutHTML = new System.Windows.Forms.CheckBox();
            this.saveoutCSV = new System.Windows.Forms.CheckBox();
            this.estimatesBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Damage Graph";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(43, 55);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(190, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Show Each Players Total DPS plot";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Player Summary";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(43, 100);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(126, 17);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "Show Total DPS Plot";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(43, 123);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(125, 17);
            this.checkBox3.TabIndex = 4;
            this.checkBox3.Text = "Show Boss DPS Plot";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Location = new System.Drawing.Point(43, 168);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(133, 17);
            this.checkBox4.TabIndex = 5;
            this.checkBox4.Text = "Show Universal Boons";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Boons Graph";
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Checked = true;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.Location = new System.Drawing.Point(43, 191);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(228, 17);
            this.checkBox5.TabIndex = 7;
            this.checkBox5.Text = "Show Important Profession specefic Boons";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox5_CheckedChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(43, 214);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(195, 17);
            this.checkBox6.TabIndex = 8;
            this.checkBox6.Text = "Show All Profession specefic Boons";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 245);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Rotation Graph";
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Checked = true;
            this.checkBox7.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox7.Location = new System.Drawing.Point(43, 295);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(104, 17);
            this.checkBox7.TabIndex = 10;
            this.checkBox7.Text = "Show Skill Icons";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.checkBox7_CheckedChanged);
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Checked = true;
            this.checkBox8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox8.Location = new System.Drawing.Point(43, 272);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(96, 17);
            this.checkBox8.TabIndex = 11;
            this.checkBox8.Text = "Parse Rotation";
            this.checkBox8.UseVisualStyleBackColor = true;
            this.checkBox8.CheckedChanged += new System.EventHandler(this.checkBox8_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(240, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "*Settings will not alter file in the middle of parseing";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 499);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Output:";
            // 
            // checkOutputLoc
            // 
            this.checkOutputLoc.AutoSize = true;
            this.checkOutputLoc.Checked = true;
            this.checkOutputLoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOutputLoc.Location = new System.Drawing.Point(27, 515);
            this.checkOutputLoc.Name = "checkOutputLoc";
            this.checkOutputLoc.Size = new System.Drawing.Size(192, 17);
            this.checkOutputLoc.TabIndex = 14;
            this.checkOutputLoc.Text = "Save Output in same folder as evtc";
            this.checkOutputLoc.UseVisualStyleBackColor = true;
            this.checkOutputLoc.CheckedChanged += new System.EventHandler(this.checkOutputLoc_CheckedChanged);
            // 
            // outFolderTextBox
            // 
            this.outFolderTextBox.Enabled = false;
            this.outFolderTextBox.Location = new System.Drawing.Point(24, 552);
            this.outFolderTextBox.Name = "outFolderTextBox";
            this.outFolderTextBox.Size = new System.Drawing.Size(412, 20);
            this.outFolderTextBox.TabIndex = 15;
            this.outFolderTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.outFolderTextBox.TextChanged += new System.EventHandler(this.outFolderTextBox_TextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(461, 549);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(45, 23);
            this.btnFolderSelect.TabIndex = 16;
            this.btnFolderSelect.Text = "Select";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.btnFolderSelect_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(100, 533);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Or save at:";
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Location = new System.Drawing.Point(27, 479);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(174, 17);
            this.checkBox9.TabIndex = 18;
            this.checkBox9.Text = "Show Event List(for debugging)";
            this.checkBox9.UseVisualStyleBackColor = true;
            this.checkBox9.CheckedChanged += new System.EventHandler(this.checkBox9_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(27, 392);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Boss";
            // 
            // checkBossSummary
            // 
            this.checkBossSummary.AutoSize = true;
            this.checkBossSummary.Checked = true;
            this.checkBossSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBossSummary.Location = new System.Drawing.Point(46, 417);
            this.checkBossSummary.Name = "checkBossSummary";
            this.checkBossSummary.Size = new System.Drawing.Size(125, 17);
            this.checkBossSummary.TabIndex = 20;
            this.checkBossSummary.Text = "Show Boss Summary";
            this.checkBossSummary.UseVisualStyleBackColor = true;
            this.checkBossSummary.CheckedChanged += new System.EventHandler(this.checkBossSummary_CheckedChanged);
            // 
            // SimpleRotCheck
            // 
            this.SimpleRotCheck.AutoSize = true;
            this.SimpleRotCheck.Checked = true;
            this.SimpleRotCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SimpleRotCheck.Location = new System.Drawing.Point(43, 318);
            this.SimpleRotCheck.Name = "SimpleRotCheck";
            this.SimpleRotCheck.Size = new System.Drawing.Size(152, 17);
            this.SimpleRotCheck.TabIndex = 21;
            this.SimpleRotCheck.Text = "Show Simple Rotation Tab";
            this.SimpleRotCheck.UseVisualStyleBackColor = true;
            this.SimpleRotCheck.CheckedChanged += new System.EventHandler(this.SimpleRotCheck_CheckedChanged);
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Checked = true;
            this.checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox10.Location = new System.Drawing.Point(49, 341);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(117, 17);
            this.checkBox10.TabIndex = 22;
            this.checkBox10.Text = "Show Auto Attacks";
            this.checkBox10.UseVisualStyleBackColor = true;
            this.checkBox10.CheckedChanged += new System.EventHandler(this.checkBox10_CheckedChanged);
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(49, 364);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(104, 17);
            this.checkBox11.TabIndex = 23;
            this.checkBox11.Text = "Large Skill Icons";
            this.checkBox11.UseVisualStyleBackColor = true;
            this.checkBox11.CheckedChanged += new System.EventHandler(this.checkBox11_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(428, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "Reset Skill List";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(428, 49);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 23);
            this.button2.TabIndex = 25;
            this.button2.Text = "Retry Skill List";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // saveoutHTML
            // 
            this.saveoutHTML.AutoSize = true;
            this.saveoutHTML.Checked = true;
            this.saveoutHTML.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveoutHTML.Location = new System.Drawing.Point(24, 587);
            this.saveoutHTML.Name = "saveoutHTML";
            this.saveoutHTML.Size = new System.Drawing.Size(105, 17);
            this.saveoutHTML.TabIndex = 26;
            this.saveoutHTML.Text = "Output as HTML";
            this.saveoutHTML.UseVisualStyleBackColor = true;
            this.saveoutHTML.CheckedChanged += new System.EventHandler(this.saveoutHTML_CheckedChanged);
            // 
            // saveoutCSV
            // 
            this.saveoutCSV.AutoSize = true;
            this.saveoutCSV.Location = new System.Drawing.Point(24, 611);
            this.saveoutCSV.Name = "saveoutCSV";
            this.saveoutCSV.Size = new System.Drawing.Size(96, 17);
            this.saveoutCSV.TabIndex = 27;
            this.saveoutCSV.Text = "Output as CSV";
            this.saveoutCSV.UseVisualStyleBackColor = true;
            this.saveoutCSV.CheckedChanged += new System.EventHandler(this.saveoutCSV_CheckedChanged);
            // 
            // estimatesBox
            // 
            this.estimatesBox.AutoSize = true;
            this.estimatesBox.Checked = true;
            this.estimatesBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.estimatesBox.Location = new System.Drawing.Point(30, 440);
            this.estimatesBox.Name = "estimatesBox";
            this.estimatesBox.Size = new System.Drawing.Size(123, 17);
            this.estimatesBox.TabIndex = 28;
            this.estimatesBox.Text = "Show Estimates Tab";
            this.estimatesBox.UseVisualStyleBackColor = true;
            this.estimatesBox.CheckedChanged += new System.EventHandler(this.estimatesBox_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(577, 368);
            this.Controls.Add(this.estimatesBox);
            this.Controls.Add(this.saveoutCSV);
            this.Controls.Add(this.saveoutHTML);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox11);
            this.Controls.Add(this.checkBox10);
            this.Controls.Add(this.SimpleRotCheck);
            this.Controls.Add(this.checkBossSummary);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBox9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnFolderSelect);
            this.Controls.Add(this.outFolderTextBox);
            this.Controls.Add(this.checkOutputLoc);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox8);
            this.Controls.Add(this.checkBox7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBox6);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkOutputLoc;
        private System.Windows.Forms.TextBox outFolderTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFolderSelect;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBossSummary;
        private System.Windows.Forms.CheckBox SimpleRotCheck;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.CheckBox checkBox11;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox saveoutHTML;
        private System.Windows.Forms.CheckBox saveoutCSV;
        private System.Windows.Forms.CheckBox estimatesBox;
    }
}