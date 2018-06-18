using System;
using System.Linq;
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
        private List<string> _logsFiles = new List<string>();
        private Controller1 controller = new Controller1();
        private bool _AnyRunning = false;
        private Queue<GridRow> _logQueue = new Queue<GridRow>();

        public MainForm()
        {
            InitializeComponent();
            btnCancel.Enabled = false;
            btnParse.Enabled = false;
        }

        /// <summary>
        /// Adds log files to the bound data source for display in the interface
        /// </summary>
        /// <param name="filesArray"></param>
        /// <param name="consoleStart"></param>
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
        /// Queues a background worker. If the 'ParseOneAtATime' setting is false, workers are run asynchronously
        /// </summary>
        /// <param name="row"></param>
        private void QueueOrRunWorker(GridRow row)
        {
            if (Properties.Settings.Default.ParseOneAtATime)
            {
                if (_AnyRunning)
                {
                    _logQueue.Enqueue(row);
                    row.Status = "Queued";
                    row.Metadata.State = RowState.Pending;
                    dgvFiles.Invalidate();
                }
                else
                {
                    row.Run();
                    _AnyRunning = true;
                }
            }
            else
            {
                row.Run();
            }
        }

        /// <summary>
        /// Runs the next background worker, if one is available
        /// </summary>
        private void RunNextWorker()
        {
            if (_logQueue.Count > 0)
            {
                GridRow row = _logQueue.Dequeue();
                row.Run();
            }
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

            bg.ThrowIfCanceled(rowData, "Cancelled");

            FileInfo fInfo = new FileInfo(rowData.Location);
            if (!fInfo.Exists)
            {
                bg.UpdateProgress(rowData, "File does not exist", 100);
                e.Cancel = true;
                throw new CancellationException(rowData);
            }

            bg.UpdateProgress(rowData, " Working...", 0);
            Controller1 control = new Controller1();

            if (fInfo.Extension.Equals(".evtc", StringComparison.OrdinalIgnoreCase) ||
                fInfo.Name.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
            {
                //Process evtc here
                bg.UpdateProgress(rowData, "10% - Reading Binary...", 10);
                control.ParseLog(rowData, fInfo.FullName);
                bg.ThrowIfCanceled(rowData, "Cancelled");
                bg.UpdateProgress(rowData, "45% - Data parsed", 45);

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

                bg.UpdateProgress(rowData, "50% - Creating File...", 50);

                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    bg.UpdateProgress(rowData, $"50% - Writing {outputType.ToUpper()}...", 50);
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (Properties.Settings.Default.SaveOutHTML)
                        {
                            control.CreateHTML(rowData, sw, settingsSnap);
                        }
                        else
                        {
                            control.CreateCSV(sw, ",");
                        }
                    }
                }

                bg.UpdateProgress(rowData, "100% - Complete", 100);
            }
            else
            {
                bg.UpdateProgress(rowData, "Not EVTC", 100);
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
            //Redraw rows
            dgvFiles.Invalidate();
        }

        /// <summary>
        /// Invoked when a BackgroundWorker completes its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            _AnyRunning = false;

            GridRow row;
            if (e.Cancelled || e.Error != null)
            {
                if (e.Error is CancellationException)
                {
                    row = ((CancellationException)e.Error).Row;
                    if (e.Error.InnerException != null)
                    {
                        row.Status = e.Error.InnerException.Message;
                    }

                    if (row.Metadata.State == RowState.ClearOnComplete)
                    {
                        gridRowBindingSource.Remove(row);
                    }
                    else
                    {
                        row.Metadata.State = RowState.Ready;
                        row.ButtonText = "Parse";
                    }
                }
            }
            else
            {
                row = (GridRow)e.Result;
                if (row.Metadata.State == RowState.ClearOnComplete)
                {
                    gridRowBindingSource.Remove(row);
                }
                else
                {
                    row.ButtonText = "Open";
                    row.Metadata.State = RowState.Complete;
                }
            }
            
            btnParse.Enabled = true;
            dgvFiles.Invalidate();

            if (Properties.Settings.Default.ParseOneAtATime)
            {
                RunNextWorker();
            }
        }
        
        /// <summary>
        /// Invoked when the 'Parse All' button is clicked. Begins processing of all files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnParse_Click(object sender, EventArgs e)
        {
            //Clear queue before parsing all
            _logQueue.Clear();

            if (_logsFiles.Count > 0)
            {
                btnParse.Enabled = false;
                btnCancel.Enabled = true;

                foreach (GridRow row in gridRowBindingSource)
                {
                    if (!row.BgWorker.IsBusy)
                    {
                        QueueOrRunWorker(row);
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
            //Clear queue so queued workers don't get started by any cancellations
            _logQueue.Clear();

            //Cancel all workers
            foreach (GridRow row in gridRowBindingSource)
            {
                if (row.Metadata.State == RowState.Pending)
                {
                    row.Metadata.State = RowState.Ready;
                }

                if (row.BgWorker.IsBusy)
                {
                    row.Cancel();
                }
                dgvFiles.Invalidate();
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
            _settingsForm = new SettingsForm();
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

            //Clear the queue so that cancelled workers don't invoke queued workers
            _logQueue.Clear();
            _logsFiles.Clear();

            for (int i = gridRowBindingSource.Count - 1; i >= 0; i--)
            {
                GridRow row = gridRowBindingSource[i] as GridRow;
                if (row.BgWorker.IsBusy)
                {
                    row.Cancel();
                    row.Metadata.State = RowState.ClearOnComplete;
                }
                else
                {
                    gridRowBindingSource.RemoveAt(i);
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

                switch (row.Metadata.State)
                {
                    case RowState.Ready:
                        QueueOrRunWorker(row);
                        btnCancel.Enabled = true;
                        break;

                    case RowState.Parsing:
                        row.Cancel();
                        dgvFiles.Invalidate();
                        break;

                    case RowState.Complete:
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