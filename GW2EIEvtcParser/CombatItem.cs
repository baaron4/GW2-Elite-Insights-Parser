using System;
using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser
{
    internal class CombatItem
    {
        public long Time { get; private set; }
        public ulong SrcAgent { get; private set; }
        public ulong DstAgent { get; private set; }
        public int Value { get; private set; }
        public int BuffDmg { get; }
        public uint OverstackValue { get; }
        public uint SkillID { get; private set; }
        public ushort SrcInstid { get; }
        public ushort DstInstid { get; }
        public ushort SrcMasterInstid { get; }
        public ushort DstMasterInstid { get; }
        public byte IFFByte { get; }
        public ArcDPSEnums.IFF IFF { get; }
        public byte IsBuff { get; }
        public byte Result { get; }
        public byte IsActivationByte { get; }
        public ArcDPSEnums.Activation IsActivation { get; }
        public byte IsBuffRemoveByte { get; }
        public ArcDPSEnums.BuffRemove IsBuffRemove { get; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public ArcDPSEnums.StateChange IsStateChange { get; }
        public byte IsFlanking { get; }
        public byte IsShields { get; }
        public byte IsOffcycle { get; }

        public uint Pad { get; }
        public byte Pad1 { get; }
        public byte Pad2 { get; }
        public byte Pad3 { get; }
        public byte Pad4 { get; }

        public bool IsExtension => IsStateChange == ArcDPSEnums.StateChange.Extension || IsStateChange == ArcDPSEnums.StateChange.ExtensionCombat;

        public bool IsEffect => IsStateChange == ArcDPSEnums.StateChange.Effect_51 || IsStateChange == ArcDPSEnums.StateChange.Effect_45;

        // Constructor
        internal CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
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
            IFF = ArcDPSEnums.GetIFF(iff);
            IsBuff = isBuff;
            Result = result;
            IsActivationByte = isActivation;
            IsActivation = ArcDPSEnums.GetActivation(isActivation);
            IsBuffRemoveByte = isBuffRemove;
            IsBuffRemove = ArcDPSEnums.GetBuffRemove(isBuffRemove);
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoving = isMoving;
            IsStateChange = ArcDPSEnums.GetStateChange(isStateChange);
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffcycle = isOffcycle;
            Pad = pad;
            // break pad
            byte[] pads = BitConverter.GetBytes(Pad);
            Pad1 = pads[0];
            Pad2 = pads[1];
            Pad3 = pads[2];
            Pad4 = pads[3];
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
                || IsStateChange == ArcDPSEnums.StateChange.Reward
                || IsStateChange == ArcDPSEnums.StateChange.TickRate
                || IsStateChange == ArcDPSEnums.StateChange.SquadMarker
                || IsStateChange == ArcDPSEnums.StateChange.InstanceStart
                ;
        }

        internal bool HasTime(IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out AbstractExtensionHandler handler))
            {
                return handler.HasTime(this);
            }
            return HasTime();
        }

        internal bool IsDamage()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None &&
                        IsActivation == ArcDPSEnums.Activation.None &&
                        IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                        ((IsBuff != 0 && Value == 0) || (IsBuff == 0))
                        ;
        }

        internal bool IsDamagingDamage()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None &&
                        IsActivation == ArcDPSEnums.Activation.None &&
                        IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                        ((IsBuff != 0 && Value == 0 && BuffDmg > 0) || (IsBuff == 0 && Value > 0))
                        ;
        }

        internal bool IsDamage(IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out AbstractExtensionHandler handler))
            {
                return handler.IsDamage(this);
            }
            return IsDamage();
        }

        internal bool IsDamagingDamage(IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out AbstractExtensionHandler handler))
            {
                return handler.IsDamagingDamage(this);
            }
            return IsDamagingDamage();
        }

        internal bool IsPhysicalDamage()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None &&
                        IsActivation == ArcDPSEnums.Activation.None &&
                        IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                        IsBuff == 0
                        ;
        }

        internal bool IsBuffDamage()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None &&
                        IsActivation == ArcDPSEnums.Activation.None &&
                        IsBuffRemove == ArcDPSEnums.BuffRemove.None &&
                        IsBuff != 0 && Value == 0
                        ;
        }

        internal bool SrcIsAgent()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None
                || IsStateChange == ArcDPSEnums.StateChange.EnterCombat
                || IsStateChange == ArcDPSEnums.StateChange.ExitCombat
                || IsStateChange == ArcDPSEnums.StateChange.ChangeUp
                || IsStateChange == ArcDPSEnums.StateChange.ChangeDead
                || IsStateChange == ArcDPSEnums.StateChange.ChangeDown
                || IsStateChange == ArcDPSEnums.StateChange.Spawn
                || IsStateChange == ArcDPSEnums.StateChange.Despawn
                || IsStateChange == ArcDPSEnums.StateChange.HealthUpdate
                || IsStateChange == ArcDPSEnums.StateChange.WeaponSwap
                || IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate
                || IsStateChange == ArcDPSEnums.StateChange.PointOfView
                || IsStateChange == ArcDPSEnums.StateChange.BuffInitial
                || IsStateChange == ArcDPSEnums.StateChange.Position
                || IsStateChange == ArcDPSEnums.StateChange.Velocity
                || IsStateChange == ArcDPSEnums.StateChange.Rotation
                || IsStateChange == ArcDPSEnums.StateChange.TeamChange
                || IsStateChange == ArcDPSEnums.StateChange.AttackTarget
                || IsStateChange == ArcDPSEnums.StateChange.Targetable
                || IsStateChange == ArcDPSEnums.StateChange.StackActive
                || IsStateChange == ArcDPSEnums.StateChange.StackReset
                || IsStateChange == ArcDPSEnums.StateChange.Guild
                || IsStateChange == ArcDPSEnums.StateChange.BreakbarState
                || IsStateChange == ArcDPSEnums.StateChange.BreakbarPercent
                || IsStateChange == ArcDPSEnums.StateChange.Marker
                || IsStateChange == ArcDPSEnums.StateChange.BarrierUpdate
                || IsStateChange == ArcDPSEnums.StateChange.Last90BeforeDown
                || IsStateChange == ArcDPSEnums.StateChange.Effect_45
                || IsStateChange == ArcDPSEnums.StateChange.Effect_51
                || IsStateChange == ArcDPSEnums.StateChange.Glider
                || IsStateChange == ArcDPSEnums.StateChange.StunBreak
                ;
        }

        internal bool SrcIsAgent(IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out AbstractExtensionHandler handler))
            {
                return handler.SrcIsAgent(this);
            }
            return SrcIsAgent();
        }

        internal bool DstIsAgent()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None
                || IsStateChange == ArcDPSEnums.StateChange.AttackTarget
                || IsStateChange == ArcDPSEnums.StateChange.BuffInitial
                || IsStateChange == ArcDPSEnums.StateChange.Effect_45
                || IsStateChange == ArcDPSEnums.StateChange.LogNPCUpdate
                || IsStateChange == ArcDPSEnums.StateChange.Effect_51
            ;
        }

        internal bool DstIsAgent(IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            if (IsExtension && Pad != 0 && extensions.TryGetValue(Pad, out AbstractExtensionHandler handler))
            {
                return handler.DstIsAgent(this);
            }
            return DstIsAgent();
        }

        internal bool IsBuffApply()
        {
            return (IsBuff != 0 && BuffDmg == 0 && Value > 0 && IsActivation == ArcDPSEnums.Activation.None && IsBuffRemove == ArcDPSEnums.BuffRemove.None && IsStateChange == ArcDPSEnums.StateChange.None) || IsStateChange == ArcDPSEnums.StateChange.BuffInitial;
        }

        internal bool IsBuffRemoval()
        {
            return IsStateChange == ArcDPSEnums.StateChange.None && IsActivation == ArcDPSEnums.Activation.None && IsBuffRemove != ArcDPSEnums.BuffRemove.None;
        }

        internal bool DstMatchesAgent(AgentItem agentItem, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
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

        internal bool SrcMatchesAgent(AgentItem agentItem, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
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
            if (IsStateChange != ArcDPSEnums.StateChange.None)
            {
                return false;
            }
            return IsActivation == ArcDPSEnums.Activation.Normal || IsActivation == ArcDPSEnums.Activation.Quickness;
        }

        internal bool EndCasting()
        {
            if (IsStateChange != ArcDPSEnums.StateChange.None)
            {
                return false;
            }
            return IsActivation == ArcDPSEnums.Activation.CancelFire || IsActivation == ArcDPSEnums.Activation.Reset || IsActivation == ArcDPSEnums.Activation.CancelCancel;
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
}
