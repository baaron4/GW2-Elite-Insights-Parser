using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class CombatItem
    {
        public long Time { get; }
        public ulong SrcAgent { get; set; }
        public ulong DstAgent { get; set; }
        public int Value { get; }
        public int BuffDmg { get; }
        public uint OverstackValue { get; }
        public long SkillID { get; }
        public ushort SrcInstid { get; set; }
        public ushort DstInstid { get; set; }
        public ushort SrcMasterInstid { get; }
        public ushort DstMasterInstid { get; }
        public ParseEnum.IFF IFF { get; }
        public ushort IsBuff { get; }
        public ParseEnum.Result Result { get; }
        public ParseEnum.Activation IsActivation { get; }
        public ParseEnum.BuffRemove IsBuffRemove { get; }
        public ushort IsNinety { get; }
        public ushort IsFifty { get; }
        public ushort IsMoving { get; }
        public ParseEnum.StateChange IsStateChange { get; }
        public ushort IsFlanking { get; }
        public ushort IsShields { get; }

        // Constructor
        public CombatItem(long time, ulong srcAgent, ulong dstAgent, int value, int buffDmg, uint overstackValue,
               long skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid, ushort dstMasterInstid, ParseEnum.IFF iff, ushort isBuff, ParseEnum.Result result,
               ParseEnum.Activation isActivation, ParseEnum.BuffRemove isBuffRemove, ushort isNinety, ushort isFifty, ushort isMoving,
               ParseEnum.StateChange isStateChange, ushort isFlanking, ushort isShields)
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
            Result = result;
            IsActivation = isActivation;
            IsBuffRemove = isBuffRemove;
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoving = isMoving;
            IsStateChange = isStateChange;
            IsFlanking = isFlanking;
            IsShields = isShields;
        }
    }
}