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
            this.LblSettingsInfoMsg = new System.Windows.Forms.Label();
            this.ChkDefaultOutputLoc = new System.Windows.Forms.CheckBox();
            this.TxtCustomSaveLoc = new System.Windows.Forms.TextBox();
            this.NumericCustomTooShort = new System.Windows.Forms.NumericUpDown();
            this.LblCustomTooShort = new System.Windows.Forms.Label();
            this.BtnFolderSelect = new System.Windows.Forms.Button();
            this.LblCustomSaveLoc = new System.Windows.Forms.Label();
            this.BtnResetSkillList = new System.Windows.Forms.Button();
            this.BtnResetTraitList = new System.Windows.Forms.Button();
            this.BtnResetSpecList = new System.Windows.Forms.Button();
            this.ChkOutputHtml = new System.Windows.Forms.CheckBox();
            this.ChkOutputCsv = new System.Windows.Forms.CheckBox();
            this.ChkPhaseParsing = new System.Windows.Forms.CheckBox();
            this.ChkMultiThreaded = new System.Windows.Forms.CheckBox();
            this.ChkCombatReplay = new System.Windows.Forms.CheckBox();
            this.ChkUploadDPSReports = new System.Windows.Forms.CheckBox();
            this.TxtDPSReportUserToken = new System.Windows.Forms.TextBox();
            this.ChkUploadDRRH = new System.Windows.Forms.CheckBox();
            this.ChkUploadRaidar = new System.Windows.Forms.CheckBox();
            this.ChkUploadWebhook = new System.Windows.Forms.CheckBox();
            this.ChkUploadSimpleMessageWebhook = new System.Windows.Forms.CheckBox();
            this.TxtUploadWebhookUrl = new System.Windows.Forms.TextBox();
            this.ChkOutputJson = new System.Windows.Forms.CheckBox();
            this.ChkIndentJSON = new System.Windows.Forms.CheckBox();
            this.TlpSettings = new System.Windows.Forms.ToolTip(this.components);
            this.GroupWebhookSettings = new System.Windows.Forms.GroupBox();
            this.LblWebhookUrl = new System.Windows.Forms.Label();
            this.ChkMultiLogs = new System.Windows.Forms.CheckBox();
            this.ChkAnonymous = new System.Windows.Forms.CheckBox();
            this.ChkHtmlExternalScripts = new System.Windows.Forms.CheckBox();
            this.ChkSaveOutTrace = new System.Windows.Forms.CheckBox();
            this.ChkDamageMods = new System.Windows.Forms.CheckBox();
            this.ChkRawTimelineArrays = new System.Windows.Forms.CheckBox();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabGeneral = new System.Windows.Forms.TabPage();
            this.GroupParsing = new System.Windows.Forms.GroupBox();
            this.ChkAutoParse = new System.Windows.Forms.CheckBox();
            this.ChkAutoAdd = new System.Windows.Forms.CheckBox();
            this.ChkSkipFailedTries = new System.Windows.Forms.CheckBox();
            this.GroupOutput = new System.Windows.Forms.GroupBox();
            this.ChkAddDuration = new System.Windows.Forms.CheckBox();
            this.ChkAddPoVProf = new System.Windows.Forms.CheckBox();
            this.GroupEncounter = new System.Windows.Forms.GroupBox();
            this.TabHTML = new System.Windows.Forms.TabPage();
            this.PictureTheme = new System.Windows.Forms.PictureBox();
            this.PanelHtml = new System.Windows.Forms.Panel();
            this.PanelTheme = new System.Windows.Forms.Panel();
            this.RadioThemeLight = new System.Windows.Forms.RadioButton();
            this.RadioThemeDark = new System.Windows.Forms.RadioButton();
            this.TabCSV = new System.Windows.Forms.TabPage();
            this.TabRaw = new System.Windows.Forms.TabPage();
            this.PanelXML = new System.Windows.Forms.Panel();
            this.ChkIndentXML = new System.Windows.Forms.CheckBox();
            this.ChkOutputXml = new System.Windows.Forms.CheckBox();
            this.PanelJson = new System.Windows.Forms.Panel();
            this.GroupRawSettings = new System.Windows.Forms.GroupBox();
            this.ChkCompressRaw = new System.Windows.Forms.CheckBox();
            this.TabUpload = new System.Windows.Forms.TabPage();
            this.DPSReportUserTokenLabel = new System.Windows.Forms.Label();
            this.TabAPI = new System.Windows.Forms.TabPage();
            this.LblResetSkill = new System.Windows.Forms.Label();
            this.LblResetTrait = new System.Windows.Forms.Label();
            this.LblResetSpec = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnDumpSettings = new System.Windows.Forms.Button();
            this.BtnLoadSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCustomTooShort)).BeginInit();
            this.GroupWebhookSettings.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.TabGeneral.SuspendLayout();
            this.GroupParsing.SuspendLayout();
            this.GroupOutput.SuspendLayout();
            this.GroupEncounter.SuspendLayout();
            this.TabHTML.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureTheme)).BeginInit();
            this.PanelHtml.SuspendLayout();
            this.PanelTheme.SuspendLayout();
            this.TabCSV.SuspendLayout();
            this.TabRaw.SuspendLayout();
            this.PanelXML.SuspendLayout();
            this.PanelJson.SuspendLayout();
            this.GroupRawSettings.SuspendLayout();
            this.TabUpload.SuspendLayout();
            this.TabAPI.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblSettingsInfoMsg
            // 
            this.LblSettingsInfoMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblSettingsInfoMsg.AutoSize = true;
            this.LblSettingsInfoMsg.Location = new System.Drawing.Point(9, 393);
            this.LblSettingsInfoMsg.Name = "LblSettingsInfoMsg";
            this.LblSettingsInfoMsg.Size = new System.Drawing.Size(251, 13);
            this.LblSettingsInfoMsg.TabIndex = 12;
            this.LblSettingsInfoMsg.Text = "*Changes will not alter files that are currently parsing";
            // 
            // ChkDefaultOutputLoc
            // 
            this.ChkDefaultOutputLoc.AutoSize = true;
            this.ChkDefaultOutputLoc.Checked = true;
            this.ChkDefaultOutputLoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkDefaultOutputLoc.Location = new System.Drawing.Point(12, 24);
            this.ChkDefaultOutputLoc.Name = "ChkDefaultOutputLoc";
            this.ChkDefaultOutputLoc.Size = new System.Drawing.Size(192, 17);
            this.ChkDefaultOutputLoc.TabIndex = 14;
            this.ChkDefaultOutputLoc.Text = "Save Output in same folder as evtc";
            this.ChkDefaultOutputLoc.UseVisualStyleBackColor = true;
            this.ChkDefaultOutputLoc.CheckedChanged += new System.EventHandler(this.ChkDefaultOutputLocationCheckedChanged);
            // 
            // TxtCustomSaveLoc
            // 
            this.TxtCustomSaveLoc.Location = new System.Drawing.Point(12, 72);
            this.TxtCustomSaveLoc.Name = "TxtCustomSaveLoc";
            this.TxtCustomSaveLoc.Size = new System.Drawing.Size(370, 20);
            this.TxtCustomSaveLoc.TabIndex = 15;
            this.TxtCustomSaveLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtCustomSaveLoc.TextChanged += new System.EventHandler(this.TxtCustomSaveLocationTextChanged);
            // 
            // NumericCustomTooShort
            // 
            this.NumericCustomTooShort.Location = new System.Drawing.Point(156, 134);
            this.NumericCustomTooShort.Maximum = 86400000;
            this.NumericCustomTooShort.Minimum = 2200;
            this.NumericCustomTooShort.Name = "NumericCustomTooShort";
            this.NumericCustomTooShort.Size = new System.Drawing.Size(90, 20);
            this.NumericCustomTooShort.TabIndex = 15;
            this.NumericCustomTooShort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericCustomTooShort.ValueChanged += new System.EventHandler(this.NumericCustomTooShortValueChanged);
            // 
            // LblCustomTooShort
            // 
            this.LblCustomTooShort.AutoSize = true;
            this.LblCustomTooShort.Location = new System.Drawing.Point(6, 136);
            this.LblCustomTooShort.Name = "LblCustomTooShort";
            this.LblCustomTooShort.Size = new System.Drawing.Size(151, 13);
            this.LblCustomTooShort.TabIndex = 17;
            this.LblCustomTooShort.Text = "Skip fights shorter than (in ms):";
            // 
            // BtnFolderSelect
            // 
            this.BtnFolderSelect.Location = new System.Drawing.Point(384, 72);
            this.BtnFolderSelect.Name = "BtnFolderSelect";
            this.BtnFolderSelect.Size = new System.Drawing.Size(45, 20);
            this.BtnFolderSelect.TabIndex = 16;
            this.BtnFolderSelect.Text = "Select";
            this.BtnFolderSelect.UseVisualStyleBackColor = true;
            this.BtnFolderSelect.Click += new System.EventHandler(this.BtnFolderSelectClick);
            // 
            // LblCustomSaveLoc
            // 
            this.LblCustomSaveLoc.AutoSize = true;
            this.LblCustomSaveLoc.Location = new System.Drawing.Point(12, 48);
            this.LblCustomSaveLoc.Name = "LblCustomSaveLoc";
            this.LblCustomSaveLoc.Size = new System.Drawing.Size(98, 13);
            this.LblCustomSaveLoc.TabIndex = 17;
            this.LblCustomSaveLoc.Text = "Other output folder:";
            // 
            // BtnResetSkillList
            // 
            this.BtnResetSkillList.Location = new System.Drawing.Point(27, 132);
            this.BtnResetSkillList.Name = "BtnResetSkillList";
            this.BtnResetSkillList.Size = new System.Drawing.Size(144, 23);
            this.BtnResetSkillList.TabIndex = 24;
            this.BtnResetSkillList.Text = "Reset Skill List ";
            this.BtnResetSkillList.UseVisualStyleBackColor = true;
            this.BtnResetSkillList.Click += new System.EventHandler(this.BtnResetSkillListClick);
            // 
            // BtnResetTraitList
            // 
            this.BtnResetTraitList.Location = new System.Drawing.Point(27, 78);
            this.BtnResetTraitList.Name = "BtnResetTraitList";
            this.BtnResetTraitList.Size = new System.Drawing.Size(144, 23);
            this.BtnResetTraitList.TabIndex = 24;
            this.BtnResetTraitList.Text = "Reset Trait List ";
            this.BtnResetTraitList.UseVisualStyleBackColor = true;
            this.BtnResetTraitList.Click += new System.EventHandler(this.BtnResetTraitListClick);
            // 
            // BtnResetSpecList
            // 
            this.BtnResetSpecList.Location = new System.Drawing.Point(27, 27);
            this.BtnResetSpecList.Name = "BtnResetSpecList";
            this.BtnResetSpecList.Size = new System.Drawing.Size(144, 23);
            this.BtnResetSpecList.TabIndex = 25;
            this.BtnResetSpecList.Text = "Reset Spec List";
            this.BtnResetSpecList.UseVisualStyleBackColor = true;
            this.BtnResetSpecList.Click += new System.EventHandler(this.BtnResetSpecListClick);
            // 
            // ChkOutputHtml
            // 
            this.ChkOutputHtml.AutoSize = true;
            this.ChkOutputHtml.Checked = true;
            this.ChkOutputHtml.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkOutputHtml.Location = new System.Drawing.Point(12, 12);
            this.ChkOutputHtml.Name = "ChkOutputHtml";
            this.ChkOutputHtml.Size = new System.Drawing.Size(105, 17);
            this.ChkOutputHtml.TabIndex = 26;
            this.ChkOutputHtml.Text = "Output as HTML";
            this.ChkOutputHtml.UseVisualStyleBackColor = true;
            this.ChkOutputHtml.CheckedChanged += new System.EventHandler(this.ChkOuputHTMLCheckedChanged);
            // 
            // ChkOutputCsv
            // 
            this.ChkOutputCsv.AutoSize = true;
            this.ChkOutputCsv.Location = new System.Drawing.Point(12, 12);
            this.ChkOutputCsv.Name = "ChkOutputCsv";
            this.ChkOutputCsv.Size = new System.Drawing.Size(96, 17);
            this.ChkOutputCsv.TabIndex = 27;
            this.ChkOutputCsv.Text = "Output as CSV";
            this.ChkOutputCsv.UseVisualStyleBackColor = true;
            this.ChkOutputCsv.CheckedChanged += new System.EventHandler(this.ChkOutputCsvCheckedChanged);
            // 
            // ChkPhaseParsing
            // 
            this.ChkPhaseParsing.AutoSize = true;
            this.ChkPhaseParsing.Checked = true;
            this.ChkPhaseParsing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkPhaseParsing.Location = new System.Drawing.Point(6, 19);
            this.ChkPhaseParsing.Name = "ChkPhaseParsing";
            this.ChkPhaseParsing.Size = new System.Drawing.Size(91, 17);
            this.ChkPhaseParsing.TabIndex = 30;
            this.ChkPhaseParsing.Text = "Parse Phases";
            this.ChkPhaseParsing.UseVisualStyleBackColor = true;
            this.ChkPhaseParsing.CheckedChanged += new System.EventHandler(this.ChkPhaseParsingCheckedChanged);
            // 
            // ChkMultiThreaded
            // 
            this.ChkMultiThreaded.AutoSize = true;
            this.ChkMultiThreaded.Location = new System.Drawing.Point(6, 19);
            this.ChkMultiThreaded.Name = "ChkMultiThreaded";
            this.ChkMultiThreaded.Size = new System.Drawing.Size(202, 17);
            this.ChkMultiThreaded.TabIndex = 29;
            this.ChkMultiThreaded.Text = "Use multi threading on log processing";
            this.TlpSettings.SetToolTip(this.ChkMultiThreaded, "Keep default value if unsure.");
            this.ChkMultiThreaded.UseVisualStyleBackColor = true;
            this.ChkMultiThreaded.CheckedChanged += new System.EventHandler(this.ChkMultiThreadedCheckedChanged);
            // 
            // ChkCombatReplay
            // 
            this.ChkCombatReplay.AutoSize = true;
            this.ChkCombatReplay.Checked = true;
            this.ChkCombatReplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkCombatReplay.Location = new System.Drawing.Point(6, 42);
            this.ChkCombatReplay.Name = "ChkCombatReplay";
            this.ChkCombatReplay.Size = new System.Drawing.Size(143, 17);
            this.ChkCombatReplay.TabIndex = 40;
            this.ChkCombatReplay.Text = "Compute Combat Replay";
            this.ChkCombatReplay.UseVisualStyleBackColor = true;
            this.ChkCombatReplay.CheckedChanged += new System.EventHandler(this.ChkCombatReplayCheckedChanged);
            // 
            // ChkUploadDPSReports
            // 
            this.ChkUploadDPSReports.AutoSize = true;
            this.ChkUploadDPSReports.Location = new System.Drawing.Point(12, 12);
            this.ChkUploadDPSReports.Name = "ChkUploadDPSReports";
            this.ChkUploadDPSReports.Size = new System.Drawing.Size(196, 17);
            this.ChkUploadDPSReports.TabIndex = 43;
            this.ChkUploadDPSReports.Text = "Upload to DPSReports Elite Insights";
            this.ChkUploadDPSReports.UseVisualStyleBackColor = true;
            this.ChkUploadDPSReports.CheckedChanged += new System.EventHandler(this.ChkUploadDPSReportsCheckedChanged);
            // 
            // TxtDPSReportUserToken
            // 
            this.TxtDPSReportUserToken.Location = new System.Drawing.Point(91, 59);
            this.TxtDPSReportUserToken.Name = "TxtDPSReportUserToken";
            this.TxtDPSReportUserToken.Size = new System.Drawing.Size(225, 20);
            this.TxtDPSReportUserToken.TabIndex = 15;
            this.TxtDPSReportUserToken.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtDPSReportUserToken.TextChanged += new System.EventHandler(this.ChkDPSReportUserTokenTextChanged);
            // 
            // ChkUploadDRRH
            // 
            this.ChkUploadDRRH.AutoSize = true;
            this.ChkUploadDRRH.Location = new System.Drawing.Point(12, 36);
            this.ChkUploadDRRH.Name = "ChkUploadDRRH";
            this.ChkUploadDRRH.Size = new System.Drawing.Size(193, 17);
            this.ChkUploadDRRH.TabIndex = 44;
            this.ChkUploadDRRH.Text = "Upload to DPSReports RaidHeroes";
            this.ChkUploadDRRH.UseVisualStyleBackColor = true;
            this.ChkUploadDRRH.CheckedChanged += new System.EventHandler(this.ChkUploadDRRHCheckedChanged);
            // 
            // ChkUploadRaidar
            // 
            this.ChkUploadRaidar.AutoSize = true;
            this.ChkUploadRaidar.Location = new System.Drawing.Point(12, 85);
            this.ChkUploadRaidar.Name = "ChkUploadRaidar";
            this.ChkUploadRaidar.Size = new System.Drawing.Size(175, 17);
            this.ChkUploadRaidar.TabIndex = 45;
            this.ChkUploadRaidar.Text = "Upload to Raidar (Not Working)";
            this.ChkUploadRaidar.UseVisualStyleBackColor = true;
            this.ChkUploadRaidar.CheckedChanged += new System.EventHandler(this.ChkUploadRaidarCheckedChanged);
            // 
            // ChkUploadWebhook
            // 
            this.ChkUploadWebhook.AutoSize = true;
            this.ChkUploadWebhook.Location = new System.Drawing.Point(12, 20);
            this.ChkUploadWebhook.Name = "ChkUploadWebhook";
            this.ChkUploadWebhook.Size = new System.Drawing.Size(185, 17);
            this.ChkUploadWebhook.TabIndex = 45;
            this.ChkUploadWebhook.Text = "Send Embed to Discord webhook";
            this.ChkUploadWebhook.UseVisualStyleBackColor = true;
            this.ChkUploadWebhook.CheckedChanged += new System.EventHandler(this.ChkUploadWebhookCheckedChanged);
            // 
            // ChkUploadSimpleMessageWebhook
            // 
            this.ChkUploadSimpleMessageWebhook.AutoSize = true;
            this.ChkUploadSimpleMessageWebhook.Location = new System.Drawing.Point(12, 68);
            this.ChkUploadSimpleMessageWebhook.Name = "ChkUploadSimpleMessageWebhook";
            this.ChkUploadSimpleMessageWebhook.Size = new System.Drawing.Size(92, 17);
            this.ChkUploadSimpleMessageWebhook.TabIndex = 45;
            this.ChkUploadSimpleMessageWebhook.Text = "Send link only";
            this.ChkUploadSimpleMessageWebhook.UseVisualStyleBackColor = true;
            this.ChkUploadSimpleMessageWebhook.CheckedChanged += new System.EventHandler(this.ChkUploadSimpleMessageWebhookCheckedChanged);
            // 
            // TxtUploadWebhookUrl
            // 
            this.TxtUploadWebhookUrl.Location = new System.Drawing.Point(110, 43);
            this.TxtUploadWebhookUrl.Name = "TxtUploadWebhookUrl";
            this.TxtUploadWebhookUrl.Size = new System.Drawing.Size(288, 20);
            this.TxtUploadWebhookUrl.TabIndex = 15;
            this.TxtUploadWebhookUrl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtUploadWebhookUrl.TextChanged += new System.EventHandler(this.TxtWebhookURLChanged);
            // 
            // ChkOutputJson
            // 
            this.ChkOutputJson.AutoSize = true;
            this.ChkOutputJson.Location = new System.Drawing.Point(12, 12);
            this.ChkOutputJson.Name = "ChkOutputJson";
            this.ChkOutputJson.Size = new System.Drawing.Size(103, 17);
            this.ChkOutputJson.TabIndex = 43;
            this.ChkOutputJson.Text = "Output as JSON";
            this.ChkOutputJson.UseVisualStyleBackColor = true;
            this.ChkOutputJson.CheckedChanged += new System.EventHandler(this.ChkOutputJSONCheckedChanged);
            // 
            // ChkIndentJSON
            // 
            this.ChkIndentJSON.AutoSize = true;
            this.ChkIndentJSON.Location = new System.Drawing.Point(3, 3);
            this.ChkIndentJSON.Name = "ChkIndentJSON";
            this.ChkIndentJSON.Size = new System.Drawing.Size(87, 17);
            this.ChkIndentJSON.TabIndex = 44;
            this.ChkIndentJSON.Text = "Indent JSON";
            this.ChkIndentJSON.UseVisualStyleBackColor = true;
            this.ChkIndentJSON.CheckedChanged += new System.EventHandler(this.ChkIndentJSONCheckedChanged);
            // 
            // TlpSettings
            // 
            this.TlpSettings.ToolTipTitle = "Setting description";
            // 
            // GroupWebhookSettings
            // 
            this.GroupWebhookSettings.Controls.Add(this.LblWebhookUrl);
            this.GroupWebhookSettings.Controls.Add(this.TxtUploadWebhookUrl);
            this.GroupWebhookSettings.Controls.Add(this.ChkUploadWebhook);
            this.GroupWebhookSettings.Controls.Add(this.ChkUploadSimpleMessageWebhook);
            this.GroupWebhookSettings.Location = new System.Drawing.Point(12, 124);
            this.GroupWebhookSettings.Name = "GroupWebhookSettings";
            this.GroupWebhookSettings.Size = new System.Drawing.Size(404, 92);
            this.GroupWebhookSettings.TabIndex = 45;
            this.GroupWebhookSettings.TabStop = false;
            this.GroupWebhookSettings.Text = "Webhook Settings";
            this.TlpSettings.SetToolTip(this.GroupWebhookSettings, "Disabled when not uploading to dps.reports");
            // 
            // LblWebhookUrl
            // 
            this.LblWebhookUrl.AutoSize = true;
            this.LblWebhookUrl.Location = new System.Drawing.Point(31, 46);
            this.LblWebhookUrl.Name = "LblWebhookUrl";
            this.LblWebhookUrl.Size = new System.Drawing.Size(73, 13);
            this.LblWebhookUrl.TabIndex = 47;
            this.LblWebhookUrl.Text = "Webhook Url:";
            // 
            // ChkMultiLogs
            // 
            this.ChkMultiLogs.AutoSize = true;
            this.ChkMultiLogs.Location = new System.Drawing.Point(6, 42);
            this.ChkMultiLogs.Name = "ChkMultiLogs";
            this.ChkMultiLogs.Size = new System.Drawing.Size(194, 17);
            this.ChkMultiLogs.TabIndex = 20;
            this.ChkMultiLogs.Text = "Parse Multiple logs at the same time";
            this.TlpSettings.SetToolTip(this.ChkMultiLogs, "Keep default value if unsure.");
            this.ChkMultiLogs.UseVisualStyleBackColor = true;
            this.ChkMultiLogs.CheckedChanged += new System.EventHandler(this.ChkMultiLogsCheckedChanged);
            // 
            // ChkAnonymous
            // 
            this.ChkAnonymous.AutoSize = true;
            this.ChkAnonymous.Location = new System.Drawing.Point(210, 24);
            this.ChkAnonymous.Name = "ChkAnonymous";
            this.ChkAnonymous.Size = new System.Drawing.Size(118, 17);
            this.ChkAnonymous.TabIndex = 20;
            this.ChkAnonymous.Text = "Anonymous Players";
            this.TlpSettings.SetToolTip(this.ChkAnonymous, "Replaces Players\' account names and character names by generic names");
            this.ChkAnonymous.UseVisualStyleBackColor = true;
            this.ChkAnonymous.CheckedChanged += new System.EventHandler(this.ChkAnonymousCheckedChanged);
            // 
            // ChkHtmlExternalScripts
            // 
            this.ChkHtmlExternalScripts.AutoSize = true;
            this.ChkHtmlExternalScripts.Location = new System.Drawing.Point(12, 12);
            this.ChkHtmlExternalScripts.Name = "ChkHtmlExternalScripts";
            this.ChkHtmlExternalScripts.Size = new System.Drawing.Size(99, 17);
            this.ChkHtmlExternalScripts.TabIndex = 46;
            this.ChkHtmlExternalScripts.Text = "External Scripts";
            this.TlpSettings.SetToolTip(this.ChkHtmlExternalScripts, "Writes static css and js scripts in own files, which are shared between all logs." +
        " Log file size decreases, but the script files have to be kept along with the ht" +
        "ml.");
            this.ChkHtmlExternalScripts.UseVisualStyleBackColor = true;
            this.ChkHtmlExternalScripts.CheckedChanged += new System.EventHandler(this.ChkHtmlExternalScriptsCheckedChanged);
            // 
            // ChkSaveOutTrace
            // 
            this.ChkSaveOutTrace.AutoSize = true;
            this.ChkSaveOutTrace.Location = new System.Drawing.Point(341, 24);
            this.ChkSaveOutTrace.Name = "ChkSaveOutTrace";
            this.ChkSaveOutTrace.Size = new System.Drawing.Size(87, 17);
            this.ChkSaveOutTrace.TabIndex = 40;
            this.ChkSaveOutTrace.Text = "Save Traces";
            this.ChkSaveOutTrace.UseVisualStyleBackColor = true;
            this.ChkSaveOutTrace.CheckedChanged += new System.EventHandler(this.ChkSaveOutTraceCheckedChanged);
            // 
            // ChkDamageMods
            // 
            this.ChkDamageMods.AutoSize = true;
            this.ChkDamageMods.Checked = true;
            this.ChkDamageMods.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkDamageMods.Location = new System.Drawing.Point(6, 65);
            this.ChkDamageMods.Name = "ChkDamageMods";
            this.ChkDamageMods.Size = new System.Drawing.Size(156, 17);
            this.ChkDamageMods.TabIndex = 20;
            this.ChkDamageMods.Text = "Compute Damage Modifiers";
            this.ChkDamageMods.UseVisualStyleBackColor = true;
            this.ChkDamageMods.CheckedChanged += new System.EventHandler(this.ChkComputeDamageModsCheckedChanged);
            // 
            // ChkRawTimelineArrays
            // 
            this.ChkRawTimelineArrays.AutoSize = true;
            this.ChkRawTimelineArrays.Location = new System.Drawing.Point(6, 45);
            this.ChkRawTimelineArrays.Name = "ChkRawTimelineArrays";
            this.ChkRawTimelineArrays.Size = new System.Drawing.Size(119, 17);
            this.ChkRawTimelineArrays.TabIndex = 20;
            this.ChkRawTimelineArrays.Text = "Add Timeline Arrays";
            this.ChkRawTimelineArrays.UseVisualStyleBackColor = true;
            this.ChkRawTimelineArrays.CheckedChanged += new System.EventHandler(this.ChkRawTimelineArraysCheckedChanged);
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.TabGeneral);
            this.TabControl.Controls.Add(this.TabHTML);
            this.TabControl.Controls.Add(this.TabCSV);
            this.TabControl.Controls.Add(this.TabRaw);
            this.TabControl.Controls.Add(this.TabUpload);
            this.TabControl.Controls.Add(this.TabAPI);
            this.TabControl.HotTrack = true;
            this.TabControl.Location = new System.Drawing.Point(12, 12);
            this.TabControl.Multiline = true;
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(505, 378);
            this.TabControl.TabIndex = 47;
            // 
            // TabGeneral
            // 
            this.TabGeneral.Controls.Add(this.GroupParsing);
            this.TabGeneral.Controls.Add(this.GroupOutput);
            this.TabGeneral.Controls.Add(this.GroupEncounter);
            this.TabGeneral.Location = new System.Drawing.Point(4, 22);
            this.TabGeneral.Name = "TabGeneral";
            this.TabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.TabGeneral.Size = new System.Drawing.Size(497, 352);
            this.TabGeneral.TabIndex = 0;
            this.TabGeneral.Text = "General";
            this.TabGeneral.UseVisualStyleBackColor = true;
            // 
            // GroupParsing
            // 
            this.GroupParsing.Controls.Add(this.ChkMultiThreaded);
            this.GroupParsing.Controls.Add(this.ChkMultiLogs);
            this.GroupParsing.Controls.Add(this.ChkAutoParse);
            this.GroupParsing.Controls.Add(this.ChkAutoAdd);
            this.GroupParsing.Controls.Add(this.ChkSkipFailedTries);
            this.GroupParsing.Controls.Add(this.NumericCustomTooShort);
            this.GroupParsing.Controls.Add(this.LblCustomTooShort);
            this.GroupParsing.Location = new System.Drawing.Point(240, 8);
            this.GroupParsing.Name = "GroupParsing";
            this.GroupParsing.Size = new System.Drawing.Size(251, 160);
            this.GroupParsing.TabIndex = 41;
            this.GroupParsing.TabStop = false;
            this.GroupParsing.Text = "Parsing";
            // 
            // ChkAutoParse
            // 
            this.ChkAutoParse.AutoSize = true;
            this.ChkAutoParse.Location = new System.Drawing.Point(6, 111);
            this.ChkAutoParse.Name = "ChkAutoParse";
            this.ChkAutoParse.Size = new System.Drawing.Size(171, 17);
            this.ChkAutoParse.TabIndex = 39;
            this.ChkAutoParse.Text = "Automatically parse added files";
            this.ChkAutoParse.UseVisualStyleBackColor = true;
            this.ChkAutoParse.CheckedChanged += new System.EventHandler(this.ChkAutoParseCheckedChanged);
            // 
            // ChkAutoAdd
            // 
            this.ChkAutoAdd.AutoSize = true;
            this.ChkAutoAdd.Location = new System.Drawing.Point(6, 88);
            this.ChkAutoAdd.Name = "ChkAutoAdd";
            this.ChkAutoAdd.Size = new System.Drawing.Size(154, 17);
            this.ChkAutoAdd.TabIndex = 40;
            this.ChkAutoAdd.Text = "Automatically add new logs";
            this.ChkAutoAdd.UseVisualStyleBackColor = true;
            this.ChkAutoAdd.CheckedChanged += new System.EventHandler(this.ChkAutoAddCheckedChanged);
            // 
            // ChkSkipFailedTries
            // 
            this.ChkSkipFailedTries.AutoSize = true;
            this.ChkSkipFailedTries.Location = new System.Drawing.Point(6, 65);
            this.ChkSkipFailedTries.Name = "ChkSkipFailedTries";
            this.ChkSkipFailedTries.Size = new System.Drawing.Size(153, 17);
            this.ChkSkipFailedTries.TabIndex = 38;
            this.ChkSkipFailedTries.Text = "Skip log generation if failed";
            this.ChkSkipFailedTries.UseVisualStyleBackColor = true;
            this.ChkSkipFailedTries.CheckedChanged += new System.EventHandler(this.ChkSkipFailedTriesCheckedChanged);
            // 
            // GroupOutput
            // 
            this.GroupOutput.Controls.Add(this.ChkAnonymous);
            this.GroupOutput.Controls.Add(this.ChkAddDuration);
            this.GroupOutput.Controls.Add(this.ChkAddPoVProf);
            this.GroupOutput.Controls.Add(this.ChkSaveOutTrace);
            this.GroupOutput.Controls.Add(this.ChkDefaultOutputLoc);
            this.GroupOutput.Controls.Add(this.BtnFolderSelect);
            this.GroupOutput.Controls.Add(this.TxtCustomSaveLoc);
            this.GroupOutput.Controls.Add(this.LblCustomSaveLoc);
            this.GroupOutput.Location = new System.Drawing.Point(12, 174);
            this.GroupOutput.Name = "GroupOutput";
            this.GroupOutput.Size = new System.Drawing.Size(444, 129);
            this.GroupOutput.TabIndex = 37;
            this.GroupOutput.TabStop = false;
            this.GroupOutput.Text = "Output";
            // 
            // ChkAddDuration
            // 
            this.ChkAddDuration.AutoSize = true;
            this.ChkAddDuration.Location = new System.Drawing.Point(240, 98);
            this.ChkAddDuration.Name = "ChkAddDuration";
            this.ChkAddDuration.Size = new System.Drawing.Size(194, 17);
            this.ChkAddDuration.TabIndex = 19;
            this.ChkAddDuration.Text = "Add encounter duration to file name";
            this.ChkAddDuration.UseVisualStyleBackColor = true;
            this.ChkAddDuration.CheckedChanged += new System.EventHandler(this.ChkAddDurationCheckedChanged);
            // 
            // ChkAddPoVProf
            // 
            this.ChkAddPoVProf.AutoSize = true;
            this.ChkAddPoVProf.Location = new System.Drawing.Point(12, 98);
            this.ChkAddPoVProf.Name = "ChkAddPoVProf";
            this.ChkAddPoVProf.Size = new System.Drawing.Size(176, 17);
            this.ChkAddPoVProf.TabIndex = 18;
            this.ChkAddPoVProf.Text = "Add PoV profession to file name";
            this.ChkAddPoVProf.UseVisualStyleBackColor = true;
            this.ChkAddPoVProf.CheckedChanged += new System.EventHandler(this.ChkAddPoVProfCheckedChanged);
            // 
            // GroupEncounter
            // 
            this.GroupEncounter.Controls.Add(this.ChkPhaseParsing);
            this.GroupEncounter.Controls.Add(this.ChkCombatReplay);
            this.GroupEncounter.Controls.Add(this.ChkDamageMods);
            this.GroupEncounter.Location = new System.Drawing.Point(12, 8);
            this.GroupEncounter.Name = "GroupEncounter";
            this.GroupEncounter.Size = new System.Drawing.Size(216, 160);
            this.GroupEncounter.TabIndex = 36;
            this.GroupEncounter.TabStop = false;
            this.GroupEncounter.Text = "Encounter";
            // 
            // TabHTML
            // 
            this.TabHTML.Controls.Add(this.PictureTheme);
            this.TabHTML.Controls.Add(this.PanelHtml);
            this.TabHTML.Controls.Add(this.ChkOutputHtml);
            this.TabHTML.Location = new System.Drawing.Point(4, 22);
            this.TabHTML.Name = "TabHTML";
            this.TabHTML.Padding = new System.Windows.Forms.Padding(3);
            this.TabHTML.Size = new System.Drawing.Size(471, 309);
            this.TabHTML.TabIndex = 1;
            this.TabHTML.Text = "HTML";
            this.TabHTML.UseVisualStyleBackColor = true;
            // 
            // PictureTheme
            // 
            this.PictureTheme.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureTheme.Location = new System.Drawing.Point(346, 30);
            this.PictureTheme.Name = "PictureTheme";
            this.PictureTheme.Size = new System.Drawing.Size(110, 74);
            this.PictureTheme.TabIndex = 49;
            this.PictureTheme.TabStop = false;
            // 
            // PanelHtml
            // 
            this.PanelHtml.Controls.Add(this.PanelTheme);
            this.PanelHtml.Controls.Add(this.ChkHtmlExternalScripts);
            this.PanelHtml.Location = new System.Drawing.Point(0, 36);
            this.PanelHtml.Name = "PanelHtml";
            this.PanelHtml.Size = new System.Drawing.Size(468, 87);
            this.PanelHtml.TabIndex = 54;
            // 
            // PanelTheme
            // 
            this.PanelTheme.Controls.Add(this.RadioThemeLight);
            this.PanelTheme.Controls.Add(this.RadioThemeDark);
            this.PanelTheme.Location = new System.Drawing.Point(252, 12);
            this.PanelTheme.Name = "PanelTheme";
            this.PanelTheme.Size = new System.Drawing.Size(156, 60);
            this.PanelTheme.TabIndex = 53;
            // 
            // RadioThemeLight
            // 
            this.RadioThemeLight.AutoSize = true;
            this.RadioThemeLight.Location = new System.Drawing.Point(0, 0);
            this.RadioThemeLight.Name = "RadioThemeLight";
            this.RadioThemeLight.Size = new System.Drawing.Size(84, 17);
            this.RadioThemeLight.TabIndex = 47;
            this.RadioThemeLight.TabStop = true;
            this.RadioThemeLight.Text = "Light Theme";
            this.RadioThemeLight.UseVisualStyleBackColor = true;
            this.RadioThemeLight.CheckedChanged += new System.EventHandler(this.RadioThemeLightCheckedChanged);
            // 
            // RadioThemeDark
            // 
            this.RadioThemeDark.AutoSize = true;
            this.RadioThemeDark.Location = new System.Drawing.Point(0, 24);
            this.RadioThemeDark.Name = "RadioThemeDark";
            this.RadioThemeDark.Size = new System.Drawing.Size(84, 17);
            this.RadioThemeDark.TabIndex = 48;
            this.RadioThemeDark.TabStop = true;
            this.RadioThemeDark.Text = "Dark Theme";
            this.RadioThemeDark.UseVisualStyleBackColor = true;
            this.RadioThemeDark.CheckedChanged += new System.EventHandler(this.RadioThemeDarkCheckedChanged);
            // 
            // TabCSV
            // 
            this.TabCSV.Controls.Add(this.ChkOutputCsv);
            this.TabCSV.Location = new System.Drawing.Point(4, 22);
            this.TabCSV.Name = "TabCSV";
            this.TabCSV.Size = new System.Drawing.Size(471, 309);
            this.TabCSV.TabIndex = 2;
            this.TabCSV.Text = "CSV";
            this.TabCSV.UseVisualStyleBackColor = true;
            // 
            // TabRaw
            // 
            this.TabRaw.Controls.Add(this.PanelXML);
            this.TabRaw.Controls.Add(this.ChkOutputXml);
            this.TabRaw.Controls.Add(this.PanelJson);
            this.TabRaw.Controls.Add(this.ChkOutputJson);
            this.TabRaw.Controls.Add(this.GroupRawSettings);
            this.TabRaw.Location = new System.Drawing.Point(4, 22);
            this.TabRaw.Name = "TabRaw";
            this.TabRaw.Size = new System.Drawing.Size(471, 309);
            this.TabRaw.TabIndex = 3;
            this.TabRaw.Text = "Raw formats";
            this.TabRaw.UseVisualStyleBackColor = true;
            // 
            // PanelXML
            // 
            this.PanelXML.Controls.Add(this.ChkIndentXML);
            this.PanelXML.Location = new System.Drawing.Point(147, 35);
            this.PanelXML.Name = "PanelXML";
            this.PanelXML.Size = new System.Drawing.Size(127, 35);
            this.PanelXML.TabIndex = 47;
            // 
            // ChkIndentXML
            // 
            this.ChkIndentXML.AutoSize = true;
            this.ChkIndentXML.Location = new System.Drawing.Point(3, 3);
            this.ChkIndentXML.Name = "ChkIndentXML";
            this.ChkIndentXML.Size = new System.Drawing.Size(81, 17);
            this.ChkIndentXML.TabIndex = 44;
            this.ChkIndentXML.Text = "Indent XML";
            this.ChkIndentXML.UseVisualStyleBackColor = true;
            this.ChkIndentXML.CheckedChanged += new System.EventHandler(this.ChkIndentXMLCheckedChanged);
            // 
            // ChkOutputXml
            // 
            this.ChkOutputXml.AutoSize = true;
            this.ChkOutputXml.Location = new System.Drawing.Point(147, 12);
            this.ChkOutputXml.Name = "ChkOutputXml";
            this.ChkOutputXml.Size = new System.Drawing.Size(97, 17);
            this.ChkOutputXml.TabIndex = 46;
            this.ChkOutputXml.Text = "Output as XML";
            this.ChkOutputXml.UseVisualStyleBackColor = true;
            this.ChkOutputXml.CheckedChanged += new System.EventHandler(this.ChkOutputXMLCheckedChanged);
            // 
            // PanelJson
            // 
            this.PanelJson.Controls.Add(this.ChkIndentJSON);
            this.PanelJson.Location = new System.Drawing.Point(14, 35);
            this.PanelJson.Name = "PanelJson";
            this.PanelJson.Size = new System.Drawing.Size(127, 35);
            this.PanelJson.TabIndex = 45;
            // 
            // GroupRawSettings
            // 
            this.GroupRawSettings.Controls.Add(this.ChkRawTimelineArrays);
            this.GroupRawSettings.Controls.Add(this.ChkCompressRaw);
            this.GroupRawSettings.Location = new System.Drawing.Point(17, 87);
            this.GroupRawSettings.Name = "GroupRawSettings";
            this.GroupRawSettings.Size = new System.Drawing.Size(150, 68);
            this.GroupRawSettings.TabIndex = 45;
            this.GroupRawSettings.TabStop = false;
            this.GroupRawSettings.Text = "Raw Format Settings";
            // 
            // ChkCompressRaw
            // 
            this.ChkCompressRaw.AutoSize = true;
            this.ChkCompressRaw.Location = new System.Drawing.Point(6, 22);
            this.ChkCompressRaw.Name = "ChkCompressRaw";
            this.ChkCompressRaw.Size = new System.Drawing.Size(118, 17);
            this.ChkCompressRaw.TabIndex = 18;
            this.ChkCompressRaw.Text = "Compress Raw files";
            this.ChkCompressRaw.UseVisualStyleBackColor = true;
            this.ChkCompressRaw.CheckedChanged += new System.EventHandler(this.ChkCompressRawCheckedChanged);
            // 
            // TabUpload
            // 
            this.TabUpload.Controls.Add(this.DPSReportUserTokenLabel);
            this.TabUpload.Controls.Add(this.ChkUploadDPSReports);
            this.TabUpload.Controls.Add(this.TxtDPSReportUserToken);
            this.TabUpload.Controls.Add(this.ChkUploadDRRH);
            this.TabUpload.Controls.Add(this.ChkUploadRaidar);
            this.TabUpload.Controls.Add(this.GroupWebhookSettings);
            this.TabUpload.Location = new System.Drawing.Point(4, 22);
            this.TabUpload.Name = "TabUpload";
            this.TabUpload.Size = new System.Drawing.Size(471, 309);
            this.TabUpload.TabIndex = 4;
            this.TabUpload.Text = "Upload";
            this.TabUpload.UseVisualStyleBackColor = true;
            // 
            // DPSReportUserTokenLabel
            // 
            this.DPSReportUserTokenLabel.AutoSize = true;
            this.DPSReportUserTokenLabel.Location = new System.Drawing.Point(19, 62);
            this.DPSReportUserTokenLabel.Name = "DPSReportUserTokenLabel";
            this.DPSReportUserTokenLabel.Size = new System.Drawing.Size(66, 13);
            this.DPSReportUserTokenLabel.TabIndex = 46;
            this.DPSReportUserTokenLabel.Text = "User Token:";
            // 
            // TabAPI
            // 
            this.TabAPI.Controls.Add(this.LblResetSkill);
            this.TabAPI.Controls.Add(this.LblResetTrait);
            this.TabAPI.Controls.Add(this.LblResetSpec);
            this.TabAPI.Controls.Add(this.BtnResetSpecList);
            this.TabAPI.Controls.Add(this.BtnResetTraitList);
            this.TabAPI.Controls.Add(this.BtnResetSkillList);
            this.TabAPI.Location = new System.Drawing.Point(4, 22);
            this.TabAPI.Name = "TabAPI";
            this.TabAPI.Size = new System.Drawing.Size(471, 309);
            this.TabAPI.TabIndex = 5;
            this.TabAPI.Text = "Maintenance";
            this.TabAPI.UseVisualStyleBackColor = true;
            // 
            // LblResetSkill
            // 
            this.LblResetSkill.AutoSize = true;
            this.LblResetSkill.Location = new System.Drawing.Point(24, 116);
            this.LblResetSkill.Name = "LblResetSkill";
            this.LblResetSkill.Size = new System.Drawing.Size(294, 13);
            this.LblResetSkill.TabIndex = 27;
            this.LblResetSkill.Text = "Resets the local skill list and loads all skills from the GW2 API";
            // 
            // LblResetTrait
            // 
            this.LblResetTrait.AutoSize = true;
            this.LblResetTrait.Location = new System.Drawing.Point(24, 62);
            this.LblResetTrait.Name = "LblResetTrait";
            this.LblResetTrait.Size = new System.Drawing.Size(289, 13);
            this.LblResetTrait.TabIndex = 28;
            this.LblResetTrait.Text = "Resets the local trait list and loads all trait from the GW2 API";
            // 
            // LblResetSpec
            // 
            this.LblResetSpec.AutoSize = true;
            this.LblResetSpec.Location = new System.Drawing.Point(24, 11);
            this.LblResetSpec.Name = "LblResetSpec";
            this.LblResetSpec.Size = new System.Drawing.Size(306, 13);
            this.LblResetSpec.TabIndex = 26;
            this.LblResetSpec.Text = "Resets the local spec list and loads all specs from the GW2 API";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(438, 413);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(84, 24);
            this.BtnClose.TabIndex = 48;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnCloseClick);
            // 
            // BtnDumpSettings
            // 
            this.BtnDumpSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDumpSettings.Location = new System.Drawing.Point(348, 413);
            this.BtnDumpSettings.Name = "BtnDumpSettings";
            this.BtnDumpSettings.Size = new System.Drawing.Size(84, 24);
            this.BtnDumpSettings.TabIndex = 49;
            this.BtnDumpSettings.Text = "Save Settings";
            this.BtnDumpSettings.UseVisualStyleBackColor = true;
            this.BtnDumpSettings.Click += new System.EventHandler(this.BtnDumpSettingsClicked);
            // 
            // BtnLoadSettings
            // 
            this.BtnLoadSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnLoadSettings.Location = new System.Drawing.Point(258, 413);
            this.BtnLoadSettings.Name = "BtnLoadSettings";
            this.BtnLoadSettings.Size = new System.Drawing.Size(84, 24);
            this.BtnLoadSettings.TabIndex = 50;
            this.BtnLoadSettings.Text = "Load Settings";
            this.BtnLoadSettings.UseVisualStyleBackColor = true;
            this.BtnLoadSettings.Click += new System.EventHandler(this.BtnLoadSettingsClicked);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(529, 449);
            this.Controls.Add(this.BtnLoadSettings);
            this.Controls.Add(this.BtnDumpSettings);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.LblSettingsInfoMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
            this.VisibleChanged += new System.EventHandler(this.SettingsFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.NumericCustomTooShort)).EndInit();
            this.GroupWebhookSettings.ResumeLayout(false);
            this.GroupWebhookSettings.PerformLayout();
            this.TabControl.ResumeLayout(false);
            this.TabGeneral.ResumeLayout(false);
            this.GroupParsing.ResumeLayout(false);
            this.GroupParsing.PerformLayout();
            this.GroupOutput.ResumeLayout(false);
            this.GroupOutput.PerformLayout();
            this.GroupEncounter.ResumeLayout(false);
            this.GroupEncounter.PerformLayout();
            this.TabHTML.ResumeLayout(false);
            this.TabHTML.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureTheme)).EndInit();
            this.PanelHtml.ResumeLayout(false);
            this.PanelHtml.PerformLayout();
            this.PanelTheme.ResumeLayout(false);
            this.PanelTheme.PerformLayout();
            this.TabCSV.ResumeLayout(false);
            this.TabCSV.PerformLayout();
            this.TabRaw.ResumeLayout(false);
            this.TabRaw.PerformLayout();
            this.PanelXML.ResumeLayout(false);
            this.PanelXML.PerformLayout();
            this.PanelJson.ResumeLayout(false);
            this.PanelJson.PerformLayout();
            this.GroupRawSettings.ResumeLayout(false);
            this.GroupRawSettings.PerformLayout();
            this.TabUpload.ResumeLayout(false);
            this.TabUpload.PerformLayout();
            this.TabAPI.ResumeLayout(false);
            this.TabAPI.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LblSettingsInfoMsg;
        private System.Windows.Forms.CheckBox ChkDefaultOutputLoc;
        private System.Windows.Forms.TextBox TxtCustomSaveLoc;
        private System.Windows.Forms.Button BtnFolderSelect;
        private System.Windows.Forms.Label LblCustomSaveLoc;
        private System.Windows.Forms.Button BtnResetSkillList;
        private System.Windows.Forms.Button BtnResetTraitList;
        private System.Windows.Forms.Button BtnResetSpecList;
        private System.Windows.Forms.CheckBox ChkOutputHtml;
        private System.Windows.Forms.CheckBox ChkOutputCsv;
        private System.Windows.Forms.CheckBox ChkPhaseParsing;
        private System.Windows.Forms.CheckBox ChkMultiThreaded;
        private System.Windows.Forms.Label LblCustomTooShort;
        private System.Windows.Forms.NumericUpDown NumericCustomTooShort;
        private System.Windows.Forms.CheckBox ChkCombatReplay;
        private System.Windows.Forms.CheckBox ChkUploadDPSReports;
        private System.Windows.Forms.TextBox TxtDPSReportUserToken;
        private System.Windows.Forms.CheckBox ChkUploadDRRH;
        private System.Windows.Forms.CheckBox ChkUploadRaidar;
        private System.Windows.Forms.GroupBox GroupWebhookSettings;
        private System.Windows.Forms.CheckBox ChkUploadWebhook;
        private System.Windows.Forms.CheckBox ChkUploadSimpleMessageWebhook;
        private System.Windows.Forms.TextBox TxtUploadWebhookUrl;
        private System.Windows.Forms.CheckBox ChkOutputJson;
        private System.Windows.Forms.CheckBox ChkIndentJSON;
        private System.Windows.Forms.CheckBox ChkCompressRaw;
        private System.Windows.Forms.ToolTip TlpSettings;
        private System.Windows.Forms.CheckBox ChkHtmlExternalScripts;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage TabGeneral;
        private System.Windows.Forms.TabPage TabHTML;
        private System.Windows.Forms.TabPage TabCSV;
        private System.Windows.Forms.TabPage TabRaw;
        private System.Windows.Forms.TabPage TabUpload;
        private System.Windows.Forms.TabPage TabAPI;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.GroupBox GroupOutput;
        private System.Windows.Forms.GroupBox GroupEncounter;
        private System.Windows.Forms.Panel PanelTheme;
        private System.Windows.Forms.RadioButton RadioThemeLight;
        private System.Windows.Forms.RadioButton RadioThemeDark;
        private System.Windows.Forms.Panel PanelHtml;
        private System.Windows.Forms.Label LblResetSkill;
        private System.Windows.Forms.Label LblResetTrait;
        private System.Windows.Forms.Label LblResetSpec;
        private System.Windows.Forms.PictureBox PictureTheme;
        private System.Windows.Forms.CheckBox ChkSkipFailedTries;
        private System.Windows.Forms.CheckBox ChkAutoAdd;
        private System.Windows.Forms.CheckBox ChkAutoParse;
        private System.Windows.Forms.CheckBox ChkAddPoVProf;
        private System.Windows.Forms.CheckBox ChkAddDuration;
        private System.Windows.Forms.Panel PanelJson;
        private System.Windows.Forms.CheckBox ChkOutputXml;
        private System.Windows.Forms.Panel PanelXML;
        private System.Windows.Forms.GroupBox GroupRawSettings;
        private System.Windows.Forms.CheckBox ChkIndentXML;
        private System.Windows.Forms.GroupBox GroupParsing;
        private System.Windows.Forms.Button BtnDumpSettings;
        private System.Windows.Forms.Button BtnLoadSettings;
        private System.Windows.Forms.CheckBox ChkAnonymous;
        private System.Windows.Forms.CheckBox ChkSaveOutTrace;
        private System.Windows.Forms.CheckBox ChkDamageMods;
        private System.Windows.Forms.CheckBox ChkMultiLogs;
        private System.Windows.Forms.CheckBox ChkRawTimelineArrays;
        private System.Windows.Forms.Label DPSReportUserTokenLabel;
        private System.Windows.Forms.Label LblWebhookUrl;
    }
}
