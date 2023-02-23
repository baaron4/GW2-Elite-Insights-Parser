using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class PlayerBuffRemoveMechanic : BuffRemoveMechanic
    {
        public PlayerBuffRemoveMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerBuffRemoveMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected override AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            return MechanicHelper.FindPlayerActor(log, agentItem);
        }
    }
}
