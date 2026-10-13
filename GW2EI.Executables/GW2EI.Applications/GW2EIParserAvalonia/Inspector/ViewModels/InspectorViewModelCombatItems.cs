using System;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    public DataGridCollectionView CombatItemsView { get; }
    public int CombatItemCount => CombatItemsView.Count;

    [ObservableProperty]
    private string? stateChangeFilter;
    partial void OnStateChangeFilterChanged(string? oldValue, string? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }
        CombatItemsView.Refresh();
    }

    private bool FilterCombatItems(object item)
    {
        if (item is not CombatItemModel combatItem)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(StateChangeFilter) &&
            !combatItem.IsStateChange.ToString().Contains(StateChangeFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

}
