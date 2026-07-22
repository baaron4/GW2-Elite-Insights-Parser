using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class DamageMechanic : CombatEventListMechanic<HealthDamageEvent>
{

    public DamageMechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(id, plotlySetting, description, severity, internalCoolDown, getter)
    {
    }
}
