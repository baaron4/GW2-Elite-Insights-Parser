namespace GW2EIEvtcParser.EIData;

public abstract class IDBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<long> MechanicIDs = [];

    //TODO(Rennorb) @perf
    protected IDBasedMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.UnionWith(mechanicIDs);
    }

}
