using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private int _runningCount;
        private bool _anyRunning;
        private readonly Queue<Operation> _logQueue = new Queue<Operation>();
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

                var goperation = new FormOperation(file, "Ready to parse");
                goperation.BgWorker.DoWork += BgWorkerDoWork;
                goperation.BgWorker.ProgressChanged += BgWorkerProgressChanged;
                goperation.BgWorker.RunWorkerCompleted += BgWorkerCompleted;

                operatorBindingSource.Add(goperation);

                if (Properties.Settings.Default.AutoParse)
                {
                    QueueOrRunWorker(goperation);
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
        /// <param name="operation"></param>
        private void QueueOrRunWorker(Operation operation)
        {
            btnClear.Enabled = false;
            btnParse.Enabled = false;
            btnCancel.Enabled = true;
            if (_anyRunning)
            {
                _logQueue.Enqueue(operation);
                operation.Status = "Queued";
                operation.State = OperationState.Pending;
                dgvFiles.Invalidate();
            }
            else
            {
                _anyRunning = true;
                operation.Run();
            }
        }

        /// <summary>
        /// Runs the next background worker, if one is available
        /// </summary>
        private void RunNextWorker()
        {
            if (_logQueue.Count > 0)
            {
                Operation operation = _logQueue.Dequeue();
                _anyRunning = true;
                operation.Run();
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
            var operationData = e.Argument as Operation;
            e.Result = operationData;
            _runningCount++;
            ProgramHelper.DoWork(operationData);

        }

        /// <summary>
        /// Invoked when a BackgroundWorker reports a change in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Redraw operations
            dgvFiles.Invalidate();
        }

        /// <summary>
        /// Invoked when a BackgroundWorker completes its task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Operation operation;
            _runningCount--;
            if (e.Cancelled || e.Error != null)
            {
                if (e.Error is CancellationException)
                {
                    operation = ((CancellationException)e.Error).Operation;
                    if (e.Error.InnerException != null)
                    {
                        operation.Status = e.Error.InnerException.Message;
                        Console.WriteLine(operation.Status);
                    }

                    if (operation.State == OperationState.ClearOnComplete)
                    {
                        operatorBindingSource.Remove(operation);
                    }
                    else
                    {
                        operation.State = OperationState.Ready;
                        operation.ButtonText = "Parse";
                    }
                }
                else
                {
                    Console.WriteLine("Something terrible has happened");
                }
            }
            else
            {
                operation = (Operation)e.Result;
                if (operation.State == OperationState.ClearOnComplete)
                {
                    operatorBindingSource.Remove(operation);
                }
                else
                {
                    operation.ButtonText = "Open";
                    operation.State = OperationState.Complete;
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

                foreach (Operation operation in operatorBindingSource)
                {
                    if (!operation.IsBusy())
                    {
                        QueueOrRunWorker(operation);
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
            var operations = new HashSet<Operation>(_logQueue);
            _logQueue.Clear();

            //Cancel all workers
            foreach (Operation operation in operatorBindingSource)
            {
                if (operation.State == OperationState.Pending)
                {
                    operation.State = OperationState.Ready;
                }

                if (operation.IsBusy())
                {
                    operation.Cancel();
                }
                else if (operations.Contains(operation))
                {
                    operation.Status = "Ready to parse";
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

            for (int i = operatorBindingSource.Count - 1; i >= 0; i--)
            {
                var operation = operatorBindingSource[i] as Operation;
                if (operation.IsBusy())
                {
                    operation.Cancel();
                    operation.State = OperationState.ClearOnComplete;
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
                var operation = (Operation)operatorBindingSource[e.RowIndex];

                switch (operation.State)
                {
                    case OperationState.Ready:
                        QueueOrRunWorker(operation);
                        btnCancel.Enabled = true;
                        break;

                    case OperationState.Parsing:
                        operation.Cancel();
                        dgvFiles.Invalidate();
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
