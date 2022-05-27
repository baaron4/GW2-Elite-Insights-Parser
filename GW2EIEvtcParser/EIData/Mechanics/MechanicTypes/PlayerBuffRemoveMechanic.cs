using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerBuffRemoveMechanic : BuffRemoveMechanic
    {
        public PlayerBuffRemoveMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerBuffRemoveMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (long mechanicID in MechanicIDs)
                {
                    foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(mechanicID))
                    {
                        if (c is BuffRemoveAllEvent rae && p.AgentItem == rae.To && Keep(rae, log))
                        {
                            mechanicLogs[this].Add(new MechanicEvent(rae.Time, this, p));
                        }
                    }
                }
                
            }
        }
    }
}
