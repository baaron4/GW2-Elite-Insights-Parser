using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class AgentDataModel
{
    public ulong Agent => AgentItem.Agent;
    public ushort InstID => AgentItem.InstID;
    public string Name => AgentItem.Name;
    public int ID => AgentItem.ID;
    // This is EI Logic
    //public int UniqueID => AgentItem.UniqueID;
    public AgentItem.AgentType Type => AgentItem.Type;
    public ParserHelper.Spec Spec => AgentItem.Spec;
    public ParserHelper.Spec BaseSpec => AgentItem.BaseSpec;

    public readonly AgentDataModel? Master;
    public long FirstAware => AgentItem.FirstAware;
    public long LastAware => AgentItem.LastAware;
    public ushort Toughness => AgentItem.Toughness;
    public ushort Healing => AgentItem.Healing;
    public ushort Condition => AgentItem.Condition;
    public ushort Concentration => AgentItem.Concentration;
    // Height is defunct, it also never worked according to deltaconnected
    public uint HitboxWidth => AgentItem.HitboxWidth;
    public bool IsPlayer => AgentItem.IsPlayer;
    public bool IsNPC => AgentItem.IsNPC;
    public bool IsFake => AgentItem.IsFake;
    public bool IsUnknown => AgentItem.IsUnknown;
    public bool IsEnglobedAgent => AgentItem.IsEnglobedAgent;
    public bool IsEnglobingAgent => AgentItem.IsEnglobingAgent;
    public int EnglobedAgentCount => AgentItem.EnglobedAgentItems.Count;
    public bool IsNotInSquadFriendlyPlayer => AgentItem.IsNotInSquadFriendlyPlayer;
    public int MergeCount => AgentItem.Merges.Count;
    public readonly List<AgentDataModel>? Merges;
    public int RegroupedCount => AgentItem.Regrouped.Count;
    public readonly List<AgentDataModel>? Regrouped;
    private readonly AgentItem AgentItem;

    public AgentDataModel(AgentItem agent)
    {
        AgentItem = agent;
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
