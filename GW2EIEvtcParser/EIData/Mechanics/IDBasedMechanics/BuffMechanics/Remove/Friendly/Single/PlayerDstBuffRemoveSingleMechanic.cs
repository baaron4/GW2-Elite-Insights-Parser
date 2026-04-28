using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstBuffRemoveSingleMechanic : PlayerBuffRemoveSingleMechanic
{
    public PlayerDstBuffRemoveSingleMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity) : base(mechanicID, plotlySetting, shortName, description, fullName, severity)
    {
    }

    public PlayerDstBuffRemoveSingleMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity)
    {
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.To;
    }
}
