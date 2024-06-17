using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using GW2EIDiscord;
using GW2EIParser.Setting;
using GW2EIParserCommons;
using GW2EIParserCommons.Exceptions;

namespace GW2EIParser
{
    internal partial class MainForm : Form
    {
        private readonly SettingsForm _settingsForm;
        private readonly List<string> _logsFiles;
        private List<ulong> _currentDiscordMessageIDs = new List<ulong>();
        private int _runningCount = 0;
        private bool _anyRunning => _runningCount > 0;
        private readonly Queue<FormOperationController> _logQueue = new Queue<FormOperationController>();

        private readonly string _traceFileName;

        private int _fileNameSorting = 0;

        private readonly ProgramHelper _programHelper;
        private MainForm(ProgramHelper programHelper)
        {
            _programHelper = programHelper;
            DateTime now = DateTime.Now;
            _traceFileName = ProgramHelper.EILogPath + "EILogs-" + now.Year + "-" + now.Month + "-" + now.Day + "-" + now.Hour + "-" + now.Minute + "-" + now.Second + ".txt";
            InitializeComponent();
            // Traces
            ChkApplicationTraces.Checked = Properties.Settings.Default.ApplicationTraces;
            ChkAutoDiscordBatch.Checked = Properties.Settings.Default.AutoDiscordBatch;
            NumericCustomPopulateLimit.Value = Properties.Settings.Default.PopulateHourLimit;
            //display version
            string version = Application.ProductVersion;
            LblVersion.Text = version;
            _logsFiles = new List<string>();
            BtnCancelAll.Enabled = false;
            BtnParse.Enabled = false;
            UpdateWatchDirectory();
            _settingsForm = new SettingsForm(_programHelper);
            _settingsForm.SettingsClosedEvent += EnableSettingsWatcher;
            _settingsForm.SettingsLoadedEvent += LoadSettingsWatcher;
            _settingsForm.WatchDirectoryUpdatedEvent += UpdateWatchDirectoryWatcher;
            FormClosing += new FormClosingEventHandler((sender, e) => Properties.Settings.Default.Save());
        }

        private void LoadSettingsWatcher(object sender, EventArgs e)
        {
            AddTraceMessage("Settings: Loaded settings");
            ChkApplicationTraces.Checked = Properties.Settings.Default.ApplicationTraces;
            ChkAutoDiscordBatch.Checked = Properties.Settings.Default.AutoDiscordBatch;
            NumericCustomPopulateLimit.Value = Properties.Settings.Default.PopulateHourLimit;
        }

        public MainForm(IEnumerable<string> filesArray, ProgramHelper programHelper) : this(programHelper)
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
                AddTraceMessage("UI: Added " + file);

                var operation = new FormOperationController(file, "Ready to parse", DgvFiles, OperatorBindingSource);

                if (Properties.Settings.Default.AutoParse)
                {
                    QueueOrRunOperation(operation);
                }
            }
            if (_fileNameSorting != 0)
            {
                SortDgvFiles();
            }

