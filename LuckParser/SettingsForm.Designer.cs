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
            this.lblRotationSection = new System.Windows.Forms.Label();
            this.chkSkillIcons = new System.Windows.Forms.CheckBox();
            this.chkRotation = new System.Windows.Forms.CheckBox();
            this.lblSettingsInfoMsg = new System.Windows.Forms.Label();
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
            this.chkPhaseParsing = new System.Windows.Forms.CheckBox();
            this.chkShow10s = new System.Windows.Forms.CheckBox();
            this.chkShow30s = new System.Windows.Forms.CheckBox();
            this.chkOneAtATime = new System.Windows.Forms.CheckBox();
            this.StatisticsSelection = new System.Windows.Forms.Label();
            this.OutputSelection = new System.Windows.Forms.Label();
            this.html = new System.Windows.Forms.Label();
            this.csv = new System.Windows.Forms.Label();
            this.lblRotationGraphSection = new System.Windows.Forms.Label();
            this.chkLightTheme = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblDamageGraphSection
            // 
            this.lblDamageGraphSection.AutoSize = true;
            this.lblDamageGraphSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDamageGraphSection.Location = new System.Drawing.Point(90, 805);
            this.lblDamageGraphSection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDamageGraphSection.Name = "lblDamageGraphSection";
            this.lblDamageGraphSection.Size = new System.Drawing.Size(185, 26);
            this.lblDamageGraphSection.TabIndex = 0;
            this.lblDamageGraphSection.Text = "Damage Graphs";
            // 
            // chkPlayerDpsPlot
            // 
            this.chkPlayerDpsPlot.AutoSize = true;
            this.chkPlayerDpsPlot.Checked = true;
            this.chkPlayerDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPlayerDpsPlot.Location = new System.Drawing.Point(120, 836);
            this.chkPlayerDpsPlot.Margin = new System.Windows.Forms.Padding(6);
            this.chkPlayerDpsPlot.Name = "chkPlayerDpsPlot";
            this.chkPlayerDpsPlot.Size = new System.Drawing.Size(374, 29);
            this.chkPlayerDpsPlot.TabIndex = 1;
            this.chkPlayerDpsPlot.Text = "Show Each Players Total DPS plot";
            this.chkPlayerDpsPlot.UseVisualStyleBackColor = true;
            this.chkPlayerDpsPlot.CheckedChanged += new System.EventHandler(this.PlayerDpsPlot_CheckedChanged);
            // 
            // lblPlayerSummarySection
            // 
            this.lblPlayerSummarySection.AutoSize = true;
            this.lblPlayerSummarySection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerSummarySection.Location = new System.Drawing.Point(518, 805);
            this.lblPlayerSummarySection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblPlayerSummarySection.Name = "lblPlayerSummarySection";
            this.lblPlayerSummarySection.Size = new System.Drawing.Size(189, 26);
            this.lblPlayerSummarySection.TabIndex = 2;
            this.lblPlayerSummarySection.Text = "Player Summary";
            // 
            // chkTotalDpsPlot
            // 
            this.chkTotalDpsPlot.AutoSize = true;
            this.chkTotalDpsPlot.Checked = true;
            this.chkTotalDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTotalDpsPlot.Location = new System.Drawing.Point(550, 836);
            this.chkTotalDpsPlot.Margin = new System.Windows.Forms.Padding(6);
            this.chkTotalDpsPlot.Name = "chkTotalDpsPlot";
            this.chkTotalDpsPlot.Size = new System.Drawing.Size(243, 29);
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
            this.chkBossDpsPlot.Location = new System.Drawing.Point(550, 880);
            this.chkBossDpsPlot.Margin = new System.Windows.Forms.Padding(6);
            this.chkBossDpsPlot.Name = "chkBossDpsPlot";
            this.chkBossDpsPlot.Size = new System.Drawing.Size(243, 29);
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
            this.chkUniversalBoons.Location = new System.Drawing.Point(102, 177);
            this.chkUniversalBoons.Margin = new System.Windows.Forms.Padding(6);
            this.chkUniversalBoons.Name = "chkUniversalBoons";
            this.chkUniversalBoons.Size = new System.Drawing.Size(293, 29);
            this.chkUniversalBoons.TabIndex = 5;
            this.chkUniversalBoons.Text = "Compute Universal Boons";
            this.chkUniversalBoons.UseVisualStyleBackColor = true;
            this.chkUniversalBoons.CheckedChanged += new System.EventHandler(this.UniversalBoons_CheckedChanged);
            // 
            // lblBoonGraphSection
            // 
            this.lblBoonGraphSection.AutoSize = true;
            this.lblBoonGraphSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBoonGraphSection.Location = new System.Drawing.Point(70, 146);
            this.lblBoonGraphSection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBoonGraphSection.Name = "lblBoonGraphSection";
            this.lblBoonGraphSection.Size = new System.Drawing.Size(79, 26);
            this.lblBoonGraphSection.TabIndex = 6;
            this.lblBoonGraphSection.Text = "Boons";
            // 
            // chkImpProfSpecBoons
            // 
            this.chkImpProfSpecBoons.AutoSize = true;
            this.chkImpProfSpecBoons.Checked = true;
            this.chkImpProfSpecBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImpProfSpecBoons.Location = new System.Drawing.Point(102, 221);
            this.chkImpProfSpecBoons.Margin = new System.Windows.Forms.Padding(6);
            this.chkImpProfSpecBoons.Name = "chkImpProfSpecBoons";
            this.chkImpProfSpecBoons.Size = new System.Drawing.Size(375, 29);
            this.chkImpProfSpecBoons.TabIndex = 7;
            this.chkImpProfSpecBoons.Text = "Compute Profession Speficic Buffs";
            this.chkImpProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkImpProfSpecBoons.CheckedChanged += new System.EventHandler(this.ImportantProfessionSpecificBoons_CheckedChanged);
            // 
            // chkAllProfSpecBoons
            // 
            this.chkAllProfSpecBoons.AutoSize = true;
            this.chkAllProfSpecBoons.Location = new System.Drawing.Point(102, 265);
            this.chkAllProfSpecBoons.Margin = new System.Windows.Forms.Padding(6);
            this.chkAllProfSpecBoons.Name = "chkAllProfSpecBoons";
            this.chkAllProfSpecBoons.Size = new System.Drawing.Size(293, 29);
            this.chkAllProfSpecBoons.TabIndex = 8;
            this.chkAllProfSpecBoons.Text = "Compute Remaining Buffs";
            this.chkAllProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkAllProfSpecBoons.CheckedChanged += new System.EventHandler(this.AllProfessionSpecificBoons_CheckedChanged);
            // 
            // lblRotationSection
            // 
            this.lblRotationSection.AutoSize = true;
            this.lblRotationSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRotationSection.Location = new System.Drawing.Point(794, 146);
            this.lblRotationSection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblRotationSection.Name = "lblRotationSection";
            this.lblRotationSection.Size = new System.Drawing.Size(101, 26);
            this.lblRotationSection.TabIndex = 9;
            this.lblRotationSection.Text = "Rotation";
            // 
            // chkSkillIcons
            // 
            this.chkSkillIcons.AutoSize = true;
            this.chkSkillIcons.Checked = true;
            this.chkSkillIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkillIcons.Location = new System.Drawing.Point(834, 221);
            this.chkSkillIcons.Margin = new System.Windows.Forms.Padding(6);
            this.chkSkillIcons.Name = "chkSkillIcons";
            this.chkSkillIcons.Size = new System.Drawing.Size(190, 29);
            this.chkSkillIcons.TabIndex = 10;
            this.chkSkillIcons.Text = "With Skill Icons";
            this.chkSkillIcons.UseVisualStyleBackColor = true;
            this.chkSkillIcons.CheckedChanged += new System.EventHandler(this.SkillIcons_CheckedChanged);
            // 
            // chkRotation
            // 
            this.chkRotation.AutoSize = true;
            this.chkRotation.Checked = true;
            this.chkRotation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRotation.Location = new System.Drawing.Point(820, 177);
            this.chkRotation.Margin = new System.Windows.Forms.Padding(6);
            this.chkRotation.Name = "chkRotation";
            this.chkRotation.Size = new System.Drawing.Size(216, 29);
            this.chkRotation.TabIndex = 11;
            this.chkRotation.Text = "Compute Rotation";
            this.chkRotation.UseVisualStyleBackColor = true;
            this.chkRotation.CheckedChanged += new System.EventHandler(this.Rotation_CheckedChanged);
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(54, 25);
            this.lblSettingsInfoMsg.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSettingsInfoMsg.Name = "lblSettingsInfoMsg";
            this.lblSettingsInfoMsg.Size = new System.Drawing.Size(516, 25);
            this.lblSettingsInfoMsg.TabIndex = 12;
            this.lblSettingsInfoMsg.Text = "*Changes will not alter files that are currently parsing";
            // 
            // chkDefaultOutputLoc
            // 
            this.chkDefaultOutputLoc.AutoSize = true;
            this.chkDefaultOutputLoc.Checked = true;
            this.chkDefaultOutputLoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDefaultOutputLoc.Location = new System.Drawing.Point(74, 425);
            this.chkDefaultOutputLoc.Margin = new System.Windows.Forms.Padding(6);
            this.chkDefaultOutputLoc.Name = "chkDefaultOutputLoc";
            this.chkDefaultOutputLoc.Size = new System.Drawing.Size(379, 29);
            this.chkDefaultOutputLoc.TabIndex = 14;
            this.chkDefaultOutputLoc.Text = "Save Output in same folder as evtc";
            this.chkDefaultOutputLoc.UseVisualStyleBackColor = true;
            this.chkDefaultOutputLoc.CheckedChanged += new System.EventHandler(this.DefaultOutputLocation_CheckedChanged);
            // 
            // txtCustomSaveLoc
            // 
            this.txtCustomSaveLoc.Enabled = false;
            this.txtCustomSaveLoc.Location = new System.Drawing.Point(54, 500);
            this.txtCustomSaveLoc.Margin = new System.Windows.Forms.Padding(6);
            this.txtCustomSaveLoc.Name = "txtCustomSaveLoc";
            this.txtCustomSaveLoc.Size = new System.Drawing.Size(820, 31);
            this.txtCustomSaveLoc.TabIndex = 15;
            this.txtCustomSaveLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomSaveLoc.TextChanged += new System.EventHandler(this.CustomSaveLocation_TextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(890, 494);
            this.btnFolderSelect.Margin = new System.Windows.Forms.Padding(6);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(90, 44);
            this.btnFolderSelect.TabIndex = 16;
            this.btnFolderSelect.Text = "Select";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.BtnFolderSelect_Click);
            // 
            // lblCustomSaveLoc
            // 
            this.lblCustomSaveLoc.AutoSize = true;
            this.lblCustomSaveLoc.Location = new System.Drawing.Point(206, 463);
            this.lblCustomSaveLoc.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblCustomSaveLoc.Name = "lblCustomSaveLoc";
            this.lblCustomSaveLoc.Size = new System.Drawing.Size(117, 25);
            this.lblCustomSaveLoc.TabIndex = 17;
            this.lblCustomSaveLoc.Text = "Or save at:";
            // 
            // chkEventListDbg
            // 
            this.chkEventListDbg.AutoSize = true;
            this.chkEventListDbg.Location = new System.Drawing.Point(96, 685);
            this.chkEventListDbg.Margin = new System.Windows.Forms.Padding(6);
            this.chkEventListDbg.Name = "chkEventListDbg";
            this.chkEventListDbg.Size = new System.Drawing.Size(350, 29);
            this.chkEventListDbg.TabIndex = 18;
            this.chkEventListDbg.Text = "Show Event List (for debugging)";
            this.chkEventListDbg.UseVisualStyleBackColor = true;
            this.chkEventListDbg.CheckedChanged += new System.EventHandler(this.EventListDebug_CheckedChanged);
            // 
            // lblBossSection
            // 
            this.lblBossSection.AutoSize = true;
            this.lblBossSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBossSection.Location = new System.Drawing.Point(518, 146);
            this.lblBossSection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblBossSection.Name = "lblBossSection";
            this.lblBossSection.Size = new System.Drawing.Size(65, 26);
            this.lblBossSection.TabIndex = 19;
            this.lblBossSection.Text = "Boss";
            // 
            // checkBossSummary
            // 
            this.checkBossSummary.AutoSize = true;
            this.checkBossSummary.Checked = true;
            this.checkBossSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBossSummary.Location = new System.Drawing.Point(552, 177);
            this.checkBossSummary.Margin = new System.Windows.Forms.Padding(6);
            this.checkBossSummary.Name = "checkBossSummary";
            this.checkBossSummary.Size = new System.Drawing.Size(235, 29);
            this.checkBossSummary.TabIndex = 20;
            this.checkBossSummary.Text = "Compute Boss Data";
            this.checkBossSummary.UseVisualStyleBackColor = true;
            this.checkBossSummary.CheckedChanged += new System.EventHandler(this.BossSummary_CheckedChanged);
            // 
            // chkSimpleRotationTab
            // 
            this.chkSimpleRotationTab.AutoSize = true;
            this.chkSimpleRotationTab.Checked = true;
            this.chkSimpleRotationTab.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSimpleRotationTab.Location = new System.Drawing.Point(120, 1005);
            this.chkSimpleRotationTab.Margin = new System.Windows.Forms.Padding(6);
            this.chkSimpleRotationTab.Name = "chkSimpleRotationTab";
            this.chkSimpleRotationTab.Size = new System.Drawing.Size(297, 29);
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
            this.chkShowAutos.Location = new System.Drawing.Point(128, 1050);
            this.chkShowAutos.Margin = new System.Windows.Forms.Padding(6);
            this.chkShowAutos.Name = "chkShowAutos";
            this.chkShowAutos.Size = new System.Drawing.Size(224, 29);
            this.chkShowAutos.TabIndex = 22;
            this.chkShowAutos.Text = "Show Auto Attacks";
            this.chkShowAutos.UseVisualStyleBackColor = true;
            this.chkShowAutos.CheckedChanged += new System.EventHandler(this.ShowAutos_CheckedChanged);
            // 
            // chkLargeSkillIcons
            // 
            this.chkLargeSkillIcons.AutoSize = true;
            this.chkLargeSkillIcons.Location = new System.Drawing.Point(128, 1094);
            this.chkLargeSkillIcons.Margin = new System.Windows.Forms.Padding(6);
            this.chkLargeSkillIcons.Name = "chkLargeSkillIcons";
            this.chkLargeSkillIcons.Size = new System.Drawing.Size(202, 29);
            this.chkLargeSkillIcons.TabIndex = 23;
            this.chkLargeSkillIcons.Text = "Large Skill Icons";
            this.chkLargeSkillIcons.UseVisualStyleBackColor = true;
            this.chkLargeSkillIcons.CheckedChanged += new System.EventHandler(this.LargeSkillIcons_CheckedChanged);
            // 
            // btnResetSkillList
            // 
            this.btnResetSkillList.Location = new System.Drawing.Point(888, 15);
            this.btnResetSkillList.Margin = new System.Windows.Forms.Padding(6);
            this.btnResetSkillList.Name = "btnResetSkillList";
            this.btnResetSkillList.Size = new System.Drawing.Size(216, 44);
            this.btnResetSkillList.TabIndex = 24;
            this.btnResetSkillList.Text = "Reset Skill List ";
            this.btnResetSkillList.UseVisualStyleBackColor = true;
            this.btnResetSkillList.Click += new System.EventHandler(this.ResetSkillList_Click);
            // 
            // btnRetrySkillList
            // 
            this.btnRetrySkillList.Location = new System.Drawing.Point(662, 15);
            this.btnRetrySkillList.Margin = new System.Windows.Forms.Padding(6);
            this.btnRetrySkillList.Name = "btnRetrySkillList";
            this.btnRetrySkillList.Size = new System.Drawing.Size(216, 44);
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
            this.chkOutputHtml.Location = new System.Drawing.Point(96, 640);
            this.chkOutputHtml.Margin = new System.Windows.Forms.Padding(6);
            this.chkOutputHtml.Name = "chkOutputHtml";
            this.chkOutputHtml.Size = new System.Drawing.Size(201, 29);
            this.chkOutputHtml.TabIndex = 26;
            this.chkOutputHtml.Text = "Output as HTML";
            this.chkOutputHtml.UseVisualStyleBackColor = true;
            this.chkOutputHtml.CheckedChanged += new System.EventHandler(this.OuputHtml_CheckedChanged);
            // 
            // chkOutputCsv
            // 
            this.chkOutputCsv.AutoSize = true;
            this.chkOutputCsv.Location = new System.Drawing.Point(912, 640);
            this.chkOutputCsv.Margin = new System.Windows.Forms.Padding(6);
            this.chkOutputCsv.Name = "chkOutputCsv";
            this.chkOutputCsv.Size = new System.Drawing.Size(186, 29);
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
            this.chkShowEstimates.Location = new System.Drawing.Point(96, 729);
            this.chkShowEstimates.Margin = new System.Windows.Forms.Padding(6);
            this.chkShowEstimates.Name = "chkShowEstimates";
            this.chkShowEstimates.Size = new System.Drawing.Size(299, 29);
            this.chkShowEstimates.TabIndex = 28;
            this.chkShowEstimates.Text = "Show Estimates Tab (WIP)";
            this.chkShowEstimates.UseVisualStyleBackColor = true;
            this.chkShowEstimates.CheckedChanged += new System.EventHandler(this.ShowEstimates_CheckedChanged);
            // 
            // chkPhaseParsing
            // 
            this.chkPhaseParsing.AutoSize = true;
            this.chkPhaseParsing.Checked = true;
            this.chkPhaseParsing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPhaseParsing.Location = new System.Drawing.Point(552, 221);
            this.chkPhaseParsing.Margin = new System.Windows.Forms.Padding(6);
            this.chkPhaseParsing.Name = "chkPhaseParsing";
            this.chkPhaseParsing.Size = new System.Drawing.Size(178, 29);
            this.chkPhaseParsing.TabIndex = 30;
            this.chkPhaseParsing.Text = "Parse Phases";
            this.chkPhaseParsing.UseVisualStyleBackColor = true;
            this.chkPhaseParsing.CheckedChanged += new System.EventHandler(this.PhaseParsing_CheckedChanged);
            // 
            // chkShow10s
            // 
            this.chkShow10s.AutoSize = true;
            this.chkShow10s.Checked = true;
            this.chkShow10s.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShow10s.Location = new System.Drawing.Point(120, 878);
            this.chkShow10s.Margin = new System.Windows.Forms.Padding(6);
            this.chkShow10s.Name = "chkShow10s";
            this.chkShow10s.Size = new System.Drawing.Size(252, 29);
            this.chkShow10s.TabIndex = 31;
            this.chkShow10s.Text = "Show 10s DPS Graph";
            this.chkShow10s.UseVisualStyleBackColor = true;
            this.chkShow10s.CheckedChanged += new System.EventHandler(this.Show10s_CheckedChanged);
            // 
            // chkShow30s
            // 
            this.chkShow30s.AutoSize = true;
            this.chkShow30s.Checked = true;
            this.chkShow30s.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShow30s.Location = new System.Drawing.Point(120, 923);
            this.chkShow30s.Margin = new System.Windows.Forms.Padding(6);
            this.chkShow30s.Name = "chkShow30s";
            this.chkShow30s.Size = new System.Drawing.Size(252, 29);
            this.chkShow30s.TabIndex = 32;
            this.chkShow30s.Text = "Show 30s DPS Graph";
            this.chkShow30s.UseVisualStyleBackColor = true;
            this.chkShow30s.CheckedChanged += new System.EventHandler(this.Show30s_CheckedChanged);
            // 
            // chkOneAtATime
            // 
            this.chkOneAtATime.AutoSize = true;
            this.chkOneAtATime.Location = new System.Drawing.Point(74, 550);
            this.chkOneAtATime.Margin = new System.Windows.Forms.Padding(6);
            this.chkOneAtATime.Name = "chkOneAtATime";
            this.chkOneAtATime.Size = new System.Drawing.Size(583, 29);
            this.chkOneAtATime.TabIndex = 29;
            this.chkOneAtATime.Text = "Parse logs one at a time (less CPU load, slower parsing)";
            this.chkOneAtATime.UseVisualStyleBackColor = true;
            this.chkOneAtATime.CheckedChanged += new System.EventHandler(this.chkOneAtATime_CheckedChanged);
            // 
            // StatisticsSelection
            // 
            this.StatisticsSelection.AutoSize = true;
            this.StatisticsSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatisticsSelection.Location = new System.Drawing.Point(44, 79);
            this.StatisticsSelection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.StatisticsSelection.Name = "StatisticsSelection";
            this.StatisticsSelection.Size = new System.Drawing.Size(200, 48);
            this.StatisticsSelection.TabIndex = 33;
            this.StatisticsSelection.Text = "Statistics";
            // 
            // OutputSelection
            // 
            this.OutputSelection.AutoSize = true;
            this.OutputSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputSelection.Location = new System.Drawing.Point(50, 354);
            this.OutputSelection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.OutputSelection.Name = "OutputSelection";
            this.OutputSelection.Size = new System.Drawing.Size(152, 48);
            this.OutputSelection.TabIndex = 34;
            this.OutputSelection.Text = "Output";
            // 
            // html
            // 
            this.html.AutoSize = true;
            this.html.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.html.Location = new System.Drawing.Point(68, 588);
            this.html.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.html.Name = "html";
            this.html.Size = new System.Drawing.Size(104, 44);
            this.html.TabIndex = 35;
            this.html.Text = "Html";
            // 
            // csv
            // 
            this.csv.AutoSize = true;
            this.csv.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.csv.Location = new System.Drawing.Point(882, 588);
            this.csv.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.csv.Name = "csv";
            this.csv.Size = new System.Drawing.Size(88, 44);
            this.csv.TabIndex = 36;
            this.csv.Text = "Csv";
            // 
            // lblRotationGraphSection
            // 
            this.lblRotationGraphSection.AutoSize = true;
            this.lblRotationGraphSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRotationGraphSection.Location = new System.Drawing.Point(90, 975);
            this.lblRotationGraphSection.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblRotationGraphSection.Name = "lblRotationGraphSection";
            this.lblRotationGraphSection.Size = new System.Drawing.Size(147, 26);
            this.lblRotationGraphSection.TabIndex = 37;
            this.lblRotationGraphSection.Text = "Rotation Tab";
            // 
            // chkLightTheme
            // 
            this.chkLightTheme.AutoSize = true;
            this.chkLightTheme.Location = new System.Drawing.Point(95, 770);
            this.chkLightTheme.Margin = new System.Windows.Forms.Padding(6);
            this.chkLightTheme.Name = "chkLightTheme";
            this.chkLightTheme.Size = new System.Drawing.Size(163, 29);
            this.chkLightTheme.TabIndex = 39;
            this.chkLightTheme.Text = "Light Theme";
            this.chkLightTheme.UseVisualStyleBackColor = true;
            this.chkLightTheme.CheckedChanged += new System.EventHandler(this.LightTheme_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1158, 1151);
            this.Controls.Add(this.chkLightTheme);
            this.Controls.Add(this.lblRotationGraphSection);
            this.Controls.Add(this.csv);
            this.Controls.Add(this.html);
            this.Controls.Add(this.OutputSelection);
            this.Controls.Add(this.StatisticsSelection);
            this.Controls.Add(this.chkShow30s);
            this.Controls.Add(this.chkShow10s);
            this.Controls.Add(this.chkPhaseParsing);
            this.Controls.Add(this.chkOneAtATime);
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
            this.Controls.Add(this.lblSettingsInfoMsg);
            this.Controls.Add(this.chkRotation);
            this.Controls.Add(this.chkSkillIcons);
            this.Controls.Add(this.lblRotationSection);
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
            this.Margin = new System.Windows.Forms.Padding(6);
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
        private System.Windows.Forms.Label lblRotationSection;
        private System.Windows.Forms.CheckBox chkSkillIcons;
        private System.Windows.Forms.CheckBox chkRotation;
        private System.Windows.Forms.Label lblSettingsInfoMsg;
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
        private System.Windows.Forms.CheckBox chkPhaseParsing;
        private System.Windows.Forms.CheckBox chkOneAtATime;
        private System.Windows.Forms.CheckBox chkShow10s;
        private System.Windows.Forms.CheckBox chkShow30s;
        private System.Windows.Forms.Label StatisticsSelection;
        private System.Windows.Forms.Label OutputSelection;
        private System.Windows.Forms.Label html;
        private System.Windows.Forms.Label csv;
        private System.Windows.Forms.Label lblRotationGraphSection;
        private System.Windows.Forms.CheckBox chkLightTheme;
    }
}