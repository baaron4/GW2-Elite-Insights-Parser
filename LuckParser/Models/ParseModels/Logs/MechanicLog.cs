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
        public string PlotlyColor => _mechanic.PlotlyColor;
        public string PlotlySymbol => _mechanic.PlotlySymbol;
        public string PlotlySize => _mechanic.PlotlySize;
        public string PlotlyName => _mechanic.PlotlyName;
        public string InGameName => _mechanic.InGameName;
        public bool Enemy => _mechanic.IsEnemyMechanic;

        public MechanicLog(long time, Mechanic mechanic,
        AbstractMasterPlayer player)
        {
            Time = time;
            _mechanic = mechanic;
            Player = player;
        }
    }
}
