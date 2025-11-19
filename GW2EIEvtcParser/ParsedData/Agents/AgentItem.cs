using System.Diagnostics.Metrics;
using System.Numerics;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.ParsedData;

public class AgentItem
{
    public struct MergedAgentItem
    {
        internal MergedAgentItem(AgentItem merged, long start, long end)
        {
            Merged = merged;
            MergeStart = start;
            MergeEnd = end;
        }
        internal void ApplyOffset(long offset)
        {
            MergeStart -= offset;
            MergeEnd -= offset;
        }
        public readonly AgentItem Merged;
        public long MergeStart { get; private set; }
        public long MergeEnd { get; private set; }
    }

    private List<MergedAgentItem>? _merges;
    public IReadOnlyList<MergedAgentItem> Merges => _merges ?? [];

    private List<MergedAgentItem>? _regrouped;
    public IReadOnlyList<MergedAgentItem> Regrouped => _regrouped ?? [];

    private AgentItem? _englobingAgentItem;
    public bool IsEnglobedAgent => _englobingAgentItem != null;
    public AgentItem EnglobingAgentItem => _englobingAgentItem ?? this;
    private List<AgentItem>? _englobedAgentItems;
    public IReadOnlyList<AgentItem> EnglobedAgentItems => _englobedAgentItems ?? [];
    public AgentItem? GeographicallyAttachedAgentItem { get; private set; }
    public bool IsEnglobingAgent => _englobedAgentItems != null;

    private static int AgentCount = 0; //TODO(Rennorb) @correctness @threadding: should this be atomic? 
    public enum AgentType { NPC, Gadget, Player, NonSquadPlayer }

    public bool IsPlayer => Type == AgentType.Player || Type == AgentType.NonSquadPlayer;
    public bool IsNPC => Type == AgentType.NPC || Type == AgentType.Gadget;

    public bool IsUnknown { get; private set; } = false;

    // Fields
    public readonly ulong Agent;
    public int ID { get; protected set; } = NonIdentifiedSpecies;
    /// <remarks>Nondeterministic.</remarks>
    public readonly int UniqueID;
    public AgentItem? Master { get; protected set; }
    public ushort InstID { get; protected set; }
    public AgentType Type { get; protected set; } = AgentType.NPC;
    public long FirstAware { get; protected set; }
    public long LastAware { get; protected set; } = long.MaxValue;

    public long HalfAware => (FirstAware + LastAware) / 2;
    public string Name { get; protected set; } = "UNKNOWN";
    public ParserHelper.Spec Spec { get; private set; } = ParserHelper.Spec.Unknown;
    public ParserHelper.Spec BaseSpec { get; private set; } = ParserHelper.Spec.Unknown;
    public ushort Toughness { get; protected set; }
    public readonly ushort Healing;
    public readonly ushort Condition;
    public readonly ushort Concentration;
    public uint HitboxWidth { get; private set; }
    public uint HitboxHeight { get; private set; }

    private readonly bool Unamed;

    public readonly bool IsFake;
    public bool IsNotInSquadFriendlyPlayer { get; private set; }

    // Constructors
    internal AgentItem(ulong agent, string name, ParserHelper.Spec spec, int id, AgentType type, ushort toughness, ushort healing, ushort condition, ushort concentration, uint hbWidth, uint hbHeight)
    {
        UniqueID = ++AgentCount;
        Agent = agent;
        Name = name;
        Spec = spec;
        BaseSpec = ParserHelper.SpecToBaseSpec(spec);
        ID = id;
        Type = type;
        Toughness = toughness;
        Healing = healing;
        Condition = condition;
        Concentration = concentration;
        HitboxWidth = hbWidth;
        HitboxHeight = hbHeight;
        //
        try
        {
            if (type == AgentType.Player)
            {
                HitboxWidth = 48;
                HitboxHeight = 240;
                string[] splitStr = Name.Split('\0');
                if (splitStr.Length < 2 || (splitStr[1].Length == 0 || splitStr[2].Length == 0 || splitStr[0].Contains('-')))
                {
                    Type = AgentType.NonSquadPlayer;
                }
            }
        }
        catch (Exception)
        {

        }
        Unamed = Name.Contains("ch" + ID + "-") || Name.Contains("gd" + ID + "-");
    }

    internal AgentItem(ulong agent, string name, ParserHelper.Spec spec, int id, AgentType type, ushort instid, ushort toughness, ushort healing, ushort condition, ushort concentration, uint hbWidth, uint hbHeight, long firstAware, long lastAware, bool isFake) : this(agent, name, spec, id, type, toughness, healing, condition, concentration, hbWidth, hbHeight)
    {
        InstID = instid;
        FirstAware = firstAware;
        LastAware = lastAware;
        IsFake = isFake;
    }

