using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstBuffRemoveSingleMechanic : PlayerBuffRemoveSingleMechanic
{
    public PlayerDstBuffRemoveSingleMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicID, id, plotlySetting, description, severity)
    {
    }

    public PlayerDstBuffRemoveSingleMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicIDs, id, plotlySetting, description, severity)
    {
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.To;
    }
}
