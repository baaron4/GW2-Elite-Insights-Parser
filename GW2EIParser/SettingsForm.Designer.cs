namespace GW2EIParser;

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
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
        LblSettingsInfoMsg = new Label();
        ChkDefaultOutputLoc = new CheckBox();
        TxtCustomSaveLocation = new TextBox();
        NumericCustomTooShort = new NumericUpDown();
        LblCustomTooShort = new Label();
        NumericMemoryLimit = new NumericUpDown();
        LblMemoryLimit = new Label();
        BtnCustomSaveLocSelect = new Button();
        LblCustomSaveLoc = new Label();
        BtnResetMapList = new Button();
        BtnResetSkillList = new Button();
        BtnResetTraitList = new Button();
        BtnResetSpecList = new Button();
        ChkOutputHtml = new CheckBox();
        ChkOutputCsv = new CheckBox();
        ChkPhaseParsing = new CheckBox();
        ChkSingleThreaded = new CheckBox();
        ChkCombatReplay = new CheckBox();
        ChkUploadDPSReports = new CheckBox();
        ChkUploadWingman = new CheckBox();
        TxtDPSReportUserToken = new TextBox();
        ChkUploadWebhook = new CheckBox();
        ChkUploadSimpleMessageWebhook = new CheckBox();
        TxtUploadWebhookUrl = new TextBox();
        ChkOutputJson = new CheckBox();
        ChkIndentJSON = new CheckBox();
        TlpSettings = new ToolTip(components);
        GroupWebhookSettings = new GroupBox();
        LblWebhookUrl = new Label();
        ChkMultiLogs = new CheckBox();
        ChkAnonymous = new CheckBox();
        ChkHtmlExternalScripts = new CheckBox();
        ChkHtmlCompressJson = new CheckBox();
        ChkDetailledWvW = new CheckBox();
        LblHtmlExternalScriptsCdn = new Label();
        LblHtmlExternalScriptsPath = new Label();
        TxtHtmlExternalScriptsCdn = new TextBox();
        TxtHtmlExternalScriptsPath = new TextBox();
        ChkSaveOutTrace = new CheckBox();
        ChkDamageMods = new CheckBox();
        ChkRawTimelineArrays = new CheckBox();
        TabControl = new TabControl();
        TabGeneral = new TabPage();
        GroupParsing = new GroupBox();
        ChkAutoParse = new CheckBox();
        ChkAutoAdd = new CheckBox();
        ChkSkipFailedTries = new CheckBox();
        GroupOutput = new GroupBox();
        ChkAddDuration = new CheckBox();
        ChkAddPoVProf = new CheckBox();
        GroupLog = new GroupBox();
        TabHTML = new TabPage();
        PictureTheme = new PictureBox();
        PanelHtml = new Panel();
        BtnHtmlExternalScriptPathSelect = new Button();
        PanelTheme = new Panel();
        RadioThemeLight = new RadioButton();
        RadioThemeDark = new RadioButton();
        TabCSV = new TabPage();
        TabRaw = new TabPage();
        PanelJson = new Panel();
        GroupRawSettings = new GroupBox();
        ChkCompressRaw = new CheckBox();
        TabUpload = new TabPage();
        DPSReportUserTokenLabel = new Label();
        TabAPI = new TabPage();
        LblResetSkill = new Label();
        LblResetTrait = new Label();
        LblResetSpec = new Label();
        BtnClose = new Button();
        BtnDumpSettings = new Button();
        BtnLoadSettings = new Button();
        LblResetMap = new Label();
        ((System.ComponentModel.ISupportInitialize)NumericCustomTooShort).BeginInit();
        ((System.ComponentModel.ISupportInitialize)NumericMemoryLimit).BeginInit();
        GroupWebhookSettings.SuspendLayout();
        TabControl.SuspendLayout();
        TabGeneral.SuspendLayout();
        GroupParsing.SuspendLayout();
        GroupOutput.SuspendLayout();
        GroupLog.SuspendLayout();
        TabHTML.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)PictureTheme).BeginInit();
        PanelHtml.SuspendLayout();
        PanelTheme.SuspendLayout();
        TabCSV.SuspendLayout();
        TabRaw.SuspendLayout();
        PanelJson.SuspendLayout();
        GroupRawSettings.SuspendLayout();
        TabUpload.SuspendLayout();
        TabAPI.SuspendLayout();
        SuspendLayout();
        // 
        // LblSettingsInfoMsg
        // 
        LblSettingsInfoMsg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        LblSettingsInfoMsg.AutoSize = true;
        LblSettingsInfoMsg.Location = new Point(10, 442);
        LblSettingsInfoMsg.Margin = new Padding(4, 0, 4, 0);
        LblSettingsInfoMsg.Name = "LblSettingsInfoMsg";
        LblSettingsInfoMsg.Size = new Size(285, 15);
        LblSettingsInfoMsg.TabIndex = 12;
        LblSettingsInfoMsg.Text = "*Changes will not alter files that are currently parsing";
        // 
        // ChkDefaultOutputLoc
        // 
        ChkDefaultOutputLoc.AutoSize = true;
        ChkDefaultOutputLoc.Checked = true;
        ChkDefaultOutputLoc.CheckState = CheckState.Checked;
        ChkDefaultOutputLoc.Location = new Point(14, 28);
        ChkDefaultOutputLoc.Margin = new Padding(4, 3, 4, 3);
        ChkDefaultOutputLoc.Name = "ChkDefaultOutputLoc";
        ChkDefaultOutputLoc.Size = new Size(208, 19);
        ChkDefaultOutputLoc.TabIndex = 14;
        ChkDefaultOutputLoc.Text = "Save Output in same folder as evtc";
        ChkDefaultOutputLoc.UseVisualStyleBackColor = true;
        ChkDefaultOutputLoc.CheckedChanged += ChkDefaultOutputLocationCheckedChanged;
        // 
        // TxtCustomSaveLocation
        // 
        TxtCustomSaveLocation.Location = new Point(14, 83);
        TxtCustomSaveLocation.Margin = new Padding(4, 3, 4, 3);
        TxtCustomSaveLocation.Name = "TxtCustomSaveLocation";
        TxtCustomSaveLocation.Size = new Size(431, 23);
        TxtCustomSaveLocation.TabIndex = 15;
        TxtCustomSaveLocation.TextAlign = HorizontalAlignment.Right;
        TxtCustomSaveLocation.TextChanged += TxtCustomSaveLocationTextChanged;
        // 
        // NumericCustomTooShort
        // 
        NumericCustomTooShort.Location = new Point(182, 155);
        NumericCustomTooShort.Margin = new Padding(4, 3, 4, 3);
        NumericCustomTooShort.Maximum = new decimal(new int[] { 86400000, 0, 0, 0 });
        NumericCustomTooShort.Minimum = new decimal(new int[] { 2200, 0, 0, 0 });
        NumericCustomTooShort.Name = "NumericCustomTooShort";
        NumericCustomTooShort.Size = new Size(105, 23);
        NumericCustomTooShort.TabIndex = 15;
        NumericCustomTooShort.TextAlign = HorizontalAlignment.Right;
        NumericCustomTooShort.Value = new decimal(new int[] { 2200, 0, 0, 0 });
        NumericCustomTooShort.ValueChanged += NumericCustomTooShortValueChanged;
        // 
        // LblCustomTooShort
        // 
        LblCustomTooShort.AutoSize = true;
        LblCustomTooShort.Location = new Point(7, 157);
        LblCustomTooShort.Margin = new Padding(4, 0, 4, 0);
        LblCustomTooShort.Name = "LblCustomTooShort";
        LblCustomTooShort.Size = new Size(164, 15);
        LblCustomTooShort.TabIndex = 17;
        LblCustomTooShort.Text = "Skip logs shorter than (in ms):";
        // 
        // NumericMemoryLimit
        // 
        NumericMemoryLimit.Location = new Point(252, 361);
        NumericMemoryLimit.Margin = new Padding(4, 3, 4, 3);
        NumericMemoryLimit.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        NumericMemoryLimit.Name = "NumericMemoryLimit";
        NumericMemoryLimit.Size = new Size(105, 23);
        NumericMemoryLimit.TabIndex = 15;
        NumericMemoryLimit.TextAlign = HorizontalAlignment.Right;
        NumericMemoryLimit.ValueChanged += NumericMemoryLimitValueChanged;
        // 
        // LblMemoryLimit
        // 
        LblMemoryLimit.AutoSize = true;
        LblMemoryLimit.Location = new Point(18, 363);
        LblMemoryLimit.Margin = new Padding(4, 0, 4, 0);
        LblMemoryLimit.Name = "LblMemoryLimit";
        LblMemoryLimit.Size = new Size(232, 15);
        LblMemoryLimit.TabIndex = 17;
        LblMemoryLimit.Text = "Kill Application if using more than (in MB):";
        TlpSettings.SetToolTip(LblMemoryLimit, "Keep default value if unsure (0).");
        // 
        // BtnCustomSaveLocSelect
        // 
        BtnCustomSaveLocSelect.Location = new Point(448, 83);
        BtnCustomSaveLocSelect.Margin = new Padding(4, 3, 4, 3);
        BtnCustomSaveLocSelect.Name = "BtnCustomSaveLocSelect";
        BtnCustomSaveLocSelect.Size = new Size(52, 23);
        BtnCustomSaveLocSelect.TabIndex = 16;
        BtnCustomSaveLocSelect.Text = "Select";
        BtnCustomSaveLocSelect.UseVisualStyleBackColor = true;
        BtnCustomSaveLocSelect.Click += BtnCustomSaveLocationSelectClick;
        // 
        // LblCustomSaveLoc
        // 
        LblCustomSaveLoc.AutoSize = true;
        LblCustomSaveLoc.Location = new Point(14, 55);
        LblCustomSaveLoc.Margin = new Padding(4, 0, 4, 0);
        LblCustomSaveLoc.Name = "LblCustomSaveLoc";
        LblCustomSaveLoc.Size = new Size(113, 15);
        LblCustomSaveLoc.TabIndex = 17;
        LblCustomSaveLoc.Text = "Other output folder:";
        // 
        // BtnResetMapList
        // 
        BtnResetMapList.Location = new Point(31, 210);
        BtnResetMapList.Margin = new Padding(4, 3, 4, 3);
        BtnResetMapList.Name = "BtnResetMapList";
        BtnResetMapList.Size = new Size(168, 27);
        BtnResetMapList.TabIndex = 24;
        BtnResetMapList.Text = "Reset Map List ";
        BtnResetMapList.UseVisualStyleBackColor = true;
        BtnResetMapList.Click += BtnResetMapListClick;
        // 
        // BtnResetSkillList
        // 
        BtnResetSkillList.Location = new Point(31, 152);
        BtnResetSkillList.Margin = new Padding(4, 3, 4, 3);
        BtnResetSkillList.Name = "BtnResetSkillList";
        BtnResetSkillList.Size = new Size(168, 27);
        BtnResetSkillList.TabIndex = 24;
        BtnResetSkillList.Text = "Reset Skill List ";
        BtnResetSkillList.UseVisualStyleBackColor = true;
        BtnResetSkillList.Click += BtnResetSkillListClick;
        // 
        // BtnResetTraitList
        // 
        BtnResetTraitList.Location = new Point(31, 90);
        BtnResetTraitList.Margin = new Padding(4, 3, 4, 3);
        BtnResetTraitList.Name = "BtnResetTraitList";
        BtnResetTraitList.Size = new Size(168, 27);
        BtnResetTraitList.TabIndex = 24;
        BtnResetTraitList.Text = "Reset Trait List ";
        BtnResetTraitList.UseVisualStyleBackColor = true;
        BtnResetTraitList.Click += BtnResetTraitListClick;
        // 
        // BtnResetSpecList
        // 
        BtnResetSpecList.Location = new Point(31, 31);
        BtnResetSpecList.Margin = new Padding(4, 3, 4, 3);
        BtnResetSpecList.Name = "BtnResetSpecList";
        BtnResetSpecList.Size = new Size(168, 27);
        BtnResetSpecList.TabIndex = 25;
        BtnResetSpecList.Text = "Reset Spec List";
        BtnResetSpecList.UseVisualStyleBackColor = true;
        BtnResetSpecList.Click += BtnResetSpecListClick;
        // 
        // ChkOutputHtml
        // 
        ChkOutputHtml.AutoSize = true;
        ChkOutputHtml.Checked = true;
        ChkOutputHtml.CheckState = CheckState.Checked;
        ChkOutputHtml.Location = new Point(14, 14);
        ChkOutputHtml.Margin = new Padding(4, 3, 4, 3);
        ChkOutputHtml.Name = "ChkOutputHtml";
        ChkOutputHtml.Size = new Size(114, 19);
        ChkOutputHtml.TabIndex = 26;
        ChkOutputHtml.Text = "Output as HTML";
        ChkOutputHtml.UseVisualStyleBackColor = true;
        ChkOutputHtml.CheckedChanged += ChkOuputHTMLCheckedChanged;
        // 
        // ChkOutputCsv
        // 
        ChkOutputCsv.AutoSize = true;
        ChkOutputCsv.Location = new Point(14, 14);
        ChkOutputCsv.Margin = new Padding(4, 3, 4, 3);
        ChkOutputCsv.Name = "ChkOutputCsv";
        ChkOutputCsv.Size = new Size(102, 19);
        ChkOutputCsv.TabIndex = 27;
        ChkOutputCsv.Text = "Output as CSV";
        ChkOutputCsv.UseVisualStyleBackColor = true;
        ChkOutputCsv.CheckedChanged += ChkOutputCsvCheckedChanged;
        // 
        // ChkPhaseParsing
        // 
        ChkPhaseParsing.AutoSize = true;
        ChkPhaseParsing.Checked = true;
        ChkPhaseParsing.CheckState = CheckState.Checked;
        ChkPhaseParsing.Location = new Point(7, 22);
        ChkPhaseParsing.Margin = new Padding(4, 3, 4, 3);
        ChkPhaseParsing.Name = "ChkPhaseParsing";
        ChkPhaseParsing.Size = new Size(93, 19);
        ChkPhaseParsing.TabIndex = 30;
        ChkPhaseParsing.Text = "Parse Phases";
        ChkPhaseParsing.UseVisualStyleBackColor = true;
        ChkPhaseParsing.CheckedChanged += ChkPhaseParsingCheckedChanged;
        // 
        // ChkSingleThreaded
        // 
        ChkSingleThreaded.AutoSize = true;
        ChkSingleThreaded.Location = new Point(7, 22);
        ChkSingleThreaded.Margin = new Padding(4, 3, 4, 3);
        ChkSingleThreaded.Name = "ChkSingleThreaded";
        ChkSingleThreaded.Size = new Size(237, 19);
        ChkSingleThreaded.TabIndex = 29;
        ChkSingleThreaded.Text = "Use a single thread when log processing";
        TlpSettings.SetToolTip(ChkSingleThreaded, "Keep default value if unsure (Unchecked).");
        ChkSingleThreaded.UseVisualStyleBackColor = true;
        ChkSingleThreaded.CheckedChanged += ChkSingleThreadedCheckedChanged;
        // 
        // ChkCombatReplay
        // 
        ChkCombatReplay.AutoSize = true;
        ChkCombatReplay.Checked = true;
        ChkCombatReplay.CheckState = CheckState.Checked;
        ChkCombatReplay.Location = new Point(7, 48);
        ChkCombatReplay.Margin = new Padding(4, 3, 4, 3);
        ChkCombatReplay.Name = "ChkCombatReplay";
        ChkCombatReplay.Size = new Size(160, 19);
        ChkCombatReplay.TabIndex = 40;
        ChkCombatReplay.Text = "Compute Combat Replay";
        ChkCombatReplay.UseVisualStyleBackColor = true;
        ChkCombatReplay.CheckedChanged += ChkCombatReplayCheckedChanged;
        // 
        // ChkUploadDPSReports
        // 
        ChkUploadDPSReports.AutoSize = true;
        ChkUploadDPSReports.Location = new Point(14, 17);
        ChkUploadDPSReports.Margin = new Padding(4, 3, 4, 3);
        ChkUploadDPSReports.Name = "ChkUploadDPSReports";
        ChkUploadDPSReports.Size = new Size(211, 19);
        ChkUploadDPSReports.TabIndex = 43;
        ChkUploadDPSReports.Text = "Upload to DPSReports Elite Insights";
        ChkUploadDPSReports.UseVisualStyleBackColor = true;
        ChkUploadDPSReports.CheckedChanged += ChkUploadDPSReportsCheckedChanged;
        // 
        // ChkUploadWingman
        // 
        ChkUploadWingman.AutoSize = true;
        ChkUploadWingman.Location = new Point(14, 74);
        ChkUploadWingman.Margin = new Padding(4, 3, 4, 3);
        ChkUploadWingman.Name = "ChkUploadWingman";
        ChkUploadWingman.Size = new Size(244, 19);
        ChkUploadWingman.TabIndex = 45;
        ChkUploadWingman.Text = "Upload to Wingman via uploadProcessed";
        ChkUploadWingman.UseVisualStyleBackColor = true;
        ChkUploadWingman.CheckedChanged += ChkUploadWingmanCheckedChanged;
        // 
        // TxtDPSReportUserToken
        // 
        TxtDPSReportUserToken.Location = new Point(106, 44);
        TxtDPSReportUserToken.Margin = new Padding(4, 3, 4, 3);
        TxtDPSReportUserToken.Name = "TxtDPSReportUserToken";
        TxtDPSReportUserToken.Size = new Size(262, 23);
        TxtDPSReportUserToken.TabIndex = 15;
        TxtDPSReportUserToken.TextAlign = HorizontalAlignment.Right;
        TxtDPSReportUserToken.TextChanged += ChkDPSReportUserTokenTextChanged;
        // 
        // ChkUploadWebhook
        // 
        ChkUploadWebhook.AutoSize = true;
        ChkUploadWebhook.Location = new Point(14, 23);
        ChkUploadWebhook.Margin = new Padding(4, 3, 4, 3);
        ChkUploadWebhook.Name = "ChkUploadWebhook";
        ChkUploadWebhook.Size = new Size(201, 19);
        ChkUploadWebhook.TabIndex = 45;
        ChkUploadWebhook.Text = "Send Embed to Discord webhook";
        ChkUploadWebhook.UseVisualStyleBackColor = true;
        ChkUploadWebhook.CheckedChanged += ChkUploadWebhookCheckedChanged;
        // 
        // ChkUploadSimpleMessageWebhook
        // 
        ChkUploadSimpleMessageWebhook.AutoSize = true;
        ChkUploadSimpleMessageWebhook.Location = new Point(14, 78);
        ChkUploadSimpleMessageWebhook.Margin = new Padding(4, 3, 4, 3);
        ChkUploadSimpleMessageWebhook.Name = "ChkUploadSimpleMessageWebhook";
        ChkUploadSimpleMessageWebhook.Size = new Size(100, 19);
        ChkUploadSimpleMessageWebhook.TabIndex = 45;
        ChkUploadSimpleMessageWebhook.Text = "Send link only";
        ChkUploadSimpleMessageWebhook.UseVisualStyleBackColor = true;
        ChkUploadSimpleMessageWebhook.CheckedChanged += ChkUploadSimpleMessageWebhookCheckedChanged;
        // 
        // TxtUploadWebhookUrl
        // 
        TxtUploadWebhookUrl.Location = new Point(128, 50);
        TxtUploadWebhookUrl.Margin = new Padding(4, 3, 4, 3);
        TxtUploadWebhookUrl.Name = "TxtUploadWebhookUrl";
        TxtUploadWebhookUrl.Size = new Size(335, 23);
        TxtUploadWebhookUrl.TabIndex = 15;
        TxtUploadWebhookUrl.TextAlign = HorizontalAlignment.Right;
        TxtUploadWebhookUrl.TextChanged += TxtWebhookURLChanged;
        // 
        // ChkOutputJson
        // 
        ChkOutputJson.AutoSize = true;
        ChkOutputJson.Location = new Point(14, 14);
        ChkOutputJson.Margin = new Padding(4, 3, 4, 3);
        ChkOutputJson.Name = "ChkOutputJson";
        ChkOutputJson.Size = new Size(109, 19);
        ChkOutputJson.TabIndex = 43;
        ChkOutputJson.Text = "Output as JSON";
        ChkOutputJson.UseVisualStyleBackColor = true;
        ChkOutputJson.CheckedChanged += ChkOutputJSONCheckedChanged;
        // 
        // ChkIndentJSON
        // 
        ChkIndentJSON.AutoSize = true;
        ChkIndentJSON.Location = new Point(4, 3);
        ChkIndentJSON.Margin = new Padding(4, 3, 4, 3);
        ChkIndentJSON.Name = "ChkIndentJSON";
        ChkIndentJSON.Size = new Size(91, 19);
        ChkIndentJSON.TabIndex = 44;
        ChkIndentJSON.Text = "Indent JSON";
        ChkIndentJSON.UseVisualStyleBackColor = true;
        ChkIndentJSON.CheckedChanged += ChkIndentJSONCheckedChanged;
        // 
        // TlpSettings
        // 
        TlpSettings.ToolTipTitle = "Setting description";
        // 
        // GroupWebhookSettings
        // 
        GroupWebhookSettings.Controls.Add(LblWebhookUrl);
        GroupWebhookSettings.Controls.Add(TxtUploadWebhookUrl);
        GroupWebhookSettings.Controls.Add(ChkUploadWebhook);
        GroupWebhookSettings.Controls.Add(ChkUploadSimpleMessageWebhook);
        GroupWebhookSettings.Location = new Point(14, 119);
        GroupWebhookSettings.Margin = new Padding(4, 3, 4, 3);
        GroupWebhookSettings.Name = "GroupWebhookSettings";
        GroupWebhookSettings.Padding = new Padding(4, 3, 4, 3);
        GroupWebhookSettings.Size = new Size(471, 106);
        GroupWebhookSettings.TabIndex = 45;
        GroupWebhookSettings.TabStop = false;
        GroupWebhookSettings.Text = "Webhook Settings";
        TlpSettings.SetToolTip(GroupWebhookSettings, "Disabled when not uploading to dps.reports");
        // 
        // LblWebhookUrl
        // 
        LblWebhookUrl.AutoSize = true;
        LblWebhookUrl.Location = new Point(36, 53);
        LblWebhookUrl.Margin = new Padding(4, 0, 4, 0);
        LblWebhookUrl.Name = "LblWebhookUrl";
        LblWebhookUrl.Size = new Size(79, 15);
        LblWebhookUrl.TabIndex = 47;
        LblWebhookUrl.Text = "Webhook Url:";
        // 
        // ChkMultiLogs
        // 
        ChkMultiLogs.AutoSize = true;
        ChkMultiLogs.Location = new Point(7, 48);
        ChkMultiLogs.Margin = new Padding(4, 3, 4, 3);
        ChkMultiLogs.Name = "ChkMultiLogs";
        ChkMultiLogs.Size = new Size(217, 19);
        ChkMultiLogs.TabIndex = 20;
        ChkMultiLogs.Text = "Parse Multiple logs at the same time";
        TlpSettings.SetToolTip(ChkMultiLogs, "Keep default value if unsure.");
        ChkMultiLogs.UseVisualStyleBackColor = true;
        ChkMultiLogs.CheckedChanged += ChkMultiLogsCheckedChanged;
        // 
        // ChkAnonymous
        // 
        ChkAnonymous.AutoSize = true;
        ChkAnonymous.Location = new Point(245, 28);
        ChkAnonymous.Margin = new Padding(4, 3, 4, 3);
        ChkAnonymous.Name = "ChkAnonymous";
        ChkAnonymous.Size = new Size(131, 19);
        ChkAnonymous.TabIndex = 20;
        ChkAnonymous.Text = "Anonymous Players";
        TlpSettings.SetToolTip(ChkAnonymous, "Replaces Players' account names and character names by generic names");
        ChkAnonymous.UseVisualStyleBackColor = true;
        ChkAnonymous.CheckedChanged += ChkAnonymousCheckedChanged;
        // 
        // ChkHtmlExternalScripts
        // 
        ChkHtmlExternalScripts.AutoSize = true;
        ChkHtmlExternalScripts.Location = new Point(14, 14);
        ChkHtmlExternalScripts.Margin = new Padding(4, 3, 4, 3);
        ChkHtmlExternalScripts.Name = "ChkHtmlExternalScripts";
        ChkHtmlExternalScripts.Size = new Size(105, 19);
        ChkHtmlExternalScripts.TabIndex = 46;
        ChkHtmlExternalScripts.Text = "External Scripts";
        TlpSettings.SetToolTip(ChkHtmlExternalScripts, "Writes static css and js scripts in own files, which are shared between all logs. Log file size decreases, but the script files have to be kept along with the html if you dont set a CDN Path");
        ChkHtmlExternalScripts.UseVisualStyleBackColor = true;
        ChkHtmlExternalScripts.CheckedChanged += ChkHtmlExternalScriptsCheckedChanged;
        // 
        // ChkHtmlCompressJson
        // 
        ChkHtmlCompressJson.AutoSize = true;
        ChkHtmlCompressJson.Location = new Point(136, 15);
        ChkHtmlCompressJson.Margin = new Padding(4, 3, 4, 3);
        ChkHtmlCompressJson.Name = "ChkHtmlCompressJson";
        ChkHtmlCompressJson.Size = new Size(105, 19);
        ChkHtmlCompressJson.TabIndex = 46;
        ChkHtmlCompressJson.Text = "Compress Json";
        TlpSettings.SetToolTip(ChkHtmlCompressJson, "Put the json inside the html file in a compressed state.");
        ChkHtmlCompressJson.UseVisualStyleBackColor = true;
        ChkHtmlCompressJson.CheckedChanged += ChkHtmlCompressCheckedChanged;
        // 
        // ChkDetailledWvW
        // 
        ChkDetailledWvW.AutoSize = true;
        ChkDetailledWvW.Location = new Point(7, 102);
        ChkDetailledWvW.Margin = new Padding(4, 3, 4, 3);
        ChkDetailledWvW.Name = "ChkDetailledWvW";
        ChkDetailledWvW.Size = new Size(142, 19);
        ChkDetailledWvW.TabIndex = 41;
        ChkDetailledWvW.Text = "Detailed WvW Parsing";
        TlpSettings.SetToolTip(ChkDetailledWvW, "Keep default value if unsure. Enabling this will make parsing significantly slower and the generated files bigger");
        ChkDetailledWvW.UseVisualStyleBackColor = true;
        ChkDetailledWvW.CheckedChanged += ChkDetailledWvWCheckedChange;
        // 
        // LblHtmlExternalScriptsCdn
        // 
        LblHtmlExternalScriptsCdn.AutoSize = true;
        LblHtmlExternalScriptsCdn.Location = new Point(10, 66);
        LblHtmlExternalScriptsCdn.Margin = new Padding(4, 0, 4, 0);
        LblHtmlExternalScriptsCdn.Name = "LblHtmlExternalScriptsCdn";
        LblHtmlExternalScriptsCdn.Size = new Size(32, 15);
        LblHtmlExternalScriptsCdn.TabIndex = 56;
        LblHtmlExternalScriptsCdn.Text = "Cdn:";
        TlpSettings.SetToolTip(LblHtmlExternalScriptsCdn, resources.GetString("LblHtmlExternalScriptsCdn.ToolTip"));
        // 
        // LblHtmlExternalScriptsPath
        // 
        LblHtmlExternalScriptsPath.AutoSize = true;
        LblHtmlExternalScriptsPath.Location = new Point(10, 39);
        LblHtmlExternalScriptsPath.Margin = new Padding(4, 0, 4, 0);
        LblHtmlExternalScriptsPath.Name = "LblHtmlExternalScriptsPath";
        LblHtmlExternalScriptsPath.Size = new Size(87, 15);
        LblHtmlExternalScriptsPath.TabIndex = 55;
        LblHtmlExternalScriptsPath.Text = "Absolute Path: ";
        TlpSettings.SetToolTip(LblHtmlExternalScriptsPath, "Fill in an absolute path of a directory here to place the external scripts at a different location then the report file.");
        // 
        // TxtHtmlExternalScriptsCdn
        // 
        TxtHtmlExternalScriptsCdn.Location = new Point(43, 62);
        TxtHtmlExternalScriptsCdn.Margin = new Padding(4, 3, 4, 3);
        TxtHtmlExternalScriptsCdn.Name = "TxtHtmlExternalScriptsCdn";
        TxtHtmlExternalScriptsCdn.Size = new Size(234, 23);
        TxtHtmlExternalScriptsCdn.TabIndex = 57;
        TxtHtmlExternalScriptsCdn.TextChanged += TxtHtmlExternalScriptCdnUrlTextChanged;
        // 
        // TxtHtmlExternalScriptsPath
        // 
        TxtHtmlExternalScriptsPath.Location = new Point(105, 36);
        TxtHtmlExternalScriptsPath.Margin = new Padding(4, 3, 4, 3);
        TxtHtmlExternalScriptsPath.Name = "TxtHtmlExternalScriptsPath";
        TxtHtmlExternalScriptsPath.Size = new Size(119, 23);
        TxtHtmlExternalScriptsPath.TabIndex = 54;
        TxtHtmlExternalScriptsPath.TextChanged += TxtHtmlExternalScriptsPathTextChanged;
        // 
        // ChkSaveOutTrace
        // 
        ChkSaveOutTrace.AutoSize = true;
        ChkSaveOutTrace.Location = new Point(385, 28);
        ChkSaveOutTrace.Margin = new Padding(4, 3, 4, 3);
        ChkSaveOutTrace.Name = "ChkSaveOutTrace";
        ChkSaveOutTrace.Size = new Size(109, 19);
        ChkSaveOutTrace.TabIndex = 40;
        ChkSaveOutTrace.Text = "Save Log Traces";
        ChkSaveOutTrace.UseVisualStyleBackColor = true;
        ChkSaveOutTrace.CheckedChanged += ChkSaveOutTraceCheckedChanged;
        // 
        // ChkDamageMods
        // 
        ChkDamageMods.AutoSize = true;
        ChkDamageMods.Checked = true;
        ChkDamageMods.CheckState = CheckState.Checked;
        ChkDamageMods.Location = new Point(7, 75);
        ChkDamageMods.Margin = new Padding(4, 3, 4, 3);
        ChkDamageMods.Name = "ChkDamageMods";
        ChkDamageMods.Size = new Size(176, 19);
        ChkDamageMods.TabIndex = 20;
        ChkDamageMods.Text = "Compute Damage Modifiers";
        ChkDamageMods.UseVisualStyleBackColor = true;
        ChkDamageMods.CheckedChanged += ChkComputeDamageModsCheckedChanged;
        // 
        // ChkRawTimelineArrays
        // 
        ChkRawTimelineArrays.AutoSize = true;
        ChkRawTimelineArrays.Location = new Point(7, 52);
        ChkRawTimelineArrays.Margin = new Padding(4, 3, 4, 3);
        ChkRawTimelineArrays.Name = "ChkRawTimelineArrays";
        ChkRawTimelineArrays.Size = new Size(133, 19);
        ChkRawTimelineArrays.TabIndex = 20;
        ChkRawTimelineArrays.Text = "Add Timeline Arrays";
        ChkRawTimelineArrays.UseVisualStyleBackColor = true;
        ChkRawTimelineArrays.CheckedChanged += ChkRawTimelineArraysCheckedChanged;
        // 
        // TabControl
        // 
        TabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        TabControl.Controls.Add(TabGeneral);
        TabControl.Controls.Add(TabHTML);
        TabControl.Controls.Add(TabCSV);
        TabControl.Controls.Add(TabRaw);
        TabControl.Controls.Add(TabUpload);
        TabControl.Controls.Add(TabAPI);
        TabControl.HotTrack = true;
        TabControl.Location = new Point(14, 14);
        TabControl.Margin = new Padding(4, 3, 4, 3);
        TabControl.Multiline = true;
        TabControl.Name = "TabControl";
        TabControl.SelectedIndex = 0;
        TabControl.Size = new Size(559, 425);
        TabControl.TabIndex = 47;
        // 
        // TabGeneral
        // 
        TabGeneral.Controls.Add(GroupParsing);
        TabGeneral.Controls.Add(GroupOutput);
        TabGeneral.Controls.Add(GroupLog);
        TabGeneral.Controls.Add(LblMemoryLimit);
        TabGeneral.Controls.Add(NumericMemoryLimit);
        TabGeneral.Location = new Point(4, 24);
        TabGeneral.Margin = new Padding(4, 3, 4, 3);
        TabGeneral.Name = "TabGeneral";
        TabGeneral.Padding = new Padding(4, 3, 4, 3);
        TabGeneral.Size = new Size(551, 397);
        TabGeneral.TabIndex = 0;
        TabGeneral.Text = "General";
        TabGeneral.UseVisualStyleBackColor = true;
        // 
        // GroupParsing
        // 
        GroupParsing.Controls.Add(ChkSingleThreaded);
        GroupParsing.Controls.Add(ChkMultiLogs);
        GroupParsing.Controls.Add(ChkAutoParse);
        GroupParsing.Controls.Add(ChkAutoAdd);
        GroupParsing.Controls.Add(ChkSkipFailedTries);
        GroupParsing.Controls.Add(NumericCustomTooShort);
        GroupParsing.Controls.Add(LblCustomTooShort);
        GroupParsing.Location = new Point(239, 9);
        GroupParsing.Margin = new Padding(4, 3, 4, 3);
        GroupParsing.Name = "GroupParsing";
        GroupParsing.Padding = new Padding(4, 3, 4, 3);
        GroupParsing.Size = new Size(293, 185);
        GroupParsing.TabIndex = 41;
        GroupParsing.TabStop = false;
        GroupParsing.Text = "Parsing";
        // 
        // ChkAutoParse
        // 
        ChkAutoParse.AutoSize = true;
        ChkAutoParse.Location = new Point(7, 128);
        ChkAutoParse.Margin = new Padding(4, 3, 4, 3);
        ChkAutoParse.Name = "ChkAutoParse";
        ChkAutoParse.Size = new Size(191, 19);
        ChkAutoParse.TabIndex = 39;
        ChkAutoParse.Text = "Automatically parse added files";
        ChkAutoParse.UseVisualStyleBackColor = true;
        ChkAutoParse.CheckedChanged += ChkAutoParseCheckedChanged;
        // 
        // ChkAutoAdd
        // 
        ChkAutoAdd.AutoSize = true;
        ChkAutoAdd.Location = new Point(7, 102);
        ChkAutoAdd.Margin = new Padding(4, 3, 4, 3);
        ChkAutoAdd.Name = "ChkAutoAdd";
        ChkAutoAdd.Size = new Size(173, 19);
        ChkAutoAdd.TabIndex = 40;
        ChkAutoAdd.Text = "Automatically add new logs";
        ChkAutoAdd.UseVisualStyleBackColor = true;
        ChkAutoAdd.CheckedChanged += ChkAutoAddCheckedChanged;
        // 
        // ChkSkipFailedTries
        // 
        ChkSkipFailedTries.AutoSize = true;
        ChkSkipFailedTries.Location = new Point(7, 75);
        ChkSkipFailedTries.Margin = new Padding(4, 3, 4, 3);
        ChkSkipFailedTries.Name = "ChkSkipFailedTries";
        ChkSkipFailedTries.Size = new Size(170, 19);
        ChkSkipFailedTries.TabIndex = 38;
        ChkSkipFailedTries.Text = "Skip log generation if failed";
        ChkSkipFailedTries.UseVisualStyleBackColor = true;
        ChkSkipFailedTries.CheckedChanged += ChkSkipFailedTriesCheckedChanged;
        // 
        // GroupOutput
        // 
        GroupOutput.Controls.Add(ChkAnonymous);
        GroupOutput.Controls.Add(ChkAddDuration);
        GroupOutput.Controls.Add(ChkAddPoVProf);
        GroupOutput.Controls.Add(ChkSaveOutTrace);
        GroupOutput.Controls.Add(ChkDefaultOutputLoc);
        GroupOutput.Controls.Add(BtnCustomSaveLocSelect);
        GroupOutput.Controls.Add(TxtCustomSaveLocation);
        GroupOutput.Controls.Add(LblCustomSaveLoc);
        GroupOutput.Location = new Point(14, 201);
        GroupOutput.Margin = new Padding(4, 3, 4, 3);
        GroupOutput.Name = "GroupOutput";
        GroupOutput.Padding = new Padding(4, 3, 4, 3);
        GroupOutput.Size = new Size(518, 149);
        GroupOutput.TabIndex = 37;
        GroupOutput.TabStop = false;
        GroupOutput.Text = "Output";
        // 
        // ChkAddDuration
        // 
        ChkAddDuration.AutoSize = true;
        ChkAddDuration.Location = new Point(280, 113);
        ChkAddDuration.Margin = new Padding(4, 3, 4, 3);
        ChkAddDuration.Name = "ChkAddDuration";
        ChkAddDuration.Size = new Size(182, 19);
        ChkAddDuration.TabIndex = 19;
        ChkAddDuration.Text = "Add log duration to file name";
        ChkAddDuration.UseVisualStyleBackColor = true;
        ChkAddDuration.CheckedChanged += ChkAddDurationCheckedChanged;
        // 
        // ChkAddPoVProf
        // 
        ChkAddPoVProf.AutoSize = true;
        ChkAddPoVProf.Location = new Point(14, 113);
        ChkAddPoVProf.Margin = new Padding(4, 3, 4, 3);
        ChkAddPoVProf.Name = "ChkAddPoVProf";
        ChkAddPoVProf.Size = new Size(196, 19);
        ChkAddPoVProf.TabIndex = 18;
        ChkAddPoVProf.Text = "Add PoV profession to file name";
        ChkAddPoVProf.UseVisualStyleBackColor = true;
        ChkAddPoVProf.CheckedChanged += ChkAddPoVProfCheckedChanged;
        // 
        // GroupLog
        // 
        GroupLog.Controls.Add(ChkDetailledWvW);
        GroupLog.Controls.Add(ChkPhaseParsing);
        GroupLog.Controls.Add(ChkCombatReplay);
        GroupLog.Controls.Add(ChkDamageMods);
        GroupLog.Location = new Point(14, 9);
        GroupLog.Margin = new Padding(4, 3, 4, 3);
        GroupLog.Name = "GroupLog";
        GroupLog.Padding = new Padding(4, 3, 4, 3);
        GroupLog.Size = new Size(218, 185);
        GroupLog.TabIndex = 36;
        GroupLog.TabStop = false;
        GroupLog.Text = "Log";
        // 
        // TabHTML
        // 
        TabHTML.Controls.Add(PictureTheme);
        TabHTML.Controls.Add(PanelHtml);
        TabHTML.Controls.Add(ChkOutputHtml);
        TabHTML.Location = new Point(4, 24);
        TabHTML.Margin = new Padding(4, 3, 4, 3);
        TabHTML.Name = "TabHTML";
        TabHTML.Padding = new Padding(4, 3, 4, 3);
        TabHTML.Size = new Size(551, 397);
        TabHTML.TabIndex = 1;
        TabHTML.Text = "HTML";
        TabHTML.UseVisualStyleBackColor = true;
        // 
        // PictureTheme
        // 
        PictureTheme.BorderStyle = BorderStyle.FixedSingle;
        PictureTheme.Location = new Point(404, 35);
        PictureTheme.Margin = new Padding(4, 3, 4, 3);
        PictureTheme.Name = "PictureTheme";
        PictureTheme.Size = new Size(128, 85);
        PictureTheme.TabIndex = 49;
        PictureTheme.TabStop = false;
        // 
        // PanelHtml
        // 
        PanelHtml.Controls.Add(BtnHtmlExternalScriptPathSelect);
        PanelHtml.Controls.Add(TxtHtmlExternalScriptsCdn);
        PanelHtml.Controls.Add(LblHtmlExternalScriptsCdn);
        PanelHtml.Controls.Add(LblHtmlExternalScriptsPath);
        PanelHtml.Controls.Add(TxtHtmlExternalScriptsPath);
        PanelHtml.Controls.Add(PanelTheme);
        PanelHtml.Controls.Add(ChkHtmlExternalScripts);
        PanelHtml.Controls.Add(ChkHtmlCompressJson);
        PanelHtml.Location = new Point(0, 42);
        PanelHtml.Margin = new Padding(4, 3, 4, 3);
        PanelHtml.Name = "PanelHtml";
        PanelHtml.Size = new Size(546, 100);
        PanelHtml.TabIndex = 54;
        // 
        // BtnHtmlExternalScriptPathSelect
        // 
        BtnHtmlExternalScriptPathSelect.Location = new Point(226, 35);
        BtnHtmlExternalScriptPathSelect.Margin = new Padding(4, 3, 4, 3);
        BtnHtmlExternalScriptPathSelect.Name = "BtnHtmlExternalScriptPathSelect";
        BtnHtmlExternalScriptPathSelect.Size = new Size(52, 25);
        BtnHtmlExternalScriptPathSelect.TabIndex = 58;
        BtnHtmlExternalScriptPathSelect.Text = "Select";
        BtnHtmlExternalScriptPathSelect.UseVisualStyleBackColor = true;
        BtnHtmlExternalScriptPathSelect.Click += BtnHtmlExternalScriptPathSelectClick;
        // 
        // PanelTheme
        // 
        PanelTheme.Controls.Add(RadioThemeLight);
        PanelTheme.Controls.Add(RadioThemeDark);
        PanelTheme.Location = new Point(294, 14);
        PanelTheme.Margin = new Padding(4, 3, 4, 3);
        PanelTheme.Name = "PanelTheme";
        PanelTheme.Size = new Size(182, 69);
        PanelTheme.TabIndex = 53;
        // 
        // RadioThemeLight
        // 
        RadioThemeLight.AutoSize = true;
        RadioThemeLight.Location = new Point(0, 0);
        RadioThemeLight.Margin = new Padding(4, 3, 4, 3);
        RadioThemeLight.Name = "RadioThemeLight";
        RadioThemeLight.Size = new Size(92, 19);
        RadioThemeLight.TabIndex = 47;
        RadioThemeLight.TabStop = true;
        RadioThemeLight.Text = "Light Theme";
        RadioThemeLight.UseVisualStyleBackColor = true;
        RadioThemeLight.CheckedChanged += RadioThemeLightCheckedChanged;
        // 
        // RadioThemeDark
        // 
        RadioThemeDark.AutoSize = true;
        RadioThemeDark.Location = new Point(0, 28);
        RadioThemeDark.Margin = new Padding(4, 3, 4, 3);
        RadioThemeDark.Name = "RadioThemeDark";
        RadioThemeDark.Size = new Size(89, 19);
        RadioThemeDark.TabIndex = 48;
        RadioThemeDark.TabStop = true;
        RadioThemeDark.Text = "Dark Theme";
        RadioThemeDark.UseVisualStyleBackColor = true;
        RadioThemeDark.CheckedChanged += RadioThemeDarkCheckedChanged;
        // 
        // TabCSV
        // 
        TabCSV.Controls.Add(ChkOutputCsv);
        TabCSV.Location = new Point(4, 24);
        TabCSV.Margin = new Padding(4, 3, 4, 3);
        TabCSV.Name = "TabCSV";
        TabCSV.Size = new Size(551, 397);
        TabCSV.TabIndex = 2;
        TabCSV.Text = "CSV";
        TabCSV.UseVisualStyleBackColor = true;
        // 
        // TabRaw
        // 
        TabRaw.Controls.Add(PanelJson);
        TabRaw.Controls.Add(ChkOutputJson);
        TabRaw.Controls.Add(GroupRawSettings);
        TabRaw.Location = new Point(4, 24);
        TabRaw.Margin = new Padding(4, 3, 4, 3);
        TabRaw.Name = "TabRaw";
        TabRaw.Size = new Size(551, 397);
        TabRaw.TabIndex = 3;
        TabRaw.Text = "Raw formats";
        TabRaw.UseVisualStyleBackColor = true;
        // 
        // PanelJson
        // 
        PanelJson.Controls.Add(ChkIndentJSON);
        PanelJson.Location = new Point(16, 40);
        PanelJson.Margin = new Padding(4, 3, 4, 3);
        PanelJson.Name = "PanelJson";
        PanelJson.Size = new Size(148, 40);
        PanelJson.TabIndex = 45;
        // 
        // GroupRawSettings
        // 
        GroupRawSettings.Controls.Add(ChkRawTimelineArrays);
        GroupRawSettings.Controls.Add(ChkCompressRaw);
        GroupRawSettings.Location = new Point(20, 100);
        GroupRawSettings.Margin = new Padding(4, 3, 4, 3);
        GroupRawSettings.Name = "GroupRawSettings";
        GroupRawSettings.Padding = new Padding(4, 3, 4, 3);
        GroupRawSettings.Size = new Size(175, 78);
        GroupRawSettings.TabIndex = 45;
        GroupRawSettings.TabStop = false;
        GroupRawSettings.Text = "Raw Format Settings";
        // 
        // ChkCompressRaw
        // 
        ChkCompressRaw.AutoSize = true;
        ChkCompressRaw.Location = new Point(7, 25);
        ChkCompressRaw.Margin = new Padding(4, 3, 4, 3);
        ChkCompressRaw.Name = "ChkCompressRaw";
        ChkCompressRaw.Size = new Size(128, 19);
        ChkCompressRaw.TabIndex = 18;
        ChkCompressRaw.Text = "Compress Raw files";
        ChkCompressRaw.UseVisualStyleBackColor = true;
        ChkCompressRaw.CheckedChanged += ChkCompressRawCheckedChanged;
        // 
        // TabUpload
        // 
        TabUpload.Controls.Add(DPSReportUserTokenLabel);
        TabUpload.Controls.Add(ChkUploadDPSReports);
        TabUpload.Controls.Add(TxtDPSReportUserToken);
        TabUpload.Controls.Add(ChkUploadWingman);
        TabUpload.Controls.Add(GroupWebhookSettings);
        TabUpload.Location = new Point(4, 24);
        TabUpload.Margin = new Padding(4, 3, 4, 3);
        TabUpload.Name = "TabUpload";
        TabUpload.Size = new Size(551, 397);
        TabUpload.TabIndex = 4;
        TabUpload.Text = "Upload";
        TabUpload.UseVisualStyleBackColor = true;
        // 
        // DPSReportUserTokenLabel
        // 
        DPSReportUserTokenLabel.AutoSize = true;
        DPSReportUserTokenLabel.Location = new Point(22, 47);
        DPSReportUserTokenLabel.Margin = new Padding(4, 0, 4, 0);
        DPSReportUserTokenLabel.Name = "DPSReportUserTokenLabel";
        DPSReportUserTokenLabel.Size = new Size(68, 15);
        DPSReportUserTokenLabel.TabIndex = 46;
        DPSReportUserTokenLabel.Text = "User Token:";
        // 
        // TabAPI
        // 
        TabAPI.Controls.Add(LblResetMap);
        TabAPI.Controls.Add(LblResetSkill);
        TabAPI.Controls.Add(LblResetTrait);
        TabAPI.Controls.Add(LblResetSpec);
        TabAPI.Controls.Add(BtnResetSpecList);
        TabAPI.Controls.Add(BtnResetTraitList);
        TabAPI.Controls.Add(BtnResetSkillList);
        TabAPI.Controls.Add(BtnResetMapList);
        TabAPI.Location = new Point(4, 24);
        TabAPI.Margin = new Padding(4, 3, 4, 3);
        TabAPI.Name = "TabAPI";
        TabAPI.Size = new Size(551, 397);
        TabAPI.TabIndex = 5;
        TabAPI.Text = "Maintenance";
        TabAPI.UseVisualStyleBackColor = true;
        // 
        // LblResetMap
        // 
        LblResetMap.AutoSize = true;
        LblResetMap.Location = new Point(31, 192);
        LblResetMap.Margin = new Padding(4, 0, 4, 0);
        LblResetMap.Name = "LblResetMap";
        LblResetMap.Size = new Size(324, 15);
        LblResetMap.TabIndex = 29;
        LblResetMap.Text = "Resets the local map list and loads all maps from the GW2 API";
        // 
        // LblResetSkill
        // 
        LblResetSkill.AutoSize = true;
        LblResetSkill.Location = new Point(28, 134);
        LblResetSkill.Margin = new Padding(4, 0, 4, 0);
        LblResetSkill.Name = "LblResetSkill";
        LblResetSkill.Size = new Size(324, 15);
        LblResetSkill.TabIndex = 27;
        LblResetSkill.Text = "Resets the local skill list and loads all skills from the GW2 API";
        // 
        // LblResetTrait
        // 
        LblResetTrait.AutoSize = true;
        LblResetTrait.Location = new Point(28, 72);
        LblResetTrait.Margin = new Padding(4, 0, 4, 0);
        LblResetTrait.Name = "LblResetTrait";
        LblResetTrait.Size = new Size(321, 15);
        LblResetTrait.TabIndex = 28;
        LblResetTrait.Text = "Resets the local trait list and loads all trait from the GW2 API";
        // 
        // LblResetSpec
        // 
        LblResetSpec.AutoSize = true;
        LblResetSpec.Location = new Point(28, 13);
        LblResetSpec.Margin = new Padding(4, 0, 4, 0);
        LblResetSpec.Name = "LblResetSpec";
        LblResetSpec.Size = new Size(332, 15);
        LblResetSpec.TabIndex = 26;
        LblResetSpec.Text = "Resets the local spec list and loads all specs from the GW2 API";
        // 
        // BtnClose
        // 
        BtnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnClose.Location = new Point(481, 465);
        BtnClose.Margin = new Padding(4, 3, 4, 3);
        BtnClose.Name = "BtnClose";
        BtnClose.Size = new Size(98, 28);
        BtnClose.TabIndex = 48;
        BtnClose.Text = "Close";
        BtnClose.UseVisualStyleBackColor = true;
        BtnClose.Click += BtnCloseClick;
        // 
        // BtnDumpSettings
        // 
        BtnDumpSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnDumpSettings.Location = new Point(376, 465);
        BtnDumpSettings.Margin = new Padding(4, 3, 4, 3);
        BtnDumpSettings.Name = "BtnDumpSettings";
        BtnDumpSettings.Size = new Size(98, 28);
        BtnDumpSettings.TabIndex = 49;
        BtnDumpSettings.Text = "Save Settings";
        BtnDumpSettings.UseVisualStyleBackColor = true;
        BtnDumpSettings.Click += BtnDumpSettingsClicked;
        // 
        // BtnLoadSettings
        // 
        BtnLoadSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnLoadSettings.Location = new Point(271, 465);
        BtnLoadSettings.Margin = new Padding(4, 3, 4, 3);
        BtnLoadSettings.Name = "BtnLoadSettings";
        BtnLoadSettings.Size = new Size(98, 28);
        BtnLoadSettings.TabIndex = 50;
        BtnLoadSettings.Text = "Load Settings";
        BtnLoadSettings.UseVisualStyleBackColor = true;
        BtnLoadSettings.Click += BtnLoadSettingsClicked;
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScroll = true;
        ClientSize = new Size(587, 507);
        Controls.Add(BtnLoadSettings);
        Controls.Add(BtnDumpSettings);
        Controls.Add(BtnClose);
        Controls.Add(TabControl);
        Controls.Add(LblSettingsInfoMsg);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        Name = "SettingsForm";
        Text = "Parse settings";
        FormClosing += SettingsFormFormClosing;
        VisibleChanged += SettingsFormLoad;
        ((System.ComponentModel.ISupportInitialize)NumericCustomTooShort).EndInit();
        ((System.ComponentModel.ISupportInitialize)NumericMemoryLimit).EndInit();
        GroupWebhookSettings.ResumeLayout(false);
        GroupWebhookSettings.PerformLayout();
        TabControl.ResumeLayout(false);
        TabGeneral.ResumeLayout(false);
        TabGeneral.PerformLayout();
        GroupParsing.ResumeLayout(false);
        GroupParsing.PerformLayout();
        GroupOutput.ResumeLayout(false);
        GroupOutput.PerformLayout();
        GroupLog.ResumeLayout(false);
        GroupLog.PerformLayout();
        TabHTML.ResumeLayout(false);
        TabHTML.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)PictureTheme).EndInit();
        PanelHtml.ResumeLayout(false);
        PanelHtml.PerformLayout();
        PanelTheme.ResumeLayout(false);
        PanelTheme.PerformLayout();
        TabCSV.ResumeLayout(false);
        TabCSV.PerformLayout();
        TabRaw.ResumeLayout(false);
        TabRaw.PerformLayout();
        PanelJson.ResumeLayout(false);
        PanelJson.PerformLayout();
        GroupRawSettings.ResumeLayout(false);
        GroupRawSettings.PerformLayout();
        TabUpload.ResumeLayout(false);
        TabUpload.PerformLayout();
        TabAPI.ResumeLayout(false);
        TabAPI.PerformLayout();
        ResumeLayout(false);
        PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label LblSettingsInfoMsg;
    private System.Windows.Forms.CheckBox ChkDefaultOutputLoc;
    private System.Windows.Forms.TextBox TxtCustomSaveLocation;
    private System.Windows.Forms.Button BtnCustomSaveLocSelect;
    private System.Windows.Forms.Label LblCustomSaveLoc;
    private System.Windows.Forms.Button BtnResetMapList;
    private System.Windows.Forms.Button BtnResetSkillList;
    private System.Windows.Forms.Button BtnResetTraitList;
    private System.Windows.Forms.Button BtnResetSpecList;
    private System.Windows.Forms.CheckBox ChkOutputHtml;
    private System.Windows.Forms.CheckBox ChkOutputCsv;
    private System.Windows.Forms.CheckBox ChkPhaseParsing;
    private System.Windows.Forms.CheckBox ChkSingleThreaded;
    private System.Windows.Forms.Label LblCustomTooShort;
    private System.Windows.Forms.NumericUpDown NumericCustomTooShort;
    private System.Windows.Forms.Label LblMemoryLimit;
    private System.Windows.Forms.NumericUpDown NumericMemoryLimit;
    private System.Windows.Forms.CheckBox ChkCombatReplay;
    private System.Windows.Forms.CheckBox ChkUploadDPSReports;
    private System.Windows.Forms.TextBox TxtDPSReportUserToken;
    private System.Windows.Forms.CheckBox ChkUploadWingman;
    private System.Windows.Forms.GroupBox GroupWebhookSettings;
    private System.Windows.Forms.CheckBox ChkUploadWebhook;
    private System.Windows.Forms.CheckBox ChkUploadSimpleMessageWebhook;
    private System.Windows.Forms.TextBox TxtUploadWebhookUrl;
    private System.Windows.Forms.CheckBox ChkOutputJson;
    private System.Windows.Forms.CheckBox ChkIndentJSON;
    private System.Windows.Forms.CheckBox ChkCompressRaw;
    private System.Windows.Forms.ToolTip TlpSettings;
    private System.Windows.Forms.CheckBox ChkHtmlExternalScripts;
    private System.Windows.Forms.CheckBox ChkHtmlCompressJson;
    private System.Windows.Forms.TabControl TabControl;
    private System.Windows.Forms.TabPage TabGeneral;
    private System.Windows.Forms.TabPage TabHTML;
    private System.Windows.Forms.TabPage TabCSV;
    private System.Windows.Forms.TabPage TabRaw;
    private System.Windows.Forms.TabPage TabUpload;
    private System.Windows.Forms.TabPage TabAPI;
    private System.Windows.Forms.Button BtnClose;
    private System.Windows.Forms.GroupBox GroupOutput;
    private System.Windows.Forms.GroupBox GroupLog;
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
    private System.Windows.Forms.GroupBox GroupRawSettings;
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
    private System.Windows.Forms.CheckBox ChkDetailledWvW;
    private System.Windows.Forms.Label LblHtmlExternalScriptsPath;
    private System.Windows.Forms.TextBox TxtHtmlExternalScriptsPath;
    private System.Windows.Forms.TextBox TxtHtmlExternalScriptsCdn;
    private System.Windows.Forms.Label LblHtmlExternalScriptsCdn;
    private System.Windows.Forms.Button BtnHtmlExternalScriptPathSelect;
    private Label LblResetMap;
}
