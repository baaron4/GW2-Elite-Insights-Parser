using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class SkillMechanic : IDBasedMechanic
    {
        public delegate bool SkillChecker(AbstractHealthDamageEvent d, ParsedEvtcLog log);

        private readonly SkillChecker _triggerCondition = null;

        protected virtual bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            return (_triggerCondition == null || _triggerCondition(c, log));
        }

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public SkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        protected abstract AgentItem GetAgentItem(AbstractHealthDamageEvent ahde);

        protected abstract AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs);

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long skillID in MechanicIDs)
            {
                foreach (AbstractHealthDamageEvent ahde in log.CombatData.GetDamageData(skillID))
                {
                    AbstractSingleActor amp = null;
                    if (Keep(ahde, log))
                    {
                        amp = GetActor(log, GetAgentItem(ahde), regroupedMobs);
                    }
                    if (amp != null)
                    {
                        mechanicLogs[this].Add(new MechanicEvent(ahde.Time, this, amp));
                    }
                }
            }
        }

    }
}
