namespace GW2EIEvtcParser.EIData;

public abstract class IDBasedMechanic<Checkable> : CounterCheckedMechanic<Checkable>
{

    protected readonly HashSet<long> MechanicIDs = [];

    protected IDBasedMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.UnionWith(mechanicIDs);
    }

}
