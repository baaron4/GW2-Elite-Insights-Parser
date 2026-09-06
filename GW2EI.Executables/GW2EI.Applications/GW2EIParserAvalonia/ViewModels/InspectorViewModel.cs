using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIParserAvalonia.InspectorContent;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

public sealed class InspectorViewModel
{
    public IReadOnlyList<CombatItemModel> CombatItems { get; } = [];
    public IReadOnlyList<AgentDataModel> AgentsData { get; } = [];
    public IReadOnlyList<SkillDataModel> SkillsData { get; } = [];

    public int CombatItemCount => CombatItems.Count;
    public int AgentCount => AgentsData.Count;
    public int SkillCount => SkillsData.Count;

    public InspectorViewModel(ParsedEvtcLog log)
    {
        CombatItems = log.CombatItems.Select(item => new CombatItemModel(item)).ToList();
        AgentsData = log.AgentData.AllAgents.Select(agent => new AgentDataModel(agent)).ToList();
        SkillsData = log.SkillData.AllSkills.Select(skill => new SkillDataModel(skill, log.SkillData)).ToList();
    }
}
