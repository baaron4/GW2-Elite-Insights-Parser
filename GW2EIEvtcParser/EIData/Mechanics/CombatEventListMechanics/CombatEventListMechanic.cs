using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class CombatEventListMechanic<T> : CheckedMechanic<T> where T : TimeCombatEvent
{

    public delegate IEnumerable<T> CombatEventsGetter(ParsedEvtcLog log, AgentItem agent);

    private readonly CombatEventsGetter _getter;

    public IEnumerable<T> GetEvents(ParsedEvtcLog log, AgentItem a)
    {
        return _getter(log, a);
    }

    public CombatEventListMechanic(int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown, CombatEventsGetter getter) : base(id, plotlySetting, description, severity, internalCoolDown)
    {
        _getter = getter;
        if (_getter == null)
        {
            throw new InvalidOperationException("Missing getter in CombatEventListMechanic");
        }
    }
}
