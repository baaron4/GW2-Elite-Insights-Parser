using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstHealthDamageMechanic : EnemyHealthDamageMechanic
{

    public EnemyDstHealthDamageMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public EnemyDstHealthDamageMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
    protected override AgentItem GetAgentItem(HealthDamageEvent ahde)
    {
        return ahde.To;
    }
}
