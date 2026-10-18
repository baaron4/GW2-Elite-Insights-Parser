using System;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIParserAvalonia.Models;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.ViewModels;

internal partial class InspectorViewModel : ObservableObject
{
    [ObservableProperty]
    public partial SkillDataModel? SelectedSkill { get; set; }

    partial void OnSelectedSkillChanged(SkillDataModel? value)
    {
        if (value?.SkillItem == null)
        {
            SelectedSkillProperties.ReplaceRange([]);
            return;
        }

        SelectedSkillProperties.ReplaceRange(EventInspector.Inspect(value.SkillItem));
    }
    [ObservableProperty]
    public partial string? SkillDataNameFilter { get; set; }

    partial void OnSkillDataNameFilterChanged(string? oldValue, string? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }
        SkillsDataView.Refresh();
    }
    public DataGridCollectionView SkillsDataView { get; }
    public int SkillCount => SkillsDataView.Count;
    public BulkObservableCollection<EventPropertyModel> SelectedSkillProperties { get; } = [];
    private bool FilterSkillDataModels(object item)
    {
        if (item is not SkillDataModel skillData)
        {
            return false;
        }
        if (!string.IsNullOrWhiteSpace(SkillDataNameFilter) &&
            skillData.Name?.Contains(SkillDataNameFilter, StringComparison.OrdinalIgnoreCase) != true)
        {
            return false;
        }

        return true;
    }

}
