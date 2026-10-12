using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemySrcBuffApplyMechanic : EnemyBuffApplyMechanic
{
    public EnemySrcBuffApplyMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemySrcBuffApplyMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    protected override AgentItem GetAgentItem(BuffApplyEvent ba)
    {
        return ba.CreditedBy;
    }
}
