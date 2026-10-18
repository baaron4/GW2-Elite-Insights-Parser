using System.Collections.Generic;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GW2EIParserAvalonia.ViewModels;

internal partial class InspectorViewModel : ObservableObject
{
    public DataGridCollectionView GadgetAnimationView { get; }
    public IReadOnlyList<string> GadgetAnimationFilterItems { get; }
    [ObservableProperty]
    public partial string? GadgetDataFilter { get; set; }

    partial void OnGadgetDataFilterChanged(string? oldValue, string? newValue)
    {
        if (DropdownFilterTriggered)
        {
            return;
        }
        if (oldValue == newValue)
        {
            return;
        }
        CombatEventsView.Refresh();
        UpdateEventTypeCounts();
    }

}
