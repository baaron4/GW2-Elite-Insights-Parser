using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class PlayerBuffApplyMechanic : BuffApplyMechanic
    {
        public PlayerBuffApplyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerBuffApplyMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected abstract AgentItem GetAgentItem(BuffApplyEvent ba);

        protected virtual void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, Player p)
        {
            mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, p));
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (long mechanicID in MechanicIDs)
                {
                    foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(mechanicID))
                    {
                        if (c is BuffApplyEvent ba && p.AgentItem == GetAgentItem(ba) && Keep(ba, log))
                        {
                            AddMechanic(log, mechanicLogs, ba, p);
                        }
                    }
                }             
            }
        }
    }
}
