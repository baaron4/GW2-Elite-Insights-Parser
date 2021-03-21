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
            this.BtnCancelAll = new System.Windows.Forms.Button();
            this.BtnSettings = new System.Windows.Forms.Button();
            this.BtnClearAll = new System.Windows.Forms.Button();
            this.DgvFiles = new System.Windows.Forms.DataGridView();
            this.LocationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParseButtonState = new System.Windows.Forms.DataGridViewButtonColumn();
            this.OperatorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TlpMainWindow = new System.Windows.Forms.ToolTip(this.components);
            this.BtnClearFailed = new System.Windows.Forms.Button();
            this.BtnDiscordBatch = new System.Windows.Forms.Button();
            this.LogFileWatcher = new System.IO.FileSystemWatcher();
            this.LblVersion = new System.Windows.Forms.Label();
            this.LblWatchingDir = new System.Windows.Forms.Label();
            this.ChkApplicationTraces = new System.Windows.Forms.CheckBox();
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
            this.BtnParse.Location = new System.Drawing.Point(444, 321);
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
            this.BtnPopulate.Location = new System.Drawing.Point(97, 345);
            this.BtnPopulate.Name = "BtnPopulate";
            this.BtnPopulate.Size = new System.Drawing.Size(154, 23);
            this.BtnPopulate.TabIndex = 10;
            this.BtnPopulate.Text = "Populate from directory";
            this.BtnPopulate.UseVisualStyleBackColor = true;
            this.BtnPopulate.Click += new System.EventHandler(this.BtnPopulateFromDirectory);
            // 
            // BtnCancelAll
            // 
            this.BtnCancelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancelAll.Location = new System.Drawing.Point(444, 347);
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
            this.BtnSettings.Location = new System.Drawing.Point(16, 345);
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
            this.BtnClearAll.Location = new System.Drawing.Point(604, 347);
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
            this.ParseButtonState});
            this.DgvFiles.DataSource = this.OperatorBindingSource;
            this.DgvFiles.GridColor = System.Drawing.SystemColors.Control;
            this.DgvFiles.Location = new System.Drawing.Point(16, 46);
            this.DgvFiles.MultiSelect = false;
            this.DgvFiles.Name = "DgvFiles";
            this.DgvFiles.ReadOnly = true;
            this.DgvFiles.Size = new System.Drawing.Size(663, 254);
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
            this.LocationDataGridViewTextBoxColumn.HeaderText = "Input File";
            this.LocationDataGridViewTextBoxColumn.Name = "LocationDataGridViewTextBoxColumn";
            this.LocationDataGridViewTextBoxColumn.ReadOnly = true;
            this.LocationDataGridViewTextBoxColumn.ToolTipText = "Double left click to open input location";
            // 
            // StatusDataGridViewTextBoxColumn
            // 
            this.StatusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.StatusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.StatusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.StatusDataGridViewTextBoxColumn.Name = "StatusDataGridViewTextBoxColumn";
            this.StatusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ParseButtonState
            // 
            this.ParseButtonState.DataPropertyName = "ButtonText";
            this.ParseButtonState.HeaderText = "Action";
            this.ParseButtonState.Name = "ParseButtonState";
            this.ParseButtonState.ReadOnly = true;
            this.ParseButtonState.ToolTipText = "Left click open files and output location\r\nRight click to copy dps.report link to" +
    " clipboard, if applicable\r\nMiddle click to only open output location";
            // 
            // OperatorBindingSource
            // 
            this.OperatorBindingSource.DataSource = typeof(GW2EIParser.FormOperationController);
            // 
            // BtnClearFailed
            // 
            this.BtnClearFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClearFailed.Location = new System.Drawing.Point(523, 347);
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
            this.BtnDiscordBatch.Location = new System.Drawing.Point(257, 345);
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
            this.LblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(16, 371);
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
            this.LblWatchingDir.Location = new System.Drawing.Point(16, 305);
            this.LblWatchingDir.Name = "LblWatchingDir";
            this.LblWatchingDir.Size = new System.Drawing.Size(412, 13);
            this.LblWatchingDir.TabIndex = 18;
            this.LblWatchingDir.Text = "Watching log dir";
            // 
            // ChkApplicationTraces
            // 
            this.ChkApplicationTraces.AutoSize = true;
            this.ChkApplicationTraces.Location = new System.Drawing.Point(19, 325);
            this.ChkApplicationTraces.Name = "ChkApplicationTraces";
            this.ChkApplicationTraces.Size = new System.Drawing.Size(114, 17);
            this.ChkApplicationTraces.TabIndex = 0;
            this.ChkApplicationTraces.Text = "Application Traces";
            this.ChkApplicationTraces.CheckedChanged += new System.EventHandler(this.ChkApplicationTracesCheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(696, 389);
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
            this.Controls.Add(this.LblHeader);
            this.Controls.Add(this.ChkApplicationTraces);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 300);
            this.Name = "MainForm";
            this.Text = "GW2 Elite Insights Parser";
            this.TransparencyKey = System.Drawing.Color.OrangeRed;
            ((System.ComponentModel.ISupportInitialize)(this.DgvFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OperatorBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LogFileWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler((sender, e) => Properties.Settings.Default.Save());
        }

        #endregion
        private System.Windows.Forms.OpenFileDialog FileDialog;
        private System.Windows.Forms.Label LblHeader;
        private System.Windows.Forms.Button BtnParse;
        private System.Windows.Forms.Button BtnPopulate;
        private System.Windows.Forms.Button BtnCancelAll;
        private System.Windows.Forms.Button BtnSettings;
        private System.Windows.Forms.Button BtnClearAll;
        private System.Windows.Forms.DataGridView DgvFiles;
        private System.Windows.Forms.BindingSource OperatorBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn ParseButtonState;
        private System.Windows.Forms.Label LblVersion;
        private System.IO.FileSystemWatcher LogFileWatcher;
        private System.Windows.Forms.Label LblWatchingDir;
        private System.Windows.Forms.Button BtnClearFailed;
        private System.Windows.Forms.ToolTip TlpMainWindow;
        private System.Windows.Forms.Button BtnDiscordBatch;
        private System.Windows.Forms.CheckBox ChkApplicationTraces;
    }
}

