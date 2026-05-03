using System.Runtime.CompilerServices;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser;

public class CombatItem
{
    public long Time { get; private set; }
    public ulong SrcAgent { get; private set; }
    public ulong DstAgent { get; private set; }
    public int Value { get; private set; }
    public readonly int BuffDmg;
    public readonly uint OverstackValue;
    public uint SkillID { get; private set; }
    public ushort SrcInstid { get; private set; }
    public ushort DstInstid { get; private set; }
    public readonly ushort SrcMasterInstid;
    public readonly ushort DstMasterInstid;
    public readonly byte IFFByte;
    public readonly IFF IFF;
    public readonly byte IsBuff;
    public readonly byte Result;
    public readonly byte IsActivationByte;
    public readonly Activation IsActivation;
    public readonly byte IsBuffRemoveByte;
    public readonly BuffRemove IsBuffRemove;
    public readonly byte IsNinety;
    public readonly byte IsFifty;
    public readonly byte IsMoving;
    public readonly StateChange IsStateChange;
    public readonly byte IsFlanking;
    public readonly byte IsShields;
    public readonly byte IsOffcycle;

    public readonly uint Pad;
    public readonly byte Pad1;
    public readonly byte Pad2;
    public readonly byte Pad3;
    public readonly byte Pad4;

    public bool IsExtension => IsStateChange == StateChange.Extension || IsStateChange == StateChange.ExtensionCombat;
    public bool IsGeographical => IsStateChange == StateChange.Position ||
            IsStateChange == StateChange.Rotation ||
            IsStateChange == StateChange.Velocity;

    public bool IsEffect => IsStateChange == StateChange.Effect_51 || IsStateChange == StateChange.Effect_45 || IsStateChange == StateChange.EffectAgentCreate || IsStateChange == StateChange.EffectAgentRemove || IsStateChange == StateChange.EffectGroundCreate || IsStateChange == StateChange.EffectGroundRemove;
    public bool IsMissile => IsStateChange == StateChange.MissileCreate || IsStateChange == StateChange.MissileLaunch || IsStateChange == StateChange.MissileRemove;

    public bool IsEssentialMetadata => IsStateChange == StateChange.IDToGUID || IsStateChange == StateChange.Language 
        || IsStateChange == StateChange.GWBuild || IsStateChange == StateChange.InstanceStart 
        || IsStateChange == StateChange.LogNPCUpdate || IsStateChange == StateChange.FractalScale 
        || IsStateChange == StateChange.Language || IsStateChange == StateChange.MapID 
        || IsStateChange == StateChange.RuleSet
        || IsStateChange == StateChange.SquadCombatEnd || IsStateChange == StateChange.SquadCombatStart 
        || IsStateChange == StateChange.TickRate;

    private readonly EvtcVersionEvent _version;

#if DEBUG
    // For debugging, to get rid of statechanges seen generally on all agents
    public bool IsSpecial => IsStateChange != StateChange.Combat && IsStateChange != StateChange.Position && IsStateChange != StateChange.Rotation && IsStateChange != StateChange.Velocity && IsStateChange != StateChange.HealthUpdate && IsStateChange != StateChange.BarrierUpdate && IsStateChange != StateChange.EnterCombat && IsStateChange != StateChange.ExitCombat && IsStateChange != StateChange.BreakbarPercent && IsStateChange != StateChange.BreakbarState && IsStateChange != StateChange.Spawn && IsStateChange != StateChange.Despawn && IsStateChange != StateChange.TeamChange && IsStateChange != StateChange.StackActive;
#endif

