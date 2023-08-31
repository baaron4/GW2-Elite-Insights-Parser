using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class SkillMechanic : IDBasedMechanic<AbstractHealthDamageEvent>
    {

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public SkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
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
