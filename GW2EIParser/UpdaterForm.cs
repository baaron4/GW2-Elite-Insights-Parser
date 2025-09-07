using System.Diagnostics;
using GW2EIUpdater;

namespace GW2EIParser;

public partial class UpdaterForm : Form
{
    private readonly Updater Updater = new();

    public UpdaterForm(Updater updater)
    {
        Updater = updater;
        InitializeComponent();

        // Add versions to grid
        gridVersions.Rows.Add([updater.CurrentVersion, updater.LatestVersion]);
        foreach (DataGridViewColumn column in gridVersions.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        // Add release notes link
        linkLblReleaseNotes.Links.Add(0, linkLblReleaseNotes.Text.Length, updater.ReleasePageURL);
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
    }

    private async void buttonUpdate_Click(object sender, EventArgs e)
    {
        if (Updater.UpdateFound)
        {
            await Updater.DownloadFileAsync();
        }
    }

    private void buttonDismiss_Click(object sender, EventArgs e)
    {
        Close();
    }
}
