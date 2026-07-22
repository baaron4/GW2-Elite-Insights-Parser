using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcBuffRemoveSingleFromMechanic : PlayerBuffRemoveSingleMechanic
{
    public PlayerSrcBuffRemoveSingleFromMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicID, id, plotlySetting, description, severity)
    {
    }

    public PlayerSrcBuffRemoveSingleFromMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicIDs, id, plotlySetting, description, severity)
    {
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.CreditedBy;
    }
}
