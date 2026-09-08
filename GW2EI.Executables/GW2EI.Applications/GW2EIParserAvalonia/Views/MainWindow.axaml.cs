using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.ViewModels;
using GW2EIParserCommons.Properties;
using GW2EIUpdater;

namespace GW2EIParserAvalonia.Views;

public sealed partial class MainWindow : Window, IDisposable
{
    private FileSystemWatcher? _logFileWatcher;
    private readonly IApplicationTrace _trace = null!;

    private const string AssetName = "GW2EI.zip";

    public MainWindow()
    {
        InitializeComponent();
        Closing += (sender, e) => Settings.Default.Save();
    }

    public MainWindow(IApplicationTrace trace) : this()
    {
        _trace = trace;

        UpdateFileWatcher();
        UpdaterInitialCheck();
    }

    private async void AddFilesButton_Click(object? sender, RoutedEventArgs e)
    {
        FilePicker();
    }

    private async void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var window = new SettingsWindow(viewModel.SettingsViewModel, _trace);
        window.Show();
        window.Closed += (sender, e) => UpdateFileWatcher();
    }

    private async void PopulateButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select log directory",
                AllowMultiple = false
            });

        var folder = folders.FirstOrDefault();

        if (folder is null)
        {
            return;
        }

        var path = folder.TryGetLocalPath();

        if (!string.IsNullOrWhiteSpace(path))
        {
            viewModel.AddFilesFromDirectory(path);
        }
    }

    private void ParseAllButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.ParseAll();
        _trace.Add("UI: Parse all files");
    }

    private void CancelAllButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.CancelAll();
        _trace.Add("UI: Cancelling all pending and ongoing parsing operations");
    }

    private void ClearAllButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.ClearAll();
        _trace.Add("UI: Clearing all logs");
    }

    private void ClearUncompletedButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.ClearUncompleted();
        _trace.Add("UI: Clearing uncompleted logs (failed to parse)");
    }

    private void UpdateFileWatcher()
    {
        _logFileWatcher?.Dispose();

        if (!Settings.Default.AutoAdd || !Directory.Exists(Settings.Default.AutoAddPath))
        {
            return;
        }

        _logFileWatcher = new FileSystemWatcher(Settings.Default.AutoAddPath)
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };

        _logFileWatcher.Created += LogFileWatcher_Created;
        _logFileWatcher.Renamed += LogFileWatcher_Renamed;
    }

    private void LogFileWatcher_Created(object sender, FileSystemEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.HandleCreatedFile(e.FullPath);
            }
        });
    }

    private void LogFileWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.HandleRenamedFile(e.OldFullPath, e.FullPath);
            }
        });
    }

    private void MainWindow_DragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer.Formats.Contains(DataFormat.File))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void MainWindow_Drop(object? sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var files = e.DataTransfer.TryGetFiles();

        if (files is null)
        {
            return;
        }

        foreach (var file in files)
        {
            var path = file.TryGetLocalPath();

            if (path is not null)
            {
                viewModel.AddFile(path);
            }
        }
    }

    private async void LogFilesGrid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        var row = (e.Source as Visual)?.FindAncestorOfType<DataGridRow>();
        var header = (e.Source as Visual)?.FindAncestorOfType<DataGridColumnHeader>();
        if (header != null || row != null)
        {
            return;
        }

        FilePicker();
    }

    public async void FilePicker()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select GW2 EVTC Combat Logs",
                AllowMultiple = true,
                FileTypeFilter =
                [
                    new FilePickerFileType("GW2 EVTC Combat Logs")
                    {
                        Patterns = SupportedFileFormats.SupportedFormats.Select(format => $"*{format}").ToList()
                    }
                ]
            });

        foreach (var file in files)
        {
            var path = file.TryGetLocalPath();
            if (path != null)
            {
                viewModel.AddFile(path);
            }
        }
    }

    private void UpdaterInitialCheck()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

#if DEBUG
        long time = 0;
#else 
        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#endif
        if (time - Settings.Default.UpdateLastChecked > 3600)
        {
            Settings.Default.UpdateLastChecked = time;
            Task.Factory.StartNew(async () =>
            {
                List<string> traces = [];
                Updater.UpdateInfo? info = await Updater.CheckForUpdate(AssetName, traces);
                if (info != null)
                {
                    Settings.Default.UpdateAvailable = info.Value.UpdateAvailable;
                    viewModel.UpdateVersionLabel(info.Value.UpdateAvailable);
                }
                traces.ForEach(x => _trace.Add("Updater: " + x));
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
        viewModel.UpdateVersionLabel(Settings.Default.UpdateAvailable);
    }

    private async void CheckUpdatesButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        _trace.Add("Updater: Checking for updates");

        var traces = new List<string>();
        Updater.UpdateInfo? info = await Updater.CheckForUpdate(AssetName, traces);
        traces.ForEach(x => _trace.Add("Updater: " + x));
#if DEBUG
        var force = true;
#else
        var force = false;
#endif
        if (info is null)
        {
            _trace.Add("Updater: UpdateInfo is null");
            return;
        }

        if (info.Value.UpdateAvailable || force)
        {
            _trace.Add("Updater: Update found, opening UI");
            Settings.Default.UpdateAvailable = info.Value.UpdateAvailable;
            viewModel.UpdateVersionLabel(info.Value.UpdateAvailable);

            var updaterWindow = new UpdaterWindow(info.Value, _trace);
            updaterWindow.UpdateStarted += (_, _) =>
            {
                updaterWindow.Close();
            };

            await updaterWindow.ShowDialog(this);
        }
        else
        {
            _trace.Add("Updater: Up to date");
            Settings.Default.UpdateAvailable = false;
            viewModel.UpdateVersionLabel(false);

            var messageWindow = new MessageWindow("Elite Insights is up to date.", _trace);

            await messageWindow.ShowDialog(this);
        }
    }

    private async void SendAllToDiscordButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var message = await viewModel.SendAllToDiscordAsync();
        var messageWindow = new MessageWindow(message, _trace);
        await messageWindow.ShowDialog(this);
    }

    public void Dispose()
    {
        _logFileWatcher?.Dispose();
    }
}
