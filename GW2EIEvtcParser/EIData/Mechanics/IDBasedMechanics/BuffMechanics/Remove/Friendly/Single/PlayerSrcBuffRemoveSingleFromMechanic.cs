using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcBuffRemoveSingleFromMechanic : PlayerBuffRemoveSingleMechanic
{
    public PlayerSrcBuffRemoveSingleFromMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, plotlySetting, shortName, description, fullName)
    {
    }

    public PlayerSrcBuffRemoveSingleFromMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, plotlySetting, shortName, description, fullName)
    {
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.CreditedBy;
    }
}
