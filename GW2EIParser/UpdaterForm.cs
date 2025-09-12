using System.Diagnostics;
using GW2EIUpdater;

namespace GW2EIParser;

partial class UpdaterForm : Form
{
    private readonly Updater.UpdateInfo _info = new ();

    public event EventHandler UpdateStartedEvent;
    public event EventHandler UpdateTracesEvent;

    public UpdaterForm(Updater.UpdateInfo info)
    {
        _info = info;
        InitializeComponent();

        // Add versions to grid
        gridVersions.Rows.Add([_info.CurrentVersion, _info.LatestVersion]);
        foreach (DataGridViewColumn column in gridVersions.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        // Add release notes link
        linkLblReleaseNotes.Links.Add(0, linkLblReleaseNotes.Text.Length, _info.ReleasePageURL);
        linkLblReleaseNotes.LinkClicked += (sender, e) =>
        {
            string link = e.Link.LinkData as string;
            if (!string.IsNullOrEmpty(link))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
        };

        // Add update size
        lblDwnlSize.Text = lblDwnlSize.Text + " " + _info.DownloadSize;
    }

    private async void buttonUpdate_Click(object sender, EventArgs e)
    {
        if (_info.UpdateAvailable)
        {
            List<string> traces = [];
            if (await Updater.DownloadAndUpdate(_info, "GW2EIUpdateTemp", "GW2EI.zip", traces))
            {
                UpdateTracesEvent(traces, null);
                UpdateStartedEvent(this, null);
            } 
            else
            {
                UpdateTracesEvent(traces, null);
            }
        }
    }

    private void buttonDismiss_Click(object sender, EventArgs e)
    {
        Close();
    }
}
