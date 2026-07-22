using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class StatusMechanic<T> : CombatEventListMechanic<T> where T : StatusEvent
{

    public StatusMechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, CombatEventsGetter getter, int internalCoolDown = 0) : base(id, plotlySetting, description, severity, getter, internalCoolDown)
    {
    }
}
