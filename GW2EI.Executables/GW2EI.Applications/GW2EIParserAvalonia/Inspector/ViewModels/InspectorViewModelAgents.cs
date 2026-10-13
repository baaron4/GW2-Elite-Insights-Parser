using System;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIParserAvalonia.Models;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    [ObservableProperty]
    private AgentDataModel? selectedAgent;
    partial void OnSelectedAgentChanged(AgentDataModel? value)
    {
        SelectedAgentProperties.ReplaceRange(EventInspector.Inspect(value));
    }
    public DataGridCollectionView AgentsDataView { get; }
    [ObservableProperty]
    private string? agentSpeciesFilter;
    private void RefreshAgentsDataView(string? oldValue, string? newValue)
    {
        if (newValue == oldValue)
        {
            return;
        }
        AgentsDataView.Refresh();
        OnPropertyChanged(nameof(AgentCount));
    }
    partial void OnAgentSpeciesFilterChanged(string? oldValue, string? newValue) => RefreshAgentsDataView(oldValue, newValue);
    [ObservableProperty]
    private string? agentNameFilter;
    partial void OnAgentNameFilterChanged(string? oldValue, string? newValue) => RefreshAgentsDataView(oldValue, newValue);
    [ObservableProperty]
    private string? agentTypeFilter;
    partial void OnAgentTypeFilterChanged(string? oldValue, string? newValue) => RefreshAgentsDataView(oldValue, newValue);
    [ObservableProperty]
    private string? agentSpecFilter;
    partial void OnAgentSpecFilterChanged(string? oldValue, string? newValue) => RefreshAgentsDataView(oldValue, newValue);
    [ObservableProperty]
    private string? agentBaseSpecFilter;
    partial void OnAgentBaseSpecFilterChanged(string? oldValue, string? newValue) => RefreshAgentsDataView(oldValue, newValue);
    private bool FilterAgentModels(object item)
    {
        if (item is not AgentDataModel agent)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentSpeciesFilter) &&
            !agent.ID.ToString().Contains(AgentSpeciesFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentNameFilter) &&
            !agent.Name.Contains(AgentNameFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentTypeFilter) &&
            !agent.Type.ToString().Contains(AgentTypeFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentSpecFilter) &&
            !agent.Spec.ToString().Contains(AgentSpecFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentBaseSpecFilter) &&
            !agent.BaseSpec.ToString().Contains(AgentBaseSpecFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    public int AgentCount => AgentsDataView.Count;
    public BulkObservableCollection<EventPropertyModel> SelectedAgentProperties { get; } = [];


}
