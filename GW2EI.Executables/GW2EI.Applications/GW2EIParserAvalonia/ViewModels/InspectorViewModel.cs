using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.InspectorContent;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

public sealed class InspectorViewModel
{
    private readonly IReadOnlyList<TimeCombatEvent> _allEvents;
    private readonly IReadOnlyList<object> _allNonTimeEvents;

    public IReadOnlyList<CombatItemModel> CombatItems { get; } = [];
    public IReadOnlyList<AgentDataModel> AgentsData { get; } = [];
    public IReadOnlyList<SkillDataModel> SkillsData { get; } = [];

    public IReadOnlyList<EventModel> Events { get; }
    public BulkObservableCollection<EventModel> VisibleEvents { get; } = [];
    public IReadOnlyList<EventTypeFilterNode> EventTypeFilterRoots { get; }

    public int CombatItemCount => CombatItems.Count;
    public int AgentCount => AgentsData.Count;
    public int SkillCount => SkillsData.Count;

    public InspectorViewModel(RawEvtcLog log)
    {
        CombatItems = log.CombatItems.Select(item => new CombatItemModel(item)).ToList();
        AgentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).ToList();
        SkillsData = log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).ToList();

        _allEvents = log.CombatData.GetAllEvents();
        _allNonTimeEvents = log.CombatData.GetAllNonTimeCombatEvents();

        Events = _allEvents.Cast<object>().Concat(_allNonTimeEvents).Select(x => new EventModel(x)).ToList();
        EventTypeFilterRoots = EventTypeFilterNode.Build(_allEvents, _allNonTimeEvents);

        foreach (var root in EventTypeFilterRoots)
        {
            root.FilterChanged += OnFilterChanged;
        }

        RefreshVisibleEvents();
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        RefreshVisibleEvents();
    }

    private void RefreshVisibleEvents()
    {
        IEnumerable<EventModel> visibleEvents = Events
            .Where(eventModel =>
                EventTypeFilterRoots.Any(root =>
                    root.IsEventVisible(eventModel.Event.GetType())));

        VisibleEvents.ReplaceRange(visibleEvents);
    }
}
