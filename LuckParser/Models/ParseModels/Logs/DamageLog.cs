using LuckParser.Parser;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageLog
    {
        public long Time { get; }
        public int Damage { get; protected set; }
        public long SkillId { get; }
        public bool IsIndirectDamage { get; protected set; }
        public bool IsCondi { get; protected set; }
        public ParseEnum.Result Result { get; }
        public bool IsNinety { get; }
        public bool IsFifty { get; }
        public bool IsMoving { get; }
        public bool IsFlanking { get; }
        public ulong SrcAgent { get; }
        public ushort SrcInstId { get; }
        public ulong DstAgent { get; }
        public ushort DstInstId { get; }
        public int ShieldDamage { get; }

        protected DamageLog(long time, CombatItem c)
        {
            Time = time;
            SkillId = c.SkillID;
            Result = c.ResultEnum;
            IsNinety = c.IsNinety > 0;
            IsFifty = c.IsFifty > 0;
            IsMoving = c.IsMoving > 0;
            IsFlanking = c.IsFlanking > 0;
            SrcAgent = c.SrcAgent;
            SrcInstId = c.SrcInstid;
            DstAgent = c.DstAgent;
            DstInstId = c.DstInstid;
            ShieldDamage = c.IsShields > 0 ? c.OverstackValue > 0 ? (int)c.OverstackValue : c.Value : 0;
        }
    }
}