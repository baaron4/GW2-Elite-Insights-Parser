using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstBuffRemoveSingleMechanic : EnemyBuffRemoveSingleMechanic
{

    public EnemyDstBuffRemoveSingleMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, plotlySetting, shortName, description, fullName)
    {
        IsEnemyMechanic = true;
    }

    public EnemyDstBuffRemoveSingleMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, plotlySetting, shortName, description, fullName)
    {
        IsEnemyMechanic = true;
    }

    protected override AgentItem GetAgentItem(AbstractBuffRemoveEvent rae)
    {
        return rae.To;
    }
}