    // Constructor
    public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
           uint skillID, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid,
           ushort dstMasterInstid, byte iff, byte isBuff,
           byte result, byte isActivation,
           byte isBuffRemove, byte isNinety, byte isFifty, byte isMoving,
           byte isStateChange, byte isFlanking, byte isShields, byte isOffcycle, uint pad,
           EvtcVersionEvent version)
    {
        Time = time;
        SrcAgent = srcAgent;
        DstAgent = dstAgent;
        Value = value;
        BuffDmg = buffDmg;
        OverstackValue = overstackValue;
        SkillID = skillID;
        SrcInstid = srcInstid;
        DstInstid = dstInstid;
        SrcMasterInstid = srcMasterInstid;
        DstMasterInstid = dstMasterInstid;
        IFFByte = iff;
        IFF = GetIFF(iff);
        IsBuff = isBuff;
        Result = result;
        IsActivationByte = isActivation;
        IsActivation = GetActivation(isActivation);
        IsBuffRemoveByte = isBuffRemove;
        IsBuffRemove = GetBuffRemove(isBuffRemove);
        IsNinety = isNinety;
        IsFifty = isFifty;
        IsMoving = isMoving;
        IsStateChange = GetStateChange(isStateChange);
        IsFlanking = isFlanking;
        IsShields = isShields;
        IsOffcycle = isOffcycle;
        Pad = pad;
        // break pad
        unsafe
        {
            Pad1 = *(byte*)&pad;
            Pad2 = *((byte*)&pad + 1);
            Pad3 = *((byte*)&pad + 2);
            Pad4 = *((byte*)&pad + 3);
        }
        _version = version;
    }

    internal CombatItem(CombatItem c)
    {
        Time = c.Time;
        SrcAgent = c.SrcAgent;
        DstAgent = c.DstAgent;
        Value = c.Value;
        BuffDmg = c.BuffDmg;
        OverstackValue = c.OverstackValue;
        SkillID = c.SkillID;
        SrcInstid = c.SrcInstid;
        DstInstid = c.DstInstid;
        SrcMasterInstid = c.SrcMasterInstid;
        DstMasterInstid = c.DstMasterInstid;
        IFFByte = c.IFFByte;
        IFF = c.IFF;
        IsBuff = c.IsBuff;
        Result = c.Result;
        IsActivationByte = c.IsActivationByte;
        IsActivation = c.IsActivation;
        IsBuffRemoveByte = c.IsBuffRemoveByte;
        IsBuffRemove = c.IsBuffRemove;
        IsNinety = c.IsNinety;
        IsFifty = c.IsFifty;
        IsMoving = c.IsMoving;
        IsStateChange = c.IsStateChange;
        IsFlanking = c.IsFlanking;
        IsShields = c.IsShields;
        IsOffcycle = c.IsOffcycle;
        Pad = c.Pad;
        Pad1 = c.Pad1;
        Pad2 = c.Pad2;
        Pad3 = c.Pad3;
        Pad4 = c.Pad4;
        _version = c._version;
    }

    internal bool HasTime()
    {
        return SrcIsAgent()
            || DstIsAgent()
            || IsStateChange == StateChange.Reward
            || IsStateChange == StateChange.TickRate
            || IsStateChange == StateChange.SquadMarker
            || IsStateChange == StateChange.SquadCombatStart
            || IsStateChange == StateChange.SquadCombatEnd
            || IsStateChange == StateChange.EffectAgentRemove
            || IsStateChange == StateChange.EffectGroundRemove
            || IsStateChange == StateChange.MapID
            || IsStateChange == StateChange.MapChange
            ;
    }

    internal bool HasTime(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.HasTime(this);
        }
        return HasTime();
    }

    internal bool IsDamageEvent()
    {
        return IsDirectDamageEvent() || IsBuffDamageEvent();
    }

    internal bool IsNonZeroDamageEvent()
    {
        if (IsDirectDamageEvent())
        {
            return Value > 0;
        }
        if (IsBuffDamageEvent())
        {
            return BuffDmg > 0;
        }
        return false;
    }

    internal bool IsDamageEvent(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.IsDamageEvent(this);
        }
        return IsDamageEvent();
    }

    internal bool IsNonZeroDamageEvent(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.IsNonZeroDamageEvent(this);
        }
        return IsNonZeroDamageEvent();
    }

    internal bool IsDirectDamageEvent()
    {
        if (_version.Build >= ArcDPSBuilds.ResultEnumRework)
        {
            return IsStateChange == StateChange.Combat &&
                    IsBuff == 0;
        }
        return IsStateChange == StateChange.Combat &&
                    IsActivation == Activation.None &&
                    IsBuffRemove == BuffRemove.None &&
                    IsBuff == 0
                    ;
    }

    internal bool IsBuffDamageEvent()
    {
        if (_version.Build >= ArcDPSBuilds.ResultEnumRework)
        {
            return IsStateChange == StateChange.Combat &&
                    IsBuff != 0;
        }
        return IsStateChange == StateChange.Combat &&
                    IsActivation == Activation.None &&
                    IsBuffRemove == BuffRemove.None &&
                    IsBuff != 0 && Value == 0
                    ;
    }

    internal bool SrcIsAgent()
    {
        return IsStateChange == StateChange.Combat
            || IsStateChange == StateChange.EnterCombat
            || IsStateChange == StateChange.ExitCombat
            || IsStateChange == StateChange.ChangeUp
            || IsStateChange == StateChange.ChangeDead
            || IsStateChange == StateChange.ChangeDown
            || IsStateChange == StateChange.Spawn
            || IsStateChange == StateChange.Despawn
            || IsStateChange == StateChange.HealthUpdate
            || IsStateChange == StateChange.WeaponSwap
            || IsStateChange == StateChange.MaxHealthUpdate
            || IsStateChange == StateChange.PointOfView
            || IsStateChange == StateChange.BuffInitial
            || IsGeographical
            || IsStateChange == StateChange.TeamChange
            || IsStateChange == StateChange.AttackTarget
            || IsStateChange == StateChange.Targetable
            || IsStateChange == StateChange.StackActive
            || IsStateChange == StateChange.StackDeactive
            || IsStateChange == StateChange.Guild
            || IsStateChange == StateChange.BreakbarState
            || IsStateChange == StateChange.BreakbarPercent
            || IsStateChange == StateChange.Marker
            || IsStateChange == StateChange.BarrierUpdate
            || IsStateChange == StateChange.Last90BeforeDown
            || IsStateChange == StateChange.Effect_45
            || IsStateChange == StateChange.Effect_51
            || IsStateChange == StateChange.EffectGroundCreate
            || IsStateChange == StateChange.EffectAgentCreate
            || IsStateChange == StateChange.Glider
            || IsStateChange == StateChange.StunBreak
            || IsStateChange == StateChange.MissileCreate
            || IsStateChange == StateChange.MissileLaunch
            || IsStateChange == StateChange.MissileRemove
            || IsStateChange == StateChange.AnimationStart
            || IsStateChange == StateChange.AnimationStop
            || IsStateChange == StateChange.BuffRemoveAll
            || IsStateChange == StateChange.BuffRemoveSingle
            || IsStateChange == StateChange.BuffApply
            ;
    }

    internal bool SrcIsAgent(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.SrcIsAgent(this);
        }
        return SrcIsAgent();
    }

    internal bool DstIsAgent()
    {
        return IsStateChange == StateChange.Combat
            || IsStateChange == StateChange.AttackTarget
            || IsStateChange == StateChange.BuffInitial
            || IsStateChange == StateChange.Effect_45
            || IsStateChange == StateChange.LogNPCUpdate
            || IsStateChange == StateChange.Effect_51
            || IsStateChange == StateChange.EffectAgentCreate
            || IsStateChange == StateChange.MissileLaunch
            || IsStateChange == StateChange.AnimationStart
            || IsStateChange == StateChange.BuffRemoveAll
            || IsStateChange == StateChange.BuffRemoveSingle
            || IsStateChange == StateChange.BuffApply
            || IsStateChange == StateChange.BuffChange
        ;
    }

    internal bool DstIsAgent(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.DstIsAgent(this);
        }
        return DstIsAgent();
    }

    internal bool IsBuffApplyOrRemoveEvent()
    {
        return IsBuffApplyEvent() || IsBuffRemoveEvent();
    }

    internal bool IsBuffApplyEvent()
    {
        if (_version.Build >= ArcDPSBuilds.BuffAppliesAndRemovesAsStateChanges)
        {
            return IsStateChange == StateChange.BuffApply || IsStateChange == StateChange.BuffChange || IsStateChange == StateChange.BuffInitial;
        }
        return (IsBuff != 0 && BuffDmg == 0 && Value > 0 && IsActivation == Activation.None && 
            IsBuffRemove == BuffRemove.None && IsStateChange == StateChange.Combat) || 
            IsStateChange == StateChange.BuffInitial;
    }

    internal bool IsBuffRemoveEvent()
    {
        if (_version.Build >= ArcDPSBuilds.BuffAppliesAndRemovesAsStateChanges)
        {
            return IsStateChange == StateChange.BuffRemoveAll || IsStateChange == StateChange.BuffRemoveSingle;
        }
        return IsStateChange == StateChange.Combat && IsActivation == Activation.None && IsBuffRemove != BuffRemove.None;
    }

    internal bool IsBuffRemoveAllEvent()
    {
        if (_version.Build >= ArcDPSBuilds.BuffAppliesAndRemovesAsStateChanges)
        {
            return IsStateChange == StateChange.BuffRemoveAll;
        }
        return IsBuffRemoveEvent() && IsBuffRemove == BuffRemove.All;
    }

    internal bool DstMatchesAgent(AgentItem agentItem, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (DstIsAgent(extensions))
        {
            return agentItem.EnglobingAgentItem.Agent == DstAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool DstMatchesAgent(AgentItem agentItem)
    {
        if (DstIsAgent())
        {
            return agentItem.EnglobingAgentItem.Agent == DstAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool SrcMatchesAgent(AgentItem agentItem, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (SrcIsAgent(extensions))
        {
            return agentItem.EnglobingAgentItem.Agent == SrcAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool SrcMatchesAgent(AgentItem agentItem)
    {
        if (SrcIsAgent())
        {
            return agentItem.EnglobingAgentItem.Agent == SrcAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool IsCastEvent()
    {
        return IsStartCastEvent() || IsEndCastEvent();
    }

    internal bool IsStartCastEvent()
    {
        if (_version.Build >= ArcDPSBuilds.AnimationAsStateChanges)
        {
            return IsStateChange == StateChange.AnimationStart;
        }
        if (IsStateChange != StateChange.Combat)
        {
            return false;
        }
        return IsActivation == Activation.Normal || IsActivation == Activation.Quickness;
    }

    internal bool IsEndCastEvent()
    {
        if (_version.Build >= ArcDPSBuilds.AnimationAsStateChanges)
        {
            return IsStateChange == StateChange.AnimationStop;
        }
        if (IsStateChange != StateChange.Combat)
        {
            return false;
        }
        return IsActivation == Activation.Minimum || IsActivation == Activation.Reset || IsActivation == Activation.Cancel || IsActivation == Activation.NoData;
    }

    ///
    internal void OverrideTime(long time)
    {
        Time = time;
    }

    internal void OverrideSrcAgent(ulong agent)
    {
        SrcAgent = agent;
    }

    internal void OverrideDstAgent(ulong agent)
    {
        DstAgent = agent;
    }

    internal void OverrideSrcAgent(AgentItem agent)
    {
        SrcAgent = agent.EnglobingAgentItem.Agent;
        SrcInstid = agent.EnglobingAgentItem.InstID;
    }

    internal void OverrideDstAgent(AgentItem agent)
    {
        DstAgent = agent.EnglobingAgentItem.Agent;
        DstInstid = agent.EnglobingAgentItem.InstID;
    }

    internal void OverrideValue(int value)
    {
        Value = value;
    }
}

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortByTime<T>(this List<T> list)  where T : CombatItem
    {
        list.AsSpan().SortStable((a, b) => a.Time.CompareTo(b.Time));
    }
}
