using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class MechanicEvent : AbstractCombatEvent
    {
        private readonly Mechanic _mechanic;
        public AbstractActor Actor { get; }
        public long Skill => _mechanic.SkillId;
        public string ShortName => _mechanic.ShortName;
        public string Description => _mechanic.Description;
        public string InGameName => _mechanic.InGameName;
        public bool Enemy => _mechanic.IsEnemyMechanic;

        public MechanicEvent(long time, Mechanic mech, AbstractActor actor) : base(time, 0)
        {
            Actor = actor;
            _mechanic = mech;
        }
    }
}
