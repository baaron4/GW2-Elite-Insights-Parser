using System;
using System.Collections.Generic;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Models;
using GW2EIParserAvalonia.Services;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
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
    private void CombatEventsViewRefresh(AgentFilterItem? oldValue, AgentFilterItem? newValue)
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

    [ObservableProperty]
    private string? skillIdFilter;
    partial void OnSkillIdFilterChanged(string? oldValue, string? newValue)
    {
        if (DropdownFilterTriggered)
        {
            return;
        }

        CombatEventsViewRefresh(oldValue, newValue);
    }
    public IReadOnlyList<string> SkillIdFilterItems { get; private set; } = [];
    [ObservableProperty]
    private string? skillNameFilter;
    partial void OnSkillNameFilterChanged(string? oldValue, string? newValue)
    {
        if (DropdownFilterTriggered)
        {
            return;
        }

        CombatEventsViewRefresh(oldValue, newValue);
    }
    public IReadOnlyList<string> SkillNameFilterItems { get; private set; } = [];
    [ObservableProperty]
    private string? guidIdFilter;
    partial void OnGuidIdFilterChanged(string? oldValue, string? newValue)
    {
        if (DropdownFilterTriggered)
        {
            return;
        }

        CombatEventsViewRefresh(oldValue, newValue);
    }
    public IReadOnlyList<string> GuidIdFilterItems { get; private set; } = [];
    [ObservableProperty]
    private string? guidFilter;
    partial void OnGuidFilterChanged(string? oldValue, string? newValue)
    {
        if (DropdownFilterTriggered)
        {
            return;
        }

        CombatEventsViewRefresh(oldValue, newValue);
    }
    public IReadOnlyList<string> GuidFilterItems { get; private set; } = [];
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

        if (!string.IsNullOrWhiteSpace(GadgetDataFilter))
        {
            if (eventModel.Event is not GadgetAnimationEvent)
            {
                return false;
            }

            if (!string.Equals(eventModel.GadgetAnimationData, GadgetDataFilter, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
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

}
