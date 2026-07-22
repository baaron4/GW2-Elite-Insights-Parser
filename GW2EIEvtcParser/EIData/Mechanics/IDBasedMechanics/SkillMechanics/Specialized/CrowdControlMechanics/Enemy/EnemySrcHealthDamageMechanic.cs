using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemySrcCrowdControlMechanic : EnemySrcSkillMechanic<CrowdControlEvent>
{

    public EnemySrcCrowdControlMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemySrcCrowdControlMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown, (log, id) => log.CombatData.GetCrowdControlData(id))
    {
        UsingEnable(log => log.CombatData.HasCrowdControlData);
    }
}
