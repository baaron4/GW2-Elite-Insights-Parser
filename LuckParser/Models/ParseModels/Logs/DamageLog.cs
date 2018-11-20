using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageLog
    {
        public long Time { get; }
        public int Damage { get; protected set; }
        public long SkillId { get; }
        public byte IsCondi { get; }
        public ParseEnum.Result Result { get; }
        public byte IsNinety { get; }
        public byte IsFifty { get; }
        public byte IsMoving { get; }
        public byte IsFlanking { get; }
        public ParseEnum.Activation Activation { get; }
        public ulong SrcAgent { get; }
        public ushort SrcInstId { get; }
        public ulong DstAgent { get; }
        public ushort DstInstId { get; }
        public uint ShieldDamage { get; }

        protected DamageLog(long time, CombatItem c)
        {
            Time = time;
            SkillId = c.SkillID;
            IsCondi = c.IsBuff;
            Result = c.Result;
            IsNinety = c.IsNinety;
            IsFifty = c.IsFifty;
            IsMoving = c.IsMoving;
            IsFlanking = c.IsFlanking;
            Activation = c.IsActivation;
            SrcAgent = c.SrcAgent;
            SrcInstId = c.SrcInstid;
            DstAgent = c.DstAgent;
            DstInstId = c.DstInstid;
            ShieldDamage = c.IsShields > 0 ? c.OverstackValue : 0;
        }
    }
}