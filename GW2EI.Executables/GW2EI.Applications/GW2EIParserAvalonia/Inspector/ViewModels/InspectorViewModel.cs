using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.Models;
using GW2EIEvtcParser.Extensions;
using Avalonia.Collections;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    #region COMBAT EVENTS
    [ObservableProperty]
    private EventModel? selectedEvent;
    partial void OnSelectedEventChanged(EventModel? value)
    {
        if (value?.Event == null)
        {
            SelectedEventProperties.ReplaceRange([]);
            return;
        }

        SelectedEventProperties.ReplaceRange(EventInspector.Inspect(value.Event));
    }

    private void CombatEventsViewRefresh(object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }
        CombatEventsView.Refresh();
    }

    [ObservableProperty]
    private string? skillIdFilter;
    partial void OnSkillIdFilterChanged(string? oldValue, string? newValue) => CombatEventsViewRefresh(oldValue, newValue);
    [ObservableProperty]
    private string? skillNameFilter;
    partial void OnSkillNameFilterChanged(string? oldValue, string? newValue) => CombatEventsViewRefresh(oldValue, newValue);
    [ObservableProperty]
    private string? guidIdFilter;
    partial void OnGuidIdFilterChanged(string? oldValue, string? newValue) => CombatEventsViewRefresh(oldValue, newValue);
    [ObservableProperty]
    private string? guidFilter;
    partial void OnGuidFilterChanged(string? oldValue, string? newValue) => CombatEventsViewRefresh(oldValue, newValue);
    // Events tab filters
    [ObservableProperty]
    private string? agentSearchText;

    public IReadOnlyList<AgentFilterItem> AgentFilterItems { get; }
    [ObservableProperty]
    private AgentFilterItem? selectedAgentFilter;

    partial void OnSelectedAgentFilterChanged(AgentFilterItem? oldValue, AgentFilterItem? newValue) => CombatEventsViewRefresh(oldValue, newValue);

    public DataGridCollectionView CombatEventsView { get; }

    private bool MassFilterChanging = false;
    private void OnFilterChanged(object? sender, EventArgs e)
    {
        if (MassFilterChanging)
        {
            return;
        }
        CombatEventsView.Refresh();
    }
    private bool FilterCombatEvents(object item)
    {
        if (item is not EventModel eventModel)
        {
            return false;
        }
        if (!IsEventTypeVisible(eventModel.Event.GetType()))
        {
            return false;
        }
        if (!string.IsNullOrWhiteSpace(SkillIdFilter) && 
            long.TryParse(SkillIdFilter, out var parsedSkillId) &&
            eventModel.SkillId != parsedSkillId)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(SkillNameFilter) && 
            eventModel.SkillName?.Contains(SkillNameFilter!, StringComparison.OrdinalIgnoreCase) != true)
        {
            return false;
        }

        if (ContentIDFilter(eventModel.ContentID, GuidIdFilter))
        {
            return false;
        }

        if (GUIDFilter(eventModel.GUIDStruct, eventModel.GUID, GuidFilter))
        {
            return false;
        }

        if (SelectedAgentFilter?.Agent is not null && !
            eventModel.AgentIds.Contains(SelectedAgentFilter.Agent))
        {
            return false;
        }
        return true;
    }

    public IReadOnlyList<EventTypeFilterNodeModel> EventTypeFilterRoots { get; }
    private bool IsEventTypeVisible(Type eventType)
    {
        return EventTypeFilterRoots.Any(root => root.IsEventVisible(eventType));
    }

    public BulkObservableCollection<EventPropertyModel> SelectedEventProperties { get; } = [];

    #endregion COMBAT EVENTS

    #region SKILLS
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
    public DataGridCollectionView SkillsDataView { get; }
    public int SkillCount => SkillsDataView.Count;
    public BulkObservableCollection<EventPropertyModel> SelectedSkillProperties { get; } = []; 
    private bool FilterSkillDataModels(object item)
    {
        if (item is not SkillDataModel skillData)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region AGENTS

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
            int.TryParse(AgentSpeciesFilter, out var id) && 
            agent.ID != id)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentNameFilter) && 
            !agent.Name.Contains(AgentNameFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentTypeFilter) &&
            Enum.TryParse<AgentItem.AgentType>(AgentTypeFilter, true, out var type) &&
            agent.Type != type)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentSpecFilter) &&
            Enum.TryParse<ParserHelper.Spec>(AgentTypeFilter, true, out var spec) &&
            agent.Spec != spec)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(AgentBaseSpecFilter) &&
            Enum.TryParse<ParserHelper.Spec>(AgentBaseSpecFilter, true, out var baseSpec) &&
            agent.BaseSpec != baseSpec)
        {
            return false;
        }

        return true;
    }

    public int AgentCount => AgentsDataView.Count;
    public BulkObservableCollection<EventPropertyModel> SelectedAgentProperties { get; } = [];

    #endregion AGENTS

    #region GUIDS

    public DataGridCollectionView SkillGUIDsView { get; }
    public DataGridCollectionView EffectGUIDsView { get; }
    public DataGridCollectionView MarkerGUIDsView { get; }
    public DataGridCollectionView SpeciesGUIDsView { get; }
    public DataGridCollectionView TeamGUIDsView { get; }
    public DataGridCollectionView EmoteGUIDsView { get; }
    public DataGridCollectionView TransformationGUIDsView { get; }

    // Content GUID tab filters
    [ObservableProperty]
    private string? contentIdFilter;
    [ObservableProperty]
    private string? contentGuidFilter;

    private void RefreshGUIDViews(string? oldValue, string? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }
        SkillGUIDsView.Refresh();
        EffectGUIDsView.Refresh();
        MarkerGUIDsView.Refresh();
        SpeciesGUIDsView.Refresh();
        TeamGUIDsView.Refresh();
        EmoteGUIDsView.Refresh();
        TransformationGUIDsView.Refresh();
    }

    partial void OnContentIdFilterChanged(string? oldValue, string? newValue) => RefreshGUIDViews(oldValue, newValue);

    partial void OnContentGuidFilterChanged(string? oldValue, string? newValue) => RefreshGUIDViews(oldValue, newValue);

    private bool FilterContentGUIDs(object item)
    {
        if (item is not ContentGUIDModel evt)
        {
            return false;
        }
        if (ContentIDFilter(evt.ContentID, ContentIdFilter))
        {
            return false;
        }

        if (GUIDFilter(evt.GUIDStruct, evt.GUID, ContentGuidFilter))
        {
            return false;
        }
        return true;
    }

    #endregion GUIDS

    #region COMBAT ITEMS

    public DataGridCollectionView CombatItemsView { get; }
    public int CombatItemCount => CombatItemsView.Count;

    [ObservableProperty]
    private string? stateChangeFilter;
    partial void OnStateChangeFilterChanged(string? oldValue, string? newValue) { 
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
        if (Enum.TryParse<ArcDPSEnums.StateChange>(StateChangeFilter, true, out var stateChange))
        {
            if (stateChange != combatItem.IsStateChange)
            {
                return false;
            }
        }
        return true;
    }

    #endregion COMBAT ITEMS

    public InspectorViewModel(RawEvtcLog log)
    {
        var combatItems = log.CombatItems.Select(item => new CombatItemModel(item));
        CombatItemsView = new(combatItems)
        {
            Filter = FilterCombatItems
        };
        #region AGENTS
        var agentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).OrderBy(agent => agent.ID).ToList();
        AgentsDataView = new(agentsData)
        {
            Filter = FilterAgentModels
        };
        AgentFilterItems = agentsData.Select(agent => new AgentFilterItem(agent)).ToList();
        #endregion AGENTS
        SkillsDataView = new(log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).OrderBy(skill => skill.ID))
        {
            Filter = FilterSkillDataModels
        };

        var allTimeEvents = log.CombatData.GetAllTimeCombatEvents();
        var allNonTimeEvents = log.CombatData.GetAllNonTimeCombatEvents();
        var allHealingExtensionEvents = log.CombatData.GetAllHealingExtensionCombatEvents();
        #region GUIDS
        var contentGUIDEvents = allNonTimeEvents.OfType<IDToGUIDEvent>().Where(x => x.IsValid).ToList();

        var skills = contentGUIDEvents.OfType<SkillGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(skill => skill.ContentID);
        SkillGUIDsView = new(skills)
        {
            Filter = FilterContentGUIDs
        };

        var effects = contentGUIDEvents.OfType<EffectGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(effect => effect.ContentID);
        EffectGUIDsView = new(effects)
        {
            Filter = FilterContentGUIDs
        };

        var markers = contentGUIDEvents.OfType<MarkerGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(marker => marker.ContentID);
        MarkerGUIDsView = new(markers)
        {
            Filter = FilterContentGUIDs
        };

        var species = contentGUIDEvents.OfType<SpeciesGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(species => species.ContentID);
        SpeciesGUIDsView = new(species)
        {
            Filter = FilterContentGUIDs
        };

        var teams = contentGUIDEvents.OfType<TeamGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(team => team.ContentID);
        TeamGUIDsView = new(teams)
        {
            Filter = FilterContentGUIDs
        };

        var emotes = contentGUIDEvents.OfType<EmoteGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(emote => emote.ContentID);
        EmoteGUIDsView = new(emotes)
        {
            Filter = FilterContentGUIDs
        };

        var transformations = contentGUIDEvents.OfType<TransformationGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(transformation => transformation.ContentID).ToList();
        TransformationGUIDsView = new(transformations)
        {
            Filter = FilterContentGUIDs
        };
        #endregion GUIDS

        EventTypeFilterRoots = EventTypeFilterNodeModel.BuildRoots(allTimeEvents, allNonTimeEvents, allHealingExtensionEvents);
        CombatEventsView = new(allTimeEvents
            .OrderBy(x => x.Time)
            .Cast<CombatEvent>()
            .Concat(allNonTimeEvents)
            .Concat(allHealingExtensionEvents.OrderBy(x => x.Time))
            .Select(x => new EventModel(x)))
        {
            Filter = FilterCombatEvents
        };
        foreach (var root in EventTypeFilterRoots)
        {
            root.FilterChanged += OnFilterChanged;
        }
    }

    private static bool GUIDFilter(Guid GUIDStruct, string GUIDStrine, string? filterValue)
    {
        return !string.IsNullOrWhiteSpace(filterValue) &&
            ((Guid.TryParse(filterValue, out var parsedGuid) && GUIDStruct != parsedGuid) ||
            (GUIDStrine.Contains(filterValue, StringComparison.OrdinalIgnoreCase) != true));
    }

    private static bool ContentIDFilter(long contentID, string? filterValue)
    {
        return !string.IsNullOrWhiteSpace(filterValue) &&
            long.TryParse(filterValue, out var parsedContentID) &&
            contentID != parsedContentID;
    }

    internal void SetCheckStateOnAllRoots(bool state)
    {
        MassFilterChanging = true;
        foreach (var root in EventTypeFilterRoots)
        {
            root.IsChecked = state;
        }
        MassFilterChanging = false;
    }
}
