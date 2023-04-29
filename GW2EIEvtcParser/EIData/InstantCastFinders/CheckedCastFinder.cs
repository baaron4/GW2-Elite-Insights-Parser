using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class CheckedCastFinder<Event> : InstantCastFinder
    {
        public delegate bool Checker(Event evt, CombatData combatData, AgentData agentData, SkillData skillData);
        protected Checker _condition { get; set; }

        protected CheckedCastFinder(long skillID) : base(skillID)
        {
        }

        internal CheckedCastFinder<Event> UsingChecker(Checker checker)
        {
            _condition = checker;
            return this;
        }

        protected bool CheckCondition(Event evt, CombatData combatData, AgentData agentData, SkillData skillData)
        {
            return _condition == null || _condition(evt, combatData, agentData, skillData);
        }
    }
}
