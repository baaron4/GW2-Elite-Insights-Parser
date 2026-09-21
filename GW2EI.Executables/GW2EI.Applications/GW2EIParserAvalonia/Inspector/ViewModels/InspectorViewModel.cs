using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Models;
using GW2EIParserAvalonia.Services;

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

    private void CombatEventsViewRefresh(string? oldValue, string? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }
        CombatEventsView.Refresh();
        UpdateEventTypeCounts();
    }
    internal bool AgentSelectFilterDropdownTriggered = false;
    private void CombatEventsViewRefresh(AgentFilterItem? oldValue, AgentFilterItem? newValue)
    {
        if (AgentSelectFilterDropdownTriggered)
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
    [ObservableProperty]
    private string? agentSearchText;
    public IReadOnlyList<AgentFilterItem> AgentFilterItems { get; }
    [ObservableProperty]
    private AgentFilterItem? selectedAgentFilter;
    [ObservableProperty]
    private AgentFilterRole selectedAgentFilterRole = AgentFilterRole.Any;
    public IReadOnlyList<AgentFilterRole> AgentFilterRoles { get; } = Enum.GetValues<AgentFilterRole>();
    partial void OnSelectedAgentFilterChanged(AgentFilterItem? oldValue, AgentFilterItem? newValue) => CombatEventsViewRefresh(oldValue, newValue);
    partial void OnSelectedAgentFilterRoleChanged(AgentFilterRole oldValue, AgentFilterRole newValue) => CombatEventsViewRefresh(oldValue.ToString(), newValue.ToString());

    public BulkObservableCollection<EventPropertyModel> SelectedEventProperties { get; } = [];
    public IReadOnlyList<EventTypeFilterNodeModel> EventTypeFilterRoots { get; }
    public DataGridCollectionView CombatEventsView { get; }
    private readonly List<EventModel> _allCombatEvents;
    private readonly Dictionary<Type, List<EventModel>> _combatEventsByType;
    private readonly BulkObservableCollection<EventModel> _visibleCombatEvents = [];
    private readonly HashSet<Type> _visibleEventTypes = [];
    private readonly record struct EventTypeMergeCursor(List<EventModel> Events, int Index);
    private bool MassFilterChanging = false;

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        if (MassFilterChanging)
        {
            return;
        }

        UpdateVisibleEventTypes();
        UpdateVisibleCombatEvents();
        UpdateEventTypeCounts();
    }

    private bool FilterCombatEvents(object item)
    {
        if (item is not EventModel eventModel)
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

        if (!MatchesSelectedAgent(eventModel))
        {
            return false;
        }
        return true;
    }

    private void UpdateVisibleEventTypes()
    {
        _visibleEventTypes.Clear();

        foreach (var root in EventTypeFilterRoots)
        {
            _visibleEventTypes.UnionWith(root.VisibleTypes);
        }
    }

    private void UpdateVisibleCombatEvents()
    {
        if (_visibleEventTypes.Count == 0)
        {
            _visibleCombatEvents.ReplaceRange([]);
            return;
        }

        if (_visibleEventTypes.Count == _combatEventsByType.Count)
        {
            _visibleCombatEvents.ReplaceRange(_allCombatEvents);
            return;
        }

        var queue = new PriorityQueue<EventTypeMergeCursor, int>();

        foreach (var type in _visibleEventTypes)
        {
            if (!_combatEventsByType.TryGetValue(type, out var events) || events.Count == 0)
            {
                continue;
            }

            queue.Enqueue(new EventTypeMergeCursor(events, 0), events[0].SourceIndex);
        }

        var visibleEvents = new List<EventModel>();

        while (queue.Count > 0)
        {
            var cursor = queue.Dequeue();

            var eventModel = cursor.Events[cursor.Index];
            visibleEvents.Add(eventModel);

            var nextIndex = cursor.Index + 1;

            if (nextIndex < cursor.Events.Count)
            {
                var nextEvent = cursor.Events[nextIndex];

                queue.Enqueue(new EventTypeMergeCursor(cursor.Events, nextIndex), nextEvent.SourceIndex);
            }
        }

        _visibleCombatEvents.ReplaceRange(visibleEvents);
    }

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

    #endregion COMBAT ITEMS

    public InspectorViewModel(EvtcLog log)
    {
        #region COMBAT ITEMS
        var combatItems = log.CombatItems.Select(item => new CombatItemModel(item)).ToList();
        CombatItemsView = new(combatItems)
        {
            Filter = FilterCombatItems
        };
        #endregion COMBAT ITEMS

        #region AGENTS
        var agentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).OrderBy(agent => agent.ID).ToList();
        AgentsDataView = new(agentsData)
        {
            Filter = FilterAgentModels
        };
        AgentFilterItems = agentsData.Where(x => !x.IsEnglobedAgent).Select(agent => new AgentFilterItem(agent)).ToList();
        #endregion AGENTS

        #region  SKILLS
        SkillsDataView = new(log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData, log.CombatData)).OrderBy(skill => skill.ID).ToList())
        {
            Filter = FilterSkillDataModels
        };
        #endregion SKILLS

        var allTimeEvents = log.CombatData.GetAllTimeCombatEvents();
        var allNonTimeEvents = log.CombatData.GetAllNonTimeCombatEvents();
        var allHealingExtensionEvents = log.CombatData.GetAllHealingExtensionCombatEvents();

        #region GUIDS
        var contentGUIDEvents = allNonTimeEvents.OfType<IDToGUIDEvent>().Where(x => x.IsValid).ToList();

        var skills = contentGUIDEvents.OfType<SkillGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(skill => skill.ContentID).ToList();
        SkillGUIDsView = new(skills)
        {
            Filter = FilterContentGUIDs
        };

        var effects = contentGUIDEvents.OfType<EffectGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(effect => effect.ContentID).ToList();
        EffectGUIDsView = new(effects)
        {
            Filter = FilterContentGUIDs
        };

        var markers = contentGUIDEvents.OfType<MarkerGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(marker => marker.ContentID).ToList();
        MarkerGUIDsView = new(markers)
        {
            Filter = FilterContentGUIDs
        };

        var species = contentGUIDEvents.OfType<SpeciesGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(species => species.ContentID).ToList();
        SpeciesGUIDsView = new(species)
        {
            Filter = FilterContentGUIDs
        };

        var teams = contentGUIDEvents.OfType<TeamGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(team => team.ContentID).ToList();
        TeamGUIDsView = new(teams)
        {
            Filter = FilterContentGUIDs
        };

        var emotes = contentGUIDEvents.OfType<EmoteGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(emote => emote.ContentID).ToList();
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
        var eventModels = allTimeEvents
            .Concat(allHealingExtensionEvents)
            .OrderBy(x => x.Time)
            .Cast<CombatEvent>()
            .Concat(allNonTimeEvents)
            .Select(x => new EventModel(x))
            .ToList();

        for (var i = 0; i < eventModels.Count; i++)
        {
            eventModels[i].SourceIndex = i;
        }

        _allCombatEvents = eventModels;
        _combatEventsByType = eventModels
            .GroupBy(x => x.EventType)
            .ToDictionary(group => group.Key, group => group.ToList());
        _visibleCombatEvents.ReplaceRange(_allCombatEvents);

        CombatEventsView = new DataGridCollectionView(_visibleCombatEvents)
        {
            Filter = FilterCombatEvents
        };
        foreach (var root in EventTypeFilterRoots)
        {
            root.FilterChanged += OnFilterChanged;
        }
    }

    private static bool GUIDFilter(Guid GUIDStruct, string GUIDString, string? filterValue)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
        {
            return false;
        }

        if (Guid.TryParse(filterValue, out var parsedGuid))
        {
            return GUIDStruct != parsedGuid;
        }

        return !GUIDString.Contains(filterValue, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContentIDFilter(long contentID, string? filterValue)
    {
        return !string.IsNullOrWhiteSpace(filterValue) &&
            !contentID.ToString().Contains(filterValue, StringComparison.OrdinalIgnoreCase);
    }

    internal void SetCheckStateOnAllRoots(bool state)
    {
        MassFilterChanging = true;

        try
        {
            foreach (var root in EventTypeFilterRoots)
            {
                root.IsChecked = state;
            }
        }
        finally
        {
            MassFilterChanging = false;
        }

        UpdateVisibleEventTypes();
        UpdateVisibleCombatEvents();
    }

    private bool MatchesSelectedAgent(EventModel eventModel)
    {
        var selectedAgent = SelectedAgentFilter?.Agent;

        if (selectedAgent is null)
        {
            return true;
        }

        var agent = selectedAgent.Value;

        return SelectedAgentFilterRole switch
        {
            AgentFilterRole.Any =>
                eventModel.SourceAgentIds.Contains(agent) ||
                eventModel.DestinationAgentIds.Contains(agent),

            AgentFilterRole.Src =>
                eventModel.SourceAgentIds.Contains(agent),

            AgentFilterRole.Dst =>
                eventModel.DestinationAgentIds.Contains(agent),

            AgentFilterRole.Both =>
                eventModel.SourceAgentIds.Contains(agent) &&
                eventModel.DestinationAgentIds.Contains(agent),

            _ => true
        };
    }

    private void UpdateEventTypeCounts()
    {
        var counts = new Dictionary<Type, int>();

        foreach (var eventModel in _allCombatEvents)
        {
            if (!FilterCombatEvents(eventModel))
            {
                continue;
            }

            counts.TryGetValue(eventModel.EventType, out var count);
            counts[eventModel.EventType] = count + 1;
        }

        foreach (var root in EventTypeFilterRoots)
        {
            root.UpdateFilteredCount(counts);
        }
    }
}
