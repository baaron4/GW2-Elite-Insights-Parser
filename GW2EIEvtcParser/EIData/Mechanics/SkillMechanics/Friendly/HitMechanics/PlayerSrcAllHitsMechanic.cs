using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcAllHitsMechanic : SkillMechanic
    {

        public PlayerSrcAllHitsMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(0, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected override AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            throw new InvalidOperationException();
        }

        protected override AgentItem GetAgentItem(AbstractHealthDamageEvent ahde)
        {
            throw new InvalidOperationException();
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (AbstractHealthDamageEvent ahde in p.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    if (Keep(ahde, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(ahde.Time, this, p));
                    }
                }
            }
        }
    }
}
