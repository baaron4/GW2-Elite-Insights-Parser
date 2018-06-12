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
            this.lblDamageGraphSection = new System.Windows.Forms.Label();
            this.chkPlayerDpsPlot = new System.Windows.Forms.CheckBox();
            this.lblPlayerSummarySection = new System.Windows.Forms.Label();
            this.chkTotalDpsPlot = new System.Windows.Forms.CheckBox();
            this.chkBossDpsPlot = new System.Windows.Forms.CheckBox();
            this.chkUniversalBoons = new System.Windows.Forms.CheckBox();
            this.lblBoonGraphSection = new System.Windows.Forms.Label();
            this.chkImpProfSpecBoons = new System.Windows.Forms.CheckBox();
            this.chkAllProfSpecBoons = new System.Windows.Forms.CheckBox();
            this.lblRotationGraphSection = new System.Windows.Forms.Label();
            this.chkSkillIcons = new System.Windows.Forms.CheckBox();
            this.chkRotation = new System.Windows.Forms.CheckBox();
            this.lblSettingsInfoMsg = new System.Windows.Forms.Label();
            this.lblOutputSection = new System.Windows.Forms.Label();
            this.chkDefaultOutputLoc = new System.Windows.Forms.CheckBox();
            this.txtCustomSaveLoc = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFolderSelect = new System.Windows.Forms.Button();
            this.lblCustomSaveLoc = new System.Windows.Forms.Label();
            this.chkEventListDbg = new System.Windows.Forms.CheckBox();
            this.lblBossSection = new System.Windows.Forms.Label();
            this.checkBossSummary = new System.Windows.Forms.CheckBox();
            this.chkSimpleRotationTab = new System.Windows.Forms.CheckBox();
            this.chkShowAutos = new System.Windows.Forms.CheckBox();
            this.chkLargeSkillIcons = new System.Windows.Forms.CheckBox();
            this.btnResetSkillList = new System.Windows.Forms.Button();
            this.btnRetrySkillList = new System.Windows.Forms.Button();
            this.chkOutputHtml = new System.Windows.Forms.CheckBox();
            this.chkOutputCsv = new System.Windows.Forms.CheckBox();
            this.chkShowEstimates = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblDamageGraphSection
            // 
            this.lblDamageGraphSection.AutoSize = true;
            this.lblDamageGraphSection.Location = new System.Drawing.Point(24, 39);
            this.lblDamageGraphSection.Name = "lblDamageGraphSection";
            this.lblDamageGraphSection.Size = new System.Drawing.Size(79, 13);
            this.lblDamageGraphSection.TabIndex = 0;
            this.lblDamageGraphSection.Text = "Damage Graph";
            // 
            // chkPlayerDpsPlot
            // 
            this.chkPlayerDpsPlot.AutoSize = true;
            this.chkPlayerDpsPlot.Checked = true;
            this.chkPlayerDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPlayerDpsPlot.Location = new System.Drawing.Point(43, 55);
            this.chkPlayerDpsPlot.Name = "chkPlayerDpsPlot";
            this.chkPlayerDpsPlot.Size = new System.Drawing.Size(190, 17);
            this.chkPlayerDpsPlot.TabIndex = 1;
            this.chkPlayerDpsPlot.Text = "Show Each Players Total DPS plot";
            this.chkPlayerDpsPlot.UseVisualStyleBackColor = true;
            this.chkPlayerDpsPlot.CheckedChanged += new System.EventHandler(this.PlayerDpsPlot_CheckedChanged);
            // 
            // lblPlayerSummarySection
            // 
            this.lblPlayerSummarySection.AutoSize = true;
            this.lblPlayerSummarySection.Location = new System.Drawing.Point(24, 75);
            this.lblPlayerSummarySection.Name = "lblPlayerSummarySection";
            this.lblPlayerSummarySection.Size = new System.Drawing.Size(82, 13);
            this.lblPlayerSummarySection.TabIndex = 2;
            this.lblPlayerSummarySection.Text = "Player Summary";
            // 
            // chkTotalDpsPlot
            // 
            this.chkTotalDpsPlot.AutoSize = true;
            this.chkTotalDpsPlot.Checked = true;
            this.chkTotalDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTotalDpsPlot.Location = new System.Drawing.Point(43, 100);
            this.chkTotalDpsPlot.Name = "chkTotalDpsPlot";
            this.chkTotalDpsPlot.Size = new System.Drawing.Size(126, 17);
            this.chkTotalDpsPlot.TabIndex = 3;
            this.chkTotalDpsPlot.Text = "Show Total DPS Plot";
            this.chkTotalDpsPlot.UseVisualStyleBackColor = true;
            this.chkTotalDpsPlot.CheckedChanged += new System.EventHandler(this.TotalDpsPlot_CheckedChanged);
            // 
            // chkBossDpsPlot
            // 
            this.chkBossDpsPlot.AutoSize = true;
            this.chkBossDpsPlot.Checked = true;
            this.chkBossDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBossDpsPlot.Location = new System.Drawing.Point(43, 123);
            this.chkBossDpsPlot.Name = "chkBossDpsPlot";
            this.chkBossDpsPlot.Size = new System.Drawing.Size(125, 17);
            this.chkBossDpsPlot.TabIndex = 4;
            this.chkBossDpsPlot.Text = "Show Boss DPS Plot";
            this.chkBossDpsPlot.UseVisualStyleBackColor = true;
            this.chkBossDpsPlot.CheckedChanged += new System.EventHandler(this.BossDpsPlot_CheckedChanged);
            // 
            // chkUniversalBoons
            // 
            this.chkUniversalBoons.AutoSize = true;
            this.chkUniversalBoons.Checked = true;
            this.chkUniversalBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUniversalBoons.Location = new System.Drawing.Point(43, 168);
            this.chkUniversalBoons.Name = "chkUniversalBoons";
            this.chkUniversalBoons.Size = new System.Drawing.Size(133, 17);
            this.chkUniversalBoons.TabIndex = 5;
            this.chkUniversalBoons.Text = "Show Universal Boons";
            this.chkUniversalBoons.UseVisualStyleBackColor = true;
            this.chkUniversalBoons.CheckedChanged += new System.EventHandler(this.UniversalBoons_CheckedChanged);
            // 
            // lblBoonGraphSection
            // 
            this.lblBoonGraphSection.AutoSize = true;
            this.lblBoonGraphSection.Location = new System.Drawing.Point(40, 152);
            this.lblBoonGraphSection.Name = "lblBoonGraphSection";
            this.lblBoonGraphSection.Size = new System.Drawing.Size(69, 13);
            this.lblBoonGraphSection.TabIndex = 6;
            this.lblBoonGraphSection.Text = "Boons Graph";
            // 
            // chkImpProfSpecBoons
            // 
            this.chkImpProfSpecBoons.AutoSize = true;
            this.chkImpProfSpecBoons.Checked = true;
            this.chkImpProfSpecBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImpProfSpecBoons.Location = new System.Drawing.Point(43, 191);
            this.chkImpProfSpecBoons.Name = "chkImpProfSpecBoons";
            this.chkImpProfSpecBoons.Size = new System.Drawing.Size(224, 17);
            this.chkImpProfSpecBoons.TabIndex = 7;
            this.chkImpProfSpecBoons.Text = "Show Important Profession specific Boons";
            this.chkImpProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkImpProfSpecBoons.CheckedChanged += new System.EventHandler(this.ImportantProfessionSpecificBoons_CheckedChanged);
            // 
            // chkAllProfSpecBoons
            // 
            this.chkAllProfSpecBoons.AutoSize = true;
            this.chkAllProfSpecBoons.Location = new System.Drawing.Point(43, 214);
            this.chkAllProfSpecBoons.Name = "chkAllProfSpecBoons";
            this.chkAllProfSpecBoons.Size = new System.Drawing.Size(191, 17);
            this.chkAllProfSpecBoons.TabIndex = 8;
            this.chkAllProfSpecBoons.Text = "Show All Profession specific Boons";
            this.chkAllProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkAllProfSpecBoons.CheckedChanged += new System.EventHandler(this.AllProfessionSpecificBoons_CheckedChanged);
            // 
            // lblRotationGraphSection
            // 
            this.lblRotationGraphSection.AutoSize = true;
            this.lblRotationGraphSection.Location = new System.Drawing.Point(40, 245);
            this.lblRotationGraphSection.Name = "lblRotationGraphSection";
            this.lblRotationGraphSection.Size = new System.Drawing.Size(79, 13);
            this.lblRotationGraphSection.TabIndex = 9;
            this.lblRotationGraphSection.Text = "Rotation Graph";
            // 
            // chkSkillIcons
            // 
            this.chkSkillIcons.AutoSize = true;
            this.chkSkillIcons.Checked = true;
            this.chkSkillIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkillIcons.Location = new System.Drawing.Point(43, 295);
            this.chkSkillIcons.Name = "chkSkillIcons";
            this.chkSkillIcons.Size = new System.Drawing.Size(104, 17);
            this.chkSkillIcons.TabIndex = 10;
            this.chkSkillIcons.Text = "Show Skill Icons";
            this.chkSkillIcons.UseVisualStyleBackColor = true;
            this.chkSkillIcons.CheckedChanged += new System.EventHandler(this.SkillIcons_CheckedChanged);
            // 
            // chkRotation
            // 
            this.chkRotation.AutoSize = true;
            this.chkRotation.Checked = true;
            this.chkRotation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRotation.Location = new System.Drawing.Point(43, 272);
            this.chkRotation.Name = "chkRotation";
            this.chkRotation.Size = new System.Drawing.Size(96, 17);
            this.chkRotation.TabIndex = 11;
            this.chkRotation.Text = "Parse Rotation";
            this.chkRotation.UseVisualStyleBackColor = true;
            this.chkRotation.CheckedChanged += new System.EventHandler(this.Rotation_CheckedChanged);
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(27, 13);
            this.lblSettingsInfoMsg.Name = "lblSettingsInfoMsg";
            this.lblSettingsInfoMsg.Size = new System.Drawing.Size(245, 13);
            this.lblSettingsInfoMsg.TabIndex = 12;
            this.lblSettingsInfoMsg.Text = "*Settings will not alter files in the middle of parseing";
            // 
            // lblOutputSection
            // 
            this.lblOutputSection.AutoSize = true;
            this.lblOutputSection.Location = new System.Drawing.Point(24, 499);
            this.lblOutputSection.Name = "lblOutputSection";
            this.lblOutputSection.Size = new System.Drawing.Size(42, 13);
            this.lblOutputSection.TabIndex = 13;
            this.lblOutputSection.Text = "Output:";
            // 
            // chkDefaultOutputLoc
            // 
            this.chkDefaultOutputLoc.AutoSize = true;
            this.chkDefaultOutputLoc.Checked = true;
            this.chkDefaultOutputLoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDefaultOutputLoc.Location = new System.Drawing.Point(27, 515);
            this.chkDefaultOutputLoc.Name = "chkDefaultOutputLoc";
            this.chkDefaultOutputLoc.Size = new System.Drawing.Size(192, 17);
            this.chkDefaultOutputLoc.TabIndex = 14;
            this.chkDefaultOutputLoc.Text = "Save Output in same folder as evtc";
            this.chkDefaultOutputLoc.UseVisualStyleBackColor = true;
            this.chkDefaultOutputLoc.CheckedChanged += new System.EventHandler(this.DefaultOutputLocation_CheckedChanged);
            // 
            // txtCustomSaveLoc
            // 
            this.txtCustomSaveLoc.Enabled = false;
            this.txtCustomSaveLoc.Location = new System.Drawing.Point(24, 552);
            this.txtCustomSaveLoc.Name = "txtCustomSaveLoc";
            this.txtCustomSaveLoc.Size = new System.Drawing.Size(412, 20);
            this.txtCustomSaveLoc.TabIndex = 15;
            this.txtCustomSaveLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomSaveLoc.TextChanged += new System.EventHandler(this.CustomSaveLocation_TextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(461, 549);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(45, 23);
            this.btnFolderSelect.TabIndex = 16;
            this.btnFolderSelect.Text = "Select";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.BtnFolderSelect_Click);
            // 
            // lblCustomSaveLoc
            // 
            this.lblCustomSaveLoc.AutoSize = true;
            this.lblCustomSaveLoc.Location = new System.Drawing.Point(100, 533);
            this.lblCustomSaveLoc.Name = "lblCustomSaveLoc";
            this.lblCustomSaveLoc.Size = new System.Drawing.Size(59, 13);
            this.lblCustomSaveLoc.TabIndex = 17;
            this.lblCustomSaveLoc.Text = "Or save at:";
            // 
            // chkEventListDbg
            // 
            this.chkEventListDbg.AutoSize = true;
            this.chkEventListDbg.Location = new System.Drawing.Point(27, 479);
            this.chkEventListDbg.Name = "chkEventListDbg";
            this.chkEventListDbg.Size = new System.Drawing.Size(177, 17);
            this.chkEventListDbg.TabIndex = 18;
            this.chkEventListDbg.Text = "Show Event List (for debugging)";
            this.chkEventListDbg.UseVisualStyleBackColor = true;
            this.chkEventListDbg.CheckedChanged += new System.EventHandler(this.EventListDebug_CheckedChanged);
            // 
            // lblBossSection
            // 
            this.lblBossSection.AutoSize = true;
            this.lblBossSection.Location = new System.Drawing.Point(27, 392);
            this.lblBossSection.Name = "lblBossSection";
            this.lblBossSection.Size = new System.Drawing.Size(30, 13);
            this.lblBossSection.TabIndex = 19;
            this.lblBossSection.Text = "Boss";
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
            this.checkBossSummary.CheckedChanged += new System.EventHandler(this.BossSummary_CheckedChanged);
            // 
            // chkSimpleRotationTab
            // 
            this.chkSimpleRotationTab.AutoSize = true;
            this.chkSimpleRotationTab.Checked = true;
            this.chkSimpleRotationTab.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSimpleRotationTab.Location = new System.Drawing.Point(43, 318);
            this.chkSimpleRotationTab.Name = "chkSimpleRotationTab";
            this.chkSimpleRotationTab.Size = new System.Drawing.Size(152, 17);
            this.chkSimpleRotationTab.TabIndex = 21;
            this.chkSimpleRotationTab.Text = "Show Simple Rotation Tab";
            this.chkSimpleRotationTab.UseVisualStyleBackColor = true;
            this.chkSimpleRotationTab.CheckedChanged += new System.EventHandler(this.SimpleRotationTab_CheckedChanged);
            // 
            // chkShowAutos
            // 
            this.chkShowAutos.AutoSize = true;
            this.chkShowAutos.Checked = true;
            this.chkShowAutos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowAutos.Location = new System.Drawing.Point(49, 341);
            this.chkShowAutos.Name = "chkShowAutos";
            this.chkShowAutos.Size = new System.Drawing.Size(117, 17);
            this.chkShowAutos.TabIndex = 22;
            this.chkShowAutos.Text = "Show Auto Attacks";
            this.chkShowAutos.UseVisualStyleBackColor = true;
            this.chkShowAutos.CheckedChanged += new System.EventHandler(this.ShowAutos_CheckedChanged);
            // 
            // chkLargeSkillIcons
            // 
            this.chkLargeSkillIcons.AutoSize = true;
            this.chkLargeSkillIcons.Location = new System.Drawing.Point(49, 364);
            this.chkLargeSkillIcons.Name = "chkLargeSkillIcons";
            this.chkLargeSkillIcons.Size = new System.Drawing.Size(104, 17);
            this.chkLargeSkillIcons.TabIndex = 23;
            this.chkLargeSkillIcons.Text = "Large Skill Icons";
            this.chkLargeSkillIcons.UseVisualStyleBackColor = true;
            this.chkLargeSkillIcons.CheckedChanged += new System.EventHandler(this.LargeSkillIcons_CheckedChanged);
            // 
            // btnResetSkillList
            // 
            this.btnResetSkillList.Location = new System.Drawing.Point(428, 13);
            this.btnResetSkillList.Name = "btnResetSkillList";
            this.btnResetSkillList.Size = new System.Drawing.Size(108, 23);
            this.btnResetSkillList.TabIndex = 24;
            this.btnResetSkillList.Text = "Reset Skill List ";
            this.btnResetSkillList.UseVisualStyleBackColor = true;
            this.btnResetSkillList.Click += new System.EventHandler(this.ResetSkillList_Click);
            // 
            // btnRetrySkillList
            // 
            this.btnRetrySkillList.Location = new System.Drawing.Point(428, 49);
            this.btnRetrySkillList.Name = "btnRetrySkillList";
            this.btnRetrySkillList.Size = new System.Drawing.Size(108, 23);
            this.btnRetrySkillList.TabIndex = 25;
            this.btnRetrySkillList.Text = "Retry Skill List";
            this.btnRetrySkillList.UseVisualStyleBackColor = true;
            this.btnRetrySkillList.Click += new System.EventHandler(this.RetrySkillList_Click);
            // 
            // chkOutputHtml
            // 
            this.chkOutputHtml.AutoSize = true;
            this.chkOutputHtml.Checked = true;
            this.chkOutputHtml.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOutputHtml.Location = new System.Drawing.Point(24, 587);
            this.chkOutputHtml.Name = "chkOutputHtml";
            this.chkOutputHtml.Size = new System.Drawing.Size(105, 17);
            this.chkOutputHtml.TabIndex = 26;
            this.chkOutputHtml.Text = "Output as HTML";
            this.chkOutputHtml.UseVisualStyleBackColor = true;
            this.chkOutputHtml.CheckedChanged += new System.EventHandler(this.OuputHtml_CheckedChanged);
            // 
            // chkOutputCsv
            // 
            this.chkOutputCsv.AutoSize = true;
            this.chkOutputCsv.Location = new System.Drawing.Point(24, 611);
            this.chkOutputCsv.Name = "chkOutputCsv";
            this.chkOutputCsv.Size = new System.Drawing.Size(96, 17);
            this.chkOutputCsv.TabIndex = 27;
            this.chkOutputCsv.Text = "Output as CSV";
            this.chkOutputCsv.UseVisualStyleBackColor = true;
            this.chkOutputCsv.CheckedChanged += new System.EventHandler(this.OutputCsv_CheckedChanged);
            // 
            // chkShowEstimates
            // 
            this.chkShowEstimates.AutoSize = true;
            this.chkShowEstimates.Checked = true;
            this.chkShowEstimates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowEstimates.Location = new System.Drawing.Point(30, 440);
            this.chkShowEstimates.Name = "chkShowEstimates";
            this.chkShowEstimates.Size = new System.Drawing.Size(153, 17);
            this.chkShowEstimates.TabIndex = 28;
            this.chkShowEstimates.Text = "Show Estimates Tab (WIP)";
            this.chkShowEstimates.UseVisualStyleBackColor = true;
            this.chkShowEstimates.CheckedChanged += new System.EventHandler(this.ShowEstimates_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(577, 368);
            this.Controls.Add(this.chkShowEstimates);
            this.Controls.Add(this.chkOutputCsv);
            this.Controls.Add(this.chkOutputHtml);
            this.Controls.Add(this.btnRetrySkillList);
            this.Controls.Add(this.btnResetSkillList);
            this.Controls.Add(this.chkLargeSkillIcons);
            this.Controls.Add(this.chkShowAutos);
            this.Controls.Add(this.chkSimpleRotationTab);
            this.Controls.Add(this.checkBossSummary);
            this.Controls.Add(this.lblBossSection);
            this.Controls.Add(this.chkEventListDbg);
            this.Controls.Add(this.lblCustomSaveLoc);
            this.Controls.Add(this.btnFolderSelect);
            this.Controls.Add(this.txtCustomSaveLoc);
            this.Controls.Add(this.chkDefaultOutputLoc);
            this.Controls.Add(this.lblOutputSection);
            this.Controls.Add(this.lblSettingsInfoMsg);
            this.Controls.Add(this.chkRotation);
            this.Controls.Add(this.chkSkillIcons);
            this.Controls.Add(this.lblRotationGraphSection);
            this.Controls.Add(this.chkAllProfSpecBoons);
            this.Controls.Add(this.chkImpProfSpecBoons);
            this.Controls.Add(this.lblBoonGraphSection);
            this.Controls.Add(this.chkUniversalBoons);
            this.Controls.Add(this.chkBossDpsPlot);
            this.Controls.Add(this.chkTotalDpsPlot);
            this.Controls.Add(this.lblPlayerSummarySection);
            this.Controls.Add(this.chkPlayerDpsPlot);
            this.Controls.Add(this.lblDamageGraphSection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDamageGraphSection;
        private System.Windows.Forms.CheckBox chkPlayerDpsPlot;
        private System.Windows.Forms.Label lblPlayerSummarySection;
        private System.Windows.Forms.CheckBox chkTotalDpsPlot;
        private System.Windows.Forms.CheckBox chkBossDpsPlot;
        private System.Windows.Forms.CheckBox chkUniversalBoons;
        private System.Windows.Forms.Label lblBoonGraphSection;
        private System.Windows.Forms.CheckBox chkImpProfSpecBoons;
        private System.Windows.Forms.CheckBox chkAllProfSpecBoons;
        private System.Windows.Forms.Label lblRotationGraphSection;
        private System.Windows.Forms.CheckBox chkSkillIcons;
        private System.Windows.Forms.CheckBox chkRotation;
        private System.Windows.Forms.Label lblSettingsInfoMsg;
        private System.Windows.Forms.Label lblOutputSection;
        private System.Windows.Forms.CheckBox chkDefaultOutputLoc;
        private System.Windows.Forms.TextBox txtCustomSaveLoc;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFolderSelect;
        private System.Windows.Forms.Label lblCustomSaveLoc;
        private System.Windows.Forms.CheckBox chkEventListDbg;
        private System.Windows.Forms.Label lblBossSection;
        private System.Windows.Forms.CheckBox checkBossSummary;
        private System.Windows.Forms.CheckBox chkSimpleRotationTab;
        private System.Windows.Forms.CheckBox chkShowAutos;
        private System.Windows.Forms.CheckBox chkLargeSkillIcons;
        private System.Windows.Forms.Button btnResetSkillList;
        private System.Windows.Forms.Button btnRetrySkillList;
        private System.Windows.Forms.CheckBox chkOutputHtml;
        private System.Windows.Forms.CheckBox chkOutputCsv;
        private System.Windows.Forms.CheckBox chkShowEstimates;
    }
}