            BtnParse.Enabled = !_anyRunning && filesArray.Any();
        }

        private void _RunOperation(FormOperationController operation)
        {
            _programHelper.ExecuteMemoryCheckTask();
            _runningCount++;
            _settingsForm.ConditionalSettingDisable(_anyRunning);
            operation.ToQueuedState();
            AddTraceMessage("Operation: Queued " + operation.InputFile);
            var cancelTokenSource = new CancellationTokenSource();// Prepare task
            Task task = Task.Run(() =>
            {
                operation.ToRunState();
                AddTraceMessage("Operation: Parsing " + operation.InputFile);
                _programHelper.DoWork(operation);
            }, cancelTokenSource.Token).ContinueWith(t =>
            {
                GC.Collect();
                cancelTokenSource.Dispose();
                _runningCount--;
                AddTraceMessage("Operation: Parsed " + operation.InputFile);
                // Exception management
                if (t.IsFaulted)
                {
                    if (t.Exception != null)
                    {
                        if (t.Exception.InnerExceptions.Count > 1)
                        {
                            operation.UpdateProgress("Program: something terrible has happened");
                        }
                        else
                        {
                            Exception ex = t.Exception.InnerExceptions[0];
                            if (!(ex is ProgramException))
                            {
                                operation.UpdateProgress("Program: something terrible has happened");
                            }
                            if (!(ex.InnerException is OperationCanceledException))
                            {
                                operation.UpdateProgress("Program: " + ex.InnerException.Message);
                            }
                            else
                            {
                                operation.UpdateProgress("Program: operation Aborted");
                            }
                        }
                    }
                    else
                    {
                        operation.UpdateProgress("Program: something terrible has happened");
                    }
                }
                if (operation.State == OperationState.ClearOnCancel)
                {
                    OperatorBindingSource.Remove(operation);
                }
                else
                {
                    if (t.IsFaulted)
                    {
                        operation.ToUnCompleteState();
                    }
                    else if (t.IsCanceled)
                    {
                        operation.UpdateProgress("Program: operation Aborted");
                        operation.ToUnCompleteState();
                    }
                    else if (t.IsCompleted)
                    {
                        operation.ToCompleteState();
                    }
                    else
                    {
                        operation.UpdateProgress("Program: something terrible has happened");
                        operation.ToUnCompleteState();
                    }
                }
                _programHelper.GenerateTraceFile(operation);
                if (operation.State != OperationState.Complete)
                {
                    operation.Reset();
                }
                _RunNextOperation();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            operation.SetContext(cancelTokenSource, task);
        }

        /// <summary>
        /// Queues an operation. If the 'MultipleLogs' setting is true, operations are run asynchronously
        /// </summary>
        /// <param name="operation"></param>
        private void QueueOrRunOperation(FormOperationController operation)
        {
            BtnClearAll.Enabled = false;
            BtnParse.Enabled = false;
            BtnCancelAll.Enabled = true;
            BtnDiscordBatch.Enabled = false;
            ChkAutoDiscordBatch.Enabled = false;
            if (_programHelper.ParseMultipleLogs() && _runningCount < _programHelper.GetMaxParallelRunning())
            {
                _RunOperation(operation);
            }
            else
            {
                if (_anyRunning)
                {
                    _logQueue.Enqueue(operation);
                    operation.ToPendingState();
                }
                else
                {
                    _RunOperation(operation);
                }
            }

        }

        /// <summary>
        /// Runs the next operation, if one is available
        /// </summary>
        private void _RunNextOperation()
        {
            if (_logQueue.Count > 0 && (_programHelper.ParseMultipleLogs() || !_anyRunning))
            {
                _RunOperation(_logQueue.Dequeue());
            }
            else
            {
                if (!_anyRunning)
                {
                    BtnParse.Enabled = true;
                    BtnClearAll.Enabled = true;
                    BtnCancelAll.Enabled = false;
                    BtnDiscordBatch.Enabled = true;
                    ChkAutoDiscordBatch.Enabled = true;
                    _settingsForm.ConditionalSettingDisable(_anyRunning);
                    AutoUpdateDiscordBatch();
                }
            }
        }

        /// <summary>
        /// Invoked when the 'Parse All' button is clicked. Begins processing of all files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnParseClick(object sender, EventArgs e)
        {
            AddTraceMessage("UI: Parse all files");
            //Clear queue before parsing all
            _logQueue.Clear();

            if (_logsFiles.Count > 0)
            {
                BtnParse.Enabled = false;
                BtnCancelAll.Enabled = true;

                foreach (FormOperationController operation in OperatorBindingSource)
                {
                    if (!operation.IsBusy())
                    {
                        QueueOrRunOperation(operation);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the 'Cancel All' button is clicked. Cancels all pending operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelAllClick(object sender, EventArgs e)
        {
            AddTraceMessage("UI: Cancelling all pending and ongoing parsing operations");
            //Clear queue so queued workers don't get started by any cancellations
            var operations = new HashSet<FormOperationController>(_logQueue);
            _logQueue.Clear();

            //Cancel all workers
            foreach (FormOperationController operation in OperatorBindingSource)
            {
                if (operation.IsBusy())
                {
                    operation.ToCancelState();
                }
                else if (operations.Contains(operation))
                {
                    operation.ToReadyState();
                }
            }

            BtnClearAll.Enabled = true;
            BtnParse.Enabled = true;
            BtnCancelAll.Enabled = false;
            BtnDiscordBatch.Enabled = true;
            ChkAutoDiscordBatch.Enabled = true;
        }

        /// <summary>
        /// Invoked when the 'Settings' button is clicked. Opens the settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettingsClick(object sender, EventArgs e)
        {
            AddTraceMessage("Settings: Opening settings");
            _settingsForm.Show();
            BtnSettings.Enabled = false;
        }

        /// <summary>
        /// Invoked when the 'Clear All' button is clicked. Cancels pending operations and clears completed & un-started operations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearAllClick(object sender, EventArgs e)
        {
            AddTraceMessage("UI: Clearing all logs");
            BtnCancelAll.Enabled = false;
            BtnParse.Enabled = false;

            //Clear the queue so that cancelled workers don't invoke queued workers
            _logQueue.Clear();
            _logsFiles.Clear();

            for (int i = OperatorBindingSource.Count - 1; i >= 0; i--)
            {
                var operation = OperatorBindingSource[i] as FormOperationController;
                if (operation.IsBusy())
                {
                    operation.ToCancelAndClearState();
                }
                else
                {
                    OperatorBindingSource.RemoveAt(i);
                }
            }
        }

        private void BtnClearFailedClick(object sender, EventArgs e)
        {
            AddTraceMessage("UI: Clearing failed to parse logs");
            for (int i = OperatorBindingSource.Count - 1; i >= 0; i--)
            {
                var operation = OperatorBindingSource[i] as FormOperationController;
                if (!operation.IsBusy() && operation.State == OperationState.UnComplete)
                {
                    OperatorBindingSource.RemoveAt(i);
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

        private void SortDgvFiles()
        {
            if (_fileNameSorting == 0)
            {
                _fileNameSorting = 1;
            }
            var auxList = new List<FormOperationController>();
            foreach (FormOperationController val in OperatorBindingSource)
            {
                auxList.Add(val);
            }
            auxList.Sort((form1, form2) =>
            {
                string right = new FileInfo(form2.InputFile).Name;
                string left = new FileInfo(form1.InputFile).Name;
                return _fileNameSorting * string.Compare(left, right, StringComparison.Ordinal);
            });
            OperatorBindingSource.Clear();
            foreach (FormOperationController val in auxList)
            {
                OperatorBindingSource.Add(val);
            }
        }

        private void DgvFilesCellContentClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                switch (e.ColumnIndex)
                {
                    case 0:
                        AddTraceMessage("UI: Sorting logs");
                        _fileNameSorting *= -1;
                        SortDgvFiles();
                        LocationDataGridViewTextBoxColumn.HeaderText = "Input File " + (_fileNameSorting < 0 ? "↓" : "↑");
                        break;
                    default:
                        break;
                }
                return;
            }
            var operation = (FormOperationController)OperatorBindingSource[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 2:
                    if (operation.State == OperationState.Complete && e.Button == MouseButtons.Right && operation.DPSReportLink != null)
                    {
                        Clipboard.SetText(operation.DPSReportLink);
                        MessageBox.Show("UI: dps.report link copied to clipbloard");
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        switch (operation.State)
                        {
                            case OperationState.Ready:
                            case OperationState.UnComplete:
                                AddTraceMessage("UI: Run single log parsing");
                                QueueOrRunOperation(operation);
                                BtnCancelAll.Enabled = true;
                                break;

                            case OperationState.Parsing:
                                AddTraceMessage("UI: Cancel single log parsing");
                                operation.ToCancelState();
                                break;

                            case OperationState.Pending:
                                var operations = new HashSet<FormOperationController>(_logQueue);
                                _logQueue.Clear();
                                operations.Remove(operation);
                                foreach (FormOperationController op in operations)
                                {
                                    _logQueue.Enqueue(op);
                                }
                                operation.ToReadyState();
                                break;
                            case OperationState.Queued:
                                operation.ToRemovalFromQueueState();
                                break;

                            case OperationState.Complete:
                                AddTraceMessage("UI: Opening generated files");
                                foreach (string path in operation.OpenableFiles)
                                {
                                    if (File.Exists(path))
                                    {
                                        System.Diagnostics.Process.Start(path);
                                    }
                                }
                                if (operation.OpenableFiles.Count < operation.GeneratedFiles.Count && operation.OutLocation != null && Directory.Exists(operation.OutLocation))
                                {
                                    System.Diagnostics.Process.Start(operation.OutLocation);
                                }
                                break;
                        }
                    }
                    else if (e.Button == MouseButtons.Middle)
                    {
                        switch (operation.State)
                        {

                            case OperationState.Complete:
                                AddTraceMessage("UI: Opening folder where outputs have been generated");
                                if (operation.OutLocation != null && Directory.Exists(operation.OutLocation))
                                {
                                    System.Diagnostics.Process.Start(operation.OutLocation);
                                }
                                break;
                        }
                    }
                    break;
                case 3:
                    if (e.Button == MouseButtons.Left)
                    {
                        switch (operation.State)
                        {
                            case OperationState.Complete:
                                AddTraceMessage("UI: Reparse log");
                                QueueOrRunOperation(operation);
                                BtnCancelAll.Enabled = true;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void DgvFilesCellContentDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            var operation = (FormOperationController)OperatorBindingSource[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 0:
                    if (e.Button == MouseButtons.Left)
                    {
                        if (File.Exists(operation.InputFile))
                        {
                            System.Diagnostics.Process.Start(new FileInfo(operation.InputFile).DirectoryName);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateWatchDirectoryWatcher(object sender, EventArgs e)
        {
            UpdateWatchDirectory();
        }

        private void BtnPopulateFromDirectory(object sender, EventArgs e)
        {
            string path = null;
            AddTraceMessage("UI: Populating from directory");
            using (var fbd = new FolderBrowserDialog())
            {
                if (Directory.Exists(Properties.Settings.Default.AutoAddPath))
                {
                    fbd.SelectedPath = Properties.Settings.Default.AutoAddPath;
                }
                fbd.ShowNewFolderButton = false;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }
            DateTime currentTime = DateTime.Now;
            if (path != null)
            {
                AddTraceMessage("UI: Adding files from " + path);
                var toAdd = new List<string>();
                foreach (string format in ProgramHelper.SupportedFormats)
                {
                    try
                    {
                        if (Properties.Settings.Default.PopulateHourLimit > 0)
                        {
                            var fileList = new DirectoryInfo(path).EnumerateFiles("*" + format, SearchOption.AllDirectories).ToList();
                            var toKeep = fileList.Where(x => (currentTime - x.CreationTime).TotalHours < Properties.Settings.Default.PopulateHourLimit).ToList();
                            toAdd.AddRange(toKeep.Select(x => x.FullName));
                        }
                        else
                        {
                            toAdd.AddRange(Directory.EnumerateFiles(path, "*" + format, SearchOption.AllDirectories));
                        }
                    }
                    catch
                    {
                        // nothing to do
                    }
                }
                AddLogFiles(toAdd);
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
            await Task.Delay(3000).ConfigureAwait(false);
            if (File.Exists(path))
            {
                AddTraceMessage("File Watcher: adding " + path);
                if (DgvFiles.InvokeRequired)
                {
                    DgvFiles.Invoke(new Action(() => AddLogFiles(new string[] { path })));
                }
                else
                {
                    AddLogFiles(new string[] { path });
                }
            }
        }

        private void LogFileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            AddTraceMessage("File Watcher: created " + e.FullPath);
            if (ProgramHelper.IsSupportedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }

        private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            AddTraceMessage("File Watcher: renamed " + e.OldFullPath + " to " + e.FullPath);
            if (ProgramHelper.IsTemporaryCompressedFormat(e.OldFullPath) && ProgramHelper.IsCompressedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
            else if (ProgramHelper.IsTemporaryFormat(e.OldFullPath) && ProgramHelper.IsSupportedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }

        private string DiscordBatch(out List<ulong> ids)
        {
            ids = new List<ulong>();
            AddTraceMessage("Discord: Sending batch to Discord");
            if (_programHelper.Settings.WebhookURL == null)
            {
                AddTraceMessage("Discord: No webhook url given");
                return "Set a discord webhook url in settings first";
            }
            var fullDpsReportLogs = new List<FormOperationController>();
            foreach (FormOperationController operation in OperatorBindingSource)
            {
                if (operation.DPSReportLink != null && operation.DPSReportLink.Contains("https"))
                {
                    fullDpsReportLogs.Add(operation);
                }
            }
            if (fullDpsReportLogs.Count == 0)
            {
                AddTraceMessage("Discord: Nothing to send");
                return "Nothing to send";
            }
            // first sort by time
            AddTraceMessage("Discord: Sorting logs by time");
            fullDpsReportLogs.Sort((x, y) =>
            {
                return DateTime.Parse(x.BasicMetaData.LogStart).CompareTo(DateTime.Parse(y.BasicMetaData.LogStart));
            });
            AddTraceMessage("Discord: Splitting logs by start day");
            var fullDpsReportsLogsByDate = fullDpsReportLogs.GroupBy(x => DateTime.Parse(x.BasicMetaData.LogStart).Date).ToDictionary(x => x.Key, x => x.ToList());
            // split the logs so that a single embed does not reach the discord embed limit and also keep a reasonable size by embed
            string message = "";
            bool start = true;
            foreach (KeyValuePair<DateTime, List<FormOperationController>> pair in fullDpsReportsLogsByDate)
            {
                if (!start)
                {
                    message += "\r\n";
                }
                start = false;
                var splitDpsReportLogs = new List<List<FormOperationController>>() { new List<FormOperationController>() };
                message += pair.Key.ToString("yyyy-MM-dd") + " - ";
                List<FormOperationController> curListToFill = splitDpsReportLogs.First();
                AddTraceMessage("Discord: Splitting message to avoid reaching discord's character limit");
                foreach (FormOperationController controller in pair.Value)
                {
                    if (curListToFill.Count < 40)
                    {
                        curListToFill.Add(controller);
                    }
                    else
                    {
                        curListToFill = new List<FormOperationController>()
                        {
                            controller
                        };
                        splitDpsReportLogs.Add(curListToFill);
                    }
                }
                foreach (List<FormOperationController> dpsReportLogs in splitDpsReportLogs)
                {
                    EmbedBuilder embedBuilder = _programHelper.GetEmbedBuilder();
                    AddTraceMessage("Discord: Creating embed for " + dpsReportLogs.Count + " logs");
                    var first = DateTime.Parse(dpsReportLogs.First().BasicMetaData.LogStart);
                    var last = DateTime.Parse(dpsReportLogs.Last().BasicMetaData.LogEnd);
                    embedBuilder.WithFooter(pair.Key.ToString("dd/MM/yyyy") + " - " + first.ToString("T") + " - " + last.ToString("T"));
                    AddTraceMessage("Discord: Sorting logs by category");
                    dpsReportLogs.Sort((x, y) =>
                    {
                        int categoryCompare = x.BasicMetaData.FightCategory.CompareTo(y.BasicMetaData.FightCategory);
                        if (categoryCompare == 0)
                        {
                            return DateTime.Parse(x.BasicMetaData.LogStart).CompareTo(DateTime.Parse(y.BasicMetaData.LogStart));
                        }
                        return categoryCompare;
                    });
                    string currentSubCategory = "";
                    var embedFieldBuilder = new EmbedFieldBuilder();
                    string fieldValue = "I can not be empty";
                    AddTraceMessage("Discord: Building embed body");
                    foreach (FormOperationController controller in dpsReportLogs)
                    {
                        string subCategory = controller.BasicMetaData.FightCategory.GetSubCategoryName();
                        string toAdd = "[" + controller.BasicMetaData.FightName + "](" + controller.DPSReportLink + ") " + (controller.BasicMetaData.FightSuccess ? " :white_check_mark: " : " :x: ") + ": " + controller.BasicMetaData.FightDuration;
                        if (subCategory != currentSubCategory)
                        {
                            embedFieldBuilder.WithValue(fieldValue);
                            embedFieldBuilder = new EmbedFieldBuilder();
                            fieldValue = "";
                            embedBuilder.AddField(embedFieldBuilder);
                            embedFieldBuilder.WithName(subCategory);
                            currentSubCategory = subCategory;
                        }
                        else if (fieldValue.Length + toAdd.Length > 1024)
                        {
                            embedFieldBuilder.WithValue(fieldValue);
                            embedFieldBuilder = new EmbedFieldBuilder();
                            fieldValue = "";
                            embedBuilder.AddField(embedFieldBuilder);
                            embedFieldBuilder.WithName(subCategory);
                        }
                        else
                        {
                            fieldValue += "\r\n";
                        }
                        fieldValue += toAdd;
                    }
                    embedFieldBuilder.WithValue(fieldValue);
                    AddTraceMessage("Discord: Sending embed");
                    try
                    {
                        ids.Add(WebhookController.SendMessage(_programHelper.Settings.WebhookURL, embedBuilder.Build(), out string curMessage));
                        AddTraceMessage("Discord: embed sent " + curMessage);
                        message += curMessage + " - ";
                    }
                    catch (Exception ex)
                    {
                        AddTraceMessage("Discord: couldn't send embed " + ex.Message);
                        message += ex.Message + " - ";
                    }
                }
            }
            return message;
        }

        private void AutoUpdateDiscordBatch()
        {
            if (!Properties.Settings.Default.AutoDiscordBatch)
            {
                return;
            }
            AddTraceMessage("Discord: Auto update Discord Batch");
            ChkAutoDiscordBatch.Enabled = false;
            foreach (ulong id in _currentDiscordMessageIDs)
            {
                AddTraceMessage("Discord: deleting existing message " + id);
                try
                {
                    WebhookController.DeleteMessage(_programHelper.Settings.WebhookURL, id, out string message);
                    AddTraceMessage("Discord: deleted existing message " + message);
                }
                catch (Exception ex)
                {
                    AddTraceMessage("Discord: couldn't deleted existing message " + ex.Message);
                }
            }
            _currentDiscordMessageIDs.Clear();
            DiscordBatch(out _currentDiscordMessageIDs);
            ChkAutoDiscordBatch.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDiscordBatchClick(object sender, EventArgs e)
        {
            BtnDiscordBatch.Enabled = false;
            ChkAutoDiscordBatch.Enabled = false;
            BtnParse.Enabled = false;
            AddTraceMessage("UI: Manual Discord Batch");
            MessageBox.Show(DiscordBatch(out _));
            //    
            BtnDiscordBatch.Enabled = !_anyRunning;
            ChkAutoDiscordBatch.Enabled = !_anyRunning;
            BtnParse.Enabled = !_anyRunning;
        }

        private void _AddTraceMessage(string message)
        {
            if (!Properties.Settings.Default.ApplicationTraces)
            {
                return;
            }
            if (!Directory.Exists(ProgramHelper.EILogPath))
            {
                Directory.CreateDirectory(ProgramHelper.EILogPath);
            }
            if (!File.Exists(_traceFileName))
            {
                using (StreamWriter sw = File.CreateText(_traceFileName))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(_traceFileName))
                {
                    sw.WriteLine(message);
                }
            }
        }

        private void AddTraceMessage(string message)
        {
            if (DgvFiles.InvokeRequired)
            {
                DgvFiles.Invoke(new Action(() => _AddTraceMessage(message)));
            }
            else
            {
                _AddTraceMessage(message);
            }
        }
        // UI 
        private void UpdateWatchDirectory()
        {
            if (Properties.Settings.Default.AutoAdd && Directory.Exists(Properties.Settings.Default.AutoAddPath))
            {
                LogFileWatcher.Path = Properties.Settings.Default.AutoAddPath;
                LblWatchingDir.Text = "Watching for log files in " + Properties.Settings.Default.AutoAddPath;
                LogFileWatcher.EnableRaisingEvents = true;
                LblWatchingDir.Visible = true;
                AddTraceMessage("Settings: Updated watch directory to " + Properties.Settings.Default.AutoAddPath);
            }
            else
            {
                Properties.Settings.Default.AutoAdd = false;
                LblWatchingDir.Visible = false;
                LogFileWatcher.EnableRaisingEvents = false;
            }
        }
        private void ChkApplicationTracesCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ApplicationTraces = ChkApplicationTraces.Checked;
            AddTraceMessage("Settings: " + (Properties.Settings.Default.ApplicationTraces ? "Enabled traces" : "Disabled traces"));
        }

        private void NumericCustomPopulateLimitValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PopulateHourLimit = (long)NumericCustomPopulateLimit.Value;
            AddTraceMessage("Settings: Updated populate function hour limit to " + Properties.Settings.Default.PopulateHourLimit);
        }
        private void ChkAutoDiscordBatchCheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoDiscordBatch = ChkAutoDiscordBatch.Checked;
            AddTraceMessage("Settings: " + (Properties.Settings.Default.AutoDiscordBatch ? "Enabled automatic discord batching" : "Disabled automatic discord batching"));
        }

        private void EnableSettingsWatcher(object sender, EventArgs e)
        {
            AddTraceMessage("Settings: Closing settings");
            BtnSettings.Enabled = true;
        }
    }
}
