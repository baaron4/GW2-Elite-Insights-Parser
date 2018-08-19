using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class Mechanic
    {
        /// <summary>
        /// PlayerBoon 
        /// BossBoon 
        /// PlayerSkill
        /// SkillOnPlayer -> put long.MaxValue in expectedValue to consider only damaging hits
        /// EnemyBoonStrip
        /// Spawn
        /// BossCast
        /// PlayerOnPlayer
        /// HitOnEnemy
        /// PlayerStatus
        /// EnemyCastStart
        /// EnemyCastEnd
        /// </summary>
        public enum MechType { PlayerBoon, EnemyBoon, SkillOnPlayer, PlayerSkill, EnemyBoonStrip, Spawn, BossCast, PlayerOnPlayer, HitOnEnemy, PlayerStatus, EnemyCastStart, EnemyCastEnd }
        // Fields
       
        private long skill_id;
        private string name;
        private string altname;
        private MechType mechType;       
        private ParseEnum.BossIDS bossid;
        private string plotlyShape;
        private int internalCoolDown;
        private long expectedValue;

        
        public Mechanic(long skill_id, string name, MechType mechtype, ParseEnum.BossIDS bossid, string plotlyShape,string friendlyName, int ICD, long expectedValue = -1)
        {
            this.skill_id = skill_id;
            this.name = name;
            this.mechType = mechtype;
            this.bossid = bossid;
            this.plotlyShape = plotlyShape;
            this.altname = friendlyName;
            this.internalCoolDown = ICD;
            this.expectedValue = expectedValue;
        }
        //getters
       
        public long GetSkill()
        {
            return skill_id;
        }
        public long GetExpectedValue()
        {
            return expectedValue;
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
    }
}
