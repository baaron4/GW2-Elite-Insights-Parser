using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstBuffApplyMechanic : PlayerBuffApplyMechanic
{
    public PlayerDstBuffApplyMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerDstBuffApplyMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    protected override AgentItem GetAgentItem(BuffApplyEvent ba)
    {
        return ba.To;
    }
}
