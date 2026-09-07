using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.InspectorContent;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

public sealed class InspectorViewModel
{
    private readonly IReadOnlyList<TimeCombatEvent> _allEvents;
    public IReadOnlyList<CombatItemModel> CombatItems { get; } = [];
    public IReadOnlyList<AgentDataModel> AgentsData { get; } = [];
    public IReadOnlyList<SkillDataModel> SkillsData { get; } = [];
    public IReadOnlyList<EventModel> Events { get; }
    public EventTypeFilterNode EventTypeFilterRoot { get; }
    public BulkObservableCollection<EventModel> VisibleEvents { get; } = [];
    public IReadOnlyList<EventTypeFilterNode> EventTypeFilterRoots => [EventTypeFilterRoot];
    public int CombatItemCount => CombatItems.Count;
    public int AgentCount => AgentsData.Count;
    public int SkillCount => SkillsData.Count;

    public InspectorViewModel(RawEvtcLog log)
    {
        CombatItems = log.CombatItems.Select(item => new CombatItemModel(item)).ToList();
        AgentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).ToList();
        SkillsData = log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).ToList();
        Events = log.CombatData.GetAllEvents().Select(x => new EventModel(x)).ToList();

        _allEvents = log.CombatData.GetAllEvents();

        EventTypeFilterRoot = EventTypeFilterNode.Build(_allEvents);
        EventTypeFilterRoot.FilterChanged += OnFilterChanged;

        RefreshVisibleEvents();
    }

    public bool IsEventVisible(EventModel eventModel)
    {
        return EventTypeFilterRoot.IsEventVisible(
            eventModel.Event.GetType());
    }
    private void OnFilterChanged(object? sender, EventArgs e)
    {
        RefreshVisibleEvents();
    }

    private void RefreshVisibleEvents()
    {
        IEnumerable<EventModel> visibleEvents = Events
            .Where(eventModel =>
                EventTypeFilterRoot.IsEventVisible(
                    eventModel.Event.GetType()));

        VisibleEvents.ReplaceRange(visibleEvents);
    }
}
