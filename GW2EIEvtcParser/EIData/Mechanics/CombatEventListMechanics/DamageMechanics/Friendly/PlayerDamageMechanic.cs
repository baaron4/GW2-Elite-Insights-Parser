using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerDamageMechanic : DamageMechanic
    {
        public PlayerDamageMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
        {
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (AbstractHealthDamageEvent c in GetEvents(log, p.AgentItem))
                {
                    if (Keep(c, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, p));
                    }
                }
            }
        }
    }
}
