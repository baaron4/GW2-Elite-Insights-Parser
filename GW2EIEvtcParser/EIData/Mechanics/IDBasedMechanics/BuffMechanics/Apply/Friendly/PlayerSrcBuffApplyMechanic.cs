using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcBuffApplyMechanic : PlayerBuffApplyMechanic
{
    public PlayerSrcBuffApplyMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }

    public PlayerSrcBuffApplyMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }

    protected override AgentItem GetAgentItem(BuffApplyEvent ba)
    {
        return ba.CreditedBy;
    }
}
