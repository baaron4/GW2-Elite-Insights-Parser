using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcBuffRemoveFromMechanic : PlayerBuffRemoveMechanic<BuffRemoveAllEvent>
{
    public PlayerSrcBuffRemoveFromMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerSrcBuffRemoveFromMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
    {
        return rae.CreditedBy;
    }
}