    internal AgentItem(AgentItem other)
    {
        UniqueID = ++AgentCount;
        Agent = other.Agent;
        Name = other.Name;
        Spec = other.Spec;
        BaseSpec = other.BaseSpec;
        ID = other.ID;
        Type = other.Type;
        Toughness = other.Toughness;
        Healing = other.Healing;
        Condition = other.Condition;
        Concentration = other.Concentration;
        HitboxWidth = other.HitboxWidth;
        HitboxHeight = other.HitboxHeight;
        InstID = other.InstID;
        Master = other.Master;
        IsFake = other.IsFake;
        Unamed = other.Unamed;
        IsNotInSquadFriendlyPlayer = other.IsNotInSquadFriendlyPlayer;
    }

    internal AgentItem()
    {
        UniqueID = ++AgentCount;
        IsUnknown = true;
    }

    internal void OverrideSpec(ParserHelper.Spec spec)
    {
        Spec = spec;
        BaseSpec = ParserHelper.SpecToBaseSpec(spec);
    }

    internal void OverrideIsNotInSquadFriendlyPlayer(bool status)
    {
        IsNotInSquadFriendlyPlayer = status;
    }

    internal void OverrideType(AgentType type, AgentData agentData)
    {
        agentData.FlagAsDirty(AgentData.AgentDataDirtyStatus.TypesDirty);
        Type = type;
    }

    internal void OverrideHitbox(uint hitboxWidth, uint hitboxHeight)
    {
        HitboxWidth = hitboxWidth;
        HitboxHeight = hitboxHeight;
    }

    internal void OverrideName(string name)
    {
        Name = name;
    }

    internal void SetInstid(ushort instid)
    {
        InstID = instid;
    }

    internal void OverrideID(int id, AgentData agentData)
    {
        if (IsPlayer)
        {
            return;
        }
        agentData.FlagAsDirty(AgentData.AgentDataDirtyStatus.SpeciesDirty);
        ID = id;
    }

    internal void OverrideID(TargetID id, AgentData agentData)
    {
        OverrideID((int)id, agentData);
    }

    internal void OverrideID(MinionID id, AgentData agentData)
    {
        OverrideID((int)id, agentData);
    }

    internal void OverrideID(ChestID id, AgentData agentData)
    {
        OverrideID((int)id, agentData);
    }

    internal void OverrideToughness(ushort toughness)
    {
        Toughness = toughness;
    }

    internal void OverrideAwareTimes(long firstAware, long lastAware)
    {
        FirstAware = firstAware;
        LastAware = lastAware;
    }

    internal void ApplyOffset(long offset)
    {
        FirstAware -= offset;
        LastAware -= offset;
        foreach (var merge in Merges)
        {
            merge.ApplyOffset(offset);
        }
    }

    internal void SetMaster(AgentItem master)
    {
        if (IsPlayer || master.Is(this))
        {
            return;
        }
        AgentItem cur = master;
        while (cur.Master != null)
        {
            cur = cur.Master;
            if (cur.Is(this))
            {
                return;
            }
        }
        Master = master.EnglobingAgentItem;
    }

    internal AgentItem GetMainAgentWhenAttackTarget(ParsedEvtcLog log)
    {
        var atEvent = log.CombatData.GetAttackTargetEventByAttackTarget(this);
        return atEvent?.Src ?? this;
    }
    internal SingleActor GetMainSingleActorWhenAttackTarget(ParsedEvtcLog log)
    {
        var atEvent = log.CombatData.GetAttackTargetEventByAttackTarget(this);
        return log.FindActor(atEvent?.Src ?? this);
    }
    public bool Is(AgentItem? ag)
    {
        if (ag == null)
        {
            return false;
        }
        return EnglobingAgentItem == ag.EnglobingAgentItem;
    }

    public bool IsMasterOrSelf(AgentItem ag)
    {
        return GetFinalMaster().Is(ag);
    }

    public bool IsMaster(AgentItem ag)
    {
        if (ag.Is(this))
        {
            return false;
        }
        return GetFinalMaster().Is(ag);
    }
    public bool IsMasterOfOrSelf(AgentItem ag)
    {
        return ag.IsMasterOrSelf(this);
    }
    public bool IsMasterOf(AgentItem ag)
    {
        if (ag.Is(this))
        {
            return false;
        }
        return ag.IsMaster(this);
    }
    public AgentItem GetFinalMaster()
    {
        AgentItem cur = this;
        while (cur.Master != null)
        {
            cur = cur.Master;
        }
        return cur;
    }

    public bool InAwareTimes(long time)
    {
        return FirstAware <= time && LastAware >= time;
    }
    public bool InAwareTimes(long start, long end)
    {
        return new Segment(FirstAware, LastAware).Intersects(start, end);
    }
    public bool InAwareTimes(SingleActor other)
    {
        return InAwareTimes(other.FirstAware, other.LastAware);
    }
    public bool InAwareTimes(AgentItem other)
    {
        return InAwareTimes(other.FirstAware, other.LastAware);
    }

