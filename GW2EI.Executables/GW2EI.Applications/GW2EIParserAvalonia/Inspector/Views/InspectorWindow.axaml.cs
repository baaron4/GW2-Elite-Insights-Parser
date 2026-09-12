using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

        CombatEventsTreeView.AddHandler(
            PointerPressedEvent,
            OnEventFilterNodePressed,
            RoutingStrategies.Tunnel
         );
    }

    public InspectorWindow(RawEvtcLog log, IApplicationTrace trace) : this()
    {
        _trace = trace;

        DataContext = new InspectorViewModel(log);
    }

    private void OnEventFilterNodePressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not InspectorViewModel inspectorViewModel)
        {
            return;
        }
        if (sender is TreeView treeView && e.Source is Avalonia.Visual visual && e.GetCurrentPoint(treeView).Properties.IsRightButtonPressed)
        {
            var treeViewItem = visual.FindAncestorOfType<TreeViewItem>();
            if (treeViewItem?.DataContext is EventTypeFilterNodeModel selectedItem)
            {
                var isChecked = selectedItem.IsChecked ?? false;
                inspectorViewModel.SetCheckStateOnAllRoots(isChecked);
                selectedItem.IsChecked = !isChecked;
            }
        }
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

    private async void AgentFilter_GotFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is AutoCompleteBox autoCompleteBox)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                autoCompleteBox.IsDropDownOpen = true;
            });
        }
    }
}
