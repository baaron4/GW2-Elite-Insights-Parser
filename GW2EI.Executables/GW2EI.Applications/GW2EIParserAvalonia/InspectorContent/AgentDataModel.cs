using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class AgentDataModel
{
    public ulong Agent => _agent.Agent;
    public ushort InstID => _agent.InstID;
    public string Name => _agent.Name;
    public int ID => _agent.ID;
    public int UniqueID => _agent.UniqueID;
    public AgentItem.AgentType Type => _agent.Type;
    public ParserHelper.Spec Spec => _agent.GetSpecAtTime(_agent.FirstAware);
    public ParserHelper.Spec BaseSpec => _agent.GetBaseSpecAtTime(_agent.FirstAware);
    public string MasterName => _agent.Master?.Name ?? string.Empty;
    public ushort MasterInstID => _agent.Master?.InstID ?? 0;
    public long FirstAware => _agent.FirstAware;
    public long LastAware => _agent.LastAware;
    public long HalfAware => _agent.HalfAware;
    public ushort Toughness => _agent.Toughness;
    public ushort Healing => _agent.Healing;
    public ushort Condition => _agent.Condition;
    public ushort Concentration => _agent.Concentration;
    public uint HitboxWidth => _agent.HitboxWidth;
    public uint HitboxHeight => _agent.HitboxHeight;
    public bool IsPlayer => _agent.IsPlayer;
    public bool IsNPC => _agent.IsNPC;
    public bool IsFake => _agent.IsFake;
    public bool IsUnknown => _agent.IsUnknown;
    public bool IsEnglobedAgent => _agent.IsEnglobedAgent;
    public bool IsEnglobingAgent => _agent.IsEnglobingAgent;
    public bool IsNotInSquadFriendlyPlayer => _agent.IsNotInSquadFriendlyPlayer;
    public int MergeCount => _agent.Merges.Count;
    public int RegroupedCount => _agent.Regrouped.Count;
    public int EnglobedAgentCount => _agent.EnglobedAgentItems.Count;
    public string PositionAttachedAgentName => _agent.PositionAttachedAgentItem?.Name ?? string.Empty;
    public AgentItem AgentItem => _agent;
    private readonly AgentItem _agent;

    public AgentDataModel(AgentItem agent)
    {
        _agent = agent;
    }
}
