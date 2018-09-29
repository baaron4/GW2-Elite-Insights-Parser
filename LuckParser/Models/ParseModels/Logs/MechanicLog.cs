namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        private readonly Mechanic _mechanic;

        public long Time { get; }
        public AbstractMasterPlayer Player { get; }
        public long Skill => _mechanic.SkillId;
        public string ShortName => _mechanic.ShortName;
        public string Description => _mechanic.Description;
        public string PlotlyShape => _mechanic.PlotlyShape;

        public MechanicLog(long time, Mechanic mechanic,
        AbstractMasterPlayer player)
        {
            Time = time;
            _mechanic = mechanic;
            Player = player;
        }
    }
}
