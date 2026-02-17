using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class StatusMechanic<T> : CombatEventListMechanic<T> where T : StatusEvent
{

    public StatusMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(plotlySetting, shortName, description, fullName, internalCoolDown, getter)
    {
    }
}
