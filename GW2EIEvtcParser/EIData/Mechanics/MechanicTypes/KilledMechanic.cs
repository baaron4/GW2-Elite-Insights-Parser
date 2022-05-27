using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class KilledMechanic : IDBasedMechanic
    {

        public KilledMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public KilledMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AgentItem a in log.AgentData.GetNPCsByID((int)mechanicID))
                {
                    AbstractSingleActor actorToUse = EnemyMechanicHelper.FindActor(log, a, regroupedMobs);
                    if (actorToUse != null)
                    {
                        foreach (DeadEvent devt in log.CombatData.GetDeadEvents(a))
                        {
                            mechanicLogs[this].Add(new MechanicEvent(devt.Time, this, actorToUse));
                        }
                    }
                }
            }          
        }
    }
}
