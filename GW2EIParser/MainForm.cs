using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW2EIParser.Exceptions;
using GW2EIParser.Setting;

namespace GW2EIParser
{
    public partial class MainForm : Form
    {
        private readonly SettingsForm _settingsForm;
        private readonly List<string> _logsFiles;
        private int _runningCount = 0;
        private bool _anyRunning => _runningCount > 0;
        private readonly Queue<FormOperationController> _logQueue = new Queue<FormOperationController>();
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

                var operation = new FormOperationController(file, "Ready to parse", dgvFiles);
                operatorBindingSource.Add(operation);

                if (Properties.Settings.Default.AutoParse)
                {
                    QueueOrRunOperation(operation);
                }
            }

            btnParse.Enabled = !Properties.Settings.Default.AutoParse;
            btnCancel.Enabled = Properties.Settings.Default.AutoParse;
        }

        private void EnableSettingsWatcher(object sender, EventArgs e)
        {
            btnSettings.Enabled = true;
        }

        private void _RunOperation(FormOperationController operation)
        {
            _runningCount++;
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
                            operation.UpdateProgress("Error Encountered: Something terrible has happened");
                        }
                        else
                        {
                            Exception ex = t.Exception.InnerExceptions[0];
                            if (!(ex is ExceptionEncompass))
                            {
                                operation.UpdateProgress("Error Encountered: Something terrible has happened");
                            }
                            if (!(ex.InnerException is OperationCanceledException))
                            {
                                operation.UpdateProgress(ex.GetFinalException().Message);
                            } 
                            else
                            {
                                operation.UpdateProgress("Operation Aborted");
                            }
                        }
                    }
                    else
                    {
                        operation.UpdateProgress("Error Encountered: Something terrible has happened");
                    }
                }
                if (operation.State == OperationState.ClearOnCancel)
                {
                    operatorBindingSource.Remove(operation);
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
                        operation.UpdateProgress("Error Encountered: Something terrible has happened");
                        operation.ToUnCompleteState();
                    }
                }
                ProgramHelper.GenerateLogFile(operation);
                RunNextOperation();
            }, TaskScheduler.FromCurrentSynchronizationContext());
            operation.SetContext(cancelTokenSource, task);
        }

        /// <summary>
        /// Queues an operation. If the 'MultipleLogs' setting is true, operations are run asynchronously
        /// </summary>
        /// <param name="operation"></param>
        private void QueueOrRunOperation(FormOperationController operation)
        {
            btnClear.Enabled = false;
            btnParse.Enabled = false;
            btnCancel.Enabled = true;
            if (Properties.Settings.Default.ParseMultipleLogs)
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
        private void RunNextOperation()
        {
            if (_logQueue.Count > 0 && _runningCount == 0)
            {
                _RunOperation(_logQueue.Dequeue());
            }
            else
            {
                if (_runningCount == 0)
                {
                    btnParse.Enabled = true;
                    btnClear.Enabled = true;
                    btnCancel.Enabled = false;
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
                btnParse.Enabled = false;
                btnCancel.Enabled = true;

                foreach (FormOperationController operation in operatorBindingSource)
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
        private void BtnCancelClick(object sender, EventArgs e)
        {
            //Clear queue so queued workers don't get started by any cancellations
            var operations = new HashSet<FormOperationController>(_logQueue);
            _logQueue.Clear();

            //Cancel all workers
            foreach (FormOperationController operation in operatorBindingSource)
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

            for (int i = operatorBindingSource.Count - 1; i >= 0; i--)
            {
                var operation = operatorBindingSource[i] as FormOperationController;
                if (operation.IsBusy())
                {
                    operation.ToCancelAndClearState();
                }
                else
                {
                    operatorBindingSource.RemoveAt(i);
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
                var operation = (FormOperationController)operatorBindingSource[e.RowIndex];

                switch (operation.State)
                {
                    case OperationState.Ready:
                        QueueOrRunOperation(operation);
                        btnCancel.Enabled = true;
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
                        string[] paths = operation.LogLocation.Split(',');
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
            if (Properties.Settings.Default.AutoAdd && Directory.Exists(Properties.Settings.Default.AutoAddPath))
            {
                logFileWatcher.Path = Properties.Settings.Default.AutoAddPath;
                labWatchingDir.Text = "Watching for log files in " + Properties.Settings.Default.AutoAddPath;
                logFileWatcher.EnableRaisingEvents = true;
                labWatchingDir.Visible = true;
            }
            else
            {
                Properties.Settings.Default.AutoAdd = false;
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
            await Task.Delay(3000).ConfigureAwait(false);
            if (File.Exists(path))
            {
                if (dgvFiles.InvokeRequired)
                {
                    dgvFiles.Invoke(new Action(() => AddLogFiles(new string[] { path })));
                }
                else
                {
                    AddLogFiles(new string[] { path });
                }
            }
        }

        private void LogFileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (ProgramHelper.IsSupportedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }

        private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (ProgramHelper.IsTemporaryFormat(e.OldFullPath) && ProgramHelper.IsCompressedFormat(e.FullPath))
            {
                AddDelayed(e.FullPath);
            }
        }
    }
}
