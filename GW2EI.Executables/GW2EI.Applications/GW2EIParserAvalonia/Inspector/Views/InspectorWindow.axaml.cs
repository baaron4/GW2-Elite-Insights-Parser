using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using GW2EIEvtcParser;
using GW2EIParserAvalonia.Models;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.ViewModels;

namespace GW2EIParserAvalonia;

public partial class InspectorWindow : Window
{
    private readonly IApplicationTrace _trace = null!;

    public InspectorWindow()
    {
        InitializeComponent();
    }

    public InspectorWindow(RawEvtcLog log, IApplicationTrace trace)
    {
        _trace = trace;

        InitializeComponent();

        DataContext = new InspectorViewModel(log);
    }

    private async void CopyGuid_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.CommandParameter is not ContentGUIDModel model)
        {
            return;
        }

        var clipboard = GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(model.GUID.ToString());
    }

    private async void CopyContentId_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.CommandParameter is not ContentGUIDModel model)
        {
            return;
        }

        var clipboard = GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(model.ContentID.ToString());
    }
}
