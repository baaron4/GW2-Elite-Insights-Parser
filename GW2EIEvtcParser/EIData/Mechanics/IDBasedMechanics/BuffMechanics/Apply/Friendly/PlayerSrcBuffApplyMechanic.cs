using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcBuffApplyMechanic : PlayerBuffApplyMechanic
{
    public PlayerSrcBuffApplyMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerSrcBuffApplyMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    protected override AgentItem GetAgentItem(BuffApplyEvent ba)
    {
        return ba.CreditedBy;
    }
}
