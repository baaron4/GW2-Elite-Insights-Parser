using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class Mechanic
    {
        /// <summary>
        /// PlayerBoon 
        /// BossBoon 
        /// PlayerSkill
        /// SkillOnPlayer
        /// EnemyBoonStrip
        /// Spawn
        /// BossCast
        /// PlayerOnPlayer
        /// HitOnEnemy
        /// PlayerStatus
        /// EnemyCastStart
        /// EnemyCastEnd -> put skill id the same but negative so that you can also track the Start of the same skill if you want
        /// </summary>
        public enum MechType { PlayerBoon, PlayerBoonRemove, EnemyBoon, SkillOnPlayer, PlayerSkill, EnemyBoonStrip, Spawn, PlayerOnPlayer, HitOnEnemy, PlayerStatus, EnemyCastStart, EnemyCastEnd }

        public delegate bool SpecialCondition(long value);
        // Fields

        private long skill_id;
        private string name;
        private string altname;
        private MechType mechType;       
        private ParseEnum.BossIDS bossid;
        private string plotlyShape;
        private int internalCoolDown;
        private SpecialCondition condition;

        
        public Mechanic(long skill_id, string name, MechType mechtype, ParseEnum.BossIDS bossid, string plotlyShape,string altname, int internalCoolDown, SpecialCondition condition = null)
        {
            this.skill_id = skill_id;
            this.name = name;
            this.mechType = mechtype;
            this.bossid = bossid;
            this.plotlyShape = plotlyShape;
            this.altname = altname;
            this.internalCoolDown = internalCoolDown;
            this.condition = condition;
        }
        //getters
       
        public long GetSkill()
        {
            return skill_id;
        }
        public SpecialCondition GetSpecialCondition()
        {
            return condition;
        }
        public string GetName()
        {
            return name;
        }
        public MechType GetMechType()
        {
            return mechType;
        }
        public ParseEnum.BossIDS GetBossID() {
            return bossid;
        }

        public string GetPlotly()
        {
            return plotlyShape;
        }
        public string GetAltName()
        {
            return altname;
        }
        public int GetICD()
        {
            return internalCoolDown;
        }

        public bool isEnemyMechanic()
        {
            return mechType == MechType.EnemyBoon || mechType == MechType.EnemyBoonStrip || mechType == MechType.EnemyCastEnd || mechType == MechType.EnemyCastStart || mechType == MechType.Spawn;
        }
    }
}
