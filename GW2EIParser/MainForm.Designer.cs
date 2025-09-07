using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System;

namespace GW2EIParser;

partial class MainForm
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
            _settingsForm.Dispose();
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        FileDialog = new OpenFileDialog();
        LblHeader = new Label();
        BtnParse = new Button();
        BtnPopulate = new Button();
        NumericCustomPopulateLimit = new NumericUpDown();
        LblCustomPopulateLimit = new Label();
        BtnCancelAll = new Button();
        BtnSettings = new Button();
        BtnClearAll = new Button();
        DgvFiles = new DataGridView();
        LocationDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
        StatusDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
        ParseButtonState = new DataGridViewButtonColumn();
        ReParseButtonState = new DataGridViewDisableButtonColumn();
        OperatorBindingSource = new BindingSource(components);
        TlpMainWindow = new ToolTip(components);
        BtnClearFailed = new Button();
        BtnDiscordBatch = new Button();
        LogFileWatcher = new FileSystemWatcher();
        LblVersion = new Label();
        LblWatchingDir = new Label();
        ChkApplicationTraces = new CheckBox();
        ChkAutoDiscordBatch = new CheckBox();
        BtnCheckUpdates = new Button();
        ((System.ComponentModel.ISupportInitialize)NumericCustomPopulateLimit).BeginInit();
        ((System.ComponentModel.ISupportInitialize)DgvFiles).BeginInit();
        ((System.ComponentModel.ISupportInitialize)OperatorBindingSource).BeginInit();
        ((System.ComponentModel.ISupportInitialize)LogFileWatcher).BeginInit();
        SuspendLayout();
        // 
        // FileDialog
        // 
        FileDialog.FileName = "openFileDialog1";
        // 
        // LblHeader
        // 
        LblHeader.AutoSize = true;
        LblHeader.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
        LblHeader.Location = new Point(14, 25);
        LblHeader.Margin = new Padding(4, 0, 4, 0);
        LblHeader.Name = "LblHeader";
        LblHeader.Size = new Size(279, 20);
        LblHeader.TabIndex = 4;
        LblHeader.Text = "Drag and Drop EVTC file(s) below";
        LblHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // BtnParse
        // 
        BtnParse.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnParse.Location = new Point(516, 359);
        BtnParse.Margin = new Padding(4, 3, 4, 3);
        BtnParse.Name = "BtnParse";
        BtnParse.Size = new Size(274, 27);
        BtnParse.TabIndex = 10;
        BtnParse.Text = "Parse All";
        BtnParse.UseVisualStyleBackColor = true;
        BtnParse.Click += BtnParseClick;
        // 
        // BtnPopulate
        // 
        BtnPopulate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        BtnPopulate.Location = new Point(220, 386);
        BtnPopulate.Margin = new Padding(4, 3, 4, 3);
        BtnPopulate.Name = "BtnPopulate";
        BtnPopulate.Size = new Size(155, 27);
        BtnPopulate.TabIndex = 10;
        BtnPopulate.Text = "Populate from directory";
        BtnPopulate.UseVisualStyleBackColor = true;
        BtnPopulate.Click += BtnPopulateFromDirectory;
        // 
        // NumericCustomPopulateLimit
        // 
        NumericCustomPopulateLimit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        NumericCustomPopulateLimit.Location = new Point(232, 360);
        NumericCustomPopulateLimit.Margin = new Padding(4, 3, 4, 3);
        NumericCustomPopulateLimit.Maximum = new decimal(new int[] { 86400000, 0, 0, 0 });
        NumericCustomPopulateLimit.Name = "NumericCustomPopulateLimit";
        NumericCustomPopulateLimit.Size = new Size(105, 23);
        NumericCustomPopulateLimit.TabIndex = 15;
        NumericCustomPopulateLimit.TextAlign = HorizontalAlignment.Right;
        NumericCustomPopulateLimit.ValueChanged += NumericCustomPopulateLimitValueChanged;
        // 
        // LblCustomPopulateLimit
        // 
        LblCustomPopulateLimit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        LblCustomPopulateLimit.AutoSize = true;
        LblCustomPopulateLimit.Location = new Point(149, 365);
        LblCustomPopulateLimit.Margin = new Padding(4, 0, 4, 0);
        LblCustomPopulateLimit.Name = "LblCustomPopulateLimit";
        LblCustomPopulateLimit.Size = new Size(75, 15);
        LblCustomPopulateLimit.TabIndex = 12;
        LblCustomPopulateLimit.Text = "Time (hours)";
        TlpMainWindow.SetToolTip(LblCustomPopulateLimit, "Files which were created before given hours ago will be ignored. Set to 0 for infinite.");
        // 
        // BtnCancelAll
        // 
        BtnCancelAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnCancelAll.Location = new Point(516, 387);
        BtnCancelAll.Margin = new Padding(4, 3, 4, 3);
        BtnCancelAll.Name = "BtnCancelAll";
        BtnCancelAll.Size = new Size(88, 27);
        BtnCancelAll.TabIndex = 11;
        BtnCancelAll.Text = "Cancel All";
        BtnCancelAll.UseVisualStyleBackColor = true;
        BtnCancelAll.Click += BtnCancelAllClick;
        // 
        // BtnSettings
        // 
        BtnSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        BtnSettings.Location = new Point(19, 387);
        BtnSettings.Margin = new Padding(4, 3, 4, 3);
        BtnSettings.Name = "BtnSettings";
        BtnSettings.Size = new Size(79, 27);
        BtnSettings.TabIndex = 15;
        BtnSettings.Text = "Settings";
        BtnSettings.UseVisualStyleBackColor = true;
        BtnSettings.Click += BtnSettingsClick;
        // 
        // BtnClearAll
        // 
        BtnClearAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnClearAll.Location = new Point(702, 387);
        BtnClearAll.Margin = new Padding(4, 3, 4, 3);
        BtnClearAll.Name = "BtnClearAll";
        BtnClearAll.Size = new Size(88, 27);
        BtnClearAll.TabIndex = 16;
        BtnClearAll.Text = "Clear All";
        BtnClearAll.UseVisualStyleBackColor = true;
        BtnClearAll.Click += BtnClearAllClick;
        // 
        // DgvFiles
        // 
        DgvFiles.AllowDrop = true;
        DgvFiles.AllowUserToAddRows = false;
        DgvFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        DgvFiles.AutoGenerateColumns = false;
        DgvFiles.BackgroundColor = SystemColors.Control;
        DgvFiles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        DgvFiles.Columns.AddRange(new DataGridViewColumn[] { LocationDataGridViewTextBoxColumn, StatusDataGridViewTextBoxColumn, ParseButtonState, ReParseButtonState });
        DgvFiles.DataSource = OperatorBindingSource;
        DgvFiles.GridColor = SystemColors.Control;
        DgvFiles.Location = new Point(19, 53);
        DgvFiles.Margin = new Padding(4, 3, 4, 3);
        DgvFiles.MultiSelect = false;
        DgvFiles.Name = "DgvFiles";
        DgvFiles.ReadOnly = true;
        DgvFiles.Size = new Size(771, 282);
        DgvFiles.TabIndex = 17;
        DgvFiles.CellMouseClick += DgvFilesCellContentClick;
        DgvFiles.CellMouseDoubleClick += DgvFilesCellContentDoubleClick;
        DgvFiles.DragDrop += DgvFilesDragDrop;
        DgvFiles.DragEnter += DgvFilesDragEnter;
        // 
        // LocationDataGridViewTextBoxColumn
        // 
        LocationDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        LocationDataGridViewTextBoxColumn.DataPropertyName = "InputFile";
        LocationDataGridViewTextBoxColumn.FillWeight = 60F;
        LocationDataGridViewTextBoxColumn.HeaderText = "Input File";
        LocationDataGridViewTextBoxColumn.Name = "LocationDataGridViewTextBoxColumn";
        LocationDataGridViewTextBoxColumn.ReadOnly = true;
        LocationDataGridViewTextBoxColumn.ToolTipText = "Double left click to open input location";
        // 
        // StatusDataGridViewTextBoxColumn
        // 
        StatusDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        StatusDataGridViewTextBoxColumn.DataPropertyName = "Status";
        StatusDataGridViewTextBoxColumn.FillWeight = 30F;
        StatusDataGridViewTextBoxColumn.HeaderText = "Status";
        StatusDataGridViewTextBoxColumn.Name = "StatusDataGridViewTextBoxColumn";
        StatusDataGridViewTextBoxColumn.ReadOnly = true;
        // 
        // ParseButtonState
        // 
        ParseButtonState.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        ParseButtonState.DataPropertyName = "ButtonText";
        ParseButtonState.FillWeight = 10F;
        ParseButtonState.HeaderText = "Action";
        ParseButtonState.Name = "ParseButtonState";
        ParseButtonState.ReadOnly = true;
        ParseButtonState.ToolTipText = "Left click open files and output location\r\nRight click to copy dps.report link to clipboard, if applicable\r\nMiddle click to only open output location";
        // 
        // ReParseButtonState
        // 
        ReParseButtonState.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        ReParseButtonState.DataPropertyName = "ReParseText";
        ReParseButtonState.FillWeight = 10F;
        ReParseButtonState.HeaderText = "Re-Parse";
        ReParseButtonState.Name = "ReParseButtonState";
        ReParseButtonState.ReadOnly = true;
        ReParseButtonState.ToolTipText = "Only useable if the file was already successfully parsed";
        // 
        // OperatorBindingSource
        // 
        OperatorBindingSource.DataSource = typeof(FormOperationController);
        // 
        // BtnClearFailed
        // 
        BtnClearFailed.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        BtnClearFailed.Location = new Point(608, 387);
        BtnClearFailed.Margin = new Padding(4, 3, 4, 3);
        BtnClearFailed.Name = "BtnClearFailed";
        BtnClearFailed.Size = new Size(88, 27);
        BtnClearFailed.TabIndex = 19;
        BtnClearFailed.Text = "Clear Failed";
        TlpMainWindow.SetToolTip(BtnClearFailed, "Removes from the list logs that could not be parsed");
        BtnClearFailed.UseVisualStyleBackColor = true;
        BtnClearFailed.Click += BtnClearFailedClick;
        // 
        // BtnDiscordBatch
        // 
        BtnDiscordBatch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        BtnDiscordBatch.Location = new Point(383, 386);
        BtnDiscordBatch.Margin = new Padding(4, 3, 4, 3);
        BtnDiscordBatch.Name = "BtnDiscordBatch";
        BtnDiscordBatch.Size = new Size(125, 27);
        BtnDiscordBatch.TabIndex = 20;
        BtnDiscordBatch.Text = "Send all to Discord";
        TlpMainWindow.SetToolTip(BtnDiscordBatch, "Send currently parsed logs with dps.report links to discord webhook in a batch");
        BtnDiscordBatch.UseVisualStyleBackColor = true;
        BtnDiscordBatch.Click += BtnDiscordBatchClick;
        // 
        // LogFileWatcher
        // 
        LogFileWatcher.EnableRaisingEvents = true;
        LogFileWatcher.IncludeSubdirectories = true;
        LogFileWatcher.SynchronizingObject = this;
        LogFileWatcher.Created += LogFileWatcher_Created;
        LogFileWatcher.Renamed += LogFileWatcher_Renamed;
        // 
        // LblVersion
        // 
        LblVersion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LblVersion.AutoSize = true;
        LblVersion.Location = new Point(15, 417);
        LblVersion.Margin = new Padding(4, 0, 4, 0);
        LblVersion.Name = "LblVersion";
        LblVersion.Size = new Size(29, 15);
        LblVersion.TabIndex = 17;
        LblVersion.Text = "V1.3";
        // 
        // LblWatchingDir
        // 
        LblWatchingDir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LblWatchingDir.AutoEllipsis = true;
        LblWatchingDir.Location = new Point(19, 340);
        LblWatchingDir.Margin = new Padding(4, 0, 4, 0);
        LblWatchingDir.Name = "LblWatchingDir";
        LblWatchingDir.Size = new Size(478, 15);
        LblWatchingDir.TabIndex = 18;
        LblWatchingDir.Text = "Watching log dir";
        // 
        // ChkApplicationTraces
        // 
        ChkApplicationTraces.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        ChkApplicationTraces.AutoSize = true;
        ChkApplicationTraces.Location = new Point(22, 364);
        ChkApplicationTraces.Margin = new Padding(4, 3, 4, 3);
        ChkApplicationTraces.Name = "ChkApplicationTraces";
        ChkApplicationTraces.Size = new Size(59, 19);
        ChkApplicationTraces.TabIndex = 0;
        ChkApplicationTraces.Text = "Traces";
        ChkApplicationTraces.CheckedChanged += ChkApplicationTracesCheckedChanged;
        // 
        // ChkAutoDiscordBatch
        // 
        ChkAutoDiscordBatch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        ChkAutoDiscordBatch.AutoSize = true;
        ChkAutoDiscordBatch.Location = new Point(383, 364);
        ChkAutoDiscordBatch.Margin = new Padding(4, 3, 4, 3);
        ChkAutoDiscordBatch.Name = "ChkAutoDiscordBatch";
        ChkAutoDiscordBatch.Size = new Size(128, 19);
        ChkAutoDiscordBatch.TabIndex = 0;
        ChkAutoDiscordBatch.Text = "Auto Discord Batch";
        ChkAutoDiscordBatch.CheckedChanged += ChkAutoDiscordBatchCheckedChanged;
        // 
        // BtnCheckUpdates
        // 
        BtnCheckUpdates.Location = new Point(105, 386);
        BtnCheckUpdates.Name = "BtnCheckUpdates";
        BtnCheckUpdates.Size = new Size(108, 27);
        BtnCheckUpdates.TabIndex = 21;
        BtnCheckUpdates.Text = "Check Updates";
        BtnCheckUpdates.UseVisualStyleBackColor = true;
        BtnCheckUpdates.Click += BtnCheckUpdates_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Menu;
        ClientSize = new Size(821, 438);
        Controls.Add(BtnCheckUpdates);
        Controls.Add(BtnDiscordBatch);
        Controls.Add(BtnClearFailed);
        Controls.Add(LblWatchingDir);
        Controls.Add(DgvFiles);
        Controls.Add(LblVersion);
        Controls.Add(BtnClearAll);
        Controls.Add(BtnSettings);
        Controls.Add(BtnCancelAll);
        Controls.Add(BtnParse);
        Controls.Add(BtnPopulate);
        Controls.Add(LblCustomPopulateLimit);
        Controls.Add(NumericCustomPopulateLimit);
        Controls.Add(LblHeader);
        Controls.Add(ChkApplicationTraces);
        Controls.Add(ChkAutoDiscordBatch);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4, 3, 4, 3);
        MinimumSize = new Size(837, 340);
        Name = "MainForm";
        Text = "GW2 Elite Insights Parser";
        TransparencyKey = Color.OrangeRed;
        ((System.ComponentModel.ISupportInitialize)NumericCustomPopulateLimit).EndInit();
        ((System.ComponentModel.ISupportInitialize)DgvFiles).EndInit();
        ((System.ComponentModel.ISupportInitialize)OperatorBindingSource).EndInit();
        ((System.ComponentModel.ISupportInitialize)LogFileWatcher).EndInit();
        ResumeLayout(false);
        PerformLayout();

    }

    #endregion
    private OpenFileDialog FileDialog;
    private Label LblHeader;
    private Button BtnParse;
    private Button BtnPopulate;
    private NumericUpDown NumericCustomPopulateLimit;
    private Label LblCustomPopulateLimit;
    private Button BtnCancelAll;
    private Button BtnSettings;
    private Button BtnClearAll;
    private DataGridView DgvFiles;
    private BindingSource OperatorBindingSource;
    private DataGridViewTextBoxColumn LocationDataGridViewTextBoxColumn;
    private DataGridViewTextBoxColumn StatusDataGridViewTextBoxColumn;
    private DataGridViewButtonColumn ParseButtonState;
    private DataGridViewDisableButtonColumn ReParseButtonState;
    private Label LblVersion;
    private FileSystemWatcher LogFileWatcher;
    private Label LblWatchingDir;
    private Button BtnClearFailed;
    private ToolTip TlpMainWindow;
    private Button BtnDiscordBatch;
    private CheckBox ChkApplicationTraces;
    private CheckBox ChkAutoDiscordBatch;
    private Button BtnCheckUpdates;

    #region CUSTOM_GRID_VIEW
    // https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/disable-buttons-in-a-button-column-in-the-datagrid?view=netframeworkdesktop-4.8&redirectedfrom=MSDN
    public class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
    {
        public DataGridViewDisableButtonColumn()
        {
            this.CellTemplate = new DataGridViewDisableButtonCell();
        }
    }

    public class DataGridViewDisableButtonCell : DataGridViewButtonCell
    {
        private bool enabledValue;
        public bool Enabled
        {
            get
            {
                return enabledValue;
            }
            set
            {
                enabledValue = value;
            }
        }

        // Override the Clone method so that the Enabled property is copied.
        public override object Clone()
        {
            DataGridViewDisableButtonCell cell =
                (DataGridViewDisableButtonCell)base.Clone();
            cell.Enabled = this.Enabled;
            return cell;
        }

        // By default, enable the button cell.
        public DataGridViewDisableButtonCell()
        {
            this.enabledValue = true;
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            // The button cell is disabled, so paint the border,
            // background, and disabled button for the cell.
            if (!this.enabledValue)
            {
                // Draw the cell background, if specified.
                if ((paintParts & DataGridViewPaintParts.Background) ==
                    DataGridViewPaintParts.Background)
                {
                    SolidBrush cellBackground =
                        new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // Draw the cell borders, if specified.
                if ((paintParts & DataGridViewPaintParts.Border) ==
                    DataGridViewPaintParts.Border)
                {
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
                        advancedBorderStyle);
                }

                // Calculate the area in which to draw the button.
                Rectangle buttonArea = cellBounds;
                Rectangle buttonAdjustment =
                    this.BorderWidths(advancedBorderStyle);
                buttonArea.X += buttonAdjustment.X;
                buttonArea.Y += buttonAdjustment.Y;
                buttonArea.Height -= buttonAdjustment.Height;
                buttonArea.Width -= buttonAdjustment.Width;

                // Draw the disabled button.
                ButtonRenderer.DrawButton(graphics, buttonArea,
                    PushButtonState.Disabled);

                // Draw the disabled button text.
                if (this.FormattedValue is String)
                {
                    TextRenderer.DrawText(graphics,
                        (string)this.FormattedValue,
                        this.DataGridView.Font,
                        buttonArea, SystemColors.GrayText);
                }
            }
            else
            {
                // The button cell is enabled, so let the base class
                // handle the painting.
                base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                    elementState, value, formattedValue, errorText,
                    cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
    #endregion
}

