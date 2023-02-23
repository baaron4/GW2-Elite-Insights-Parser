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

        protected override AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            return MechanicHelper.FindPlayerActor(log, agentItem);
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(mechanicID))
                {
                    AbstractSingleActor amp = null;
                    if (c is BuffApplyEvent ba && Keep(ba, log))
                    {
                        amp = MechanicHelper.FindPlayerActor(log, GetAgentItem(ba));
                    }
                    if (amp != null)
                    {
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, amp));
                    }
                }
            }
        }
    }
}
