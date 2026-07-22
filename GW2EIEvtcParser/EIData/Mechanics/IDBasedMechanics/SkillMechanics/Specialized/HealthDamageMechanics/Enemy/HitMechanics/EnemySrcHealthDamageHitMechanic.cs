using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemySrcHealthDamageHitMechanic : EnemySrcHealthDamageMechanic
{
    protected override bool Keep(HealthDamageEvent c, ParsedEvtcLog log)
    {
        return c.HasHit && base.Keep(c, log);
    }

    public EnemySrcHealthDamageHitMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemySrcHealthDamageHitMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
}
