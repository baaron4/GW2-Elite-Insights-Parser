using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcAllHitsMechanic : PlayerSrcHitMechanic
    {

        public PlayerSrcAllHitsMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(0, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
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
                foreach (AbstractHealthDamageEvent ahde in (Minions ? p.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd) : p.GetJustActorDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd)))
                {
                    if (Keep(ahde, log))
                    {
                        InsertMechanic(log, mechanicLogs, ahde.Time, p);
                    }
                }
            }
        }
    }
}
