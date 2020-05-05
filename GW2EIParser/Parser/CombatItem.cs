using System;
using System.Collections.Generic;

namespace GW2EIParser.Parser
{
    public class CombatItem
    {
        public long Time { get; private set; }
        public ulong SrcAgent { get; private set; }
        public ulong DstAgent { get; private set; }
        public int Value { get; private set; }
        public int BuffDmg { get; }
        public uint OverstackValue { get; }
        public long SkillID { get; private set; }
        public ushort SrcInstid { get; }
        public ushort DstInstid { get; }
        public ushort SrcMasterInstid { get; }
        public ushort DstMasterInstid { get; }
        public ParseEnum.IFF IFF { get; }
        public byte IsBuff { get; }
        public byte Result { get; }
        public ParseEnum.Activation IsActivation { get; }
        public ParseEnum.BuffRemove IsBuffRemove { get; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public ParseEnum.StateChange IsStateChange { get; }
        public byte IsFlanking { get; }
        public byte IsShields { get; }
        public byte IsOffcycle { get; }

        public uint Pad { get; }

        // Constructor
        public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
               long skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid,
               ushort dstMasterInstid, ParseEnum.IFF iff, byte isBuff,
               byte result, ParseEnum.Activation isActivation,
               ParseEnum.BuffRemove isBuffRemove, byte isNinety, byte isFifty, byte isMoving,
               ParseEnum.StateChange isStateChange, byte isFlanking, byte isShields, byte isOffcycle, uint pad)
        {
            this.Time = time;
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
            IFF = iff;
            IsBuff = isBuff;
            Result = result;
            IsActivation = isActivation;
            IsBuffRemove = isBuffRemove;
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoving = isMoving;
            IsStateChange = isStateChange;
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffcycle = isOffcycle;
            Pad = pad;
        }

        public CombatItem(CombatItem c)
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
            IFF = c.IFF;
            IsBuff = c.IsBuff;
            Result = c.Result;
            IsActivation = c.IsActivation;
            IsBuffRemove = c.IsBuffRemove;
            IsNinety = c.IsNinety;
            IsFifty = c.IsFifty;
            IsMoving = c.IsMoving;
            IsStateChange = c.IsStateChange;
            IsFlanking = c.IsFlanking;
            IsShields = c.IsShields;
            IsOffcycle = c.IsOffcycle;
            Pad = c.Pad;
        }


        public void OverrideTime(long time)
        {
            if (IsStateChange.HasTime())
            {
                Time = time;
            }
        }


        public byte[] BreakPad()
        {
            return BitConverter.GetBytes(Pad);
        }

        public void OverrideSrcAgent(ulong agent)
        {
            SrcAgent = agent;
        }

        public void OverrideDstAgent(ulong agent)
        {
            DstAgent = agent;
        }

        public void OverrideValue(int value)
        {
            Value = value;
        }
    }
}
