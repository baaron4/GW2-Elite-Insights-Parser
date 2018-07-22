namespace LuckParser.Models.ParseModels
{
    public class Mechanic
    {
        //X indicates not gathering mechanic in mech logs
        /// <summary>
        /// 0 - PlayerBoon 
        /// 1 - BossBoon 
        /// 2 - PlayerSkill X
        /// 3 - SkillOnPlayer
        /// 4 - EnemyBoonStrip
        /// 5 - Spawn X
        /// 6 - BossCast X
        /// 7 - PlayerOnPlayer
        /// </summary>
        public enum MechType { PlayerBoon, EnemyBoon, SkillOnPlayer, PlayerSkill, EnemyBoonStrip, Spawn, BossCast, PlayerOnPlayer }
        // Fields
       
        private int skill_id;
        private string name;
        private string altname;
        private MechType mechType;       
        private ushort bossid;
        private string plotlyShape;
        private float InternalCoolDown;

        public Mechanic( int skill_id, string name, MechType mechtype, ushort bossid, string plotlyShape,string friendlyName,float ICD)
        {
            this.skill_id = skill_id;
            this.name = name;
            this.mechType = mechtype;
            this.bossid = bossid;
            this.plotlyShape = plotlyShape;
            this.altname = friendlyName;
            this.InternalCoolDown = ICD;
        }
        //getters
       
        public int GetSkill()
        {
            return skill_id;
        }
        public string GetName()
        {
            return name;
        }
        public MechType GetMechType()
        {
            return mechType;
        }
        public ushort GetBossID() {
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
        public float GetICD()
        {
            return InternalCoolDown;
        }
    }
}
