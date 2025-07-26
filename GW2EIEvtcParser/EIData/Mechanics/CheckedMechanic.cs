using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public abstract class CheckedMechanic<Checkable> : Mechanic
{

    public delegate bool Checker(Checkable evt, ParsedEvtcLog log);
    protected List<Checker> Checkers { get; private set; }


    public delegate long TimeClamper(long time, ParsedEvtcLog log);
    private TimeClamper _timeClamper;

    protected CheckedMechanic(MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        Checkers = [];
    }

    internal CheckedMechanic<Checkable> UsingChecker(Checker checker)
    {
        Checkers.Add(checker);
        return this;
    }

    internal CheckedMechanic<Checkable> UsingTimeClamper(TimeClamper clamper)
    {
        _timeClamper = clamper;
        return this;
    }

    protected void InsertMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, long time, SingleActor actor)
    {
        if (actor != null)
        {
            long timeToUse = time;
            if (_timeClamper != null)
            {
                timeToUse = _timeClamper(time, log);
            }
            if (actor.AgentItem.ParentAgentItemOf.Count > 0)
            {
                actor = log.FindActor(actor.AgentItem.FindActiveAgent(time));
            }
            mechanicLogs[this].Add(new MechanicEvent(timeToUse, this, actor));
        }
    }

    protected virtual bool Keep(Checkable checkable, ParsedEvtcLog log)
    {
        return Checkers.All(checker => checker(checkable, log));
    }

}
