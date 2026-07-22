namespace GW2EIEvtcParser.EIData;

public abstract class IDBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<long> MechanicIDs = [];

    protected IDBasedMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(id, plotlySetting, description, severity, internalCoolDown)
    {
        MechanicIDs.UnionWith(mechanicIDs);
    }

}
