
namespace GW2EIEvtcParser.EIData;

public abstract class StringBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<GUID> MechanicIDs = [];

    protected StringBasedMechanic(GUID mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.Add(mechanicID);
    }

    protected StringBasedMechanic(ReadOnlySpan<GUID> mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.ReserveAdditional(mechanicIDs.Length);
        for(int i = 0; i < mechanicIDs.Length; i++)
        {
            MechanicIDs.Add(mechanicIDs[i]);
        }
    }

}
