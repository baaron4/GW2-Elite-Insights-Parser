using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcHealthDamageHitMechanic : PlayerSrcHealthDamageMechanic
{
    protected override bool Keep(HealthDamageEvent c, ParsedEvtcLog log)
    {
        return c.HasHit && base.Keep(c, log);
    }

    public PlayerSrcHealthDamageHitMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerSrcHealthDamageHitMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
}
