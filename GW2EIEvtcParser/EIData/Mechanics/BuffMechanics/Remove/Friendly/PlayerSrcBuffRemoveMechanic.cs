using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcBuffRemoveFromMechanic : PlayerBuffRemoveMechanic
    {
        public PlayerSrcBuffRemoveFromMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerSrcBuffRemoveFromMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
        {
            return rae.CreditedBy;
        }
    }
}
