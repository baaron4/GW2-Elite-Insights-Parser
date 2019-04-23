using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using LuckParser.Controllers;
using LuckParser.Parser;
using System.Text;
using System.Threading.Tasks;
using LuckParser.Exceptions;
using LuckParser.Builders;
using LuckParser.Setting;
using LuckParser.Models;

namespace LuckParser
{
    public partial class MainForm : Form
    {
        private SettingsForm _settingsForm;
        private readonly List<string> _logsFiles;
        private int _runningCount;
        private bool _anyRunning;
        private readonly Queue<GridRow> _logQueue = new Queue<GridRow>();
        private MainForm()
        {
            InitializeComponent();
            //display version
            string version = Application.ProductVersion;
            VersionLabel.Text = version;
            _logsFiles = new List<string>();
            btnCancel.Enabled = false;
            btnParse.Enabled = false;
            UpdateWatchDirectory();
            _settingsForm = new SettingsForm(this);
            _settingsForm.SettingsClosedEvent += EnableSettingsWatcher;
            _settingsForm.WatchDirectoryUpdatedEvent += UpdateWatchDirectoryWatcher;
        }

        public MainForm(IEnumerable<string> filesArray) : this()
        {
            AddLogFiles(filesArray);
        }

        /// <summary>
        /// Adds log files to the bound data source for display in the interface
        /// </summary>
        /// <param name="filesArray"></param>
        private void AddLogFiles(IEnumerable<string> filesArray)
        {
            foreach (string file in filesArray)
            {
                if (_logsFiles.Contains(file))
                {
                    //Don't add doubles
                    continue;
                }

                _logsFiles.Add(file);

                GridRow gRow = new GridRow(file, "Ready to parse")
                {
                    BgWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true }
                };
                gRow.BgWorker.DoWork += BgWorkerDoWork;
                gRow.BgWorker.ProgressChanged += BgWorkerProgressChanged;
                gRow.BgWorker.RunWorkerCompleted += BgWorkerCompleted;

                gridRowBindingSource.Add(gRow);

                if (Properties.Settings.Default.AutoParse)
                {
                    QueueOrRunWorker(gRow);
                }
            }

            btnParse.Enabled = !Properties.Settings.Default.AutoParse;
            btnCancel.Enabled = Properties.Settings.Default.AutoParse;
        }

        private void EnableSettingsWatcher(object sender, EventArgs e)
        {
            btnSettings.Enabled = true;
        }

        /// <summary>
        /// Queues a background worker. If the 'ParseOneAtATime' setting is false, workers are run asynchronously
        /// </summary>
        /// <param name="row"></param>
        private void QueueOrRunWorker(GridRow row)
        {
            btnClear.Enabled = false;
            btnParse.Enabled = false;
            btnCancel.Enabled = true;
            if (Properties.Settings.Default.ParseOneAtATime)
            {
                if (_anyRunning)
                {
                    _logQueue.Enqueue(row);
                    row.Status = "Queued";
                    row.Metadata.State = RowState.Pending;
                    dgvFiles.Invalidate();
                }
                else
                {
                    _anyRunning = true;
                    row.Run();
                }
            }
            else
            {
                row.Status = "Waiting for a thread";
                row.Metadata.State = RowState.Pending;
                row.Run();
            }
        }

