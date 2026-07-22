using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyCastStartMechanic : EnemyCastMechanic
{
    protected override long GetTime(CastEvent evt)
    {
        return evt.Time;
    }

    public EnemyCastStartMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemyCastStartMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
}
