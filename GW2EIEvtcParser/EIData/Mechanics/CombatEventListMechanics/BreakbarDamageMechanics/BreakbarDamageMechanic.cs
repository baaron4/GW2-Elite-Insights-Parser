using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class BreakbarDamageMechanic : CombatEventListMechanic<BreakbarDamageEvent>
{

    public BreakbarDamageMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(plotlySetting, shortName, description, fullName, severity, internalCoolDown, getter)
    {
    }
}
