using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class CombatItem
    {
        public long Time { get; set; }
        public ulong SrcAgent { get; set; }
        public ulong DstAgent { get; set; }
        public int Value { get; }
        public int BuffDmg { get; }
        public uint OverstackValue { get; }
        public long SkillID { get; }
        public ushort SrcInstid { get; set; }
        public ushort DstInstid { get; set; }
        public ushort SrcMasterInstid { get; set; }
        public ushort DstMasterInstid { get; set; }
        public ParseEnum.IFF IFF { get; }
        public byte IsBuff { get; }
        public byte NonEnumResult { get; }
        public ParseEnum.Result Result => ParseEnum.GetResult(NonEnumResult);
        public ParseEnum.Activation IsActivation { get; }
        public ParseEnum.BuffRemove IsBuffRemove { get; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public ParseEnum.StateChange IsStateChange { get; }
        public byte IsFlanking { get; }
        public byte IsShields { get; }
        public byte IsOffcycle { get; }

        // Constructor
        public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
               long skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid, ushort dstMasterInstid, ParseEnum.IFF iff, byte isBuff, byte result,
               ParseEnum.Activation isActivation, ParseEnum.BuffRemove isBuffRemove, byte isNinety, byte isFifty, byte isMoving,
               ParseEnum.StateChange isStateChange, byte isFlanking, byte isShields, byte isOffcycle)
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
            IFF = iff;
            IsBuff = isBuff;
            NonEnumResult = result;
            IsActivation = isActivation;
            IsBuffRemove = isBuffRemove;
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoving = isMoving;
            IsStateChange = isStateChange;
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffcycle = isOffcycle;
        }
    }
}