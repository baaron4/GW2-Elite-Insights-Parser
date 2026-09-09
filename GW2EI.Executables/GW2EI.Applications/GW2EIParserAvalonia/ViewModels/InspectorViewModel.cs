using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.InspectorContent;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    [ObservableProperty]
    private EventModel? selectedEvent;
    [ObservableProperty]
    private AgentDataModel? selectedAgent;
    [ObservableProperty]
    private SkillDataModel? selectedSkill;
    [ObservableProperty]
    private string? skillIdFilter;
    [ObservableProperty]
    private string? skillNameFilter;
    [ObservableProperty]
    private string? guidFilter;

    private readonly IReadOnlyList<TimeCombatEvent> _allTimeEvents;
    private readonly IReadOnlyList<NonTimeCombatEvent> _allNonTimeEvents;

    public IReadOnlyList<CombatItemModel> CombatItems { get; } = [];
    public IReadOnlyList<AgentDataModel> AgentsData { get; } = [];
    public IReadOnlyList<SkillDataModel> SkillsData { get; } = [];

    public IReadOnlyList<EventModel> Events { get; }
    public BulkObservableCollection<EventModel> VisibleEvents { get; } = [];
    public IReadOnlyList<EventTypeFilterNode> EventTypeFilterRoots { get; }

    public BulkObservableCollection<EventPropertyModel> SelectedEventProperties { get; } = [];
    public BulkObservableCollection<EventPropertyModel> SelectedAgentProperties { get; } = [];
    public BulkObservableCollection<EventPropertyModel> SelectedSkillProperties { get; } = [];

    public IReadOnlyList<ContentGUIDModel> Skills { get; }
    public IReadOnlyList<ContentGUIDModel> Effects { get; }
    public IReadOnlyList<ContentGUIDModel> Markers { get; }
    public IReadOnlyList<ContentGUIDModel> Species { get; }
    public IReadOnlyList<ContentGUIDModel> Teams { get; }
    public IReadOnlyList<ContentGUIDModel> Emotes { get; }
    public IReadOnlyList<ContentGUIDModel> Transformations { get; }

    public int CombatItemCount => CombatItems.Count;
    public int AgentCount => AgentsData.Count;
    public int SkillCount => SkillsData.Count;

    public InspectorViewModel(RawEvtcLog log)
    {
        CombatItems = log.CombatItems.Select(item => new CombatItemModel(item)).ToList();
        AgentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).OrderBy(agent => agent.ID).ToList();
        SkillsData = log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).OrderBy(skill => skill.ID).ToList();

        _allTimeEvents = log.CombatData.GetAllTimeCombatEvents();
        _allNonTimeEvents = log.CombatData.GetAllNonTimeCombatEvents();

        var contentGUIDEvents = _allNonTimeEvents.OfType<IDToGUIDEvent>().Where(x => x.IsValid).ToList();
        Skills = contentGUIDEvents.OfType<SkillGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();
        Effects = contentGUIDEvents.OfType<EffectGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();
        Markers = contentGUIDEvents.OfType<MarkerGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();
        Species = contentGUIDEvents.OfType<SpeciesGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();
        Teams = contentGUIDEvents.OfType<TeamGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();
        Emotes = contentGUIDEvents.OfType<EmoteGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();
        Transformations = contentGUIDEvents.OfType<TransformationGUIDEvent>().Select(x => new ContentGUIDModel(x)).ToList();

        Events = _allTimeEvents.OrderBy(x => x.Time).Cast<object>().Concat(_allNonTimeEvents).Select(x => new EventModel(x)).ToList();
        EventTypeFilterRoots = EventTypeFilterNode.Build(_allTimeEvents, _allNonTimeEvents);

        foreach (var root in EventTypeFilterRoots)
        {
            root.FilterChanged += OnFilterChanged;
        }

        RefreshVisibleEvents();
    }

    private void OnFilterChanged(object? sender, EventArgs e) => RefreshVisibleEvents();

    partial void OnSkillIdFilterChanged(string? value) => RefreshVisibleEvents();

    partial void OnSkillNameFilterChanged(string? value) => RefreshVisibleEvents();

    partial void OnGuidFilterChanged(string? value) => RefreshVisibleEvents();

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
