using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcAllHitsMechanic : SkillMechanic
    {

        private bool Minions { get; set; } = false;

        public PlayerSrcAllHitsMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(0, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerSrcAllHitsMechanic WithMinions(bool minions)
        {
            Minions = minions;
            return this;
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
                        mechanicLogs[this].Add(new MechanicEvent(ahde.Time, this, p));
                    }
                }
            }
        }
    }
}
