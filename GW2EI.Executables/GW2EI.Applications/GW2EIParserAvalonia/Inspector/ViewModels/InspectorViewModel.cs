using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Services;
using GW2EIParserAvalonia.Models;
using GW2EIEvtcParser.Extensions;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    [ObservableProperty]
    private EventModel? selectedEvent;
    [ObservableProperty]
    private AgentDataModel? selectedAgent;
    [ObservableProperty]
    private SkillDataModel? selectedSkill;

    // Events tab filters
    [ObservableProperty]
    private string? agentSearchText;
    [ObservableProperty]
    private AgentFilterItem? selectedAgentFilter;
    [ObservableProperty]
    private string? skillIdFilter;
    [ObservableProperty]
    private string? skillNameFilter;
    [ObservableProperty]
    private string? guidIdFilter;
    [ObservableProperty]
    private string? guidFilter;

    // Content GUID tab filters
    [ObservableProperty]
    private string? contentIdFilter;
    [ObservableProperty]
    private string? contentGuidFilter;

    private readonly IReadOnlyList<TimeCombatEvent> _allTimeEvents;
    private readonly IReadOnlyList<NonTimeCombatEvent> _allNonTimeEvents;
    private readonly IReadOnlyList<EXTHealingExtensionEvent> _allHealingExtensionEvents;

    public IReadOnlyList<CombatItemModel> CombatItems { get; } = [];
    public IReadOnlyList<AgentDataModel> AgentsData { get; } = [];
    public IReadOnlyList<SkillDataModel> SkillsData { get; } = [];

    public IReadOnlyList<EventModel> Events { get; }
    public BulkObservableCollection<EventModel> VisibleEvents { get; } = [];
    public IReadOnlyList<EventTypeFilterNodeModel> EventTypeFilterRoots { get; }

    public IReadOnlyList<AgentFilterItem> AgentFilterItems { get; }

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

    public BulkObservableCollection<ContentGUIDModel> VisibleSkills { get; } = [];
    public BulkObservableCollection<ContentGUIDModel> VisibleEffects { get; } = [];
    public BulkObservableCollection<ContentGUIDModel> VisibleMarkers { get; } = [];
    public BulkObservableCollection<ContentGUIDModel> VisibleSpecies { get; } = [];
    public BulkObservableCollection<ContentGUIDModel> VisibleTeams { get; } = [];
    public BulkObservableCollection<ContentGUIDModel> VisibleEmotes { get; } = [];
    public BulkObservableCollection<ContentGUIDModel> VisibleTransformations { get; } = [];

    public int CombatItemCount => CombatItems.Count;
    public int AgentCount => AgentsData.Count;
    public int SkillCount => SkillsData.Count;

    public InspectorViewModel(RawEvtcLog log)
    {
        CombatItems = log.CombatItems.Select(item => new CombatItemModel(item)).ToList();
        AgentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).OrderBy(agent => agent.ID).ToList();
        SkillsData = log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).OrderBy(skill => skill.ID).ToList();

        AgentFilterItems = AgentsData.Select(agent => new AgentFilterItem(agent)).ToList();

        _allTimeEvents = log.CombatData.GetAllTimeCombatEvents();
        _allNonTimeEvents = log.CombatData.GetAllNonTimeCombatEvents();
        _allHealingExtensionEvents = log.CombatData.GetAllHealingExtensionCombatEvents();

        var contentGUIDEvents = _allNonTimeEvents.OfType<IDToGUIDEvent>().Where(x => x.IsValid).ToList();
        Skills = contentGUIDEvents.OfType<SkillGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(skill => skill.ContentID).ToList();
        Effects = contentGUIDEvents.OfType<EffectGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(effect => effect.ContentID).ToList();
        Markers = contentGUIDEvents.OfType<MarkerGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(marker => marker.ContentID).ToList();
        Species = contentGUIDEvents.OfType<SpeciesGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(species => species.ContentID).ToList();
        Teams = contentGUIDEvents.OfType<TeamGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(team => team.ContentID).ToList();
        Emotes = contentGUIDEvents.OfType<EmoteGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(emote => emote.ContentID).ToList();
        Transformations = contentGUIDEvents.OfType<TransformationGUIDEvent>().Select(x => new ContentGUIDModel(x)).OrderBy(transformation => transformation.ContentID).ToList();

        Events = _allTimeEvents.OrderBy(x => x.Time).Cast<CombatEvent>().Concat(_allNonTimeEvents).Concat(_allHealingExtensionEvents).Select(x => new EventModel(x)).ToList();
        EventTypeFilterRoots = EventTypeFilterNodeModel.BuildRoots(_allTimeEvents, _allNonTimeEvents, _allHealingExtensionEvents);

        foreach (var root in EventTypeFilterRoots)
        {
            root.FilterChanged += OnFilterChanged;
        }

        RefreshVisibleEvents();
        RefreshVisibleContentGUIDs();
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

    partial void OnGuidIdFilterChanged(string? value) => RefreshVisibleEvents();

    partial void OnSelectedAgentFilterChanged(AgentFilterItem? value) => RefreshVisibleEvents();

    partial void OnAgentSearchTextChanged(string? value) => OnPropertyChanged(nameof(FilteredAgentFilterItems));

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

            if (!ContentIDFilter(eventModel.ContentID, GuidIdFilter))
            {
                return false;
            }

            if (!GUIDFilter(eventModel.GUIDStruct, GuidFilter))
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

    partial void OnContentIdFilterChanged(string? value) => RefreshVisibleContentGUIDs();

    partial void OnContentGuidFilterChanged(string? value) => RefreshVisibleContentGUIDs();

    private void RefreshVisibleContentGUIDs()
    {
        VisibleSkills.ReplaceRange(FilterContentGUIDs(Skills));
        VisibleEffects.ReplaceRange(FilterContentGUIDs(Effects));
        VisibleMarkers.ReplaceRange(FilterContentGUIDs(Markers));
        VisibleSpecies.ReplaceRange(FilterContentGUIDs(Species));
        VisibleTeams.ReplaceRange(FilterContentGUIDs(Teams));
        VisibleEmotes.ReplaceRange(FilterContentGUIDs(Emotes));
        VisibleTransformations.ReplaceRange(FilterContentGUIDs(Transformations));
    }

    private IEnumerable<ContentGUIDModel> FilterContentGUIDs(IEnumerable<ContentGUIDModel> source)
    {
        return source.Where(model =>
        {
            if (!ContentIDFilter(model.ContentID, ContentIdFilter))
            {
                return false;
            }

            if (!GUIDFilter(model.GUIDStruct, ContentGuidFilter))
            {
                return false;
            }

            return true;
        });
    }

    private static bool ContentIDFilter(long contentId, string? filter)
    {
        if (!string.IsNullOrWhiteSpace(filter) && !contentId.ToString().Contains(filter!, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        return true;
    }

    private static bool GUIDFilter(Guid guid, string? filter)
    {
        Guid exactGuid = default;
        bool hasGuidFilter = !string.IsNullOrWhiteSpace(filter);
        bool hasExactGuidFilter = hasGuidFilter && filter!.Length == 32 && Guid.TryParse(filter, out exactGuid);

        if (hasGuidFilter)
        {
            if ((hasExactGuidFilter && guid != exactGuid) || !guid.ToString("N").ToUpperInvariant().Contains(filter!, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerable<AgentFilterItem> FilteredAgentFilterItems =>
    string.IsNullOrWhiteSpace(AgentSearchText)
        ? AgentFilterItems
        : AgentFilterItems.Where(agent =>
            agent.DisplayName.Contains(
                AgentSearchText,
                StringComparison.OrdinalIgnoreCase));
}
