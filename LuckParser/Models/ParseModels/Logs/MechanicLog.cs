namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        private readonly Mechanic _mechanic;

        public long Time { get; }
        public AbstractMasterPlayer Player { get; }
        public long Skill => _mechanic.GetSkill();
        public string Description => _mechanic.GetDescription();
        public string Plotly => _mechanic.GetPlotly();

        public MechanicLog(long time, Mechanic mechanic,
        AbstractMasterPlayer player)
        {
            Time = time;
            _mechanic = mechanic;
            Player = player;
        }
    }
}
