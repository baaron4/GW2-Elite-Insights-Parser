using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstBuffRemoveMechanic : EnemyBuffRemoveMechanic<BuffRemoveAllEvent>
{

    public EnemyDstBuffRemoveMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    public EnemyDstBuffRemoveMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
    {
        return rae.To;
    }
}
