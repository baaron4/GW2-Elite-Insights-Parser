using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstBuffRemoveMechanic : EnemyBuffRemoveMechanic<BuffRemoveAllEvent>
{

    public EnemyDstBuffRemoveMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    public EnemyDstBuffRemoveMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
    {
        return rae.To;
    }
}
