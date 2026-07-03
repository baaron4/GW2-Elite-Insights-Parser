using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcHealthDamageMechanic : PlayerSrcSkillMechanic<HealthDamageEvent>
{

    public PlayerSrcHealthDamageMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }

    public PlayerSrcHealthDamageMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity, internalCoolDown, (log, id) => log.CombatData.GetDamageData(id))
    {
    }

    protected override AgentItem GetAgentItem(HealthDamageEvent ahde)
    {
        return ahde.From;
    }
}
