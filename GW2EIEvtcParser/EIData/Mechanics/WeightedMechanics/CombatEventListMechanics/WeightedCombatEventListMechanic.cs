using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal abstract class WeightedCombatEventListMechanic<T> : WeightedCheckedMechanic<T> where T : TimeCombatEvent
{

    public delegate IEnumerable<T> CombatEventsGetter(ParsedEvtcLog log, AgentItem agent);

    private readonly CombatEventsGetter _getter;

    public IEnumerable<T> GetEvents(ParsedEvtcLog log, AgentItem a)
    {
        return _getter(log, a);
    }

    public WeightedCombatEventListMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        _getter = getter;
        if (_getter == null)
        {
            throw new InvalidOperationException("Missing getter in CombatEventListMechanic");
        }
    }
}
