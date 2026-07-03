using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcHealthDamageHitMechanic : PlayerSrcHealthDamageMechanic
{
    protected override bool Keep(HealthDamageEvent c, ParsedEvtcLog log)
    {
        return c.HasHit && base.Keep(c, log);
    }

    public PlayerSrcHealthDamageHitMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }

    public PlayerSrcHealthDamageHitMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }
}
