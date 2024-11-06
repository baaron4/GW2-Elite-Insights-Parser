namespace GW2EIEvtcParser.EIData;

public abstract class IDBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<long> MechanicIDs = new();

    protected IDBasedMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.Add(mechanicID);
    }

    //TODO(Rennorb) @perf
    protected IDBasedMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.UnionWith(mechanicIDs);
    }

}
