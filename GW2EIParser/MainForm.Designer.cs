using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System;

namespace GW2EIParser
{
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FileDialog = new System.Windows.Forms.OpenFileDialog();
            this.LblHeader = new System.Windows.Forms.Label();
            this.BtnParse = new System.Windows.Forms.Button();
            this.BtnPopulate = new System.Windows.Forms.Button();
            this.NumericCustomPopulateLimit = new System.Windows.Forms.NumericUpDown();
            this.LblCustomPopulateLimit = new System.Windows.Forms.Label();
            this.BtnCancelAll = new System.Windows.Forms.Button();
            this.BtnSettings = new System.Windows.Forms.Button();
            this.BtnClearAll = new System.Windows.Forms.Button();
            this.DgvFiles = new System.Windows.Forms.DataGridView();
            this.LocationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParseButtonState = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ReParseButtonState = new DataGridViewDisableButtonColumn();
            this.OperatorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TlpMainWindow = new System.Windows.Forms.ToolTip(this.components);
            this.BtnClearFailed = new System.Windows.Forms.Button();
            this.BtnDiscordBatch = new System.Windows.Forms.Button();
            this.LogFileWatcher = new System.IO.FileSystemWatcher();
            this.LblVersion = new System.Windows.Forms.Label();
            this.LblWatchingDir = new System.Windows.Forms.Label();
            this.ChkApplicationTraces = new System.Windows.Forms.CheckBox();
            this.ChkAutoDiscordBatch = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCustomPopulateLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OperatorBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogFileWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // FileDialog
            // 
            this.FileDialog.FileName = "openFileDialog1";
            // 
            // LblHeader
            // 
            this.LblHeader.AutoSize = true;
            this.LblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblHeader.Location = new System.Drawing.Point(12, 22);
            this.LblHeader.Name = "LblHeader";
            this.LblHeader.Size = new System.Drawing.Size(279, 20);
            this.LblHeader.TabIndex = 4;
            this.LblHeader.Text = "Drag and Drop EVTC file(s) below";
            this.LblHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BtnParse
            // 
            this.BtnParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnParse.Location = new System.Drawing.Point(442, 305);
            this.BtnParse.Name = "BtnParse";
            this.BtnParse.Size = new System.Drawing.Size(235, 23);
            this.BtnParse.TabIndex = 10;
            this.BtnParse.Text = "Parse All";
            this.BtnParse.UseVisualStyleBackColor = true;
            this.BtnParse.Click += new System.EventHandler(this.BtnParseClick);
            // 
            // BtnPopulate
            // 
            this.BtnPopulate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnPopulate.Location = new System.Drawing.Point(261, 335);
            this.BtnPopulate.Name = "BtnPopulate";
            this.BtnPopulate.Size = new System.Drawing.Size(154, 23);
            this.BtnPopulate.TabIndex = 10;
            this.BtnPopulate.Text = "Populate from directory";
            this.BtnPopulate.UseVisualStyleBackColor = true;
            this.BtnPopulate.Click += new System.EventHandler(this.BtnPopulateFromDirectory);
            // 
            // NumericCustomPopulateLimit
            // 
            this.NumericCustomPopulateLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumericCustomPopulateLimit.Location = new System.Drawing.Point(325, 311);
            this.NumericCustomPopulateLimit.Maximum = new decimal(new int[] {
            86400000,
            0,
            0,
            0});
            this.NumericCustomPopulateLimit.Name = "NumericCustomPopulateLimit";
            this.NumericCustomPopulateLimit.Size = new System.Drawing.Size(90, 20);
            this.NumericCustomPopulateLimit.TabIndex = 15;
            this.NumericCustomPopulateLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumericCustomPopulateLimit.ValueChanged += new System.EventHandler(this.NumericCustomPopulateLimitValueChanged);
            // 
            // LblCustomPopulateLimit
            // 
            this.LblCustomPopulateLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblCustomPopulateLimit.AutoSize = true;
            this.LblCustomPopulateLimit.Location = new System.Drawing.Point(258, 315);
            this.LblCustomPopulateLimit.Name = "LblCustomPopulateLimit";
            this.LblCustomPopulateLimit.Size = new System.Drawing.Size(65, 13);
            this.LblCustomPopulateLimit.TabIndex = 12;
            this.LblCustomPopulateLimit.Text = "Time (hours)";
            this.TlpMainWindow.SetToolTip(this.LblCustomPopulateLimit, "Files which were created before given hours ago will be ignored. Set to 0 for inf" +
        "inite.");
            // 
            // BtnCancelAll
            // 
            this.BtnCancelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancelAll.Location = new System.Drawing.Point(442, 335);
            this.BtnCancelAll.Name = "BtnCancelAll";
            this.BtnCancelAll.Size = new System.Drawing.Size(75, 23);
            this.BtnCancelAll.TabIndex = 11;
            this.BtnCancelAll.Text = "Cancel All";
            this.BtnCancelAll.UseVisualStyleBackColor = true;
            this.BtnCancelAll.Click += new System.EventHandler(this.BtnCancelAllClick);
            // 
            // BtnSettings
            // 
            this.BtnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSettings.Location = new System.Drawing.Point(16, 335);
            this.BtnSettings.Name = "BtnSettings";
            this.BtnSettings.Size = new System.Drawing.Size(75, 23);
            this.BtnSettings.TabIndex = 15;
            this.BtnSettings.Text = "Settings";
            this.BtnSettings.UseVisualStyleBackColor = true;
            this.BtnSettings.Click += new System.EventHandler(this.BtnSettingsClick);
            // 
            // BtnClearAll
            // 
            this.BtnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClearAll.Location = new System.Drawing.Point(602, 335);
            this.BtnClearAll.Name = "BtnClearAll";
            this.BtnClearAll.Size = new System.Drawing.Size(75, 23);
            this.BtnClearAll.TabIndex = 16;
            this.BtnClearAll.Text = "Clear All";
            this.BtnClearAll.UseVisualStyleBackColor = true;
            this.BtnClearAll.Click += new System.EventHandler(this.BtnClearAllClick);
            // 
            // DgvFiles
            // 
            this.DgvFiles.AllowDrop = true;
            this.DgvFiles.AllowUserToAddRows = false;
            this.DgvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvFiles.AutoGenerateColumns = false;
            this.DgvFiles.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LocationDataGridViewTextBoxColumn,
            this.StatusDataGridViewTextBoxColumn,
            this.ParseButtonState,
            this.ReParseButtonState});
            this.DgvFiles.DataSource = this.OperatorBindingSource;
            this.DgvFiles.GridColor = System.Drawing.SystemColors.Control;
            this.DgvFiles.Location = new System.Drawing.Point(16, 46);
            this.DgvFiles.MultiSelect = false;
            this.DgvFiles.Name = "DgvFiles";
            this.DgvFiles.ReadOnly = true;
            this.DgvFiles.Size = new System.Drawing.Size(661, 244);
            this.DgvFiles.TabIndex = 17;
            this.DgvFiles.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvFilesCellContentClick);
            this.DgvFiles.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvFilesCellContentDoubleClick);
            this.DgvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.DgvFilesDragDrop);
            this.DgvFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.DgvFilesDragEnter);
            // 
            // LocationDataGridViewTextBoxColumn
            // 
            this.LocationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LocationDataGridViewTextBoxColumn.DataPropertyName = "InputFile";
            this.LocationDataGridViewTextBoxColumn.FillWeight = 60F;
            this.LocationDataGridViewTextBoxColumn.HeaderText = "Input File";
            this.LocationDataGridViewTextBoxColumn.Name = "LocationDataGridViewTextBoxColumn";
            this.LocationDataGridViewTextBoxColumn.ReadOnly = true;
            this.LocationDataGridViewTextBoxColumn.ToolTipText = "Double left click to open input location";
            // 
            // StatusDataGridViewTextBoxColumn
            // 
            this.StatusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.StatusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.StatusDataGridViewTextBoxColumn.FillWeight = 30F;
            this.StatusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.StatusDataGridViewTextBoxColumn.Name = "StatusDataGridViewTextBoxColumn";
            this.StatusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ParseButtonState
            // 
            this.ParseButtonState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ParseButtonState.DataPropertyName = "ButtonText";
            this.ParseButtonState.FillWeight = 10F;
            this.ParseButtonState.HeaderText = "Action";
            this.ParseButtonState.Name = "ParseButtonState";
            this.ParseButtonState.ReadOnly = true;
            this.ParseButtonState.ToolTipText = "Left click open files and output location\r\nRight click to copy dps.report link to" +
    " clipboard, if applicable\r\nMiddle click to only open output location";
            // 
            // ReParseButtonState
            // 
            this.ReParseButtonState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ReParseButtonState.DataPropertyName = "ReParseText";
            this.ReParseButtonState.FillWeight = 10F;
            this.ReParseButtonState.HeaderText = "Re-Parse";
            this.ReParseButtonState.Name = "ReParseButtonState";
            this.ReParseButtonState.ReadOnly = true;
            this.ReParseButtonState.ToolTipText = "Only useable if the file was already successfully parsed";
            // 
            // OperatorBindingSource
            // 
            this.OperatorBindingSource.DataSource = typeof(GW2EIParser.FormOperationController);
            // 
            // BtnClearFailed
            // 
            this.BtnClearFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClearFailed.Location = new System.Drawing.Point(521, 335);
            this.BtnClearFailed.Name = "BtnClearFailed";
            this.BtnClearFailed.Size = new System.Drawing.Size(75, 23);
            this.BtnClearFailed.TabIndex = 19;
            this.BtnClearFailed.Text = "Clear Failed";
            this.TlpMainWindow.SetToolTip(this.BtnClearFailed, "Removes from the list logs that could not be parsed");
            this.BtnClearFailed.UseVisualStyleBackColor = true;
            this.BtnClearFailed.Click += new System.EventHandler(this.BtnClearFailedClick);
            // 
            // BtnDiscordBatch
            // 
            this.BtnDiscordBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnDiscordBatch.Location = new System.Drawing.Point(101, 335);
            this.BtnDiscordBatch.Name = "BtnDiscordBatch";
            this.BtnDiscordBatch.Size = new System.Drawing.Size(154, 23);
            this.BtnDiscordBatch.TabIndex = 20;
            this.BtnDiscordBatch.Text = "Send all to Discord";
            this.TlpMainWindow.SetToolTip(this.BtnDiscordBatch, "Send currently parsed logs with dps.report links to discord webhook in a batch");
            this.BtnDiscordBatch.UseVisualStyleBackColor = true;
            this.BtnDiscordBatch.Click += new System.EventHandler(this.BtnDiscordBatchClick);
            // 
            // LogFileWatcher
            // 
            this.LogFileWatcher.EnableRaisingEvents = true;
            this.LogFileWatcher.IncludeSubdirectories = true;
            this.LogFileWatcher.SynchronizingObject = this;
            this.LogFileWatcher.Created += new System.IO.FileSystemEventHandler(this.LogFileWatcher_Created);
            this.LogFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.LogFileWatcher_Renamed);
            // 
            // LblVersion
            // 
            this.LblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(13, 361);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(29, 13);
            this.LblVersion.TabIndex = 17;
            this.LblVersion.Text = "V1.3";
            // 
            // LblWatchingDir
            // 
            this.LblWatchingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LblWatchingDir.AutoEllipsis = true;
            this.LblWatchingDir.Location = new System.Drawing.Point(16, 295);
            this.LblWatchingDir.Name = "LblWatchingDir";
            this.LblWatchingDir.Size = new System.Drawing.Size(410, 13);
            this.LblWatchingDir.TabIndex = 18;
            this.LblWatchingDir.Text = "Watching log dir";
            // 
            // ChkApplicationTraces
            // 
            this.ChkApplicationTraces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkApplicationTraces.AutoSize = true;
            this.ChkApplicationTraces.Location = new System.Drawing.Point(19, 315);
            this.ChkApplicationTraces.Name = "ChkApplicationTraces";
            this.ChkApplicationTraces.Size = new System.Drawing.Size(59, 17);
            this.ChkApplicationTraces.TabIndex = 0;
            this.ChkApplicationTraces.Text = "Traces";
            this.ChkApplicationTraces.CheckedChanged += new System.EventHandler(this.ChkApplicationTracesCheckedChanged);
            // 
            // ChkAutoDiscordBatch
            // 
            this.ChkAutoDiscordBatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkAutoDiscordBatch.AutoSize = true;
            this.ChkAutoDiscordBatch.Location = new System.Drawing.Point(101, 315);
            this.ChkAutoDiscordBatch.Name = "ChkAutoDiscordBatch";
            this.ChkAutoDiscordBatch.Size = new System.Drawing.Size(118, 17);
            this.ChkAutoDiscordBatch.TabIndex = 0;
            this.ChkAutoDiscordBatch.Text = "Auto Discord Batch";
            this.ChkAutoDiscordBatch.CheckedChanged += new System.EventHandler(this.ChkAutoDiscordBatchCheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(704, 380);
            this.Controls.Add(this.BtnDiscordBatch);
            this.Controls.Add(this.BtnClearFailed);
            this.Controls.Add(this.LblWatchingDir);
            this.Controls.Add(this.DgvFiles);
            this.Controls.Add(this.LblVersion);
            this.Controls.Add(this.BtnClearAll);
            this.Controls.Add(this.BtnSettings);
            this.Controls.Add(this.BtnCancelAll);
            this.Controls.Add(this.BtnParse);
            this.Controls.Add(this.BtnPopulate);
            this.Controls.Add(this.LblCustomPopulateLimit);
            this.Controls.Add(this.NumericCustomPopulateLimit);
            this.Controls.Add(this.LblHeader);
            this.Controls.Add(this.ChkApplicationTraces);
            this.Controls.Add(this.ChkAutoDiscordBatch);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(720, 300);
            this.Name = "MainForm";
            this.Text = "GW2 Elite Insights Parser";
            this.TransparencyKey = System.Drawing.Color.OrangeRed;
            ((System.ComponentModel.ISupportInitialize)(this.NumericCustomPopulateLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OperatorBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogFileWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.Label LblHeader;
        private System.Windows.Forms.Button BtnParse;
        private System.Windows.Forms.Button BtnPopulate;
        private System.Windows.Forms.NumericUpDown NumericCustomPopulateLimit;
        private System.Windows.Forms.Label LblCustomPopulateLimit;
        private System.Windows.Forms.Button BtnCancelAll;
        private System.Windows.Forms.Button BtnSettings;
        private System.Windows.Forms.Button BtnClearAll;
        private System.Windows.Forms.DataGridView DgvFiles;
        private System.Windows.Forms.BindingSource OperatorBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn ParseButtonState;
        private DataGridViewDisableButtonColumn ReParseButtonState;
        private System.Windows.Forms.Label LblVersion;
        private System.IO.FileSystemWatcher LogFileWatcher;
        private System.Windows.Forms.Label LblWatchingDir;
        private System.Windows.Forms.Button BtnClearFailed;
        private System.Windows.Forms.ToolTip TlpMainWindow;
        private System.Windows.Forms.Button BtnDiscordBatch;
        private System.Windows.Forms.CheckBox ChkApplicationTraces;
        private System.Windows.Forms.CheckBox ChkAutoDiscordBatch;

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
}

