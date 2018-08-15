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
            this.skill_id = c.getSkillID();
            this.buff = c.isBuff();
            this.result = c.getResult();
            this.is_ninety = c.isNinety();
            this.is_moving = c.isMoving();
            this.is_flanking = c.isFlanking();
            this.is_activation = c.isActivation();
            this.src_agent = c.getSrcAgent();
            this.src_instid = c.getSrcInstid();
            this.is_shields = c.isShields();
            this.dst_agent = c.getDstAgent();
            this.dst_instid = c.getDstInstid();

        }
        // Getters
        public long getTime()
        {
            return time;
        }

        public int getDamage()
        {
            return damage;
        }

        public long getID()
        {
            return skill_id;
        }

        public int isCondi()
        {
            return buff;
        }

        public ParseEnum.Result getResult()
        {
            return result;
        }

        public ushort isNinety()
        {
            return is_ninety;
        }

        public ushort isMoving()
        {
            return is_moving;
        }

        public ushort isFlanking()
        {
            return is_flanking;
        }
        public ParseEnum.Activation isActivation()
        {
            return is_activation;
        }
        public ushort isShields() {
            return is_shields;
        }
        public ulong getSrcAgent()
        {
            return src_agent;
        }
        public ushort getSrcInstidt()
        {
            return src_instid;
        }

        public ulong getDstAgent()
        {
            return dst_agent;
        }
        public ushort getDstInstidt()
        {
            return dst_instid;
        }
    }
}