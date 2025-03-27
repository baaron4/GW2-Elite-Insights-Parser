using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstBuffRemoveSingleMechanic : PlayerBuffRemoveSingleMechanic
{
    public PlayerDstBuffRemoveSingleMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, plotlySetting, shortName, description, fullName)
    {
    }

    public PlayerDstBuffRemoveSingleMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, plotlySetting, shortName, description, fullName)
    {
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.To;
    }
}
