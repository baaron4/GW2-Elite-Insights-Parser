namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        // Fields
        private long time;
        private long skill_id;
        private string name;
        private int damage;
        private AbstractMasterPlayer player;
        private string plotlyShape;

        public MechanicLog(long time, long skill_id,string name, int damage, AbstractMasterPlayer player,string plotlyShape) {
            this.time = time;
            this.skill_id = skill_id;
            this.name = name;
            this.damage = damage;
            this.player = player;
            this.plotlyShape = plotlyShape;
        }
        //getters
        public long GetTime() {
            return time;
        }
        public long GetSkill()
        {
            return skill_id;
        }
        public string GetName() {
            return name;
        }
        public int GetDamage()
        {
            return damage;
        }
        public AbstractMasterPlayer GetPlayer()
        {
            return player;
        }
        public string GetPlotly() {
            return plotlyShape;
        }
    }
}
