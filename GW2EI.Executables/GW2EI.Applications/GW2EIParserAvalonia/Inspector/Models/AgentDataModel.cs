using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class AgentDataModel
{
    public ulong Agent => _agentItem.Agent;
    public ushort InstID => _agentItem.InstID;
    public string Name => _agentItem.Name;
    public int ID => _agentItem.ID;
    // This is EI Logic
    //public int UniqueID => AgentItem.UniqueID;
    public AgentItem.AgentType Type => _agentItem.Type;
    public ParserHelper.Spec Spec => _agentItem.Spec;
    public ParserHelper.Spec BaseSpec => _agentItem.BaseSpec;

    public readonly AgentDataModel? Master;
    public long FirstAware => _agentItem.FirstAware;
    public long LastAware => _agentItem.LastAware;
    public ushort Toughness => _agentItem.Toughness;
    public ushort Healing => _agentItem.Healing;
    public ushort Condition => _agentItem.Condition;
    public ushort Concentration => _agentItem.Concentration;
    // Height is defunct, it also never worked according to deltaconnected
    public uint HitboxWidth => _agentItem.HitboxWidth;
    public bool IsPlayer => _agentItem.IsPlayer;
    public bool IsNPC => _agentItem.IsNPC;
    public bool IsFake => _agentItem.IsFake;
    public bool IsUnknown => _agentItem.IsUnknown;
    public bool IsEnglobedAgent => _agentItem.IsEnglobedAgent;
    public bool IsEnglobingAgent => _agentItem.IsEnglobingAgent;
    public int EnglobedAgentCount => _agentItem.EnglobedAgentItems.Count;
    public bool IsNotInSquadFriendlyPlayer => _agentItem.IsNotInSquadFriendlyPlayer;
    public int MergeCount => _agentItem.Merges.Count;
    public readonly List<AgentDataModel>? Merges;
    public int RegroupedCount => _agentItem.Regrouped.Count;
    public readonly List<AgentDataModel>? Regrouped;
    private readonly AgentItem _agentItem;

    public AgentDataModel(AgentItem agent)
    {
        _agentItem = agent;
        if (agent.Master != null)
        {
            Master = new AgentDataModel(agent.Master);
        }
        if (MergeCount > 0)
        {
            Merges = agent.Merges.Select(x => new AgentDataModel(x.Merged)).ToList();
        }
        if (RegroupedCount > 0)
        {
            Regrouped = agent.Regrouped.Select(x => new AgentDataModel(x.Merged)).ToList();
        }
    }
}