    /// <summary>
    /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
    /// </summary>
    public bool HasBuff(ParsedEvtcLog log, long buffID, long time, long window = 0)
    {
        return log.FindActor(this).HasBuff(log, buffID, time, window);
    }

    /// <summary>
    /// Checks if a buff is present on the actor and applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
    /// </summary>
    public bool HasBuff(ParsedEvtcLog log, SingleActor by, long buffID, long time)
    {
        return log.FindActor(this).HasBuff(log, by, buffID, time);
    }

    /// <summary>
    /// Checks if the buffs are present on the actor.  Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise.
    /// </summary>
    public bool HasAnyBuff(ParsedEvtcLog log, IEnumerable<long> buffIDs, long time, long window = 0)
    {
        return buffIDs.Any(id => log.FindActor(this).HasBuff(log, id, time, window));
    }

    public Segment GetBuffStatus(ParsedEvtcLog log, long buffID, long time)
    {
        return log.FindActor(this).GetBuffStatus(log, buffID, time);
    }

    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffID, long start, long end)
    {
        return log.FindActor(this).GetBuffStatus(log, buffID, start, end);
    }

    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffID)
    {
        return log.FindActor(this).GetBuffStatus(log, buffID);
    }

    public Segment GetBuffStatus(ParsedEvtcLog log, SingleActor by, long buffID, long time)
    {
        return log.FindActor(this).GetBuffStatus(log, by, buffID, time);
    }

    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, SingleActor by, long buffID, long start, long end)
    {
        return log.FindActor(this).GetBuffStatus(log, by, buffID, start, end);
    }
    public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, SingleActor by, long buffID)
    {
        return log.FindActor(this).GetBuffStatus(log, by, buffID);
    }


    /// <summary>
    /// Checks if the agent will go into downstate before the next time they go above 90% health, or the fight ends.
    /// </summary>
    /// <param name="time">Current log time</param>
    /// <returns><see langword="true"/> if the agent will down before the next time they go above 90% health, otherwise <see langword="false"/>.</returns>
    public bool IsDownedBeforeNext90(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).IsDownBeforeNext90(log, time);
    }

    public (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs, IReadOnlyList<Segment> actives) GetStatus(ParsedEvtcLog log)
    {
        return log.FindActor(this).GetStatus(log);
    }

    public (IReadOnlyList<Segment> breakbarNones, IReadOnlyList<Segment> breakbarActives, IReadOnlyList<Segment> breakbarImmunes, IReadOnlyList<Segment> breakbarRecoverings) GetBreakbarStatus(ParsedEvtcLog log)
    {
        return log.FindActor(this).GetBreakbarStatus(log);
    }

    /// <summary>
    /// Checks if the agent is downed at given time.
    /// </summary>
    /// <param name="time">Downed time.</param>
    /// <returns><see langword="true"/> if the agent is downed, otherwise <see langword="false"/>.</returns>
    public bool IsDowned(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).IsDowned(log, time);
    }

    /// <summary>
    /// Checks if the agent is downed during a segment of time.
    /// </summary>
    /// <param name="start">Start time.</param>
    /// <param name="end">End Time.</param>
    /// <returns><see langword="true"/> if the agent is downed, otherwise <see langword="false"/>.</returns>
    public bool IsDowned(ParsedEvtcLog log, long start, long end)
    {
        return log.FindActor(this).IsDowned(log, start, end);
    }

    /// <summary>
    /// Checks if the agent is dead at given time
    /// </summary>
    /// <param name="time">Death time.</param>
    /// <returns><see langword="true"/> if the agent is dead, otherwise <see langword="false"/>.</returns>
    public bool IsDead(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).IsDead(log, time);
    }

    /// <summary>
    /// Checks if the agent is dead during a segment of time.
    /// </summary>
    /// <param name="start">Start time.</param>
    /// <param name="end">End Time.</param>
    /// <returns><see langword="true"/> if the agent is dead, otherwise <see langword="false"/>.</returns>
    public bool IsDead(ParsedEvtcLog log, long start, long end)
    {
        return log.FindActor(this).IsDead(log, start, end);
    }

    /// <summary>
    /// Checks if the agent is dc/not spawned at given time
    /// </summary>
    /// <param name="time">Presence time.</param>
    /// <returns><see langword="true"/> if the agent isn't present, otherwise <see langword="false"/>.</returns>
    public bool IsDC(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).IsDC(log, time);
    }

    /// <summary>
    /// Checks if the agent is dc/not spawned during a segment of time.
    /// </summary>
    /// <param name="start">Start time.</param>
    /// <param name="end">End Time.</param>
    /// <returns><see langword="true"/> if the agent isn't present, otherwise <see langword="false"/>.</returns>
    public bool IsDC(ParsedEvtcLog log, long start, long end)
    {
        return log.FindActor(this).IsDC(log, start, end);
    }

    public double GetCurrentHealthPercent(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).GetCurrentHealthPercent(log, time);
    }

    public double GetCurrentBarrierPercent(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).GetCurrentBarrierPercent(log, time);
    }

    public bool TryGetCurrentPosition(ParsedEvtcLog log, long time, out Vector3 position, long forwardWindow = 0)
    {
        return log.FindActor(this).TryGetCurrentPosition(log, time, out position, forwardWindow);
    }
    public bool TryGetCurrentInterpolatedPosition(ParsedEvtcLog log, long time, out Vector3 position)
    {
        return log.FindActor(this).TryGetCurrentInterpolatedPosition(log, time, out position);
    }

    public bool TryGetCurrentFacingDirection(ParsedEvtcLog log, long time, out Vector3 facing, long forwardWindow = 0)
    {
        return log.FindActor(this).TryGetCurrentFacingDirection(log, time, out facing, forwardWindow);
    }

    public BreakbarState GetCurrentBreakbarState(ParsedEvtcLog log, long time)
    {
        return log.FindActor(this).GetCurrentBreakbarState(log, time);
    }

    public bool IsUnamedSpecies()
    {
        if (IsPlayer)
        {
            return false;
        }
        return IsNonIdentifiedSpecies() || Unamed;
    }

    public bool IsNonIdentifiedSpecies()
    {
        if (IsPlayer)
        {
            return false;
        }
        return IsUnknown || IsAnySpecies([NonIdentifiedSpecies, TargetID.WorldVersusWorld, TargetID.Environment, TargetID.Instance, TargetID.DummyTarget]);
    }

    public bool IsSpecies(int id)
    {
        if (IsPlayer)
        {
            return false;
        }
        return ID == id;
    }


    public bool IsSpecies(TargetID id)
    {
        return IsSpecies((int)id);
    }

    public bool IsSpecies(MinionID id)
    {
        return IsSpecies((int)id);
    }

    public bool IsSpecies(ChestID id)
    {
        return IsSpecies((int)id);
    }

    public bool IsAnySpecies(IEnumerable<int> ids)
    {
        return ids.Any(IsSpecies);
    }

    public bool IsAnySpecies(IEnumerable<TargetID> ids)
    {
        return ids.Any(IsSpecies);
    }

    public bool IsAnySpecies(IEnumerable<MinionID> ids)
    {
        return ids.Any(IsSpecies);
    }

    public bool IsAnySpecies(IEnumerable<ChestID> ids)
    {
        return ids.Any(IsSpecies);
    }
    internal void AddMergeFrom(AgentItem mergedFrom, long start, long end)
    {
        _merges ??= [];
        _merges.Add(new MergedAgentItem(mergedFrom, start, end));
    }

    internal void AddRegroupedFrom(AgentItem regroupedFrom)
    {
        _regrouped ??= [];
        _regrouped.Add(new MergedAgentItem(regroupedFrom, regroupedFrom.FirstAware, regroupedFrom.LastAware));
    }

    private void AddEnglobedAgentItem(AgentItem child, AgentData agentData)
    {
        if (_englobedAgentItems == null)
        {
            _englobedAgentItems = [];
        }
        _englobedAgentItems.Add(child);
        agentData.FlagAsDirty(AgentData.AgentDataDirtyStatus.TypesDirty | AgentData.AgentDataDirtyStatus.SpeciesDirty);
    }

    internal void GeographicallyAttachTo(AgentItem geographicallyAttached)
    {
        GeographicallyAttachedAgentItem = geographicallyAttached;
    }
    internal void SetEnglobingAgentItem(AgentItem parent, AgentData agentData)
    {
        _englobingAgentItem = parent;
        parent.AddEnglobedAgentItem(this, agentData);
    }

    internal AgentItem FindEnglobedAgentItem(long time)
    {
        if (!IsEnglobingAgent)
        {
            return this;
        }
        return EnglobedAgentItems.FirstOrDefault(x => x.InAwareTimes(time)) ?? this;
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? FirstByAware<T>(this IReadOnlyList<T> agents) where T : AgentItem
    {
        (T? Agent, long FirstAware) result = (default, long.MaxValue);
        foreach(var agent in agents)
        {
            if(agent.FirstAware < result.FirstAware)
            {
                result = (agent, agent.FirstAware);
            }
        }
        return result.Agent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByFirstAware<T>(this List<T> list) where T : AgentItem
    {
        list.AsSpan().SortStable((a, b) => a.FirstAware.CompareTo(b.FirstAware));
    }
}
