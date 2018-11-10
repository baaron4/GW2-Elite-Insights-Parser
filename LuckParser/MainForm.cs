using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser
{
    public partial class MainForm : Form
    {
        private SettingsForm _settingsForm;
        private readonly List<string> _logsFiles;
        private bool _anyRunning;
        private readonly Queue<GridRow> _logQueue = new Queue<GridRow>();
        public MainForm()
        {
            InitializeComponent();
            //display version
            string version = Application.ProductVersion;
            VersionLabel.Text = version;
            _logsFiles = new List<string>();
            btnCancel.Enabled = false;
            btnParse.Enabled = false;
            UpdateWatchDirectory();
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
                gRow.BgWorker.DoWork += BgWorkerDoWork;
                gRow.BgWorker.ProgressChanged += BgWorkerProgressChanged;
                gRow.BgWorker.RunWorkerCompleted += BgWorkerCompleted;

                gridRowBindingSource.Add(gRow);

                if (Properties.Settings.Default.AutoParse)
                {
                    gRow.Run();
                }
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
                if (_anyRunning)
                {
                    _logQueue.Enqueue(row);
                    row.Status = "Queued";
                    row.Metadata.State = RowState.Pending;
                    dgvFiles.Invalidate();
                }
                else
                {
                    row.Run();
                    _anyRunning = true;
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
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            System.Globalization.CultureInfo before = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            BackgroundWorker bg = sender as BackgroundWorker;
            GridRow rowData = e.Argument as GridRow;
            UploadController up_controller = null;
            e.Result = rowData;

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
                string[] uploadresult = new string[3] { "","",""};
                bg.UpdateProgress(rowData, " Working...", 0);

                SettingsContainer settings = new SettingsContainer(Properties.Settings.Default);
                Parser parser = new Parser(settings);

                if (fInfo.Extension.Equals(".evtc", StringComparison.OrdinalIgnoreCase) ||
                    fInfo.Name.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
                {
                    //Process evtc here
                    bg.UpdateProgress(rowData, "10% - Reading Binary...", 10);
                    ParsedLog log = parser.ParseLog(rowData, fInfo.FullName);
                    bg.ThrowIfCanceled(rowData);
                    bg.UpdateProgress(rowData, "35% - Data parsed", 35);
                    bool uploadAuthorized = !Properties.Settings.Default.SkipFailedTries || (Properties.Settings.Default.SkipFailedTries && log.FightData.Success);
                    if (Properties.Settings.Default.UploadToDPSReports && uploadAuthorized)
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
                    if (Properties.Settings.Default.UploadToDPSReportsRH && uploadAuthorized)
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
                    if (Properties.Settings.Default.UploadToRaidar && uploadAuthorized)
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
                    if (Properties.Settings.Default.SkipFailedTries)
                    {
                        if (!log.FightData.Success)
                        {
                            throw new SkipException();
                        }
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
                    if (saveDirectory == null)
                    {
                        throw new CancellationException(rowData, new Exception("Invalid save directory"));
                    }
                    string result = log.FightData.Success ? "kill" : "fail";
                    string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_"+(log.FightData.FightDuration/1000).ToString()+"s" : "";
                    string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_"+log.PlayerList.Find(x => x.AgentItem.Name.Split(':')[0] == log.LogData.PoV.Split(':')[0]).Prof.ToLower() : "";



                    StatisticsCalculator statisticsCalculator = new StatisticsCalculator(settings);
                    StatisticsCalculator.Switches switches = new StatisticsCalculator.Switches();
                    if (Properties.Settings.Default.SaveOutHTML)
                    {
                        HTMLBuilder.UpdateStatisticSwitches(switches);
                    }
                    if (Properties.Settings.Default.SaveOutCSV)
                    {
                        CSVBuilder.UpdateStatisticSwitches(switches);
                    }
                    if (Properties.Settings.Default.SaveOutJSON)
                    {
                        JSONBuilder.UpdateStatisticSwitches(switches);
                    }
                    Statistics statistics = statisticsCalculator.CalculateStatistics(log, switches);
                    bg.UpdateProgress(rowData, "85% - Statistics computed", 85);
                    string fName = fInfo.Name.Split('.')[0];
                    fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";
                    bg.UpdateProgress(rowData, "90% - Creating File...", 90);
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
                            if (Properties.Settings.Default.NewHtmlMode)
                            {
                                var builder = new HTMLBuilderNew(log, settings, statistics);
                                builder.CreateHTML(sw, saveDirectory.FullName);
                            }
                            else
                            {
                                var builder = new HTMLBuilder(log, settings, statistics, uploadresult);
                                builder.CreateHTML(sw);
                            }
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
                            var builder = new CSVBuilder(sw, ",", log, settings, statistics,uploadresult);
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
                        rowData.LogLocation += splitString + outputFile;
                        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        using (var sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            var builder = new JSONBuilder(sw, log, settings, statistics, false, uploadresult);
                            builder.CreateJSON();
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
            _anyRunning = false;

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
        private void BtnSettingsClick(object sender, EventArgs e)
        {
            _settingsForm = new SettingsForm(this);
            _settingsForm.Show();
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
            btnParse.Enabled = true;
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
                        string[] files = row.LogLocation.Split(',');
                        foreach (string fileLoc in files)
                        {
                            if (File.Exists(fileLoc))
                            {
                                System.Diagnostics.Process.Start(fileLoc);
                            }
                        }
                        break;

                }
            }
        }

        public void UpdateWatchDirectory()
        {
            if (Properties.Settings.Default.AutoAdd)
            {
                logFileWatcher.Path = Properties.Settings.Default.AutoAddPath;
                labWatchingDir.Text = "Watching for log files in " + Properties.Settings.Default.AutoAddPath;
                logFileWatcher.EnableRaisingEvents = true;
                labWatchingDir.Visible = true;
            } else
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
            if ((e.FullPath.EndsWith(".zip") && !e.FullPath.EndsWith(".tmp.zip")) || e.FullPath.EndsWith(".evtc"))
            {
                AddDelayed(e.FullPath);
            }
        }

        private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (e.OldFullPath.EndsWith(".tmp.zip") && e.FullPath.EndsWith(".zip")) {
                AddDelayed(e.FullPath);
            }
        }
    }
}
