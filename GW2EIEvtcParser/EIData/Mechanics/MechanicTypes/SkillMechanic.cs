using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class SkillMechanic : Mechanic
    {
        public delegate bool SkillChecker(AbstractHealthDamageEvent d, ParsedEvtcLog log);

        private readonly SkillChecker _triggerCondition = null;

        protected virtual bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
