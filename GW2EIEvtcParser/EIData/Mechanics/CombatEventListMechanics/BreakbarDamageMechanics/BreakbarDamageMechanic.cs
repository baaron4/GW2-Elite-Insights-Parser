using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class BreakbarDamageMechanic : CombatEventListMechanic<BreakbarDamageEvent>
{

    public BreakbarDamageMechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(id, plotlySetting, description, severity, internalCoolDown, getter)
    {
    }
}
