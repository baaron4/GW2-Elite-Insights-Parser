using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public abstract class DamageLog
    {
        // Fields
        private long time;
        protected int damage;
        private long skill_id;
        private int buff;
        private ParseEnum.Result result;
        private ushort is_ninety;
        private ushort is_moving;
        private ushort is_flanking;
        private ParseEnum.Activation is_activation;
        private ushort is_shields;
        private ulong src_agent;
        private ushort src_instid;
        private ulong dst_agent;
        private ushort dst_instid;

        // Constructor
        public DamageLog(long time, CombatItem c)
        {
            this.time = time;
            this.skill_id = c.GetSkillID();
            this.buff = c.IsBuff();
            this.result = c.GetResult();
            this.is_ninety = c.IsNinety();
            this.is_moving = c.IsMoving();
            this.is_flanking = c.IsFlanking();
            this.is_activation = c.IsActivation();
            this.src_agent = c.GetSrcAgent();
            this.src_instid = c.GetSrcInstid();
            this.is_shields = c.IsShields();
            this.dst_agent = c.GetDstAgent();
            this.dst_instid = c.GetDstInstid();

        }
        // Getters
        public long GetTime()
        {
            return time;
        }

        public int GetDamage()
        {
            return damage;
        }

        public long GetID()
        {
            return skill_id;
        }

        public int IsCondi()
        {
            return buff;
        }

        public ParseEnum.Result GetResult()
        {
            return result;
        }

        public ushort IsNinety()
        {
            return is_ninety;
        }

        public ushort IsMoving()
        {
            return is_moving;
        }

        public ushort IsFlanking()
        {
            return is_flanking;
        }
        public ParseEnum.Activation IsActivation()
        {
            return is_activation;
        }
        public ushort IsShields() {
            return is_shields;
        }
        public ulong GetSrcAgent()
        {
            return src_agent;
        }
        public ushort GetSrcInstidt()
        {
            return src_instid;
        }

        public ulong GetDstAgent()
        {
            return dst_agent;
        }
        public ushort GetDstInstidt()
        {
            return dst_instid;
        }
    }
}