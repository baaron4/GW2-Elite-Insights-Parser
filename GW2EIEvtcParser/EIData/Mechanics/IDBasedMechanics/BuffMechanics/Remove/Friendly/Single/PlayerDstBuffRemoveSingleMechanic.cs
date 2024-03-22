using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerDstBuffRemoveSingleMechanic : PlayerBuffRemoveSingleMechanic
    {
        public PlayerDstBuffRemoveSingleMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName)
        {
        }

        public PlayerDstBuffRemoveSingleMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName)
        {
        }

        protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
        {
            return rae.To;
        }
    }
}
