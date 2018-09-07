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
            this.chkCombatReplay = new System.Windows.Forms.CheckBox();
            this.chkShowCl = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.UploadDPSReports_checkbox = new System.Windows.Forms.CheckBox();
            this.UploadDRRH_check = new System.Windows.Forms.CheckBox();
            this.UploadRaidar_check = new System.Windows.Forms.CheckBox();
            this.json = new System.Windows.Forms.Label();
            this.chkOutputJson = new System.Windows.Forms.CheckBox();
            this.chkIndentJSON = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblDamageGraphSection
            // 
            this.lblDamageGraphSection.AutoSize = true;
            this.lblDamageGraphSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDamageGraphSection.Location = new System.Drawing.Point(45, 419);
            this.lblDamageGraphSection.Name = "lblDamageGraphSection";
            this.lblDamageGraphSection.Size = new System.Drawing.Size(97, 13);
            this.lblDamageGraphSection.TabIndex = 0;
            this.lblDamageGraphSection.Text = "Damage Graphs";
            // 
            // chkPlayerDpsPlot
            // 
            this.chkPlayerDpsPlot.AutoSize = true;
            this.chkPlayerDpsPlot.Checked = true;
            this.chkPlayerDpsPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPlayerDpsPlot.Location = new System.Drawing.Point(60, 435);
            this.chkPlayerDpsPlot.Name = "chkPlayerDpsPlot";
            this.chkPlayerDpsPlot.Size = new System.Drawing.Size(190, 17);
            this.chkPlayerDpsPlot.TabIndex = 1;
            this.chkPlayerDpsPlot.Text = "Show Each Players Total DPS plot";
            this.chkPlayerDpsPlot.UseVisualStyleBackColor = true;
            this.chkPlayerDpsPlot.CheckedChanged += new System.EventHandler(this.PlayerDpsPlotCheckedChanged);
            // 
            // chkUniversalBoons
            // 
            this.chkUniversalBoons.AutoSize = true;
            this.chkUniversalBoons.Checked = true;
            this.chkUniversalBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUniversalBoons.Location = new System.Drawing.Point(51, 92);
            this.chkUniversalBoons.Name = "chkUniversalBoons";
            this.chkUniversalBoons.Size = new System.Drawing.Size(148, 17);
            this.chkUniversalBoons.TabIndex = 5;
            this.chkUniversalBoons.Text = "Compute Universal Boons";
            this.chkUniversalBoons.UseVisualStyleBackColor = true;
            this.chkUniversalBoons.CheckedChanged += new System.EventHandler(this.UniversalBoonsCheckedChanged);
            // 
            // lblBoonGraphSection
            // 
            this.lblBoonGraphSection.AutoSize = true;
            this.lblBoonGraphSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBoonGraphSection.Location = new System.Drawing.Point(35, 76);
            this.lblBoonGraphSection.Name = "lblBoonGraphSection";
            this.lblBoonGraphSection.Size = new System.Drawing.Size(42, 13);
            this.lblBoonGraphSection.TabIndex = 6;
            this.lblBoonGraphSection.Text = "Boons";
            // 
            // chkImpProfSpecBoons
            // 
            this.chkImpProfSpecBoons.AutoSize = true;
            this.chkImpProfSpecBoons.Checked = true;
            this.chkImpProfSpecBoons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImpProfSpecBoons.Location = new System.Drawing.Point(51, 115);
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
            this.chkAllProfSpecBoons.Location = new System.Drawing.Point(51, 138);
            this.chkAllProfSpecBoons.Name = "chkAllProfSpecBoons";
            this.chkAllProfSpecBoons.Size = new System.Drawing.Size(148, 17);
            this.chkAllProfSpecBoons.TabIndex = 8;
            this.chkAllProfSpecBoons.Text = "Compute Remaining Buffs";
            this.chkAllProfSpecBoons.UseVisualStyleBackColor = true;
            this.chkAllProfSpecBoons.CheckedChanged += new System.EventHandler(this.AllProfessionSpecificBoonsCheckedChanged);
            // 
            // lblRotationSection
            // 
            this.lblRotationSection.AutoSize = true;
            this.lblRotationSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRotationSection.Location = new System.Drawing.Point(397, 76);
            this.lblRotationSection.Name = "lblRotationSection";
            this.lblRotationSection.Size = new System.Drawing.Size(55, 13);
            this.lblRotationSection.TabIndex = 9;
            this.lblRotationSection.Text = "Rotation";
            // 
            // chkSkillIcons
            // 
            this.chkSkillIcons.AutoSize = true;
            this.chkSkillIcons.Checked = true;
            this.chkSkillIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkillIcons.Location = new System.Drawing.Point(417, 115);
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
            this.chkRotation.Location = new System.Drawing.Point(410, 92);
            this.chkRotation.Name = "chkRotation";
            this.chkRotation.Size = new System.Drawing.Size(111, 17);
            this.chkRotation.TabIndex = 11;
            this.chkRotation.Text = "Compute Rotation";
            this.chkRotation.UseVisualStyleBackColor = true;
            this.chkRotation.CheckedChanged += new System.EventHandler(this.RotationCheckedChanged);
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(27, 13);
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
            this.chkDefaultOutputLoc.Location = new System.Drawing.Point(37, 221);
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
            this.txtCustomSaveLoc.Location = new System.Drawing.Point(27, 260);
            this.txtCustomSaveLoc.Name = "txtCustomSaveLoc";
            this.txtCustomSaveLoc.Size = new System.Drawing.Size(412, 20);
            this.txtCustomSaveLoc.TabIndex = 15;
            this.txtCustomSaveLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomSaveLoc.TextChanged += new System.EventHandler(this.CustomSaveLocationTextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(445, 257);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(45, 23);
            this.btnFolderSelect.TabIndex = 16;
            this.btnFolderSelect.Text = "Select";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.BtnFolderSelectClick);
            // 
            // lblCustomSaveLoc
            // 
            this.lblCustomSaveLoc.AutoSize = true;
            this.lblCustomSaveLoc.Location = new System.Drawing.Point(103, 241);
            this.lblCustomSaveLoc.Name = "lblCustomSaveLoc";
            this.lblCustomSaveLoc.Size = new System.Drawing.Size(59, 13);
            this.lblCustomSaveLoc.TabIndex = 17;
            this.lblCustomSaveLoc.Text = "Or save at:";
            // 
            // chkEventListDbg
            // 
            this.chkEventListDbg.AutoSize = true;
            this.chkEventListDbg.Location = new System.Drawing.Point(48, 356);
            this.chkEventListDbg.Name = "chkEventListDbg";
            this.chkEventListDbg.Size = new System.Drawing.Size(177, 17);
            this.chkEventListDbg.TabIndex = 18;
            this.chkEventListDbg.Text = "Show Event List (for debugging)";
            this.chkEventListDbg.UseVisualStyleBackColor = true;
            this.chkEventListDbg.CheckedChanged += new System.EventHandler(this.EventListDebugCheckedChanged);
            // 
            // lblBossSection
            // 
            this.lblBossSection.AutoSize = true;
            this.lblBossSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBossSection.Location = new System.Drawing.Point(259, 76);
            this.lblBossSection.Name = "lblBossSection";
            this.lblBossSection.Size = new System.Drawing.Size(34, 13);
            this.lblBossSection.TabIndex = 19;
            this.lblBossSection.Text = "Boss";
            // 
            // checkBossSummary
            // 
            this.checkBossSummary.AutoSize = true;
            this.checkBossSummary.Checked = true;
            this.checkBossSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBossSummary.Location = new System.Drawing.Point(276, 92);
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
            this.chkSimpleRotationTab.Location = new System.Drawing.Point(274, 435);
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
            this.chkShowAutos.Location = new System.Drawing.Point(278, 458);
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
            this.chkLargeSkillIcons.Location = new System.Drawing.Point(278, 481);
            this.chkLargeSkillIcons.Name = "chkLargeSkillIcons";
            this.chkLargeSkillIcons.Size = new System.Drawing.Size(104, 17);
            this.chkLargeSkillIcons.TabIndex = 23;
            this.chkLargeSkillIcons.Text = "Large Skill Icons";
            this.chkLargeSkillIcons.UseVisualStyleBackColor = true;
            this.chkLargeSkillIcons.CheckedChanged += new System.EventHandler(this.LargeSkillIconsCheckedChanged);
            // 
            // btnResetSkillList
            // 
            this.btnResetSkillList.Location = new System.Drawing.Point(444, 8);
            this.btnResetSkillList.Name = "btnResetSkillList";
            this.btnResetSkillList.Size = new System.Drawing.Size(108, 23);
            this.btnResetSkillList.TabIndex = 24;
            this.btnResetSkillList.Text = "Reset Skill List ";
            this.btnResetSkillList.UseVisualStyleBackColor = true;
            this.btnResetSkillList.Click += new System.EventHandler(this.ResetSkillListClick);
            // 
            // btnRetrySkillList
            // 
            this.btnRetrySkillList.Location = new System.Drawing.Point(331, 8);
            this.btnRetrySkillList.Name = "btnRetrySkillList";
            this.btnRetrySkillList.Size = new System.Drawing.Size(108, 23);
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
            this.chkOutputHtml.Location = new System.Drawing.Point(48, 333);
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
            this.chkOutputCsv.Location = new System.Drawing.Point(456, 333);
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
            this.chkShowEstimates.Location = new System.Drawing.Point(48, 379);
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
            this.chkPhaseParsing.Location = new System.Drawing.Point(276, 115);
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
            this.chkShow10s.Location = new System.Drawing.Point(60, 457);
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
            this.chkShow30s.Location = new System.Drawing.Point(60, 480);
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
            this.chkOneAtATime.Location = new System.Drawing.Point(37, 286);
            this.chkOneAtATime.Name = "chkOneAtATime";
            this.chkOneAtATime.Size = new System.Drawing.Size(287, 17);
            this.chkOneAtATime.TabIndex = 29;
            this.chkOneAtATime.Text = "Parse logs one at a time (less CPU load, slower parsing)";
            this.chkOneAtATime.UseVisualStyleBackColor = true;
            this.chkOneAtATime.CheckedChanged += new System.EventHandler(this.ChkOneAtATimeCheckedChanged);
            // 
            // StatisticsSelection
            // 
            this.StatisticsSelection.AutoSize = true;
            this.StatisticsSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatisticsSelection.Location = new System.Drawing.Point(22, 41);
            this.StatisticsSelection.Name = "StatisticsSelection";
            this.StatisticsSelection.Size = new System.Drawing.Size(109, 25);
            this.StatisticsSelection.TabIndex = 33;
            this.StatisticsSelection.Text = "Statistics";
            // 
            // OutputSelection
            // 
            this.OutputSelection.AutoSize = true;
            this.OutputSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputSelection.Location = new System.Drawing.Point(25, 184);
            this.OutputSelection.Name = "OutputSelection";
            this.OutputSelection.Size = new System.Drawing.Size(82, 25);
            this.OutputSelection.TabIndex = 34;
            this.OutputSelection.Text = "Output";
            // 
            // html
            // 
            this.html.AutoSize = true;
            this.html.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.html.Location = new System.Drawing.Point(34, 306);
            this.html.Name = "html";
            this.html.Size = new System.Drawing.Size(52, 24);
            this.html.TabIndex = 35;
            this.html.Text = "Html";
            // 
            // csv
            // 
            this.csv.AutoSize = true;
            this.csv.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.csv.Location = new System.Drawing.Point(441, 306);
            this.csv.Name = "csv";
            this.csv.Size = new System.Drawing.Size(44, 24);
            this.csv.TabIndex = 36;
            this.csv.Text = "Csv";
            // 
            // lblRotationGraphSection
            // 
            this.lblRotationGraphSection.AutoSize = true;
            this.lblRotationGraphSection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRotationGraphSection.Location = new System.Drawing.Point(259, 419);
            this.lblRotationGraphSection.Name = "lblRotationGraphSection";
            this.lblRotationGraphSection.Size = new System.Drawing.Size(81, 13);
            this.lblRotationGraphSection.TabIndex = 37;
            this.lblRotationGraphSection.Text = "Rotation Tab";
            // 
            // chkLightTheme
            // 
            this.chkLightTheme.AutoSize = true;
            this.chkLightTheme.Location = new System.Drawing.Point(48, 400);
            this.chkLightTheme.Name = "chkLightTheme";
            this.chkLightTheme.Size = new System.Drawing.Size(85, 17);
            this.chkLightTheme.TabIndex = 39;
            this.chkLightTheme.Text = "Light Theme";
            this.chkLightTheme.UseVisualStyleBackColor = true;
            this.chkLightTheme.CheckedChanged += new System.EventHandler(this.LightThemeCheckedChanged);
            // 
            // chkCombatReplay
            // 
            this.chkCombatReplay.AutoSize = true;
            this.chkCombatReplay.Checked = true;
            this.chkCombatReplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCombatReplay.Location = new System.Drawing.Point(410, 138);
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
            this.chkShowCl.Location = new System.Drawing.Point(60, 502);
            this.chkShowCl.Name = "chkShowCl";
            this.chkShowCl.Size = new System.Drawing.Size(145, 17);
            this.chkShowCl.TabIndex = 41;
            this.chkShowCl.Text = "Show Cleave Only Graph";
            this.chkShowCl.UseVisualStyleBackColor = true;
            this.chkShowCl.CheckedChanged += new System.EventHandler(this.ShowClCheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(614, 306);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 24);
            this.label1.TabIndex = 42;
            this.label1.Text = "Upload";
            // 
            // UploadDPSReports_checkbox
            // 
            this.UploadDPSReports_checkbox.AutoSize = true;
            this.UploadDPSReports_checkbox.Location = new System.Drawing.Point(589, 333);
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
            this.UploadDRRH_check.Location = new System.Drawing.Point(589, 356);
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
            this.UploadRaidar_check.Location = new System.Drawing.Point(589, 379);
            this.UploadRaidar_check.Name = "UploadRaidar_check";
            this.UploadRaidar_check.Size = new System.Drawing.Size(175, 17);
            this.UploadRaidar_check.TabIndex = 45;
            this.UploadRaidar_check.Text = "Upload to Raidar (Not Working)";
            this.UploadRaidar_check.UseVisualStyleBackColor = true;
            this.UploadRaidar_check.CheckedChanged += new System.EventHandler(this.UploadRaidar_check_CheckedChanged);
            // 
            // json
            // 
            this.json.AutoSize = true;
            this.json.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.json.Location = new System.Drawing.Point(440, 356);
            this.json.Name = "json";
            this.json.Size = new System.Drawing.Size(54, 24);
            this.json.TabIndex = 42;
            this.json.Text = "Json";
            // 
            // chkOutputJson
            // 
            this.chkOutputJson.AutoSize = true;
            this.chkOutputJson.Location = new System.Drawing.Point(456, 383);
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
            this.chkIndentJSON.Location = new System.Drawing.Point(456, 406);
            this.chkIndentJSON.Name = "chkIndentJSON";
            this.chkIndentJSON.Size = new System.Drawing.Size(87, 17);
            this.chkIndentJSON.TabIndex = 44;
            this.chkIndentJSON.Text = "Indent JSON";
            this.chkIndentJSON.UseVisualStyleBackColor = true;
            this.chkIndentJSON.CheckedChanged += new System.EventHandler(this.chkIndentJSONCheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(853, 553);
            this.Controls.Add(this.UploadRaidar_check);
            this.Controls.Add(this.UploadDRRH_check);
            this.Controls.Add(this.UploadDPSReports_checkbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkIndentJSON);
            this.Controls.Add(this.chkOutputJson);
            this.Controls.Add(this.json);
            this.Controls.Add(this.chkShowCl);
            this.Controls.Add(this.chkCombatReplay);
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
            this.Controls.Add(this.chkPlayerDpsPlot);
            this.Controls.Add(this.lblDamageGraphSection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
            this.Load += new System.EventHandler(this.SettingsFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDamageGraphSection;
        private System.Windows.Forms.CheckBox chkPlayerDpsPlot;
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
        private System.Windows.Forms.CheckBox chkCombatReplay;
        private System.Windows.Forms.CheckBox chkShowCl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox UploadDPSReports_checkbox;
        private System.Windows.Forms.CheckBox UploadDRRH_check;
        private System.Windows.Forms.CheckBox UploadRaidar_check;
        private System.Windows.Forms.Label json;
        private System.Windows.Forms.CheckBox chkOutputJson;
        private System.Windows.Forms.CheckBox chkIndentJSON;
    }
}