using System.Collections.Generic;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    public DataGridCollectionView GadgetAnimationView { get; }
    public IReadOnlyList<string> GadgetAnimationFilterItems { get; }
    [ObservableProperty]
    private string? gadgetDataFilter;
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
