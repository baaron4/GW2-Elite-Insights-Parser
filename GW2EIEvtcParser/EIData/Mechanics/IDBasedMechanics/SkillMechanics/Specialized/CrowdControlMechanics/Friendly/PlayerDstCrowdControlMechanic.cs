using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstCrowdControlMechanic : PlayerDstSkillMechanic<CrowdControlEvent>
{

    public PlayerDstCrowdControlMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown, (log, id) => log.CombatData.GetCrowdControlData(id))
    {
    }

    public PlayerDstCrowdControlMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown, (log, id) => log.CombatData.GetCrowdControlData(id))
    {
    }
}
