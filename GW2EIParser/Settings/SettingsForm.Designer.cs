namespace GW2EIParser.Setting
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
            this.btnResetTraitList = new System.Windows.Forms.Button();
            this.btnResetSpecList = new System.Windows.Forms.Button();
            this.chkOutputHtml = new System.Windows.Forms.CheckBox();
            this.chkOutputCsv = new System.Windows.Forms.CheckBox();
            this.chkPhaseParsing = new System.Windows.Forms.CheckBox();
            this.chkMultiThreaded = new System.Windows.Forms.CheckBox();
            this.chkCombatReplay = new System.Windows.Forms.CheckBox();
            this.UploadDPSReports_checkbox = new System.Windows.Forms.CheckBox();
            this.UploadDRRH_check = new System.Windows.Forms.CheckBox();
            this.UploadRaidar_check = new System.Windows.Forms.CheckBox();
            this.UploadWebhook_check = new System.Windows.Forms.CheckBox();
            this.UploadtxtWebhookUrl = new System.Windows.Forms.TextBox();
            this.chkOutputJson = new System.Windows.Forms.CheckBox();
            this.chkIndentJSON = new System.Windows.Forms.CheckBox();
            this.settingTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.chkMultiLogs = new System.Windows.Forms.CheckBox();
            this.chkAnonymous = new System.Windows.Forms.CheckBox();
            this.chkHtmlExternalScripts = new System.Windows.Forms.CheckBox();
            this.chkSaveOutTrace = new System.Windows.Forms.CheckBox();
            this.chkDamageMods = new System.Windows.Forms.CheckBox();
            this.chkRawTimelineArrays = new System.Windows.Forms.CheckBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.boxParsing = new System.Windows.Forms.GroupBox();
            this.chkAutoParse = new System.Windows.Forms.CheckBox();
            this.chkAutoAdd = new System.Windows.Forms.CheckBox();
            this.chkB_SkipFailedTries = new System.Windows.Forms.CheckBox();
            this.groupOutput = new System.Windows.Forms.GroupBox();
            this.chkAddDuration = new System.Windows.Forms.CheckBox();
            this.chkAddPoVProf = new System.Windows.Forms.CheckBox();
            this.groupEncounter = new System.Windows.Forms.GroupBox();
            this.tabHTML = new System.Windows.Forms.TabPage();
            this.imgTheme = new System.Windows.Forms.PictureBox();
            this.panelHtml = new System.Windows.Forms.Panel();
            this.panelTheme = new System.Windows.Forms.Panel();
            this.radioThemeLight = new System.Windows.Forms.RadioButton();
            this.radioThemeDark = new System.Windows.Forms.RadioButton();
            this.tabCSV = new System.Windows.Forms.TabPage();
            this.tabRaw = new System.Windows.Forms.TabPage();
            this.panelXML = new System.Windows.Forms.Panel();
            this.chkIndentXML = new System.Windows.Forms.CheckBox();
            this.chkOutputXml = new System.Windows.Forms.CheckBox();
            this.panelJson = new System.Windows.Forms.Panel();
            this.groupRawSettings = new System.Windows.Forms.GroupBox();
            this.chkCompressRaw = new System.Windows.Forms.CheckBox();
            this.tabUpload = new System.Windows.Forms.TabPage();
            this.groupWebhookSettings = new System.Windows.Forms.GroupBox();
            this.tabAPI = new System.Windows.Forms.TabPage();
            this.resetSkillLabel = new System.Windows.Forms.Label();
            this.resetTraitLabel = new System.Windows.Forms.Label();
            this.resetSpecLabel = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.dumpButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.boxParsing.SuspendLayout();
            this.groupOutput.SuspendLayout();
            this.groupEncounter.SuspendLayout();
            this.tabHTML.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTheme)).BeginInit();
            this.panelHtml.SuspendLayout();
            this.panelTheme.SuspendLayout();
            this.tabCSV.SuspendLayout();
            this.tabRaw.SuspendLayout();
            this.panelXML.SuspendLayout();
            this.panelJson.SuspendLayout();
            this.groupRawSettings.SuspendLayout();
            this.tabUpload.SuspendLayout();
            this.groupWebhookSettings.SuspendLayout();
            this.tabAPI.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(9, 350);
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
            this.btnResetSkillList.Location = new System.Drawing.Point(27, 132);
            this.btnResetSkillList.Name = "btnResetSkillList";
            this.btnResetSkillList.Size = new System.Drawing.Size(144, 23);
            this.btnResetSkillList.TabIndex = 24;
            this.btnResetSkillList.Text = "Reset Skill List ";
            this.btnResetSkillList.UseVisualStyleBackColor = true;
            this.btnResetSkillList.Click += new System.EventHandler(this.ResetSkillListClick);
            // 
            // btnResetTraitList
            // 
            this.btnResetTraitList.Location = new System.Drawing.Point(27, 78);
            this.btnResetTraitList.Name = "btnResetTraitList";
            this.btnResetTraitList.Size = new System.Drawing.Size(144, 23);
            this.btnResetTraitList.TabIndex = 24;
            this.btnResetTraitList.Text = "Reset Trait List ";
            this.btnResetTraitList.UseVisualStyleBackColor = true;
            this.btnResetTraitList.Click += new System.EventHandler(this.ResetTraitListClick);
            // 
            // btnResetSpecList
            // 
            this.btnResetSpecList.Location = new System.Drawing.Point(27, 27);
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
            // chkMultiThreaded
            // 
            this.chkMultiThreaded.AutoSize = true;
            this.chkMultiThreaded.Location = new System.Drawing.Point(6, 19);
            this.chkMultiThreaded.Name = "chkMultiThreaded";
            this.chkMultiThreaded.Size = new System.Drawing.Size(202, 17);
            this.chkMultiThreaded.TabIndex = 29;
            this.chkMultiThreaded.Text = "Use multi threading on log processing";
            this.settingTooltip.SetToolTip(this.chkMultiThreaded, "Keep default value if unsure.");
            this.chkMultiThreaded.UseVisualStyleBackColor = true;
            this.chkMultiThreaded.CheckedChanged += new System.EventHandler(this.ChkMultiThreadedCheckedChanged);
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
            // UploadWebhook_check
            // 
            this.UploadWebhook_check.AutoSize = true;
            this.UploadWebhook_check.Location = new System.Drawing.Point(12, 19);
            this.UploadWebhook_check.Name = "UploadWebhook_check";
            this.UploadWebhook_check.Size = new System.Drawing.Size(185, 17);
            this.UploadWebhook_check.TabIndex = 45;
            this.UploadWebhook_check.Text = "Send Embed to Discord webhook";
            this.settingTooltip.SetToolTip(this.UploadWebhook_check, "Disabled with Multiple Parse Logs and when npt uploading to dps.reports");
            this.UploadWebhook_check.UseVisualStyleBackColor = true;
            this.UploadWebhook_check.CheckedChanged += new System.EventHandler(this.UploadWebhook_check_CheckedChanged);
            // 
            // UploadtxtWebhookUrl
            // 
            this.UploadtxtWebhookUrl.Location = new System.Drawing.Point(12, 42);
            this.UploadtxtWebhookUrl.Name = "UploadtxtWebhookUrl";
            this.UploadtxtWebhookUrl.Size = new System.Drawing.Size(370, 20);
            this.UploadtxtWebhookUrl.TabIndex = 15;
            this.UploadtxtWebhookUrl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.UploadtxtWebhookUrl.TextChanged += new System.EventHandler(this.WebhookURLChanged);
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
            // settingTooltip
            // 
            this.settingTooltip.ToolTipTitle = "Setting description";
            // 
            // chkMultiLogs
            // 
            this.chkMultiLogs.AutoSize = true;
            this.chkMultiLogs.Location = new System.Drawing.Point(6, 42);
            this.chkMultiLogs.Name = "chkMultiLogs";
            this.chkMultiLogs.Size = new System.Drawing.Size(194, 17);
            this.chkMultiLogs.TabIndex = 20;
            this.chkMultiLogs.Text = "Parse Multiple logs at the same time";
            this.settingTooltip.SetToolTip(this.chkMultiLogs, "Keep default value if unsure.");
            this.chkMultiLogs.UseVisualStyleBackColor = true;
            this.chkMultiLogs.CheckedChanged += new System.EventHandler(this.ChkMultiLogs_CheckedChanged);
            // 
            // chkAnonymous
            // 
            this.chkAnonymous.AutoSize = true;
            this.chkAnonymous.Location = new System.Drawing.Point(228, 24);
            this.chkAnonymous.Name = "chkAnonymous";
            this.chkAnonymous.Size = new System.Drawing.Size(118, 17);
            this.chkAnonymous.TabIndex = 20;
            this.chkAnonymous.Text = "Anonymous Players";
            this.settingTooltip.SetToolTip(this.chkAnonymous, "Replaces Players\' account names and character names by generic names");
            this.chkAnonymous.UseVisualStyleBackColor = true;
            this.chkAnonymous.CheckedChanged += new System.EventHandler(this.ChkAnonymous_CheckedChanged);
            // 
            // chkHtmlExternalScripts
            // 
            this.chkHtmlExternalScripts.AutoSize = true;
            this.chkHtmlExternalScripts.Location = new System.Drawing.Point(12, 12);
            this.chkHtmlExternalScripts.Name = "chkHtmlExternalScripts";
            this.chkHtmlExternalScripts.Size = new System.Drawing.Size(99, 17);
            this.chkHtmlExternalScripts.TabIndex = 46;
            this.chkHtmlExternalScripts.Text = "External Scripts";
            this.settingTooltip.SetToolTip(this.chkHtmlExternalScripts, "Writes static css and js scripts in own files, which are shared between all logs." +
        " Log file size decreases, but the script files have to be kept along with the ht" +
        "ml.");
            this.chkHtmlExternalScripts.UseVisualStyleBackColor = true;
            this.chkHtmlExternalScripts.CheckedChanged += new System.EventHandler(this.ChkHtmlExternalScripts_CheckedChanged);
            // 
            // chkSaveOutTrace
            // 
            this.chkSaveOutTrace.AutoSize = true;
            this.chkSaveOutTrace.Location = new System.Drawing.Point(6, 134);
            this.chkSaveOutTrace.Name = "chkSaveOutTrace";
            this.chkSaveOutTrace.Size = new System.Drawing.Size(87, 17);
            this.chkSaveOutTrace.TabIndex = 40;
            this.chkSaveOutTrace.Text = "Save Traces";
            this.chkSaveOutTrace.UseVisualStyleBackColor = true;
            this.chkSaveOutTrace.CheckedChanged += new System.EventHandler(this.ChkSaveOutTrace_CheckedChanged);
            // 
            // chkDamageMods
            // 
            this.chkDamageMods.AutoSize = true;
            this.chkDamageMods.Checked = true;
            this.chkDamageMods.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDamageMods.Location = new System.Drawing.Point(6, 65);
            this.chkDamageMods.Name = "chkDamageMods";
            this.chkDamageMods.Size = new System.Drawing.Size(156, 17);
            this.chkDamageMods.TabIndex = 20;
            this.chkDamageMods.Text = "Compute Damage Modifiers";
            this.chkDamageMods.UseVisualStyleBackColor = true;
            this.chkDamageMods.CheckedChanged += new System.EventHandler(this.ChkComputeDamageMods_CheckedChanged);
            // 
            // chkRawTimelineArrays
            // 
            this.chkRawTimelineArrays.AutoSize = true;
            this.chkRawTimelineArrays.Location = new System.Drawing.Point(6, 45);
            this.chkRawTimelineArrays.Name = "chkRawTimelineArrays";
            this.chkRawTimelineArrays.Size = new System.Drawing.Size(119, 17);
            this.chkRawTimelineArrays.TabIndex = 20;
            this.chkRawTimelineArrays.Text = "Add Timeline Arrays";
            this.chkRawTimelineArrays.UseVisualStyleBackColor = true;
            this.chkRawTimelineArrays.CheckedChanged += new System.EventHandler(this.ChkRawTimelineArrays_CheckedChanged);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Controls.Add(this.tabHTML);
            this.tabControl.Controls.Add(this.tabCSV);
            this.tabControl.Controls.Add(this.tabRaw);
            this.tabControl.Controls.Add(this.tabUpload);
            this.tabControl.Controls.Add(this.tabAPI);
            this.tabControl.HotTrack = true;
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(479, 335);
            this.tabControl.TabIndex = 47;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.boxParsing);
            this.tabGeneral.Controls.Add(this.groupOutput);
            this.tabGeneral.Controls.Add(this.groupEncounter);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(471, 309);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // boxParsing
            // 
            this.boxParsing.Controls.Add(this.chkMultiThreaded);
            this.boxParsing.Controls.Add(this.chkMultiLogs);
            this.boxParsing.Controls.Add(this.chkAutoParse);
            this.boxParsing.Controls.Add(this.chkSaveOutTrace);
            this.boxParsing.Controls.Add(this.chkAutoAdd);
            this.boxParsing.Controls.Add(this.chkB_SkipFailedTries);
            this.boxParsing.Location = new System.Drawing.Point(240, 8);
            this.boxParsing.Name = "boxParsing";
            this.boxParsing.Size = new System.Drawing.Size(216, 160);
            this.boxParsing.TabIndex = 41;
            this.boxParsing.TabStop = false;
            this.boxParsing.Text = "Parsing";
            // 
            // chkAutoParse
            // 
            this.chkAutoParse.AutoSize = true;
            this.chkAutoParse.Location = new System.Drawing.Point(6, 111);
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
            this.chkAutoAdd.Location = new System.Drawing.Point(6, 88);
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
            this.chkB_SkipFailedTries.Location = new System.Drawing.Point(6, 65);
            this.chkB_SkipFailedTries.Name = "chkB_SkipFailedTries";
            this.chkB_SkipFailedTries.Size = new System.Drawing.Size(153, 17);
            this.chkB_SkipFailedTries.TabIndex = 38;
            this.chkB_SkipFailedTries.Text = "Skip log generation if failed";
            this.chkB_SkipFailedTries.UseVisualStyleBackColor = true;
            this.chkB_SkipFailedTries.CheckedChanged += new System.EventHandler(this.ChkB_SkipFailedTries_CheckedChanged);
            // 
            // groupOutput
            // 
            this.groupOutput.Controls.Add(this.chkAnonymous);
            this.groupOutput.Controls.Add(this.chkAddDuration);
            this.groupOutput.Controls.Add(this.chkAddPoVProf);
            this.groupOutput.Controls.Add(this.chkDefaultOutputLoc);
            this.groupOutput.Controls.Add(this.btnFolderSelect);
            this.groupOutput.Controls.Add(this.txtCustomSaveLoc);
            this.groupOutput.Controls.Add(this.lblCustomSaveLoc);
            this.groupOutput.Location = new System.Drawing.Point(12, 174);
            this.groupOutput.Name = "groupOutput";
            this.groupOutput.Size = new System.Drawing.Size(444, 129);
            this.groupOutput.TabIndex = 37;
            this.groupOutput.TabStop = false;
            this.groupOutput.Text = "Output";
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
            // groupEncounter
            // 
            this.groupEncounter.Controls.Add(this.chkPhaseParsing);
            this.groupEncounter.Controls.Add(this.chkCombatReplay);
            this.groupEncounter.Controls.Add(this.chkDamageMods);
            this.groupEncounter.Location = new System.Drawing.Point(12, 8);
            this.groupEncounter.Name = "groupEncounter";
            this.groupEncounter.Size = new System.Drawing.Size(216, 160);
            this.groupEncounter.TabIndex = 36;
            this.groupEncounter.TabStop = false;
            this.groupEncounter.Text = "Encounter";
            // 
            // tabHTML
            // 
            this.tabHTML.Controls.Add(this.imgTheme);
            this.tabHTML.Controls.Add(this.panelHtml);
            this.tabHTML.Controls.Add(this.chkOutputHtml);
            this.tabHTML.Location = new System.Drawing.Point(4, 22);
            this.tabHTML.Name = "tabHTML";
            this.tabHTML.Padding = new System.Windows.Forms.Padding(3);
            this.tabHTML.Size = new System.Drawing.Size(471, 309);
            this.tabHTML.TabIndex = 1;
            this.tabHTML.Text = "HTML";
            this.tabHTML.UseVisualStyleBackColor = true;
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
            this.panelHtml.Controls.Add(this.panelTheme);
            this.panelHtml.Controls.Add(this.chkHtmlExternalScripts);
            this.panelHtml.Location = new System.Drawing.Point(0, 36);
            this.panelHtml.Name = "panelHtml";
            this.panelHtml.Size = new System.Drawing.Size(468, 87);
            this.panelHtml.TabIndex = 54;
            // 
            // panelTheme
            // 
            this.panelTheme.Controls.Add(this.radioThemeLight);
            this.panelTheme.Controls.Add(this.radioThemeDark);
            this.panelTheme.Location = new System.Drawing.Point(252, 12);
            this.panelTheme.Name = "panelTheme";
            this.panelTheme.Size = new System.Drawing.Size(156, 60);
            this.panelTheme.TabIndex = 53;
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
            // tabCSV
            // 
            this.tabCSV.Controls.Add(this.chkOutputCsv);
            this.tabCSV.Location = new System.Drawing.Point(4, 22);
            this.tabCSV.Name = "tabCSV";
            this.tabCSV.Size = new System.Drawing.Size(471, 309);
            this.tabCSV.TabIndex = 2;
            this.tabCSV.Text = "CSV";
            this.tabCSV.UseVisualStyleBackColor = true;
            // 
            // tabRaw
            // 
            this.tabRaw.Controls.Add(this.panelXML);
            this.tabRaw.Controls.Add(this.chkOutputXml);
            this.tabRaw.Controls.Add(this.panelJson);
            this.tabRaw.Controls.Add(this.chkOutputJson);
            this.tabRaw.Controls.Add(this.groupRawSettings);
            this.tabRaw.Location = new System.Drawing.Point(4, 22);
            this.tabRaw.Name = "tabRaw";
            this.tabRaw.Size = new System.Drawing.Size(471, 309);
            this.tabRaw.TabIndex = 3;
            this.tabRaw.Text = "Raw formats";
            this.tabRaw.UseVisualStyleBackColor = true;
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
            this.chkOutputXml.Size = new System.Drawing.Size(97, 17);
            this.chkOutputXml.TabIndex = 46;
            this.chkOutputXml.Text = "Output as XML";
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
            // groupRawSettings
            // 
            this.groupRawSettings.Controls.Add(this.chkRawTimelineArrays);
            this.groupRawSettings.Controls.Add(this.chkCompressRaw);
            this.groupRawSettings.Location = new System.Drawing.Point(17, 87);
            this.groupRawSettings.Name = "groupRawSettings";
            this.groupRawSettings.Size = new System.Drawing.Size(150, 68);
            this.groupRawSettings.TabIndex = 45;
            this.groupRawSettings.TabStop = false;
            this.groupRawSettings.Text = "Raw Format Settings";
            // 
            // chkCompressRaw
            // 
            this.chkCompressRaw.AutoSize = true;
            this.chkCompressRaw.Location = new System.Drawing.Point(6, 22);
            this.chkCompressRaw.Name = "chkCompressRaw";
            this.chkCompressRaw.Size = new System.Drawing.Size(118, 17);
            this.chkCompressRaw.TabIndex = 18;
            this.chkCompressRaw.Text = "Compress Raw files";
            this.chkCompressRaw.UseVisualStyleBackColor = true;
            this.chkCompressRaw.CheckedChanged += new System.EventHandler(this.ChkCompressRaw_CheckedChanged);
            // 
            // tabUpload
            // 
            this.tabUpload.Controls.Add(this.UploadDPSReports_checkbox);
            this.tabUpload.Controls.Add(this.UploadDRRH_check);
            this.tabUpload.Controls.Add(this.UploadRaidar_check);
            this.tabUpload.Controls.Add(this.groupWebhookSettings);
            this.tabUpload.Location = new System.Drawing.Point(4, 22);
            this.tabUpload.Name = "tabUpload";
            this.tabUpload.Size = new System.Drawing.Size(471, 309);
            this.tabUpload.TabIndex = 4;
            this.tabUpload.Text = "Upload";
            this.tabUpload.UseVisualStyleBackColor = true;
            // 
            // groupWebhookSettings
            // 
            this.groupWebhookSettings.Controls.Add(this.UploadtxtWebhookUrl);
            this.groupWebhookSettings.Controls.Add(this.UploadWebhook_check);
            this.groupWebhookSettings.Location = new System.Drawing.Point(17, 87);
            this.groupWebhookSettings.Name = "groupWebhookSettings";
            this.groupWebhookSettings.Size = new System.Drawing.Size(404, 92);
            this.groupWebhookSettings.TabIndex = 45;
            this.groupWebhookSettings.TabStop = false;
            this.groupWebhookSettings.Text = "Webhook Settings";
            // 
            // tabAPI
            // 
            this.tabAPI.Controls.Add(this.resetSkillLabel);
            this.tabAPI.Controls.Add(this.resetTraitLabel);
            this.tabAPI.Controls.Add(this.resetSpecLabel);
            this.tabAPI.Controls.Add(this.btnResetSpecList);
            this.tabAPI.Controls.Add(this.btnResetTraitList);
            this.tabAPI.Controls.Add(this.btnResetSkillList);
            this.tabAPI.Location = new System.Drawing.Point(4, 22);
            this.tabAPI.Name = "tabAPI";
            this.tabAPI.Size = new System.Drawing.Size(471, 309);
            this.tabAPI.TabIndex = 5;
            this.tabAPI.Text = "Maintenance";
            this.tabAPI.UseVisualStyleBackColor = true;
            // 
            // resetSkillLabel
            // 
            this.resetSkillLabel.AutoSize = true;
            this.resetSkillLabel.Location = new System.Drawing.Point(24, 116);
            this.resetSkillLabel.Name = "resetSkillLabel";
            this.resetSkillLabel.Size = new System.Drawing.Size(294, 13);
            this.resetSkillLabel.TabIndex = 27;
            this.resetSkillLabel.Text = "Resets the local skill list and loads all skills from the GW2 API";
            // 
            // resetTraitLabel
            // 
            this.resetTraitLabel.AutoSize = true;
            this.resetTraitLabel.Location = new System.Drawing.Point(24, 62);
            this.resetTraitLabel.Name = "resetTraitLabel";
            this.resetTraitLabel.Size = new System.Drawing.Size(289, 13);
            this.resetTraitLabel.TabIndex = 28;
            this.resetTraitLabel.Text = "Resets the local trait list and loads all trait from the GW2 API";
            // 
            // resetSpecLabel
            // 
            this.resetSpecLabel.AutoSize = true;
            this.resetSpecLabel.Location = new System.Drawing.Point(24, 11);
            this.resetSpecLabel.Name = "resetSpecLabel";
            this.resetSpecLabel.Size = new System.Drawing.Size(306, 13);
            this.resetSpecLabel.TabIndex = 26;
            this.resetSpecLabel.Text = "Resets the local spec list and loads all specs from the GW2 API";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(412, 370);
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
            this.dumpButton.Location = new System.Drawing.Point(322, 370);
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
            this.loadButton.Location = new System.Drawing.Point(232, 370);
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
            this.ClientSize = new System.Drawing.Size(503, 406);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.dumpButton);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lblSettingsInfoMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
            this.VisibleChanged += new System.EventHandler(this.SettingsFormLoad);
            this.tabControl.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.boxParsing.ResumeLayout(false);
            this.boxParsing.PerformLayout();
            this.groupOutput.ResumeLayout(false);
            this.groupOutput.PerformLayout();
            this.groupEncounter.ResumeLayout(false);
            this.groupEncounter.PerformLayout();
            this.tabHTML.ResumeLayout(false);
            this.tabHTML.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTheme)).EndInit();
            this.panelHtml.ResumeLayout(false);
            this.panelHtml.PerformLayout();
            this.panelTheme.ResumeLayout(false);
            this.panelTheme.PerformLayout();
            this.tabCSV.ResumeLayout(false);
            this.tabCSV.PerformLayout();
            this.tabRaw.ResumeLayout(false);
            this.tabRaw.PerformLayout();
            this.panelXML.ResumeLayout(false);
            this.panelXML.PerformLayout();
            this.panelJson.ResumeLayout(false);
            this.panelJson.PerformLayout();
            this.groupRawSettings.ResumeLayout(false);
            this.groupRawSettings.PerformLayout();
            this.tabUpload.ResumeLayout(false);
            this.tabUpload.PerformLayout();
            this.groupWebhookSettings.ResumeLayout(false);
            this.groupWebhookSettings.PerformLayout();
            this.tabAPI.ResumeLayout(false);
            this.tabAPI.PerformLayout();
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
        private System.Windows.Forms.Button btnResetTraitList;
        private System.Windows.Forms.Button btnResetSpecList;
        private System.Windows.Forms.CheckBox chkOutputHtml;
        private System.Windows.Forms.CheckBox chkOutputCsv;
        private System.Windows.Forms.CheckBox chkPhaseParsing;
        private System.Windows.Forms.CheckBox chkMultiThreaded;
        private System.Windows.Forms.CheckBox chkCombatReplay;
        private System.Windows.Forms.CheckBox UploadDPSReports_checkbox;
        private System.Windows.Forms.CheckBox UploadDRRH_check;
        private System.Windows.Forms.CheckBox UploadRaidar_check;
        private System.Windows.Forms.GroupBox groupWebhookSettings;
        private System.Windows.Forms.CheckBox UploadWebhook_check;
        private System.Windows.Forms.TextBox UploadtxtWebhookUrl;
        private System.Windows.Forms.CheckBox chkOutputJson;
        private System.Windows.Forms.CheckBox chkIndentJSON;
        private System.Windows.Forms.CheckBox chkCompressRaw;
        private System.Windows.Forms.ToolTip settingTooltip;
        private System.Windows.Forms.CheckBox chkHtmlExternalScripts;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabHTML;
        private System.Windows.Forms.TabPage tabCSV;
        private System.Windows.Forms.TabPage tabRaw;
        private System.Windows.Forms.TabPage tabUpload;
        private System.Windows.Forms.TabPage tabAPI;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.GroupBox groupOutput;
        private System.Windows.Forms.GroupBox groupEncounter;
        private System.Windows.Forms.Panel panelTheme;
        private System.Windows.Forms.RadioButton radioThemeLight;
        private System.Windows.Forms.RadioButton radioThemeDark;
        private System.Windows.Forms.Panel panelHtml;
        private System.Windows.Forms.Label resetSkillLabel;
        private System.Windows.Forms.Label resetTraitLabel;
        private System.Windows.Forms.Label resetSpecLabel;
        private System.Windows.Forms.PictureBox imgTheme;
        private System.Windows.Forms.CheckBox chkB_SkipFailedTries;
        private System.Windows.Forms.CheckBox chkAutoAdd;
        private System.Windows.Forms.CheckBox chkAutoParse;
        private System.Windows.Forms.CheckBox chkAddPoVProf;
        private System.Windows.Forms.CheckBox chkAddDuration;
        private System.Windows.Forms.Panel panelJson;
        private System.Windows.Forms.CheckBox chkOutputXml;
        private System.Windows.Forms.Panel panelXML;
        private System.Windows.Forms.GroupBox groupRawSettings;
        private System.Windows.Forms.CheckBox chkIndentXML;
        private System.Windows.Forms.GroupBox boxParsing;
        private System.Windows.Forms.Button dumpButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.CheckBox chkAnonymous;
        private System.Windows.Forms.CheckBox chkSaveOutTrace;
        private System.Windows.Forms.CheckBox chkDamageMods;
        private System.Windows.Forms.CheckBox chkMultiLogs;
        private System.Windows.Forms.CheckBox chkRawTimelineArrays;
    }
}
