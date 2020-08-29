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
            this.MinimumSize = new System.Drawing.Size(600, 300);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.lblHeader = new System.Windows.Forms.Label();
            this.btnParse = new System.Windows.Forms.Button();
            this.btnPopulate = new System.Windows.Forms.Button();
            this.btnCancelAll = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.dgvFiles = new System.Windows.Forms.DataGridView();
            this.ParseButtonState = new System.Windows.Forms.DataGridViewButtonColumn();
            this.TlpMainWindow = new System.Windows.Forms.ToolTip(this.components);
            this.btnClearFailed = new System.Windows.Forms.Button();
            this.logFileWatcher = new System.IO.FileSystemWatcher();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.labWatchingDir = new System.Windows.Forms.Label();
            this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operatorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logFileWatcher)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.operatorBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(12, 22);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(279, 20);
            this.lblHeader.TabIndex = 4;
            this.lblHeader.Text = "Drag and Drop EVTC file(s) below";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnParse
            // 
            this.btnParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnParse.Location = new System.Drawing.Point(444, 307);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(235, 23);
            this.btnParse.TabIndex = 10;
            this.btnParse.Text = "Parse All";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.BtnParseClick);
            // 
            // btnPopulate
            // 
            this.btnPopulate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPopulate.Location = new System.Drawing.Point(97, 331);
            this.btnPopulate.Name = "btnPopulate";
            this.btnPopulate.Size = new System.Drawing.Size(154, 23);
            this.btnPopulate.TabIndex = 10;
            this.btnPopulate.Text = "Populate from directory";
            this.btnPopulate.UseVisualStyleBackColor = true;
            this.btnPopulate.Click += new System.EventHandler(this.BtnPopulateFromDirectory);
            // 
            // btnCancelAll
            // 
            this.btnCancelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelAll.Location = new System.Drawing.Point(444, 333);
            this.btnCancelAll.Name = "btnCancelAll";
            this.btnCancelAll.Size = new System.Drawing.Size(75, 23);
            this.btnCancelAll.TabIndex = 11;
            this.btnCancelAll.Text = "Cancel All";
            this.btnCancelAll.UseVisualStyleBackColor = true;
            this.btnCancelAll.Click += new System.EventHandler(this.BtnCancelAllClick);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSettings.Location = new System.Drawing.Point(16, 331);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSettings.TabIndex = 15;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettingsClick);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAll.Location = new System.Drawing.Point(604, 333);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 16;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.BtnClearAllClick);
            // 
            // dgvFiles
            // 
            this.dgvFiles.AllowDrop = true;
            this.dgvFiles.AllowUserToAddRows = false;
            this.dgvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFiles.AutoGenerateColumns = false;
            this.dgvFiles.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.locationDataGridViewTextBoxColumn,
                this.statusDataGridViewTextBoxColumn,
                this.ParseButtonState
                }
            );
            this.dgvFiles.DataSource = this.operatorBindingSource;
            this.dgvFiles.GridColor = System.Drawing.SystemColors.Control;
            this.dgvFiles.Location = new System.Drawing.Point(16, 46);
            this.dgvFiles.MultiSelect = false;
            this.dgvFiles.Name = "dgvFiles";
            this.dgvFiles.ReadOnly = true;
            this.dgvFiles.Size = new System.Drawing.Size(663, 255);
            this.dgvFiles.TabIndex = 17;
            this.dgvFiles.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvFilesCellContentClick);
            this.dgvFiles.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvFilesCellContentDoubleClick);
            this.dgvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.DgvFilesDragDrop);
            this.dgvFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.DgvFilesDragEnter);
            // 
            // ParseButtonState
            // 
            this.ParseButtonState.DataPropertyName = "ButtonText";
            this.ParseButtonState.HeaderText = "Action";
            this.ParseButtonState.Name = "ParseButtonState";
            this.ParseButtonState.ReadOnly = true;
            this.ParseButtonState.ToolTipText =
                "Left click open files and output location" + 
                System.Environment.NewLine +
                "Right click to copy dps.report link to clipboard, if applicable" + 
                System.Environment.NewLine + 
                "Middle click to only open output location";
            // 
            // btnClearFailed
            // 
            this.btnClearFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearFailed.Location = new System.Drawing.Point(523, 333);
            this.btnClearFailed.Name = "btnClearFailed";
            this.btnClearFailed.Size = new System.Drawing.Size(75, 23);
            this.btnClearFailed.TabIndex = 19;
            this.btnClearFailed.Text = "Clear Failed";
            this.TlpMainWindow.SetToolTip(this.btnClearFailed, "Removes from the list logs that could not be parsed");
            this.btnClearFailed.UseVisualStyleBackColor = true;
            this.btnClearFailed.Click += new System.EventHandler(this.BtnClearFailedClick);
            // 
            // logFileWatcher
            // 
            this.logFileWatcher.EnableRaisingEvents = true;
            this.logFileWatcher.IncludeSubdirectories = true;
            this.logFileWatcher.SynchronizingObject = this;
            this.logFileWatcher.Created += new System.IO.FileSystemEventHandler(this.LogFileWatcher_Created);
            this.logFileWatcher.Renamed += new System.IO.RenamedEventHandler(this.LogFileWatcher_Renamed);
            // 
            // VersionLabel
            // 
            this.VersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(16, 357);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(29, 13);
            this.VersionLabel.TabIndex = 17;
            this.VersionLabel.Text = "V1.3";
            // 
            // labWatchingDir
            // 
            this.labWatchingDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labWatchingDir.AutoEllipsis = true;
            this.labWatchingDir.Location = new System.Drawing.Point(16, 312);
            this.labWatchingDir.Name = "labWatchingDir";
            this.labWatchingDir.Size = new System.Drawing.Size(412, 13);
            this.labWatchingDir.TabIndex = 18;
            this.labWatchingDir.Text = "Watching log dir";
            // 
            // locationDataGridViewTextBoxColumn
            // 
            this.locationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.locationDataGridViewTextBoxColumn.DataPropertyName = "InputFile";
            this.locationDataGridViewTextBoxColumn.HeaderText = "Input File";
            this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
            this.locationDataGridViewTextBoxColumn.ReadOnly = true;
            this.locationDataGridViewTextBoxColumn.ToolTipText = "Double left click to open input location";
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // operatorBindingSource
            // 
            this.operatorBindingSource.DataSource = typeof(GW2EIParser.FormOperationController);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.ClientSize = new System.Drawing.Size(696, 375);
            this.Controls.Add(this.btnClearFailed);
            this.Controls.Add(this.labWatchingDir);
            this.Controls.Add(this.dgvFiles);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnCancelAll);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnPopulate);
            this.Controls.Add(this.lblHeader);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "GW2 Elite Insights Parser";
            this.TransparencyKey = System.Drawing.Color.OrangeRed;
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logFileWatcher)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.operatorBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Button btnPopulate;
        private System.Windows.Forms.Button btnCancelAll;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.DataGridView dgvFiles;
        private System.Windows.Forms.BindingSource operatorBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn ParseButtonState;
        private System.Windows.Forms.Label VersionLabel;
        private System.IO.FileSystemWatcher logFileWatcher;
        private System.Windows.Forms.Label labWatchingDir;
        private System.Windows.Forms.Button btnClearFailed;
        private System.Windows.Forms.ToolTip TlpMainWindow;
    }
}

