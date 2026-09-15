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
    [ObservableProperty]
    private EventModel? selectedEvent;
    [ObservableProperty]
    private SkillDataModel? selectedSkill;
    [ObservableProperty]
    private string? skillIdFilter;
    [ObservableProperty]
    private string? skillNameFilter;
    [ObservableProperty]
    private string? guidFilter;
    // Events tab filters
    [ObservableProperty]
    private string? agentSearchText;
    public IReadOnlyList<AgentFilterItem> AgentFilterItems { get; }
    [ObservableProperty]
    private AgentFilterItem? selectedAgentFilter;

    private readonly IReadOnlyList<TimeCombatEvent> _allTimeEvents;
    private readonly IReadOnlyList<NonTimeCombatEvent> _allNonTimeEvents;
    private readonly IReadOnlyList<EXTHealingExtensionEvent> _allHealingExtensionEvents;
    public IReadOnlyList<SkillDataModel> SkillsData { get; } = [];

    public IReadOnlyList<EventModel> Events { get; }
    public BulkObservableCollection<EventModel> VisibleEvents { get; } = [];
    public IReadOnlyList<EventTypeFilterNodeModel> EventTypeFilterRoots { get; }

    public BulkObservableCollection<EventPropertyModel> SelectedEventProperties { get; } = [];
    public BulkObservableCollection<EventPropertyModel> SelectedAgentProperties { get; } = [];
    public BulkObservableCollection<EventPropertyModel> SelectedSkillProperties { get; } = [];

    #region AGENTS

    [ObservableProperty]
    private AgentDataModel? selectedAgent;
    public DataGridCollectionView AgentsDataView { get; }
    [ObservableProperty]
    private string? agentSpeciesFilter;
    private void RefreshAgentsDataView()
    {
        AgentsDataView.Refresh();
        OnPropertyChanged(nameof(AgentCount));
    }
    partial void OnAgentSpeciesFilterChanged(string? value) => RefreshAgentsDataView();
    [ObservableProperty]
    private string? agentNameFilter;
    partial void OnAgentNameFilterChanged(string? value) => RefreshAgentsDataView();
    [ObservableProperty]
    private string? agentTypeFilter;
    partial void OnAgentTypeFilterChanged(string? value) => RefreshAgentsDataView();
    [ObservableProperty]
    private string? agentSpecFilter;
    partial void OnAgentSpecFilterChanged(string? value) => RefreshAgentsDataView();
    [ObservableProperty]
    private string? agentBaseSpecFilter;
    partial void OnAgentBaseSpecFilterChanged(string? value) => RefreshAgentsDataView();
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

    private void RefreshGUIDViews()
    {
        SkillGUIDsView.Refresh();
        EffectGUIDsView.Refresh();
        MarkerGUIDsView.Refresh();
        SpeciesGUIDsView.Refresh();
        TeamGUIDsView.Refresh();
        EmoteGUIDsView.Refresh();
        TransformationGUIDsView.Refresh();
    }

    partial void OnContentIdFilterChanged(string? value) => RefreshGUIDViews();

    partial void OnContentGuidFilterChanged(string? value) => RefreshGUIDViews();

    private bool FilterContentGUIDs(object item)
    {
        if (item is not ContentGUIDModel evt)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ContentIdFilter) && !evt.ContentID.ToString().Contains(ContentIdFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ContentGuidFilter) && !evt.GUID.ToString().Contains(ContentGuidFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        return true;
    }

    #endregion GUIDS
    public int SkillCount => SkillsData.Count;

    #region COMBAT ITEMS

    public DataGridCollectionView CombatItemsView { get; }
    public int CombatItemCount => CombatItemsView.Count;

    [ObservableProperty]
    private string? stateChangeFilter;
    partial void OnStateChangeFilterChanged(string? value) { 
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
        SkillsData = log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).OrderBy(skill => skill.ID).ToList();

        _allTimeEvents = log.CombatData.GetAllTimeCombatEvents();
        _allNonTimeEvents = log.CombatData.GetAllNonTimeCombatEvents();
        _allHealingExtensionEvents = log.CombatData.GetAllHealingExtensionCombatEvents();
        #region GUIDS
        var contentGUIDEvents = _allNonTimeEvents.OfType<IDToGUIDEvent>().Where(x => x.IsValid).ToList();

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
        Events = _allTimeEvents.OrderBy(x => x.Time).Cast<CombatEvent>().Concat(_allNonTimeEvents).Concat(_allHealingExtensionEvents.OrderBy(x => x.Time)).Select(x => new EventModel(x)).ToList();
        EventTypeFilterRoots = EventTypeFilterNodeModel.BuildRoots(_allTimeEvents, _allNonTimeEvents, _allHealingExtensionEvents);

        foreach (var root in EventTypeFilterRoots)
        {
            root.FilterChanged += OnFilterChanged;
        }

        RefreshVisibleEvents();
    }

    internal void SetCheckStateOnAllRoots(bool state)
    {
        foreach (var root in EventTypeFilterRoots)
        {
            root.IsChecked = state;
        }
    }

    private void OnFilterChanged(object? sender, EventArgs e) => RefreshVisibleEvents();

    partial void OnSkillIdFilterChanged(string? value) => RefreshVisibleEvents();

    partial void OnSkillNameFilterChanged(string? value) => RefreshVisibleEvents();

    partial void OnGuidFilterChanged(string? value) => RefreshVisibleEvents();

    partial void OnSelectedAgentFilterChanged(AgentFilterItem? value) => RefreshVisibleEvents();

    private void RefreshVisibleEvents()
    {
        long? skillId = null;

        if (!string.IsNullOrWhiteSpace(SkillIdFilter))
        {
            if (!long.TryParse(SkillIdFilter, out var parsedSkillId))
            {
                VisibleEvents.Clear();
                return;
            }

            skillId = parsedSkillId;
        }

        bool hasSkillNameFilter = !string.IsNullOrWhiteSpace(SkillNameFilter);
        bool hasGuidFilter = !string.IsNullOrWhiteSpace(GuidFilter);
        ulong? agentFilter = SelectedAgentFilter?.Agent;

        var visibleEvents = Events.Where(eventModel =>
        {
            if (!IsEventTypeVisible(eventModel.Event.GetType()))
            {
                return false;
            }

            if (skillId is not null && eventModel.SkillId != skillId)
            {
                return false;
            }

            if (hasSkillNameFilter && eventModel.SkillName?.Contains(SkillNameFilter!, StringComparison.OrdinalIgnoreCase) != true)
            {
                return false;
            }

            if (hasGuidFilter && eventModel.Guid?.Contains(GuidFilter!, StringComparison.OrdinalIgnoreCase) != true)
            {
                return false;
            }

            if (agentFilter is not null && !eventModel.AgentIds.Contains(agentFilter.Value))
            {
                return false;
            }

            return true;
        });

        VisibleEvents.ReplaceRange(visibleEvents);
    }

    partial void OnSelectedEventChanged(EventModel? value)
    {
        if (value?.Event == null)
        {
            SelectedEventProperties.ReplaceRange([]);
            return;
        }

        SelectedEventProperties.ReplaceRange(EventInspector.Inspect(value.Event));
    }

    partial void OnSelectedAgentChanged(AgentDataModel? value)
    {
        SelectedAgentProperties.ReplaceRange(EventInspector.Inspect(value));
    }

    partial void OnSelectedSkillChanged(SkillDataModel? value)
    {
        if (value?.SkillItem == null)
        {
            SelectedSkillProperties.ReplaceRange([]);
            return;
        }

        SelectedSkillProperties.ReplaceRange(EventInspector.Inspect(value.SkillItem));
    }

    private bool IsEventTypeVisible(Type eventType)
    {
        return EventTypeFilterRoots.Any(root => root.IsEventVisible(eventType));
    }
}
