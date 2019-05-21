namespace LuckParser.Setting
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
            this.lblSettingsInfoMsg = new System.Windows.Forms.Label();
            this.chkDefaultOutputLoc = new System.Windows.Forms.CheckBox();
            this.txtCustomSaveLoc = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFolderSelect = new System.Windows.Forms.Button();
            this.lblCustomSaveLoc = new System.Windows.Forms.Label();
            this.btnResetSkillList = new System.Windows.Forms.Button();
            this.btnResetSpecList = new System.Windows.Forms.Button();
            this.chkOutputHtml = new System.Windows.Forms.CheckBox();
            this.chkOutputCsv = new System.Windows.Forms.CheckBox();
            this.chkShowEstimates = new System.Windows.Forms.CheckBox();
            this.chkPhaseParsing = new System.Windows.Forms.CheckBox();
            this.chkOneAtATime = new System.Windows.Forms.CheckBox();
            this.chkCombatReplay = new System.Windows.Forms.CheckBox();
            this.UploadDPSReports_checkbox = new System.Windows.Forms.CheckBox();
            this.UploadDRRH_check = new System.Windows.Forms.CheckBox();
            this.UploadRaidar_check = new System.Windows.Forms.CheckBox();
            this.chkOutputJson = new System.Windows.Forms.CheckBox();
            this.chkIndentJSON = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkHtmlExternalScripts = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAutoParse = new System.Windows.Forms.CheckBox();
            this.chkAutoAdd = new System.Windows.Forms.CheckBox();
            this.chkB_SkipFailedTries = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkAnonymous = new System.Windows.Forms.CheckBox();
            this.chkAddDuration = new System.Windows.Forms.CheckBox();
            this.chkAddPoVProf = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.imgTheme = new System.Windows.Forms.PictureBox();
            this.panelHtml = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioThemeLight = new System.Windows.Forms.RadioButton();
            this.radioThemeDark = new System.Windows.Forms.RadioButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panelXML = new System.Windows.Forms.Panel();
            this.chkIndentXML = new System.Windows.Forms.CheckBox();
            this.chkOutputXml = new System.Windows.Forms.CheckBox();
            this.panelJson = new System.Windows.Forms.Panel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.dumpButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTheme)).BeginInit();
            this.panelHtml.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panelXML.SuspendLayout();
            this.panelJson.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(9, 292);
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
            // btnResetSpecList
            // 
            this.btnResetSpecList.Location = new System.Drawing.Point(24, 48);
            this.btnResetSpecList.Name = "btnResetSpecList";
            this.btnResetSpecList.Size = new System.Drawing.Size(144, 23);
            this.btnResetSpecList.TabIndex = 25;
            this.btnResetSpecList.Text = "Reset Spec List";
            this.btnResetSpecList.UseVisualStyleBackColor = true;
            this.btnResetSpecList.Click += new System.EventHandler(this.ResetSpecListClick);
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
            this.chkPhaseParsing.Location = new System.Drawing.Point(6, 19);
            this.chkPhaseParsing.Name = "chkPhaseParsing";
            this.chkPhaseParsing.Size = new System.Drawing.Size(91, 17);
            this.chkPhaseParsing.TabIndex = 30;
            this.chkPhaseParsing.Text = "Parse Phases";
            this.chkPhaseParsing.UseVisualStyleBackColor = true;
            this.chkPhaseParsing.CheckedChanged += new System.EventHandler(this.PhaseParsingCheckedChanged);
            // 
            // chkOneAtATime
            // 
            this.chkOneAtATime.AutoSize = true;
            this.chkOneAtATime.Location = new System.Drawing.Point(6, 19);
            this.chkOneAtATime.Name = "chkOneAtATime";
            this.chkOneAtATime.Size = new System.Drawing.Size(139, 17);
            this.chkOneAtATime.TabIndex = 29;
            this.chkOneAtATime.Text = "Parse logs one at a time";
            this.toolTip1.SetToolTip(this.chkOneAtATime, "Slower parsing but less ressource hungry. Keep default value if unsure.");
            this.chkOneAtATime.UseVisualStyleBackColor = true;
            this.chkOneAtATime.CheckedChanged += new System.EventHandler(this.ChkOneAtATimeCheckedChanged);
            // 
            // chkCombatReplay
            // 
            this.chkCombatReplay.AutoSize = true;
            this.chkCombatReplay.Checked = true;
            this.chkCombatReplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCombatReplay.Location = new System.Drawing.Point(6, 42);
            this.chkCombatReplay.Name = "chkCombatReplay";
            this.chkCombatReplay.Size = new System.Drawing.Size(143, 17);
            this.chkCombatReplay.TabIndex = 40;
            this.chkCombatReplay.Text = "Compute Combat Replay";
            this.chkCombatReplay.UseVisualStyleBackColor = true;
            this.chkCombatReplay.CheckedChanged += new System.EventHandler(this.ChkCombatReplayCheckedChanged);
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
            this.chkIndentJSON.Location = new System.Drawing.Point(3, 3);
            this.chkIndentJSON.Name = "chkIndentJSON";
            this.chkIndentJSON.Size = new System.Drawing.Size(87, 17);
            this.chkIndentJSON.TabIndex = 44;
            this.chkIndentJSON.Text = "Indent JSON";
            this.chkIndentJSON.UseVisualStyleBackColor = true;
            this.chkIndentJSON.CheckedChanged += new System.EventHandler(this.ChkIndentJSONCheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "Setting description";
            // 
            // chkHtmlExternalScripts
            // 
            this.chkHtmlExternalScripts.AutoSize = true;
            this.chkHtmlExternalScripts.Location = new System.Drawing.Point(12, 12);
            this.chkHtmlExternalScripts.Name = "chkHtmlExternalScripts";
            this.chkHtmlExternalScripts.Size = new System.Drawing.Size(99, 17);
            this.chkHtmlExternalScripts.TabIndex = 46;
            this.chkHtmlExternalScripts.Text = "External Scripts";
            this.chkHtmlExternalScripts.UseVisualStyleBackColor = true;
            this.chkHtmlExternalScripts.CheckedChanged += new System.EventHandler(this.ChkHtmlExternalScripts_CheckedChanged);
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
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(479, 277);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(471, 251);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkOneAtATime);
            this.groupBox1.Controls.Add(this.chkAutoParse);
            this.groupBox1.Controls.Add(this.chkAutoAdd);
            this.groupBox1.Controls.Add(this.chkB_SkipFailedTries);
            this.groupBox1.Location = new System.Drawing.Point(240, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 108);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parsing";
            // 
            // chkAutoParse
            // 
            this.chkAutoParse.AutoSize = true;
            this.chkAutoParse.Location = new System.Drawing.Point(6, 85);
            this.chkAutoParse.Name = "chkAutoParse";
            this.chkAutoParse.Size = new System.Drawing.Size(171, 17);
            this.chkAutoParse.TabIndex = 39;
            this.chkAutoParse.Text = "Automatically parse added files";
            this.chkAutoParse.UseVisualStyleBackColor = true;
            this.chkAutoParse.CheckedChanged += new System.EventHandler(this.ChkAutoParse_CheckedChanged);
            // 
            // chkAutoAdd
            // 
            this.chkAutoAdd.AutoSize = true;
            this.chkAutoAdd.Location = new System.Drawing.Point(6, 65);
            this.chkAutoAdd.Name = "chkAutoAdd";
            this.chkAutoAdd.Size = new System.Drawing.Size(154, 17);
            this.chkAutoAdd.TabIndex = 40;
            this.chkAutoAdd.Text = "Automatically add new logs";
            this.chkAutoAdd.UseVisualStyleBackColor = true;
            this.chkAutoAdd.CheckedChanged += new System.EventHandler(this.ChkAutoAdd_CheckedChanged);
            // 
            // chkB_SkipFailedTries
            // 
            this.chkB_SkipFailedTries.AutoSize = true;
            this.chkB_SkipFailedTries.Location = new System.Drawing.Point(6, 42);
            this.chkB_SkipFailedTries.Name = "chkB_SkipFailedTries";
            this.chkB_SkipFailedTries.Size = new System.Drawing.Size(141, 17);
            this.chkB_SkipFailedTries.TabIndex = 38;
            this.chkB_SkipFailedTries.Text = "Skip generating log if fail";
            this.chkB_SkipFailedTries.UseVisualStyleBackColor = true;
            this.chkB_SkipFailedTries.CheckedChanged += new System.EventHandler(this.ChkB_SkipFailedTries_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkAnonymous);
            this.groupBox3.Controls.Add(this.chkAddDuration);
            this.groupBox3.Controls.Add(this.chkAddPoVProf);
            this.groupBox3.Controls.Add(this.chkDefaultOutputLoc);
            this.groupBox3.Controls.Add(this.btnFolderSelect);
            this.groupBox3.Controls.Add(this.txtCustomSaveLoc);
            this.groupBox3.Controls.Add(this.lblCustomSaveLoc);
            this.groupBox3.Location = new System.Drawing.Point(12, 132);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 123);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output";
            // 
            // checkAnonymous
            // 
            this.chkAnonymous.AutoSize = true;
            this.chkAnonymous.Location = new System.Drawing.Point(228, 24);
            this.chkAnonymous.Name = "checkAnonymous";
            this.chkAnonymous.Size = new System.Drawing.Size(118, 17);
            this.chkAnonymous.TabIndex = 20;
            this.chkAnonymous.Text = "Anonymous Players";
            this.toolTip1.SetToolTip(this.chkAnonymous, "Replaces Players\' account names and character names by generic names");
            this.chkAnonymous.UseVisualStyleBackColor = true;
            this.chkAnonymous.CheckedChanged += new System.EventHandler(this.ChkAnonymous_CheckedChanged);
            // 
            // chkAddDuration
            // 
            this.chkAddDuration.AutoSize = true;
            this.chkAddDuration.Location = new System.Drawing.Point(240, 98);
            this.chkAddDuration.Name = "chkAddDuration";
            this.chkAddDuration.Size = new System.Drawing.Size(194, 17);
            this.chkAddDuration.TabIndex = 19;
            this.chkAddDuration.Text = "Add encounter duration to file name";
            this.chkAddDuration.UseVisualStyleBackColor = true;
            this.chkAddDuration.CheckedChanged += new System.EventHandler(this.ChkAddDuration_CheckedChanged);
            // 
            // chkAddPoVProf
            // 
            this.chkAddPoVProf.AutoSize = true;
            this.chkAddPoVProf.Location = new System.Drawing.Point(12, 98);
            this.chkAddPoVProf.Name = "chkAddPoVProf";
            this.chkAddPoVProf.Size = new System.Drawing.Size(176, 17);
            this.chkAddPoVProf.TabIndex = 18;
            this.chkAddPoVProf.Text = "Add PoV profession to file name";
            this.chkAddPoVProf.UseVisualStyleBackColor = true;
            this.chkAddPoVProf.CheckedChanged += new System.EventHandler(this.ChkAddPoVProf_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkPhaseParsing);
            this.groupBox2.Controls.Add(this.chkCombatReplay);
            this.groupBox2.Location = new System.Drawing.Point(12, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(216, 108);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Encounter";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.imgTheme);
            this.tabPage2.Controls.Add(this.panelHtml);
            this.tabPage2.Controls.Add(this.chkOutputHtml);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(471, 251);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "HTML";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // imgTheme
            // 
            this.imgTheme.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imgTheme.Location = new System.Drawing.Point(346, 30);
            this.imgTheme.Name = "imgTheme";
            this.imgTheme.Size = new System.Drawing.Size(110, 74);
            this.imgTheme.TabIndex = 49;
            this.imgTheme.TabStop = false;
            // 
            // panelHtml
            // 
            this.panelHtml.Controls.Add(this.panel1);
            this.panelHtml.Controls.Add(this.chkShowEstimates);
            this.panelHtml.Controls.Add(this.chkHtmlExternalScripts);
            this.panelHtml.Location = new System.Drawing.Point(0, 36);
            this.panelHtml.Name = "panelHtml";
            this.panelHtml.Size = new System.Drawing.Size(468, 87);
            this.panelHtml.TabIndex = 54;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioThemeLight);
            this.panel1.Controls.Add(this.radioThemeDark);
            this.panel1.Location = new System.Drawing.Point(252, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(156, 60);
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
            this.radioThemeLight.CheckedChanged += new System.EventHandler(this.RadioThemeLight_CheckedChanged);
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
            this.radioThemeDark.CheckedChanged += new System.EventHandler(this.RadioThemeDark_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkOutputCsv);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(471, 251);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "CSV";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panelXML);
            this.tabPage4.Controls.Add(this.chkOutputXml);
            this.tabPage4.Controls.Add(this.panelJson);
            this.tabPage4.Controls.Add(this.chkOutputJson);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(471, 251);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Raw formats";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panelXML
            // 
            this.panelXML.Controls.Add(this.chkIndentXML);
            this.panelXML.Location = new System.Drawing.Point(147, 35);
            this.panelXML.Name = "panelXML";
            this.panelXML.Size = new System.Drawing.Size(127, 35);
            this.panelXML.TabIndex = 47;
            // 
            // chkIndentXML
            // 
            this.chkIndentXML.AutoSize = true;
            this.chkIndentXML.Location = new System.Drawing.Point(3, 3);
            this.chkIndentXML.Name = "chkIndentXML";
            this.chkIndentXML.Size = new System.Drawing.Size(81, 17);
            this.chkIndentXML.TabIndex = 44;
            this.chkIndentXML.Text = "Indent XML";
            this.chkIndentXML.UseVisualStyleBackColor = true;
            this.chkIndentXML.CheckedChanged += new System.EventHandler(this.ChkIndentXMLCheckedChanged);
            // 
            // chkOutputXml
            // 
            this.chkOutputXml.AutoSize = true;
            this.chkOutputXml.Location = new System.Drawing.Point(147, 12);
            this.chkOutputXml.Name = "chkOutputXml";
            this.chkOutputXml.Size = new System.Drawing.Size(127, 17);
            this.chkOutputXml.TabIndex = 46;
            this.chkOutputXml.Text = "Output as XML (WIP)";
            this.chkOutputXml.UseVisualStyleBackColor = true;
            this.chkOutputXml.CheckedChanged += new System.EventHandler(this.OutputXMLCheckedChanged);
            // 
            // panelJson
            // 
            this.panelJson.Controls.Add(this.chkIndentJSON);
            this.panelJson.Location = new System.Drawing.Point(14, 35);
            this.panelJson.Name = "panelJson";
            this.panelJson.Size = new System.Drawing.Size(127, 35);
            this.panelJson.TabIndex = 45;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.UploadDPSReports_checkbox);
            this.tabPage5.Controls.Add(this.UploadDRRH_check);
            this.tabPage5.Controls.Add(this.UploadRaidar_check);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(471, 251);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Upload";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label2);
            this.tabPage6.Controls.Add(this.label1);
            this.tabPage6.Controls.Add(this.btnResetSpecList);
            this.tabPage6.Controls.Add(this.btnResetSkillList);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(471, 251);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Maintenance";
            this.tabPage6.UseVisualStyleBackColor = true;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Resets the local spec list and loads all specs from the GW2 API";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(412, 312);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(84, 24);
            this.cmdClose.TabIndex = 48;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdClose_Click);
            // 
            // dumpButton
            // 
            this.dumpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dumpButton.Location = new System.Drawing.Point(322, 312);
            this.dumpButton.Name = "dumpButton";
            this.dumpButton.Size = new System.Drawing.Size(84, 24);
            this.dumpButton.TabIndex = 49;
            this.dumpButton.Text = "Save Settings";
            this.dumpButton.UseVisualStyleBackColor = true;
            this.dumpButton.Click += new System.EventHandler(this.SettingsDump_Click);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.Location = new System.Drawing.Point(232, 312);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(84, 24);
            this.loadButton.TabIndex = 50;
            this.loadButton.Text = "Load Settings";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.SettingsLoad_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(503, 348);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.dumpButton);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTheme)).EndInit();
            this.panelHtml.ResumeLayout(false);
            this.panelHtml.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.panelXML.ResumeLayout(false);
            this.panelXML.PerformLayout();
            this.panelJson.ResumeLayout(false);
            this.panelJson.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblSettingsInfoMsg;
        private System.Windows.Forms.CheckBox chkDefaultOutputLoc;
        private System.Windows.Forms.TextBox txtCustomSaveLoc;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFolderSelect;
        private System.Windows.Forms.Label lblCustomSaveLoc;
        private System.Windows.Forms.Button btnResetSkillList;
        private System.Windows.Forms.Button btnResetSpecList;
        private System.Windows.Forms.CheckBox chkOutputHtml;
        private System.Windows.Forms.CheckBox chkOutputCsv;
        private System.Windows.Forms.CheckBox chkShowEstimates;
        private System.Windows.Forms.CheckBox chkPhaseParsing;
        private System.Windows.Forms.CheckBox chkOneAtATime;
        private System.Windows.Forms.CheckBox chkCombatReplay;
        private System.Windows.Forms.CheckBox UploadDPSReports_checkbox;
        private System.Windows.Forms.CheckBox UploadDRRH_check;
        private System.Windows.Forms.CheckBox UploadRaidar_check;
        private System.Windows.Forms.CheckBox chkOutputJson;
        private System.Windows.Forms.CheckBox chkIndentJSON;
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioThemeLight;
        private System.Windows.Forms.RadioButton radioThemeDark;
        private System.Windows.Forms.Panel panelHtml;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox imgTheme;
        private System.Windows.Forms.CheckBox chkB_SkipFailedTries;
        private System.Windows.Forms.CheckBox chkAutoAdd;
        private System.Windows.Forms.CheckBox chkAutoParse;
        private System.Windows.Forms.CheckBox chkAddPoVProf;
        private System.Windows.Forms.CheckBox chkAddDuration;
        private System.Windows.Forms.Panel panelJson;
        private System.Windows.Forms.CheckBox chkOutputXml;
        private System.Windows.Forms.Panel panelXML;
        private System.Windows.Forms.CheckBox chkIndentXML;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button dumpButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.CheckBox chkAnonymous;
    }
}