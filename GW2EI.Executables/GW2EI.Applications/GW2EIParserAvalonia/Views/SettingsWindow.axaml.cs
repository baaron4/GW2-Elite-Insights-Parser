using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.ViewModels;
using GW2EIParserCommons;
using GW2EIParserCommons.Properties;

namespace GW2EIParserAvalonia.Views;

public partial class SettingsWindow : Window
{
    private readonly IApplicationTrace _trace = null!;

    public SettingsWindow()
    {
        InitializeComponent();
        Closing += (sender, e) =>
        {
            Settings.Default.Save();
        };
    }

    public SettingsWindow(SettingsViewModel viewModel, IApplicationTrace trace) : this()
    {

        DataContext = viewModel;
        _trace = trace;
        viewModel.AutoAddFolderRequested += ViewModel_AutoAddFolderRequested;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        _trace.Add("UI: Settings closed");
        Close();
    }

    private async void LoadSettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel viewModel)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Load a Configuration file",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Conf file")
                    {
                        Patterns = [ "*.conf" ]
                    }
                }
            });

        if (files.Count > 0)
        {
            viewModel.ApplyLoadedSettings(files[0].Path.LocalPath);
            _trace.Add($"UI: Settings loaded from {files[0].Path.LocalPath}");
        }
    }
    private async void SaveSettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel)
        {
            return;
        }
        var file = await StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Save a Configuration file",
                SuggestedFileName = "config.conf",
                DefaultExtension = "conf",
                ShowOverwritePrompt = true,
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Conf file")
                    {
                        Patterns = [ "*.conf" ]
                    }
                }
            });

        if (file != null)
        {
            string dump = CustomSettingsManager.DumpSettings();
            await using Stream stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            byte[] settings = new UTF8Encoding(true).GetBytes(dump);
            stream.Write(settings, 0, settings.Length);
            _trace.Add($"UI: Settings saved to {file.Path.LocalPath}");
        }
    }

    private async void ViewModel_AutoAddFolderRequested(object? sender, EventArgs e)
    {
        if (DataContext is not SettingsViewModel viewModel)
        {
            return;
        }

        var startLocation = await StorageProvider.TryGetFolderFromPathAsync(viewModel.AutoAddPath);

        var folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select watch directory",
                SuggestedStartLocation = startLocation,
                AllowMultiple = false
            });
        var folder = folders.FirstOrDefault();

        if (folder == null || string.IsNullOrWhiteSpace(folder.Path.LocalPath))
        {
            viewModel.AutoAddPath = string.Empty;
            viewModel.AutoAdd = false;
            return;
        }
        viewModel.AutoAddPath = folder.Path.LocalPath;
        viewModel.AutoAdd = true;
    }

    private async void SelectOutLocationFolder_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel viewModel)
        {
            return;
        }

        var startLocation = await StorageProvider.TryGetFolderFromPathAsync(viewModel.OutLocation);

        var folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select output directory",
                SuggestedStartLocation = startLocation,
                AllowMultiple = false
            });

        var folder = folders.FirstOrDefault();

        if (folder != null)
        {
            viewModel.OutLocation = folder.Path.LocalPath;
        }
    }
    private async void SelectExternalScriptFolder_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel viewModel)
        {
            return;
        }

        var startLocation = await StorageProvider.TryGetFolderFromPathAsync(viewModel.HtmlExternalScriptsPath);

        var folders = await StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select external asset directory",
                SuggestedStartLocation = startLocation,
                AllowMultiple = false
            });

        var folder = folders.FirstOrDefault();

        if (folder != null)
        {
            viewModel.HtmlExternalScriptsPath = folder.Path.LocalPath;
        }
    }

    private async void ResetMapButton_Click(object sender, RoutedEventArgs e)
    {
        ProgramHelper.APIController.WriteAPIMapsToFile(ProgramHelper.MapAPICacheLocation);
        var messageWindow = new MessageWindow("Map List has been redone", _trace);
        await messageWindow.ShowDialog(this);
    }

    private async void ResetSkillButton_Click(object sender, RoutedEventArgs e)
    {
        ProgramHelper.APIController.WriteAPISkillsToFile(ProgramHelper.SkillAPICacheLocation);
        var messageWindow = new MessageWindow("Skill List has been redone", _trace);
        await messageWindow.ShowDialog(this);
    }

    private async void ResetTraitButton_Click(object sender, RoutedEventArgs e)
    {
        ProgramHelper.APIController.WriteAPITraitsToFile(ProgramHelper.TraitAPICacheLocation);
        var messageWindow = new MessageWindow("Trait List has been redone", _trace);
        await messageWindow.ShowDialog(this);
    }

    private async void ResetSpecButton_Click(object sender, RoutedEventArgs e)
    {
        ProgramHelper.APIController.WriteAPISpecsToFile(ProgramHelper.SpecAPICacheLocation);
        var messageWindow = new MessageWindow("Spec List has been redone", _trace);
        await messageWindow.ShowDialog(this);
    }
}
