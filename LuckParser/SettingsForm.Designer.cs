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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.chkPlayerDpsPlot = new System.Windows.Forms.CheckBox();
            this.chkUniversalBoons = new System.Windows.Forms.CheckBox();
            this.chkImpProfSpecBoons = new System.Windows.Forms.CheckBox();
            this.chkAllProfSpecBoons = new System.Windows.Forms.CheckBox();
            this.chkSkillIcons = new System.Windows.Forms.CheckBox();
            this.chkRotation = new System.Windows.Forms.CheckBox();
            this.lblSettingsInfoMsg = new System.Windows.Forms.Label();
            this.chkDefaultOutputLoc = new System.Windows.Forms.CheckBox();
            this.txtCustomSaveLoc = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFolderSelect = new System.Windows.Forms.Button();
            this.lblCustomSaveLoc = new System.Windows.Forms.Label();
            this.chkEventListDbg = new System.Windows.Forms.CheckBox();
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
            this.chkCombatReplay = new System.Windows.Forms.CheckBox();
            this.chkShowCl = new System.Windows.Forms.CheckBox();
            this.UploadDPSReports_checkbox = new System.Windows.Forms.CheckBox();
            this.UploadDRRH_check = new System.Windows.Forms.CheckBox();
            this.UploadRaidar_check = new System.Windows.Forms.CheckBox();
            this.chkOutputJson = new System.Windows.Forms.CheckBox();
            this.chkIndentJSON = new System.Windows.Forms.CheckBox();
            this.chkHtmlExperimental = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkHtmlExternalScripts = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioThemeLight = new System.Windows.Forms.RadioButton();
            this.radioThemeDark = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.grpGraphs = new System.Windows.Forms.GroupBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.cmdClose = new System.Windows.Forms.Button();
            this.panelHtml = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelJson = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.grpGraphs.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.panelHtml.SuspendLayout();
            this.panelJson.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkPlayerDpsPlot
            // 
            this.chkPlayerDpsPlot.AutoSize = true;
            this.chkPlayerDpsPlot.Checked = true;
            this.chkPlayerDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPlayerDpsPlot.Location = new System.Drawing.Point(12, 24);
            this.chkPlayerDpsPlot.Name = "chkPlayerDpsPlot";
            this.chkPlayerDpsPlot.Size = new System.Drawing.Size(137, 17);
            this.chkPlayerDpsPlot.TabIndex = 1;
            this.chkPlayerDpsPlot.Text = "Show Total DPS Graph";
            this.chkPlayerDpsPlot.UseVisualStyleBackColor = true;
            this.chkPlayerDpsPlot.CheckedChanged += new System.EventHandler(this.PlayerDpsPlotCheckedChanged);
            // 
            // chkUniversalBoons
            // 
            this.chkUniversalBoons.AutoSize = true;
            this.chkUniversalBoons.Checked = true;
            this.chkUniversalBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUniversalBoons.Location = new System.Drawing.Point(12, 24);
            this.chkUniversalBoons.Name = "chkUniversalBoons";
            this.chkUniversalBoons.Size = new System.Drawing.Size(148, 17);
            this.chkUniversalBoons.TabIndex = 5;
            this.chkUniversalBoons.Text = "Compute Universal Boons";
            this.chkUniversalBoons.UseVisualStyleBackColor = true;
            this.chkUniversalBoons.CheckedChanged += new System.EventHandler(this.UniversalBoonsCheckedChanged);
            // 
            // chkImpProfSpecBoons
            // 
            this.chkImpProfSpecBoons.AutoSize = true;
            this.chkImpProfSpecBoons.Checked = true;
            this.chkImpProfSpecBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImpProfSpecBoons.Location = new System.Drawing.Point(12, 48);
            this.chkImpProfSpecBoons.Name = "chkImpProfSpecBoons";
            this.chkImpProfSpecBoons.Size = new System.Drawing.Size(188, 17);
            this.chkImpProfSpecBoons.TabIndex = 7;
            this.chkImpProfSpecBoons.Text = "Compute Profession Specific Buffs";
            this.chkImpProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkImpProfSpecBoons.CheckedChanged += new System.EventHandler(this.ImportantProfessionSpecificBoonsCheckedChanged);
            // 
            // chkAllProfSpecBoons
            // 
            this.chkAllProfSpecBoons.AutoSize = true;
            this.chkAllProfSpecBoons.Location = new System.Drawing.Point(12, 72);
            this.chkAllProfSpecBoons.Name = "chkAllProfSpecBoons";
            this.chkAllProfSpecBoons.Size = new System.Drawing.Size(148, 17);
            this.chkAllProfSpecBoons.TabIndex = 8;
            this.chkAllProfSpecBoons.Text = "Compute Remaining Buffs";
            this.chkAllProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkAllProfSpecBoons.CheckedChanged += new System.EventHandler(this.AllProfessionSpecificBoonsCheckedChanged);
            // 
            // chkSkillIcons
            // 
            this.chkSkillIcons.AutoSize = true;
            this.chkSkillIcons.Checked = true;
            this.chkSkillIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkillIcons.Location = new System.Drawing.Point(12, 48);
            this.chkSkillIcons.Name = "chkSkillIcons";
            this.chkSkillIcons.Size = new System.Drawing.Size(99, 17);
            this.chkSkillIcons.TabIndex = 10;
            this.chkSkillIcons.Text = "With Skill Icons";
            this.chkSkillIcons.UseVisualStyleBackColor = true;
            this.chkSkillIcons.CheckedChanged += new System.EventHandler(this.SkillIconsCheckedChanged);
            // 
            // chkRotation
            // 
            this.chkRotation.AutoSize = true;
            this.chkRotation.Checked = true;
            this.chkRotation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRotation.Location = new System.Drawing.Point(12, 24);
            this.chkRotation.Name = "chkRotation";
            this.chkRotation.Size = new System.Drawing.Size(111, 17);
            this.chkRotation.TabIndex = 11;
            this.chkRotation.Text = "Compute Rotation";
            this.chkRotation.UseVisualStyleBackColor = true;
            this.chkRotation.CheckedChanged += new System.EventHandler(this.RotationCheckedChanged);
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(12, 422);
            this.lblSettingsInfoMsg.Name = "lblSettingsInfoMsg";
            this.lblSettingsInfoMsg.Size = new System.Drawing.Size(251, 13);
            this.lblSettingsInfoMsg.TabIndex = 12;
            this.lblSettingsInfoMsg.Text = "*Changes will not alter files that are currently parsing";
            // 
            // chkDefaultOutputLoc
            // 
            this.chkDefaultOutputLoc.AutoSize = true;
            this.chkDefaultOutputLoc.Checked = true;
            this.chkDefaultOutputLoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDefaultOutputLoc.Location = new System.Drawing.Point(12, 24);
            this.chkDefaultOutputLoc.Name = "chkDefaultOutputLoc";
            this.chkDefaultOutputLoc.Size = new System.Drawing.Size(192, 17);
            this.chkDefaultOutputLoc.TabIndex = 14;
            this.chkDefaultOutputLoc.Text = "Save Output in same folder as evtc";
            this.chkDefaultOutputLoc.UseVisualStyleBackColor = true;
            this.chkDefaultOutputLoc.CheckedChanged += new System.EventHandler(this.DefaultOutputLocationCheckedChanged);
            // 
            // txtCustomSaveLoc
            // 
            this.txtCustomSaveLoc.Enabled = false;
            this.txtCustomSaveLoc.Location = new System.Drawing.Point(12, 72);
            this.txtCustomSaveLoc.Name = "txtCustomSaveLoc";
            this.txtCustomSaveLoc.Size = new System.Drawing.Size(370, 20);
            this.txtCustomSaveLoc.TabIndex = 15;
            this.txtCustomSaveLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomSaveLoc.TextChanged += new System.EventHandler(this.CustomSaveLocationTextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(384, 72);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(45, 20);
            this.btnFolderSelect.TabIndex = 16;
            this.btnFolderSelect.Text = "Select";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.BtnFolderSelectClick);
            // 
            // lblCustomSaveLoc
            // 
            this.lblCustomSaveLoc.AutoSize = true;
            this.lblCustomSaveLoc.Location = new System.Drawing.Point(12, 48);
            this.lblCustomSaveLoc.Name = "lblCustomSaveLoc";
            this.lblCustomSaveLoc.Size = new System.Drawing.Size(98, 13);
            this.lblCustomSaveLoc.TabIndex = 17;
            this.lblCustomSaveLoc.Text = "Other output folder:";
            // 
            // chkEventListDbg
            // 
            this.chkEventListDbg.AutoSize = true;
            this.chkEventListDbg.Location = new System.Drawing.Point(12, 12);
            this.chkEventListDbg.Name = "chkEventListDbg";
            this.chkEventListDbg.Size = new System.Drawing.Size(177, 17);
            this.chkEventListDbg.TabIndex = 18;
            this.chkEventListDbg.Text = "Show Event List (for debugging)";
            this.chkEventListDbg.UseVisualStyleBackColor = true;
            this.chkEventListDbg.CheckedChanged += new System.EventHandler(this.EventListDebugCheckedChanged);
            // 
            // checkBossSummary
            // 
            this.checkBossSummary.AutoSize = true;
            this.checkBossSummary.Checked = true;
            this.checkBossSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBossSummary.Location = new System.Drawing.Point(12, 24);
            this.checkBossSummary.Name = "checkBossSummary";
            this.checkBossSummary.Size = new System.Drawing.Size(120, 17);
            this.checkBossSummary.TabIndex = 20;
            this.checkBossSummary.Text = "Compute Boss Data";
            this.checkBossSummary.UseVisualStyleBackColor = true;
            this.checkBossSummary.CheckedChanged += new System.EventHandler(this.BossSummaryCheckedChanged);
            // 
            // chkSimpleRotationTab
            // 
            this.chkSimpleRotationTab.AutoSize = true;
            this.chkSimpleRotationTab.Checked = true;
            this.chkSimpleRotationTab.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSimpleRotationTab.Location = new System.Drawing.Point(12, 24);
            this.chkSimpleRotationTab.Name = "chkSimpleRotationTab";
            this.chkSimpleRotationTab.Size = new System.Drawing.Size(152, 17);
            this.chkSimpleRotationTab.TabIndex = 21;
            this.chkSimpleRotationTab.Text = "Show Simple Rotation Tab";
            this.chkSimpleRotationTab.UseVisualStyleBackColor = true;
            this.chkSimpleRotationTab.CheckedChanged += new System.EventHandler(this.SimpleRotationTabCheckedChanged);
            // 
            // chkShowAutos
            // 
            this.chkShowAutos.AutoSize = true;
            this.chkShowAutos.Checked = true;
            this.chkShowAutos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowAutos.Location = new System.Drawing.Point(12, 48);
            this.chkShowAutos.Name = "chkShowAutos";
            this.chkShowAutos.Size = new System.Drawing.Size(117, 17);
            this.chkShowAutos.TabIndex = 22;
            this.chkShowAutos.Text = "Show Auto Attacks";
            this.chkShowAutos.UseVisualStyleBackColor = true;
            this.chkShowAutos.CheckedChanged += new System.EventHandler(this.ShowAutosCheckedChanged);
            // 
            // chkLargeSkillIcons
            // 
            this.chkLargeSkillIcons.AutoSize = true;
            this.chkLargeSkillIcons.Location = new System.Drawing.Point(12, 72);
            this.chkLargeSkillIcons.Name = "chkLargeSkillIcons";
            this.chkLargeSkillIcons.Size = new System.Drawing.Size(104, 17);
            this.chkLargeSkillIcons.TabIndex = 23;
            this.chkLargeSkillIcons.Text = "Large Skill Icons";
            this.chkLargeSkillIcons.UseVisualStyleBackColor = true;
            this.chkLargeSkillIcons.CheckedChanged += new System.EventHandler(this.LargeSkillIconsCheckedChanged);
            // 
            // btnResetSkillList
            // 
            this.btnResetSkillList.Location = new System.Drawing.Point(24, 204);
            this.btnResetSkillList.Name = "btnResetSkillList";
            this.btnResetSkillList.Size = new System.Drawing.Size(144, 23);
            this.btnResetSkillList.TabIndex = 24;
            this.btnResetSkillList.Text = "Reset Skill List ";
            this.btnResetSkillList.UseVisualStyleBackColor = true;
            this.btnResetSkillList.Click += new System.EventHandler(this.ResetSkillListClick);
            // 
            // btnRetrySkillList
            // 
            this.btnRetrySkillList.Location = new System.Drawing.Point(24, 48);
            this.btnRetrySkillList.Name = "btnRetrySkillList";
            this.btnRetrySkillList.Size = new System.Drawing.Size(144, 23);
            this.btnRetrySkillList.TabIndex = 25;
            this.btnRetrySkillList.Text = "Retry Skill List";
            this.btnRetrySkillList.UseVisualStyleBackColor = true;
            this.btnRetrySkillList.Click += new System.EventHandler(this.RetrySkillListClick);
            // 
            // chkOutputHtml
            // 
            this.chkOutputHtml.AutoSize = true;
            this.chkOutputHtml.Checked = true;
            this.chkOutputHtml.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOutputHtml.Location = new System.Drawing.Point(12, 12);
            this.chkOutputHtml.Name = "chkOutputHtml";
            this.chkOutputHtml.Size = new System.Drawing.Size(105, 17);
            this.chkOutputHtml.TabIndex = 26;
            this.chkOutputHtml.Text = "Output as HTML";
            this.chkOutputHtml.UseVisualStyleBackColor = true;
            this.chkOutputHtml.CheckedChanged += new System.EventHandler(this.OuputCheckedChanged);
            // 
            // chkOutputCsv
            // 
            this.chkOutputCsv.AutoSize = true;
            this.chkOutputCsv.Location = new System.Drawing.Point(12, 12);
            this.chkOutputCsv.Name = "chkOutputCsv";
            this.chkOutputCsv.Size = new System.Drawing.Size(96, 17);
            this.chkOutputCsv.TabIndex = 27;
            this.chkOutputCsv.Text = "Output as CSV";
            this.chkOutputCsv.UseVisualStyleBackColor = true;
            this.chkOutputCsv.CheckedChanged += new System.EventHandler(this.OutputCsvCheckedChanged);
            // 
            // chkShowEstimates
            // 
            this.chkShowEstimates.AutoSize = true;
            this.chkShowEstimates.Checked = true;
            this.chkShowEstimates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowEstimates.Location = new System.Drawing.Point(12, 36);
            this.chkShowEstimates.Name = "chkShowEstimates";
            this.chkShowEstimates.Size = new System.Drawing.Size(153, 17);
            this.chkShowEstimates.TabIndex = 28;
            this.chkShowEstimates.Text = "Show Estimates Tab (WIP)";
            this.chkShowEstimates.UseVisualStyleBackColor = true;
            this.chkShowEstimates.CheckedChanged += new System.EventHandler(this.ShowEstimatesCheckedChanged);
            // 
            // chkPhaseParsing
            // 
            this.chkPhaseParsing.AutoSize = true;
            this.chkPhaseParsing.Checked = true;
            this.chkPhaseParsing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPhaseParsing.Location = new System.Drawing.Point(12, 48);
            this.chkPhaseParsing.Name = "chkPhaseParsing";
            this.chkPhaseParsing.Size = new System.Drawing.Size(91, 17);
            this.chkPhaseParsing.TabIndex = 30;
            this.chkPhaseParsing.Text = "Parse Phases";
            this.chkPhaseParsing.UseVisualStyleBackColor = true;
            this.chkPhaseParsing.CheckedChanged += new System.EventHandler(this.PhaseParsingCheckedChanged);
            // 
            // chkShow10s
            // 
            this.chkShow10s.AutoSize = true;
            this.chkShow10s.Checked = true;
            this.chkShow10s.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShow10s.Location = new System.Drawing.Point(12, 72);
            this.chkShow10s.Name = "chkShow10s";
            this.chkShow10s.Size = new System.Drawing.Size(130, 17);
            this.chkShow10s.TabIndex = 31;
            this.chkShow10s.Text = "Show 10s DPS Graph";
            this.chkShow10s.UseVisualStyleBackColor = true;
            this.chkShow10s.CheckedChanged += new System.EventHandler(this.Show10sCheckedChanged);
            // 
            // chkShow30s
            // 
            this.chkShow30s.AutoSize = true;
            this.chkShow30s.Checked = true;
            this.chkShow30s.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShow30s.Location = new System.Drawing.Point(12, 96);
            this.chkShow30s.Name = "chkShow30s";
            this.chkShow30s.Size = new System.Drawing.Size(130, 17);
            this.chkShow30s.TabIndex = 32;
            this.chkShow30s.Text = "Show 30s DPS Graph";
            this.chkShow30s.UseVisualStyleBackColor = true;
            this.chkShow30s.CheckedChanged += new System.EventHandler(this.Show30sCheckedChanged);
            // 
            // chkOneAtATime
            // 
            this.chkOneAtATime.AutoSize = true;
            this.chkOneAtATime.Location = new System.Drawing.Point(24, 252);
            this.chkOneAtATime.Name = "chkOneAtATime";
            this.chkOneAtATime.Size = new System.Drawing.Size(287, 17);
            this.chkOneAtATime.TabIndex = 29;
            this.chkOneAtATime.Text = "Parse logs one at a time (less CPU load, slower parsing)";
            this.chkOneAtATime.UseVisualStyleBackColor = true;
            this.chkOneAtATime.CheckedChanged += new System.EventHandler(this.ChkOneAtATimeCheckedChanged);
            // 
            // chkCombatReplay
            // 
            this.chkCombatReplay.AutoSize = true;
            this.chkCombatReplay.Checked = true;
            this.chkCombatReplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCombatReplay.Location = new System.Drawing.Point(12, 72);
            this.chkCombatReplay.Name = "chkCombatReplay";
            this.chkCombatReplay.Size = new System.Drawing.Size(143, 17);
            this.chkCombatReplay.TabIndex = 40;
            this.chkCombatReplay.Text = "Compute Combat Replay";
            this.chkCombatReplay.UseVisualStyleBackColor = true;
            this.chkCombatReplay.CheckedChanged += new System.EventHandler(this.ChkCombatReplayCheckedChanged);
            // 
            // chkShowCl
            // 
            this.chkShowCl.AutoSize = true;
            this.chkShowCl.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkShowCl.Location = new System.Drawing.Point(12, 48);
            this.chkShowCl.Name = "chkShowCl";
            this.chkShowCl.Size = new System.Drawing.Size(145, 17);
            this.chkShowCl.TabIndex = 41;
            this.chkShowCl.Text = "Show Cleave Only Graph";
            this.chkShowCl.UseVisualStyleBackColor = true;
            this.chkShowCl.CheckedChanged += new System.EventHandler(this.ShowClCheckedChanged);
            // 
            // UploadDPSReports_checkbox
            // 
            this.UploadDPSReports_checkbox.AutoSize = true;
            this.UploadDPSReports_checkbox.Location = new System.Drawing.Point(12, 12);
            this.UploadDPSReports_checkbox.Name = "UploadDPSReports_checkbox";
            this.UploadDPSReports_checkbox.Size = new System.Drawing.Size(196, 17);
            this.UploadDPSReports_checkbox.TabIndex = 43;
            this.UploadDPSReports_checkbox.Text = "Upload to DPSReports Elite Insights";
            this.UploadDPSReports_checkbox.UseVisualStyleBackColor = true;
            this.UploadDPSReports_checkbox.CheckedChanged += new System.EventHandler(this.UploadDPSReports_checkbox_CheckedChanged);
            // 
            // UploadDRRH_check
            // 
            this.UploadDRRH_check.AutoSize = true;
            this.UploadDRRH_check.Location = new System.Drawing.Point(12, 36);
            this.UploadDRRH_check.Name = "UploadDRRH_check";
            this.UploadDRRH_check.Size = new System.Drawing.Size(193, 17);
            this.UploadDRRH_check.TabIndex = 44;
            this.UploadDRRH_check.Text = "Upload to DPSReports RaidHeroes";
            this.UploadDRRH_check.UseVisualStyleBackColor = true;
            this.UploadDRRH_check.CheckedChanged += new System.EventHandler(this.UploadDRRH_check_CheckedChanged);
            // 
            // UploadRaidar_check
            // 
            this.UploadRaidar_check.AutoSize = true;
            this.UploadRaidar_check.Location = new System.Drawing.Point(12, 60);
            this.UploadRaidar_check.Name = "UploadRaidar_check";
            this.UploadRaidar_check.Size = new System.Drawing.Size(175, 17);
            this.UploadRaidar_check.TabIndex = 45;
            this.UploadRaidar_check.Text = "Upload to Raidar (Not Working)";
            this.UploadRaidar_check.UseVisualStyleBackColor = true;
            this.UploadRaidar_check.CheckedChanged += new System.EventHandler(this.UploadRaidar_check_CheckedChanged);
            // 
            // chkOutputJson
            // 
            this.chkOutputJson.AutoSize = true;
            this.chkOutputJson.Location = new System.Drawing.Point(12, 12);
            this.chkOutputJson.Name = "chkOutputJson";
            this.chkOutputJson.Size = new System.Drawing.Size(103, 17);
            this.chkOutputJson.TabIndex = 43;
            this.chkOutputJson.Text = "Output as JSON";
            this.chkOutputJson.UseVisualStyleBackColor = true;
            this.chkOutputJson.CheckedChanged += new System.EventHandler(this.OutputJSONCheckedChanged);
            // 
            // chkIndentJSON
            // 
            this.chkIndentJSON.AutoSize = true;
            this.chkIndentJSON.Location = new System.Drawing.Point(12, 12);
            this.chkIndentJSON.Name = "chkIndentJSON";
            this.chkIndentJSON.Size = new System.Drawing.Size(87, 17);
            this.chkIndentJSON.TabIndex = 44;
            this.chkIndentJSON.Text = "Indent JSON";
            this.chkIndentJSON.UseVisualStyleBackColor = true;
            this.chkIndentJSON.CheckedChanged += new System.EventHandler(this.chkIndentJSONCheckedChanged);
            // 
            // chkHtmlExperimental
            // 
            this.chkHtmlExperimental.AutoSize = true;
            this.chkHtmlExperimental.Location = new System.Drawing.Point(12, 24);
            this.chkHtmlExperimental.Name = "chkHtmlExperimental";
            this.chkHtmlExperimental.Size = new System.Drawing.Size(146, 17);
            this.chkHtmlExperimental.TabIndex = 45;
            this.chkHtmlExperimental.Text = "Experimental Mode (WIP)";
            this.chkHtmlExperimental.UseVisualStyleBackColor = true;
            this.chkHtmlExperimental.CheckedChanged += new System.EventHandler(this.chkHtmlExperimental_CheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "Setting description";
            // 
            // chkHtmlExternalScripts
            // 
            this.chkHtmlExternalScripts.AutoSize = true;
            this.chkHtmlExternalScripts.Location = new System.Drawing.Point(12, 48);
            this.chkHtmlExternalScripts.Name = "chkHtmlExternalScripts";
            this.chkHtmlExternalScripts.Size = new System.Drawing.Size(99, 17);
            this.chkHtmlExternalScripts.TabIndex = 46;
            this.chkHtmlExternalScripts.Text = "External Scripts";
            this.chkHtmlExternalScripts.UseVisualStyleBackColor = true;
            this.chkHtmlExternalScripts.CheckedChanged += new System.EventHandler(this.chkHtmlExternalScripts_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(479, 398);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.chkOneAtATime);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(471, 372);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkDefaultOutputLoc);
            this.groupBox3.Controls.Add(this.btnFolderSelect);
            this.groupBox3.Controls.Add(this.txtCustomSaveLoc);
            this.groupBox3.Controls.Add(this.lblCustomSaveLoc);
            this.groupBox3.Location = new System.Drawing.Point(12, 132);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 108);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBossSummary);
            this.groupBox2.Controls.Add(this.chkPhaseParsing);
            this.groupBox2.Location = new System.Drawing.Point(240, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(216, 108);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Boss";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkUniversalBoons);
            this.groupBox1.Controls.Add(this.chkAllProfSpecBoons);
            this.groupBox1.Controls.Add(this.chkImpProfSpecBoons);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 108);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Boons";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panelHtml);
            this.tabPage2.Controls.Add(this.chkOutputHtml);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(471, 372);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "HTML";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioThemeLight);
            this.panel1.Controls.Add(this.radioThemeDark);
            this.panel1.Location = new System.Drawing.Point(252, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(204, 48);
            this.panel1.TabIndex = 53;
            // 
            // radioThemeLight
            // 
            this.radioThemeLight.AutoSize = true;
            this.radioThemeLight.Location = new System.Drawing.Point(0, 0);
            this.radioThemeLight.Name = "radioThemeLight";
            this.radioThemeLight.Size = new System.Drawing.Size(84, 17);
            this.radioThemeLight.TabIndex = 47;
            this.radioThemeLight.TabStop = true;
            this.radioThemeLight.Text = "Light Theme";
            this.radioThemeLight.UseVisualStyleBackColor = true;
            this.radioThemeLight.CheckedChanged += new System.EventHandler(this.radioThemeLight_CheckedChanged);
            // 
            // radioThemeDark
            // 
            this.radioThemeDark.AutoSize = true;
            this.radioThemeDark.Location = new System.Drawing.Point(0, 24);
            this.radioThemeDark.Name = "radioThemeDark";
            this.radioThemeDark.Size = new System.Drawing.Size(84, 17);
            this.radioThemeDark.TabIndex = 48;
            this.radioThemeDark.TabStop = true;
            this.radioThemeDark.Text = "Dark Theme";
            this.radioThemeDark.UseVisualStyleBackColor = true;
            this.radioThemeDark.CheckedChanged += new System.EventHandler(this.radioThemeDark_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkHtmlExperimental);
            this.groupBox7.Controls.Add(this.chkHtmlExternalScripts);
            this.groupBox7.Location = new System.Drawing.Point(240, 72);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(216, 132);
            this.groupBox7.TabIndex = 52;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Experimental Mode";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkSimpleRotationTab);
            this.groupBox6.Controls.Add(this.chkShowAutos);
            this.groupBox6.Controls.Add(this.chkLargeSkillIcons);
            this.groupBox6.Location = new System.Drawing.Point(240, 216);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(216, 108);
            this.groupBox6.TabIndex = 51;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Simple Rotation Tab";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkRotation);
            this.groupBox5.Controls.Add(this.chkSkillIcons);
            this.groupBox5.Controls.Add(this.chkCombatReplay);
            this.groupBox5.Location = new System.Drawing.Point(12, 216);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(216, 108);
            this.groupBox5.TabIndex = 50;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Player Rotation";
            // 
            // grpGraphs
            // 
            this.grpGraphs.Controls.Add(this.chkPlayerDpsPlot);
            this.grpGraphs.Controls.Add(this.chkShow10s);
            this.grpGraphs.Controls.Add(this.chkShow30s);
            this.grpGraphs.Controls.Add(this.chkShowCl);
            this.grpGraphs.Location = new System.Drawing.Point(12, 72);
            this.grpGraphs.Name = "grpGraphs";
            this.grpGraphs.Size = new System.Drawing.Size(216, 132);
            this.grpGraphs.TabIndex = 49;
            this.grpGraphs.TabStop = false;
            this.grpGraphs.Text = "Damage Graphs";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkOutputCsv);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(471, 372);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "CSV";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panelJson);
            this.tabPage4.Controls.Add(this.chkOutputJson);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(471, 372);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "JSON";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.UploadDPSReports_checkbox);
            this.tabPage5.Controls.Add(this.UploadDRRH_check);
            this.tabPage5.Controls.Add(this.UploadRaidar_check);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(471, 372);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Upload";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label2);
            this.tabPage6.Controls.Add(this.label1);
            this.tabPage6.Controls.Add(this.btnRetrySkillList);
            this.tabPage6.Controls.Add(this.btnResetSkillList);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(471, 372);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Maintenance";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(395, 421);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(84, 24);
            this.cmdClose.TabIndex = 48;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // panelHtml
            // 
            this.panelHtml.Controls.Add(this.chkEventListDbg);
            this.panelHtml.Controls.Add(this.panel1);
            this.panelHtml.Controls.Add(this.chkShowEstimates);
            this.panelHtml.Controls.Add(this.groupBox7);
            this.panelHtml.Controls.Add(this.grpGraphs);
            this.panelHtml.Controls.Add(this.groupBox6);
            this.panelHtml.Controls.Add(this.groupBox5);
            this.panelHtml.Location = new System.Drawing.Point(0, 36);
            this.panelHtml.Name = "panelHtml";
            this.panelHtml.Size = new System.Drawing.Size(468, 336);
            this.panelHtml.TabIndex = 54;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(310, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Updates missing skills in the local skill storage from the GW2 API";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(294, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Resets the local skill list and loads all skills from the GW2 API";
            // 
            // panelJson
            // 
            this.panelJson.Controls.Add(this.chkIndentJSON);
            this.panelJson.Location = new System.Drawing.Point(0, 36);
            this.panelJson.Name = "panelJson";
            this.panelJson.Size = new System.Drawing.Size(468, 144);
            this.panelJson.TabIndex = 45;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(503, 458);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblSettingsInfoMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
            this.Load += new System.EventHandler(this.SettingsFormLoad);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.grpGraphs.ResumeLayout(false);
            this.grpGraphs.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.panelHtml.ResumeLayout(false);
            this.panelHtml.PerformLayout();
            this.panelJson.ResumeLayout(false);
            this.panelJson.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkPlayerDpsPlot;
        private System.Windows.Forms.CheckBox chkUniversalBoons;
        private System.Windows.Forms.CheckBox chkImpProfSpecBoons;
        private System.Windows.Forms.CheckBox chkAllProfSpecBoons;
        private System.Windows.Forms.CheckBox chkSkillIcons;
        private System.Windows.Forms.CheckBox chkRotation;
        private System.Windows.Forms.Label lblSettingsInfoMsg;
        private System.Windows.Forms.CheckBox chkDefaultOutputLoc;
        private System.Windows.Forms.TextBox txtCustomSaveLoc;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFolderSelect;
        private System.Windows.Forms.Label lblCustomSaveLoc;
        private System.Windows.Forms.CheckBox chkEventListDbg;
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
        private System.Windows.Forms.CheckBox chkCombatReplay;
        private System.Windows.Forms.CheckBox chkShowCl;
        private System.Windows.Forms.CheckBox UploadDPSReports_checkbox;
        private System.Windows.Forms.CheckBox UploadDRRH_check;
        private System.Windows.Forms.CheckBox UploadRaidar_check;
        private System.Windows.Forms.CheckBox chkOutputJson;
        private System.Windows.Forms.CheckBox chkIndentJSON;
        private System.Windows.Forms.CheckBox chkHtmlExperimental;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkHtmlExternalScripts;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioThemeLight;
        private System.Windows.Forms.RadioButton radioThemeDark;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox grpGraphs;
        private System.Windows.Forms.Panel panelHtml;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelJson;
    }
}