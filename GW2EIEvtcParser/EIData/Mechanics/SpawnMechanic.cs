using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class SpawnMechanic : IDBasedMechanic<AbstractSingleActor>
    {
        public SpawnMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public SpawnMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AgentItem a in log.AgentData.GetNPCsByID((int)mechanicID))
                {
                    AbstractSingleActor amp = MechanicHelper.FindEnemyActor(log, a, regroupedMobs);
                    if (amp != null && Keep(amp, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(a.FirstAware, this, amp));
                    }
                }
            }
        }
    }
}
