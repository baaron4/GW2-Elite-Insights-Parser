using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstBuffRemoveMechanic : PlayerBuffRemoveMechanic<BuffRemoveAllEvent>
{
    private bool _withMinions;
    public PlayerDstBuffRemoveMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerDstBuffRemoveMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerDstBuffRemoveMechanic WithMinions()
    {
        _withMinions = true;
        return this;
    }

    private static bool OnlyMinionsChecker(BuffRemoveAllEvent brae, ParsedEvtcLog log)
    {
        return !brae.To.Is(brae.To.GetFinalMaster());
    }

    public PlayerDstBuffRemoveMechanic OnlyMinions(bool onlyMinions)
    {
        if (onlyMinions)
        {
            Checkers.Add(OnlyMinionsChecker);
        }
        else
        {
            Checkers.Remove(OnlyMinionsChecker);
        }
        return WithMinions();
    }

    protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
    {
        return _withMinions ? rae.To.GetFinalMaster() : rae.To;
    }
}
