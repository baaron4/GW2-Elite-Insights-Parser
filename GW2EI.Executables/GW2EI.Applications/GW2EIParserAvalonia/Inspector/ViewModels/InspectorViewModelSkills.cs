using System;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIParserAvalonia.Models;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    [ObservableProperty]
    private SkillDataModel? selectedSkill;
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
    private string? skillDataNameFilter;
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
