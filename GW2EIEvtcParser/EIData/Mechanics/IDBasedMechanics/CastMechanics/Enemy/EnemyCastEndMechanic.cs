using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyCastEndMechanic : EnemyCastMechanic
{
    protected override long GetTime(CastEvent evt)
    {
        return evt.EndTime;
    }

    public EnemyCastEndMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemyCastEndMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
}
