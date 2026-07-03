using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcBuffRemoveSingleFromMechanic : PlayerBuffRemoveSingleMechanic
{
    public PlayerSrcBuffRemoveSingleFromMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity) : base(mechanicID, plotlySetting, shortName, description, fullName, severity)
    {
    }

    public PlayerSrcBuffRemoveSingleFromMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity)
    {
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.CreditedBy;
    }
}
