using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcBuffRemoveFromMechanic : PlayerBuffRemoveMechanic<BuffRemoveAllEvent>
    {
        public PlayerSrcBuffRemoveFromMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerSrcBuffRemoveFromMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
        {
            return rae.CreditedBy;
        }
    }
}
