using System;

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
        public uint SkillID { get; private set; }
        public ushort SrcInstid { get; }
        public ushort DstInstid { get; }
        public ushort SrcMasterInstid { get; }
        public ushort DstMasterInstid { get; }
        public byte IFF { get; }
        public ParseEnum.IFF IFFEnum { get; }
        public byte IsBuff { get; }
        public byte Result { get; }
        public byte IsActivation { get; }
        public ParseEnum.Activation IsActivationEnum { get; }
        public byte IsBuffRemove { get; }
        public ParseEnum.BuffRemove IsBuffRemoveEnum { get; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public byte IsStateChange { get; }
        public ParseEnum.StateChange IsStateChangeEnum { get; }
        public byte IsFlanking { get; }
        public byte IsShields { get; }
        public byte IsOffcycle { get; }

        public uint Pad { get; }
        public byte Pad1 { get; }
        public byte Pad2 { get; }
        public byte Pad3 { get; }
        public byte Pad4 { get; }

        // Constructor
        public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
               uint skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid,
               ushort dstMasterInstid, byte iff, byte isBuff,
               byte result, byte isActivation,
               byte isBuffRemove, byte isNinety, byte isFifty, byte isMoving,
               byte isStateChange, byte isFlanking, byte isShields, byte isOffcycle, uint pad)
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
            IFFEnum = ParseEnum.GetIFF(iff);
            IsBuff = isBuff;
            Result = result;
            IsActivation = isActivation;
            IsActivationEnum = ParseEnum.GetActivation(isActivation);
            IsBuffRemove = isBuffRemove;
            IsBuffRemoveEnum = ParseEnum.GetBuffRemove(isBuffRemove);
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoving = isMoving;
            IsStateChange = isStateChange;
            IsStateChangeEnum = ParseEnum.GetStateChange(isStateChange);
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffcycle = isOffcycle;
            Pad = pad;
            // break pad
            var pads = BitConverter.GetBytes(Pad);
            Pad1 = pads[0];
            Pad2 = pads[1];
            Pad3 = pads[2];
            Pad4 = pads[3];
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
            IFFEnum = c.IFFEnum;
            IsBuff = c.IsBuff;
            Result = c.Result;
            IsActivation = c.IsActivation;
            IsActivationEnum = c.IsActivationEnum;
            IsBuffRemove = c.IsBuffRemove;
            IsBuffRemoveEnum = c.IsBuffRemoveEnum;
            IsNinety = c.IsNinety;
            IsFifty = c.IsFifty;
            IsMoving = c.IsMoving;
            IsStateChange = c.IsStateChange;
            IsStateChangeEnum = c.IsStateChangeEnum;
            IsFlanking = c.IsFlanking;
            IsShields = c.IsShields;
            IsOffcycle = c.IsOffcycle;
            Pad = c.Pad;
            Pad1 = c.Pad1;
            Pad2 = c.Pad2;
            Pad3 = c.Pad3;
            Pad4 = c.Pad4;
        }


        public void OverrideTime(long time)
        {
            if (IsStateChangeEnum.HasTime())
            {
                Time = time;
            }
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
