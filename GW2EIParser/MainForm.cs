using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using GW2EIDiscord;
using GW2EIEvtcParser;
using GW2EIParser.Exceptions;
using GW2EIParser.Setting;

namespace GW2EIParser
{
    internal partial class MainForm : Form
    {
        private readonly SettingsForm _settingsForm;
        private readonly List<string> _logsFiles;
        private int _runningCount = 0;
        private bool _anyRunning => _runningCount > 0;
        private readonly Queue<FormOperationController> _logQueue = new Queue<FormOperationController>();

        private int _fileNameSorting = 0;
        private MainForm()
        {
            InitializeComponent();
            //display version
            string version = Application.ProductVersion;
            LblVersion.Text = version;
            _logsFiles = new List<string>();
            BtnCancelAll.Enabled = false;
            BtnParse.Enabled = false;
            UpdateWatchDirectory();
            _settingsForm = new SettingsForm();
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

                var operation = new FormOperationController(file, "Ready to parse", DgvFiles);
                OperatorBindingSource.Add(operation);

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

        private void EnableSettingsWatcher(object sender, EventArgs e)
        {
            BtnSettings.Enabled = true;
        }

        private void _RunOperation(FormOperationController operation)
        {
            _runningCount++;
            _settingsForm.ConditionalSettingDisable(_anyRunning);
            operation.ToQueuedState();
            var cancelTokenSource = new CancellationTokenSource();// Prepare task
            Task task = Task.Run(() =>
            {
                operation.ToRunState();
                ProgramHelper.DoWork(operation);
            }, cancelTokenSource.Token).ContinueWith(t =>
            {
                cancelTokenSource.Dispose();
                _runningCount--;
                // Exception management
                if (t.IsFaulted)
                {
                    if (t.Exception != null)
                    {
                        if (t.Exception.InnerExceptions.Count > 1)
                        {
                            operation.UpdateProgress("Something terrible has happened");
                        }
                        else
                        {
                            Exception ex = t.Exception.InnerExceptions[0];
                            if (!(ex is ProgramException))
                            {
                                operation.UpdateProgress("Something terrible has happened");
                            }
                            if (!(ex.InnerException is OperationCanceledException))
                            {
                                operation.UpdateProgress(ex.InnerException.Message);
                            }
                            else
                            {
                                operation.UpdateProgress("Operation Aborted");
                            }
                        }
                    }
                    else
                    {
                        operation.UpdateProgress("Something terrible has happened");
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
                        operation.UpdateProgress("Operation Aborted");
                        operation.ToUnCompleteState();
                    }
                    else if (t.IsCompleted)
                    {
                        operation.ToCompleteState();
                    }
                    else
                    {
                        operation.UpdateProgress("Something terrible has happened");
                        operation.ToUnCompleteState();
                    }
                }
                ProgramHelper.GenerateTraceFile(operation);
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
            if (Properties.Settings.Default.ParseMultipleLogs && _runningCount < ProgramHelper.GetMaxParallelRunning())
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
            if (_logQueue.Count > 0 && (Properties.Settings.Default.ParseMultipleLogs || !_anyRunning))
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
                    _settingsForm.ConditionalSettingDisable(_anyRunning);
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
        }

        /// <summary>
        /// Invoked when the 'Settings' button is clicked. Opens the settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettingsClick(object sender, EventArgs e)
        {
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
                return _fileNameSorting * string.Compare(left, right);
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
                        MessageBox.Show("dps.report link copied to clipbloard");
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        switch (operation.State)
                        {
                            case OperationState.Ready:
                            case OperationState.UnComplete:
                                QueueOrRunOperation(operation);
                                BtnCancelAll.Enabled = true;
                                break;

                            case OperationState.Parsing:
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
                                foreach (string path in operation.OpenableFiles)
                                {
                                    if (File.Exists(path))
                                    {
                                        System.Diagnostics.Process.Start(path);
                                    }
                                }
                                if (operation.OutLocation != null && Directory.Exists(operation.OutLocation))
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
                                if (operation.OutLocation != null && Directory.Exists(operation.OutLocation))
                                {
                                    System.Diagnostics.Process.Start(operation.OutLocation);
                                }
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
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = false;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }
            if (path != null)
            {
                var toAdd = new List<string>();
                foreach (string format in ParserHelper.GetSupportedFormats())
                {
                    try
                    {
                        toAdd.AddRange(Directory.EnumerateFiles(path, "*" + format, SearchOption.AllDirectories));
                    }
                    catch
                    {
                        // nothing to do
                    }
                }
                AddLogFiles(toAdd);
            }
        }

        private void UpdateWatchDirectory()
        {
            if (Properties.Settings.Default.AutoAdd && Directory.Exists(Properties.Settings.Default.AutoAddPath))
            {
                LogFileWatcher.Path = Properties.Settings.Default.AutoAddPath;
                LblWatchingDir.Text = "Watching for log files in " + Properties.Settings.Default.AutoAddPath;
                LogFileWatcher.EnableRaisingEvents = true;
                LblWatchingDir.Visible = true;
            }
            else
            {
                Properties.Settings.Default.AutoAdd = false;
                LblWatchingDir.Visible = false;
                LogFileWatcher.EnableRaisingEvents = false;
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
            if (ParserHelper.IsSupportedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }

        private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (ParserHelper.IsTemporaryFormat(e.OldFullPath) && ParserHelper.IsCompressedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDiscordBatchClick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WebhookURL == null)
            {
                MessageBox.Show("Set a discord webhook url in settings first");
                return;
            }
            var fullDpsReportLogs = new List<FormOperationController>();         
            foreach (FormOperationController operation in OperatorBindingSource)
            {
                if (operation.DPSReportLink != null && operation.DPSReportLink.Contains("https"))
                {
                    fullDpsReportLogs.Add(operation);
                }
            }
            if (!fullDpsReportLogs.Any())
            {
                MessageBox.Show("Nothing to send");
                return;
            }
            BtnDiscordBatch.Enabled = false;
            BtnParse.Enabled = false;
            // first sort by time
            fullDpsReportLogs.Sort((x, y) =>
            {
                return DateTime.Parse(x.BasicMetaData.LogStart).CompareTo(DateTime.Parse(y.BasicMetaData.LogStart));
            });
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
                    EmbedBuilder embedBuilder = ProgramHelper.GetEmbedBuilder();
                    embedBuilder.WithCurrentTimestamp();
                    embedBuilder.WithFooter(pair.Key.ToString("yyyy-MM-dd"));
                    dpsReportLogs.Sort((x, y) =>
                    {
                        var categoryCompare = x.BasicMetaData.FightCategory.CompareTo(y.BasicMetaData.FightCategory);
                        if (categoryCompare == 0)
                        {
                            return DateTime.Parse(x.BasicMetaData.LogStart).CompareTo(DateTime.Parse(y.BasicMetaData.LogStart));
                        }
                        return categoryCompare;
                    });
                    string currentSubCategory = "";
                    var embedFieldBuilder = new EmbedFieldBuilder();
                    var fieldValue = "I can not be empty";
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
                        }
                        else
                        {
                            fieldValue += "\r\n";
                        }
                        fieldValue += toAdd;
                    }
                    embedFieldBuilder.WithValue(fieldValue);
                    message += new WebhookController(Properties.Settings.Default.WebhookURL, embedBuilder.Build()).SendMessage() + " - ";
                }
            }
           
            MessageBox.Show(message);
            //    
            BtnDiscordBatch.Enabled = !_anyRunning;
            BtnParse.Enabled = !_anyRunning;
        }
    }
}
