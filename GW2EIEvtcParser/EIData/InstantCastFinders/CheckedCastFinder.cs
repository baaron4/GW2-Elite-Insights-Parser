using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class CheckedCastFinder<Event> : InstantCastFinder
    {
        public delegate bool Checker(Event evt, CombatData combatData, AgentData agentData, SkillData skillData);
        protected List<Checker> _checkers { get; set; }

        protected CheckedCastFinder(long skillID) : base(skillID)
        {
            _checkers = new List<Checker>();
        }

        internal CheckedCastFinder<Event> UsingChecker(Checker checker)
        {
            _checkers.Add(checker);
            return this;
        }

        protected bool CheckCondition(Event evt, CombatData combatData, AgentData agentData, SkillData skillData)
        {
            return _checkers.All(checker => checker(evt, combatData, agentData, skillData));
        }
    }
}
