using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

public partial class InspectorViewModel : ObservableObject
{
    internal bool DropdownFilterTriggered = false;

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
        var agentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent, log)).OrderBy(agent => agent.ID).ToList();
        AgentsDataView = new(agentsData)
        {
            Filter = FilterAgentModels
        };
        AgentFilterItems = agentsData.Where(x => !x.IsEnglobedAgent).Select(agent => new AgentFilterItem(agent)).ToList();
        #endregion AGENTS

        #region  SKILLS
        SkillsDataView = new(log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).OrderBy(skill => skill.ID).ToList())
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

        #region GADGET ANIMATION
        var gadgetData = allTimeEvents.OfType<GadgetAnimationEvent>().Select(x => new GadgetAnimationModel(x)).GroupBy(x => x.Data).Select(g => g.First()).OrderBy(x => x.Data).ToList();
        GadgetAnimationView = new(gadgetData);
        GadgetAnimationFilterItems = gadgetData.Select(x => x.Data).ToList();
        #endregion GADGET ANIMATION

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
        #region FilterItems
        SkillNameFilterItems = eventModels
            .Where(x => !string.IsNullOrWhiteSpace(x.SkillName))
            .Select(x => x.SkillName!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        SkillIdFilterItems = eventModels
            .Where(x => x.SkillId.HasValue)
            .Select(x => x.SkillId!.Value)
            .Distinct()
            .OrderBy(x => x)
            .Select(x => x.ToString())
            .ToList();

        GuidFilterItems = eventModels
            .Where(x => !string.IsNullOrWhiteSpace(x.GUID))
            .Select(x => x.GUID)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        GuidIdFilterItems = eventModels
            .Where(x => x.ContentID != 0)
            .Select(x => x.ContentID)
            .Distinct()
            .OrderBy(x => x)
            .Select(x => x.ToString())
            .ToList();
        #endregion FilterItems
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
