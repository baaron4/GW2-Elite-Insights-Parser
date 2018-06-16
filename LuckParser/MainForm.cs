using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using LuckParser.Controllers;

namespace LuckParser
{
    public partial class MainForm : Form
    {
        private SettingsForm _settingsForm;
        List<string> _logsFiles;
        Controller1 controller = new Controller1();

        public MainForm()
        {
            InitializeComponent();
            _logsFiles = new List<string>();
            btnCancel.Enabled = false;
            btnParse.Enabled = false;
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            _logsFiles = new List<string>();
            AddLogFiles(args);
        }

        /// <summary>
        /// Adds log files to the bound data source for display in the interface
        /// </summary>
        /// <param name="filesArray"></param>
        private void AddLogFiles(string[] filesArray)
        {
            foreach (string file in filesArray)
            {
                if (_logsFiles.Contains(file))
                {
                    //Don't add doubles
                    continue;
                }

                _logsFiles.Add(file);

                GridRow gRow = new GridRow(file, " ")
                {
                    BgWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true }
                };
                gRow.BgWorker.DoWork += BgWorker_DoWork;
                gRow.BgWorker.ProgressChanged += BgWorker_ProgressChanged;
                gRow.BgWorker.RunWorkerCompleted += BgWorker_Completed;
                
                gridRowBindingSource.Add(gRow);
            }

            btnParse.Enabled = true;
        }

        /// <summary>
        /// Invoked when a BackgroundWorker begins working.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bg = sender as BackgroundWorker;
            GridRow rowData = e.Argument as GridRow;

            e.Result = rowData;

            if (bg.CancellationPending)
            {
                rowData.Status = "Cancelled";
                bg.ReportProgress(100, rowData);
                e.Cancel = true;
                throw new CancellationException(rowData);
            }

            FileInfo fInfo = new FileInfo(rowData.Location);
            if (!fInfo.Exists)
            {
                rowData.Status = "File does not exist";
                bg.ReportProgress(100, rowData);
                e.Cancel = true;
                throw new CancellationException(rowData);
            }

            rowData.Status = "20% - Working...";
            bg.ReportProgress(20, rowData);
            Controller1 control = new Controller1();

            if (fInfo.Extension.Equals(".evtc", StringComparison.OrdinalIgnoreCase) ||
                fInfo.Name.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
            {
                //Process evtc here
                rowData.Status = "40% - Reading Binary...";
                bg.ReportProgress(40, rowData);
                control.ParseLog(fInfo.FullName);

                if (bg.CancellationPending)
                {
                    rowData.Status = "Cancelled";
                    bg.ReportProgress(100, rowData);
                    e.Cancel = true;
                    throw new CancellationException(rowData);
                }

                //Creating File
                //save location
                DirectoryInfo saveDirectory;
                if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
                {
                    //Default save directory
                    saveDirectory = fInfo.Directory;
                }
                else
                {
                    //Customised save directory
                    saveDirectory = new DirectoryInfo(Properties.Settings.Default.OutLocation);
                }

                string bossid = control.getBossData().getID().ToString();
                string result = "fail";

                if (control.getLogData().getBosskill())
                {
                    result = "kill";
                }

                bool[] settingsSnap = new bool[] {
                    Properties.Settings.Default.DPSGraphTotals,
                    Properties.Settings.Default.PlayerGraphTotals,
                    Properties.Settings.Default.PlayerGraphBoss,
                    Properties.Settings.Default.PlayerBoonsUniversal,
                    Properties.Settings.Default.PlayerBoonsImpProf,
                    Properties.Settings.Default.PlayerBoonsAllProf,
                    Properties.Settings.Default.PlayerRot,
                    Properties.Settings.Default.PlayerRotIcons,
                    Properties.Settings.Default.EventList,
                    Properties.Settings.Default.BossSummary,
                    Properties.Settings.Default.SimpleRotation,
                    Properties.Settings.Default.ShowAutos,
                    Properties.Settings.Default.LargeRotIcons,
                    Properties.Settings.Default.ShowEstimates
                };

                string outputType = Properties.Settings.Default.SaveOutHTML ? "html" : "csv";
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fInfo.Name}_{HTMLHelper.GetLink(bossid + "-ext")}_{result}.{outputType}"
                );

