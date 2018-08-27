namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        // Fields
        private readonly long _time;
        private readonly Mechanic _mechanic;
        private readonly AbstractMasterPlayer _player;

        public MechanicLog(long time, Mechanic mechanic,
        AbstractMasterPlayer player)
        {
            _time = time;
            _mechanic = mechanic;
            _player = player;
        }
        //getters
        public long GetTime()
        {
            return _time;
        }
        public long GetSkill()
        {
            return _mechanic.GetSkill();
        }
        public AbstractMasterPlayer GetPlayer()
        {
            return _player;
        }
        public string GetDescription()
        {
            return _mechanic.GetDescription();
        }
        public string GetPlotly()
        {
            return _mechanic.GetPlotly();
        }
    }
}