        /// <summary>
        /// Runs the next background worker, if one is available
        /// </summary>
        private void RunNextWorker()
        {
            if (Properties.Settings.Default.ParseOneAtATime)
            {
                _anyRunning = false;
            }
            if (_logQueue.Count > 0)
            {
                GridRow row = _logQueue.Dequeue();
                _anyRunning = true;
                row.Run();
            }
            else
            {
                if (_runningCount == 0)
                {
                    _anyRunning = false;
                    btnParse.Enabled = true;
                    btnClear.Enabled = true;
                    btnCancel.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Invoked when a BackgroundWorker begins working.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            System.Globalization.CultureInfo before = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            BackgroundWorker bg = sender as BackgroundWorker;
            GridRow rowData = e.Argument as GridRow;
            UploadController up_controller = null;
            e.Result = rowData;

            _runningCount++;
            _anyRunning = true;
            bg.ThrowIfCanceled(rowData);

            try
            {
                FileInfo fInfo = new FileInfo(rowData.Location);
                if (fInfo == null || !fInfo.Exists)
                {
                    bg.UpdateProgress(rowData, "File does not exist", 100);
                    e.Cancel = true;
                    throw new CancellationException(rowData);
                }
                //Upload Process
                Task<string> DREITask = null;
                Task<string> DRRHTask = null;
                Task<string> RaidarTask = null;
                string[] uploadresult = new string[3] { "", "", "" };
                bg.UpdateProgress(rowData, " Working...", 0);
                
                ParsingController parser = new ParsingController();

                if (!GeneralHelper.HasFormat())
                {
                    throw new CancellationException(rowData, new Exception("No output format has been selected"));
                }

                if (GeneralHelper.IsSupportedFormat(fInfo.Name))
                {
                    //Process evtc here
                    bg.UpdateProgress(rowData, "10% - Reading Binary...", 10);
                    ParsedLog log = parser.ParseLog(rowData, fInfo.FullName);
                    bg.ThrowIfCanceled(rowData);
                    bg.UpdateProgress(rowData, "35% - Data parsed", 35);
                    if (Properties.Settings.Default.UploadToDPSReports)
                    {
                        bg.UpdateProgress(rowData, " 40% - Uploading to DPSReports using EI...", 40);
                        if (up_controller == null)
                        {
                            up_controller = new UploadController();
                        }
                        DREITask = Task.Factory.StartNew(() => up_controller.UploadDPSReportsEI(fInfo));
                        if (DREITask != null)
                        {
                            while (!DREITask.IsCompleted)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            uploadresult[0] = DREITask.Result;
                        }
                        else
                        {
                            uploadresult[0] = "Failed to Define Upload Task";
                        }
                    }
                    bg.ThrowIfCanceled(rowData);
                    if (Properties.Settings.Default.UploadToDPSReportsRH)
                    {
                        bg.UpdateProgress(rowData, " 40% - Uploading to DPSReports using RH...", 40);
                        if (up_controller == null)
                        {
                            up_controller = new UploadController();
                        }
                        DRRHTask = Task.Factory.StartNew(() => up_controller.UploadDPSReportsRH(fInfo));
                        if (DRRHTask != null)
                        {
                            while (!DRRHTask.IsCompleted)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            uploadresult[1] = DRRHTask.Result;
                        }
                        else
                        {
                            uploadresult[1] = "Failed to Define Upload Task";
                        }
                    }
                    bg.ThrowIfCanceled(rowData);
                    if (Properties.Settings.Default.UploadToRaidar)
                    {
                        bg.UpdateProgress(rowData, " 40% - Uploading to Raidar...", 40);
                        if (up_controller == null)
                        {
                            up_controller = new UploadController();
                        }
                        RaidarTask = Task.Factory.StartNew(() => up_controller.UploadRaidar(fInfo));
                        if (RaidarTask != null)
                        {
                            while (!RaidarTask.IsCompleted)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            uploadresult[2] = RaidarTask.Result;
                        }
                        else
                        {
                            uploadresult[2] = "Failed to Define Upload Task";
                        }
                    }
                    bg.ThrowIfCanceled(rowData);
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
                    if (saveDirectory == null)
                    {
                        throw new CancellationException(rowData, new Exception("Invalid save directory"));
                    }
                    string result = log.FightData.Success ? "kill" : "fail";
                    string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_" + (log.FightData.FightDuration / 1000).ToString() + "s" : "";
                    string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_" + log.PlayerList.Find(x => x.AgentItem.Name.Split(':')[0] == log.LogData.PoV.Split(':')[0]).Prof.ToLower() : "";
                    
                    bg.ThrowIfCanceled(rowData);
                    bg.UpdateProgress(rowData, "85% - Statistics computed", 85);
                    bg.ThrowIfCanceled(rowData);
                    string fName = fInfo.Name.Split('.')[0];
                    fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";
                    bg.UpdateProgress(rowData, "90% - Creating File...", 90);
                    bg.ThrowIfCanceled(rowData);
                    if (Properties.Settings.Default.SaveOutHTML)
                    {
                        string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.html"
                        );
                        rowData.LogLocation = outputFile;
                        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        using (var sw = new StreamWriter(fs))
                        {
                            var builder = new HTMLBuilder(log, uploadresult);
                            builder.CreateHTML(sw, saveDirectory.FullName);
                        }
                    }
                    if (Properties.Settings.Default.SaveOutCSV)
                    {
                        string outputFile = Path.Combine(
                            saveDirectory.FullName,
                            $"{fName}.csv"
                        );
                        string splitString = "";
                        if (rowData.LogLocation != null) { splitString = ","; }
                        rowData.LogLocation += splitString + outputFile;
                        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                        {
                            var builder = new CSVBuilder(sw, ",", log, uploadresult);
                            builder.CreateCSV();
                        }
                    }
                    if (Properties.Settings.Default.SaveOutJSON)
                    {
                        string outputFile = Path.Combine(
                            saveDirectory.FullName,
                            $"{fName}.json"
                        );
                        string splitString = "";
                        if (rowData.LogLocation != null) { splitString = ","; }
                        rowData.LogLocation += splitString + saveDirectory.FullName;
                        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        using (var sw = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                        {
                            var builder = new RawFormatBuilder(sw, log, uploadresult);
                            builder.CreateJSON();
                        }
                    }

                    if (Properties.Settings.Default.SaveOutXML)
                    {
                        string outputFile = Path.Combine(
                            saveDirectory.FullName,
                            $"{fName}.xml"
                        );
                        string splitString = "";
                        if (rowData.LogLocation != null) { splitString = ","; }
                        rowData.LogLocation += splitString + saveDirectory.FullName;
                        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        using (var sw = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                        {
                            var builder = new RawFormatBuilder(sw, log, uploadresult);
                            builder.CreateXML();
                        }
                    }

                    bg.UpdateProgress(rowData, $"100% - Complete_{log.FightData.Logic.Extension}_{result}", 100);
                }
                else
                {
                    bg.UpdateProgress(rowData, "Not EVTC", 100);
                    e.Cancel = true;
                    Console.Error.Write("Not EVTC");
                    throw new CancellationException(rowData);
                }

            }
            catch (SkipException s)
            {
                Console.Write(s.Message);
                throw new CancellationException(rowData, s);
            }
            catch (TooShortException t)
            {
                Console.Write(t.Message);
                throw new CancellationException(rowData, t);
            }
            catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
            {
                Console.Error.Write(ex.Message);
                throw new CancellationException(rowData, ex);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = before;
            }

        }

        /// <summary>
        /// Invoked when a BackgroundWorker reports a change in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Redraw rows
            dgvFiles.Invalidate();
        }

        /// <summary>
        /// Invoked when a BackgroundWorker completes its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GridRow row;
            _runningCount--;
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
            dgvFiles.Invalidate();
            RunNextWorker();
        }

        /// <summary>
        /// Invoked when the 'Parse All' button is clicked. Begins processing of all files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnParseClick(object sender, EventArgs e)
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
        private void BtnCancelClick(object sender, EventArgs e)
        {
            //Clear queue so queued workers don't get started by any cancellations
            HashSet<GridRow> rows = new HashSet<GridRow>(_logQueue);
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
                else if (rows.Contains(row))
                {
                    row.Status = "Ready to parse";
                }
                dgvFiles.Invalidate();
            }

            btnClear.Enabled = true;
            btnParse.Enabled = true;
            btnCancel.Enabled = false;
        }

