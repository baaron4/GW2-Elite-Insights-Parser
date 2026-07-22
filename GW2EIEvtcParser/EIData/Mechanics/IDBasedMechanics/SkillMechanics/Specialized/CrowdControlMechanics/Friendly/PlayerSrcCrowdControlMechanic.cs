using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcCrowdControlMechanic : PlayerSrcSkillMechanic<CrowdControlEvent>
{

    public PlayerSrcCrowdControlMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerSrcCrowdControlMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown, (log, id) => log.CombatData.GetCrowdControlData(id))
    {
        UsingEnable(log => log.CombatData.HasCrowdControlData);
    }
}
