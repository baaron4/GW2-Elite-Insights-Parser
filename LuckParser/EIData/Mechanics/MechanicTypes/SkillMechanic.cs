using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{

    public abstract class SkillMechanic : Mechanic
    {
        public delegate bool SkillChecker(AbstractDamageEvent d, ParsedLog log);

        private readonly SkillChecker _triggerCondition = null;

        protected virtual bool Keep(AbstractDamageEvent c, ParsedLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}