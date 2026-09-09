using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIParserAvalonia.Models;

public sealed class AgentDataModel
{
    public ulong Agent { get; }
    public ushort InstID { get; }
    public string Name { get; }
    public int ID { get; }
    public int UniqueID { get; }
    public AgentItem.AgentType Type { get; }
    public ParserHelper.Spec Spec { get; }
    public ParserHelper.Spec BaseSpec { get; }
    public string MasterName { get; }
    public ushort MasterInstID { get; }
    public long FirstAware { get; }
    public long LastAware { get; }
    public long HalfAware { get; }
    public ushort Toughness { get; }
    public ushort Healing { get; }
    public ushort Condition { get; }
    public ushort Concentration { get; }
    public uint HitboxWidth { get; }
    public uint HitboxHeight { get; }
    public bool IsPlayer { get; }
    public bool IsNPC { get; }
    public bool IsFake { get; }
    public bool IsUnknown { get; }
    public bool IsEnglobedAgent { get; }
    public bool IsEnglobingAgent { get; }
    public bool IsNotInSquadFriendlyPlayer { get; }
    public int MergeCount { get; }
    public int RegroupedCount { get; }
    public int EnglobedAgentCount { get; }
    public string PositionAttachedAgentName { get; }
    public AgentItem AgentItem { get; }

    public AgentDataModel(AgentItem agent)
    {
        AgentItem = agent;

        Agent = agent.Agent;
        InstID = agent.InstID;
        Name = agent.Name;
        ID = agent.ID;
        UniqueID = agent.UniqueID;
        Type = agent.Type;

        FirstAware = agent.FirstAware;
        LastAware = agent.LastAware;
        HalfAware = agent.HalfAware;

        Spec = agent.GetSpecAtTime(FirstAware);
        BaseSpec = agent.GetBaseSpecAtTime(FirstAware);

        MasterName = agent.Master?.Name ?? string.Empty;
        MasterInstID = agent.Master?.InstID ?? 0;

        Toughness = agent.Toughness;
        Healing = agent.Healing;
        Condition = agent.Condition;
        Concentration = agent.Concentration;

        HitboxWidth = agent.HitboxWidth;
        HitboxHeight = agent.HitboxHeight;

        IsPlayer = agent.IsPlayer;
        IsNPC = agent.IsNPC;
        IsFake = agent.IsFake;
        IsUnknown = agent.IsUnknown;
        IsEnglobedAgent = agent.IsEnglobedAgent;
        IsEnglobingAgent = agent.IsEnglobingAgent;
        IsNotInSquadFriendlyPlayer = agent.IsNotInSquadFriendlyPlayer;

        MergeCount = agent.Merges.Count;
        RegroupedCount = agent.Regrouped.Count;
        EnglobedAgentCount = agent.EnglobedAgentItems.Count;

        PositionAttachedAgentName = agent.PositionAttachedAgentItem?.Name ?? string.Empty;
    }
}
