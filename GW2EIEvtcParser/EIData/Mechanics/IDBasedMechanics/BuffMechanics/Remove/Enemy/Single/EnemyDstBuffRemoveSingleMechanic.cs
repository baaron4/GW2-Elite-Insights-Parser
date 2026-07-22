using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstBuffRemoveSingleMechanic : EnemyBuffRemoveSingleMechanic
{

    public EnemyDstBuffRemoveSingleMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicID, id, plotlySetting, description, severity)
    {
        IsEnemyMechanic = true;
    }

    public EnemyDstBuffRemoveSingleMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicIDs, id, plotlySetting, description, severity)
    {
        IsEnemyMechanic = true;
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.To;
    }
}
