using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GW2EIParserAvalonia.Services;
using GW2EIParserCommons.Properties;
using GW2EIUpdater;

namespace GW2EIParserAvalonia.Views;

public partial class UpdaterWindow : Window
{
    private readonly Updater.UpdateInfo _info;
    private readonly IApplicationTrace _trace = null!;

    public event EventHandler? UpdateStarted;

    public UpdaterWindow()
    {
        InitializeComponent();
    }

    public UpdaterWindow(Updater.UpdateInfo info, IApplicationTrace trace)
    {
        InitializeComponent();

        _info = info;
        _trace = trace;

        CurrentVersionText.Text = info.CurrentVersion;
        LatestVersionText.Text = info.LatestVersion;
        DownloadSizeText.Text = $"Download Size: {info.DownloadSize}";
    }

    private async void UpdateButton_Click(object? sender, RoutedEventArgs e)
    {
        var traces = new List<string>();

        if (await Updater.DownloadAndUpdate(_info, traces))
        {
            Settings.Default.UpdateAvailable = false;

            foreach (var trace in traces)
            {
                _trace.Add("Updater: " + trace);
            }

            UpdateStarted?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            foreach (var trace in traces)
            {
                _trace.Add("Updater: " + trace);
            }

            var messageWindow = new MessageWindow("Update Failed. Please update manually.", _trace);

            await messageWindow.ShowDialog(this);
        }
    }

    private void DismissButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ReleaseNotesButton_Click(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = _info.ReleasePageURL,
            UseShellExecute = true
        });
    }
}
