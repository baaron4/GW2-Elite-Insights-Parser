using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageLog
    {
        public long Time { get; }
        public int Damage { get; protected set; }
        public long SkillId { get; }
        public int IsCondi { get; }
        public ParseEnum.Result Result { get; }
        public ushort IsNinety { get; }
        public ushort IsMoving { get; }
        public ushort IsFlanking { get; }
        public ParseEnum.Activation Activation { get; }
        public ushort IsShields { get; }
        public ulong SrcAgent { get; }
        public ushort SrcInstId { get; }
        public ulong DstAgent { get; }
        public ushort DstInstId { get; }

        protected DamageLog(long time, CombatItem c)
        {
            Time = time;
            SkillId = c.SkillID;
            IsCondi = c.IsBuff;
            Result = c.Result;
            IsNinety = c.IsNinety;
            IsMoving = c.IsMoving;
            IsFlanking = c.IsFlanking;
            Activation = c.IsActivation;
            SrcAgent = c.SrcAgent;
            SrcInstId = c.SrcInstid;
            IsShields = c.IsShields;
            DstAgent = c.DstAgent;
            DstInstId = c.DstInstid;
        }
    }
}