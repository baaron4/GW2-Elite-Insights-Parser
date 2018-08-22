namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        // Fields
        private long time;
        private Mechanic mechanic;
        private AbstractMasterPlayer player;

        public MechanicLog(long time, Mechanic mechanic,
        AbstractMasterPlayer player)
        {
            this.time = time;
            this.mechanic = mechanic;
            this.player = player;
        }
        //getters
        public long GetTime()
        {
            return time;
        }
        public long GetSkill()
        {
            return mechanic.GetSkill();
        }
        public AbstractMasterPlayer GetPlayer()
        {
            return player;
        }
        public string GetName()
        {
            return mechanic.GetName();
        }
        public string GetPlotly()
        {
            return mechanic.GetPlotly();
        }
    }
}
