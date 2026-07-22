
namespace GW2EIEvtcParser.EIData;

public abstract class StringBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<GUID> MechanicIDs = [];

    protected StringBasedMechanic(ReadOnlySpan<GUID> mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(id, plotlySetting, description, severity, internalCoolDown)
    {
        MechanicIDs.ReserveAdditional(mechanicIDs.Length);
        for(int i = 0; i < mechanicIDs.Length; i++)
        {
            MechanicIDs.Add(mechanicIDs[i]);
        }
    }

}
