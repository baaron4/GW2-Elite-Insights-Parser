using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public abstract class CounterCheckedMechanic<Checkable> : CheckedMechanic<Checkable>
{

    protected CounterCheckedMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    protected void InsertMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, long time, SingleActor actor)
    {
        (long timeToUse, actor) = ComputeTimeAndActor(log, actor, time);
        InsertMechanicWithSubMechanics(log, mechanicLogs, time, actor, new CounterMechanicEvent(timeToUse, this, actor));
    }

}
