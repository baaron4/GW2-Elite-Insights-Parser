using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstBuffRemoveSingleMechanic : EnemyBuffRemoveSingleMechanic
{

    public EnemyDstBuffRemoveSingleMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity) : base(mechanicID, plotlySetting, shortName, description, fullName, severity)
    {
        IsEnemyMechanic = true;
    }

    public EnemyDstBuffRemoveSingleMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity)
    {
        IsEnemyMechanic = true;
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.To;
    }
}
