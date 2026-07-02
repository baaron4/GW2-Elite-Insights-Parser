using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerCastStartMechanic : PlayerCastMechanic
{
    protected override long GetTime(CastEvent evt)
    {
        return evt.Time;
    }

    public PlayerCastStartMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }

    public PlayerCastStartMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }
}
