namespace GW2EIEvtcParser.EIData;

public abstract class IDBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<long> MechanicIDs = [];

    protected IDBasedMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
        MechanicIDs.UnionWith(mechanicIDs);
    }

}
