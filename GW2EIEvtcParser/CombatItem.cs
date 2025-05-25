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
    public readonly ushort SrcInstid;
    public readonly ushort DstInstid;
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

    public bool IsEffect => IsStateChange == StateChange.Effect_51 || IsStateChange == StateChange.Effect_45 || IsStateChange == StateChange.EffectAgentCreate || IsStateChange == StateChange.EffectAgentRemove || IsStateChange == StateChange.EffectGroundCreate || IsStateChange == StateChange.EffectGroundRemove;
    public bool IsMissile => IsStateChange == StateChange.MissileCreate || IsStateChange == StateChange.MissileLaunch || IsStateChange == StateChange.MissileRemove;

    public bool IsEssentialMetadata => IsStateChange == StateChange.IDToGUID || IsStateChange == StateChange.Language 
        || IsStateChange == StateChange.GWBuild || IsStateChange == StateChange.InstanceStart 
        || IsStateChange == StateChange.LogNPCUpdate || IsStateChange == StateChange.FractalScale 
        || IsStateChange == StateChange.Language || IsStateChange == StateChange.MapID 
        || IsStateChange == StateChange.RuleSet || IsStateChange == StateChange.ShardId 
        || IsStateChange == StateChange.SquadCombatEnd || IsStateChange == StateChange.SquadCombatStart 
        || IsStateChange == StateChange.TickRate;

#if DEBUG
    // For debugging, to get rid of statechanges seen generally on all agents
    public bool IsSpecial => IsStateChange != StateChange.None && IsStateChange != StateChange.Position && IsStateChange != StateChange.Rotation && IsStateChange != StateChange.Velocity && IsStateChange != StateChange.HealthUpdate && IsStateChange != StateChange.BarrierUpdate && IsStateChange != StateChange.EnterCombat && IsStateChange != StateChange.ExitCombat && IsStateChange != StateChange.BreakbarPercent && IsStateChange != StateChange.BreakbarState && IsStateChange != StateChange.Spawn && IsStateChange != StateChange.Despawn && IsStateChange != StateChange.TeamChange && IsStateChange != StateChange.StackActive;
#endif

    // Constructor
    public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
           uint skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid,
           ushort dstMasterInstid, byte iff, byte isBuff,
           byte result, byte isActivation,
           byte isBuffRemove, byte isNinety, byte isFifty, byte isMoving,
           byte isStateChange, byte isFlanking, byte isShields, byte isOffcycle, uint pad)
    {
        Time = time;
        SrcAgent = srcAgent;
        DstAgent = dstAgent;
        Value = value;
        BuffDmg = buffDmg;
        OverstackValue = overstackValue;
        SkillID = skillId;
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
    }

    internal bool HasTime()
    {
        return SrcIsAgent()
            || DstIsAgent()
            || IsStateChange == StateChange.Reward
            || IsStateChange == StateChange.TickRate
            || IsStateChange == StateChange.SquadMarker
            || IsStateChange == StateChange.InstanceStart
            || IsStateChange == StateChange.SquadCombatStart
            || IsStateChange == StateChange.SquadCombatEnd
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

    internal bool IsGeographical()
    {
        return IsStateChange == StateChange.Position ||
                IsStateChange == StateChange.Rotation ||
                IsStateChange == StateChange.Velocity
                ;
    }

    internal bool IsDamage()
    {
        return IsStateChange == StateChange.None &&
                    IsActivation == Activation.None &&
                    IsBuffRemove == BuffRemove.None &&
                    ((IsBuff != 0 && Value == 0) || (IsBuff == 0))
                    ;
    }

    internal bool IsDamagingDamage()
    {
        return IsStateChange == StateChange.None &&
                    IsActivation == Activation.None &&
                    IsBuffRemove == BuffRemove.None &&
                    ((IsBuff != 0 && Value == 0 && BuffDmg > 0) || (IsBuff == 0 && Value > 0))
                    ;
    }

    internal bool IsDamage(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.IsDamage(this);
        }
        return IsDamage();
    }

    internal bool IsDamagingDamage(IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out var handler))
        {
            return handler.IsDamagingDamage(this);
        }
        return IsDamagingDamage();
    }

    internal bool IsPhysicalDamage()
    {
        return IsStateChange == StateChange.None &&
                    IsActivation == Activation.None &&
                    IsBuffRemove == BuffRemove.None &&
                    IsBuff == 0
                    ;
    }

    internal bool IsBuffDamage()
    {
        return IsStateChange == StateChange.None &&
                    IsActivation == Activation.None &&
                    IsBuffRemove == BuffRemove.None &&
                    IsBuff != 0 && Value == 0
                    ;
    }

    internal bool SrcIsAgent()
    {
        return IsStateChange == StateChange.None
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
            || IsGeographical()
            || IsStateChange == StateChange.TeamChange
            || IsStateChange == StateChange.AttackTarget
            || IsStateChange == StateChange.Targetable
            || IsStateChange == StateChange.StackActive
            || IsStateChange == StateChange.StackReset
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
        return IsStateChange == StateChange.None
            || IsStateChange == StateChange.AttackTarget
            || IsStateChange == StateChange.BuffInitial
            || IsStateChange == StateChange.Effect_45
            || IsStateChange == StateChange.LogNPCUpdate
            || IsStateChange == StateChange.Effect_51
            || IsStateChange == StateChange.EffectAgentCreate
            || IsStateChange == StateChange.MissileLaunch
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

    internal bool IsBuffApply()
    {
        return (IsBuff != 0 && BuffDmg == 0 && Value > 0 && IsActivation == Activation.None && IsBuffRemove == BuffRemove.None && IsStateChange == StateChange.None) || IsStateChange == StateChange.BuffInitial;
    }

    internal bool IsBuffRemoval()
    {
        return IsStateChange == StateChange.None && IsActivation == Activation.None && IsBuffRemove != BuffRemove.None;
    }

    internal bool DstMatchesAgent(AgentItem agentItem, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (DstIsAgent(extensions))
        {
            return agentItem.Agent == DstAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool DstMatchesAgent(AgentItem agentItem)
    {
        if (DstIsAgent())
        {
            return agentItem.Agent == DstAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool SrcMatchesAgent(AgentItem agentItem, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (SrcIsAgent(extensions))
        {
            return agentItem.Agent == SrcAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool SrcMatchesAgent(AgentItem agentItem)
    {
        if (SrcIsAgent())
        {
            return agentItem.Agent == SrcAgent && agentItem.InAwareTimes(Time);
        }
        return false;
    }

    internal bool StartCasting()
    {
        if (IsStateChange != StateChange.None)
        {
            return false;
        }
        return IsActivation == Activation.Normal || IsActivation == Activation.Quickness;
    }

    internal bool EndCasting()
    {
        if (IsStateChange != StateChange.None)
        {
            return false;
        }
        return IsActivation == Activation.CancelFire || IsActivation == Activation.Reset || IsActivation == Activation.CancelCancel;
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
