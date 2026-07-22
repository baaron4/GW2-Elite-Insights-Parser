using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstCrowdControlMechanic : EnemyDstSkillMechanic<CrowdControlEvent>
{

    public EnemyDstCrowdControlMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemyDstCrowdControlMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown, (log, id) => log.CombatData.GetCrowdControlData(id))
    {
        UsingEnable(log => log.CombatData.HasCrowdControlData);
    }
}