                rowData.LogLocation = outputFile;
                rowData.Status = "60% - Creating File...";
                bg.ReportProgress(60, rowData);

                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    bg.ReportProgress(80, $"Writing {outputType.ToUpper()}...");
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (Properties.Settings.Default.SaveOutHTML)
                        {
                            control.CreateHTML(sw, settingsSnap);
                        }
                        else
                        {
                            control.CreateCSV(sw, ",");
                        }
                    }
                }

                rowData.Status = "100% - Complete";
                bg.ReportProgress(100, rowData);
                rowData.ButtonState = GridRow.STATUS_OPEN;
            }
            else
            {
                rowData.Status = "Not EVTC";
                e.Cancel = true;
                throw new CancellationException(rowData);
            }
        }

        /// <summary>
        /// Invoked when a BackgroundWorker reports a change in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            dgvFiles.Invalidate();
        }

        /// <summary>
        /// Invoked when a BackgroundWorker completes its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            GridRow row;
            if (e.Cancelled || e.Error != null)
            {
                if (e.Error is CancellationException)
                {
                    row = ((CancellationException)e.Error).Row;
                    row.ButtonState = GridRow.STATUS_PARSE;
                }
            }
            else
            {
                row = (GridRow)e.Result;
                row.ButtonState = GridRow.STATUS_OPEN;
            }
            
            btnParse.Enabled = true;
            dgvFiles.Invalidate();
        }


        /// <summary>
        /// Invoked when the 'Parse All' button is clicked. Begins processing of all files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnParse_Click(object sender, EventArgs e)
        {
            if (_logsFiles.Count > 0)
            {
                btnParse.Enabled = false;
                btnCancel.Enabled = true;

                foreach (GridRow row in gridRowBindingSource)
                {
                    if (!row.BgWorker.IsBusy)
                    {
                        row.Run();
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the 'Cancel All' button is clicked. Cancels all pending operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            //Cancel all workers
            foreach (GridRow row in gridRowBindingSource)
            {
                if (row.BgWorker.IsBusy)
                {
                    row.Cancel();
                    dgvFiles.Invalidate();
                }
            }

            btnClear.Enabled = true;
            btnParse.Enabled = true;
        }

        /// <summary>
        /// Invoked when the 'Settings' button is clicked. Opens the settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettings_Click(object sender, EventArgs e)
        {
            _settingsForm = new SettingsForm(/*settingArray,this*/);
            _settingsForm.Show();
        }

        /// <summary>
        /// Invoked when the 'Clear All' button is clicked. Cancels pending operations and clears completed & un-started operations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            btnParse.Enabled = false;

            for (int i = gridRowBindingSource.Count - 1; i >= 0; i--)
            {
                GridRow row = gridRowBindingSource[i] as GridRow;
                if (row.BgWorker.IsBusy)
                {
                    row.Cancel();
                }
                else
                {
                    dgvFiles.Rows.RemoveAt(i);
                    _logsFiles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Invoked when a file is dropped onto the datagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvFiles_DragDrop(object sender, DragEventArgs e)
        {
            btnParse.Enabled = true;
            string[] filesArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            AddLogFiles(filesArray);
        }

        /// <summary>
        /// Invoked when a dragged file enters the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// Invoked when a the content of a datagridview cell is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                GridRow row = (GridRow)gridRowBindingSource[e.RowIndex];

                switch (row.ButtonState)
                {
                    case GridRow.STATUS_PARSE:
                        row.Run();
                        btnCancel.Enabled = true;
                        break;

                    case GridRow.STATUS_CANCEL:
                        row.Cancel();
                        dgvFiles.Invalidate();
                        btnParse.Enabled = true;
                        break;

                    case GridRow.STATUS_OPEN:
                        string fileLoc = row.LogLocation;
                        if (File.Exists(fileLoc))
                        {
                            System.Diagnostics.Process.Start(fileLoc);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}