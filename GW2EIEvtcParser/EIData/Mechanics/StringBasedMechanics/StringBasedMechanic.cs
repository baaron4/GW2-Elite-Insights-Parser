namespace GW2EIEvtcParser.EIData;

public abstract class StringBasedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected readonly HashSet<GUID> MechanicIDs = [];

    protected StringBasedMechanic(GUID mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.Add(mechanicID);
    }

    protected StringBasedMechanic(ReadOnlySpan<GUID> mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        MechanicIDs.ReserveAdditional(MechanicIDs.Count);
        for(int i = 0; i < MechanicIDs.Count; i++)
        {
            MechanicIDs.Add(mechanicIDs[i]);
        }
    }

}