        /// <summary>
        /// Invoked when the 'Settings' button is clicked. Opens the settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettingsClick(object sender, EventArgs e)
        {          
            _settingsForm.Show();
            btnSettings.Enabled = false;
        }

        /// <summary>
        /// Invoked when the 'Clear All' button is clicked. Cancels pending operations and clears completed & un-started operations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearClick(object sender, EventArgs e)
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
        private void DgvFilesDragDrop(object sender, DragEventArgs e)
        {
            string[] filesArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            AddLogFiles(filesArray);
        }

        /// <summary>
        /// Invoked when a dragged file enters the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvFilesDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// Invoked when a the content of a datagridview cell is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvFilesCellContentClick(object sender, DataGridViewCellEventArgs e)
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
                        string[] paths = row.LogLocation.Split(',');
                        foreach (string path in paths)
                        {
                            if (File.Exists(path) || Directory.Exists(path))
                            {
                                System.Diagnostics.Process.Start(path);
                            }
                        }
                        break;

                }
            }
        }

        private void UpdateWatchDirectoryWatcher(object sender, EventArgs e)
        {
            UpdateWatchDirectory();
        }

        private void UpdateWatchDirectory()
        {
            if (Properties.Settings.Default.AutoAdd)
            {
                logFileWatcher.Path = Properties.Settings.Default.AutoAddPath;
                labWatchingDir.Text = "Watching for log files in " + Properties.Settings.Default.AutoAddPath;
                logFileWatcher.EnableRaisingEvents = true;
                labWatchingDir.Visible = true;
            }
            else
            {
                labWatchingDir.Visible = false;
                logFileWatcher.EnableRaisingEvents = false;
            }
        }

        /// <summary>
        /// Waits 3 seconds, checks if the file still exists and then adds it to the queue.
        /// This is neccessary because:
        /// 1.) Arc needs some time to complete writing the log file. The watcher gets triggered as soon as the writing starts.
        /// 2.) When Arc is configured to use ZIP compression, the log file is still created as usual, but after the file is written
        ///     it is then zipped and deleted again. Therefore the watcher gets triggered twice, first for the .evtc and then for the .zip.
        /// 3.) Zipping the file also needs time, so we have to wait a bit there too.
        /// </summary>
        /// <param name="path"></param>
        private async void AddDelayed(string path)
        {
            await Task.Delay(3000);
            if (File.Exists(path))
            {
                AddLogFiles(new string[] { path });
            }
        }

        private void LogFileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (GeneralHelper.IsSupportedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }

        private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (GeneralHelper.IsTemporaryFormat(e.OldFullPath) && GeneralHelper.IsCompressedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }
    }
}
