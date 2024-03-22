using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcBuffRemoveSingleFromMechanic : PlayerBuffRemoveSingleMechanic
    {
        public PlayerSrcBuffRemoveSingleFromMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName)
        {
        }

        public PlayerSrcBuffRemoveSingleFromMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName)
        {
        }

        protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
        {
            return rae.CreditedBy;
        }
    }
}
