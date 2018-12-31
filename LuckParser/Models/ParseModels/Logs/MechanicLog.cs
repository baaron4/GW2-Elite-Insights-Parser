namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        private readonly Mechanic _mechanic;

        public long Time { get; }
        public AbstractActor Actor { get; }
        public long Skill => _mechanic.SkillId;
        public string ShortName => _mechanic.ShortName;
        public string Description => _mechanic.Description;
        public string InGameName => _mechanic.InGameName;
        public bool Enemy => _mechanic.IsEnemyMechanic;

        public MechanicLog(long time, Mechanic mechanic,
        AbstractActor actor)
        {
            Time = time;
            _mechanic = mechanic;
            Actor = actor;
        }
    }
}
