using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public abstract class CheckedMechanic<Checkable> : Mechanic
{

    public delegate bool Checker(Checkable evt, ParsedEvtcLog log);
    protected List<Checker> Checkers { get; private set; }


    internal delegate long TimeClamper(long time, ParsedEvtcLog log);
    private TimeClamper _timeClamper;


    internal delegate bool SingleActorChecker(long time, SingleActor actor, ParsedEvtcLog log);
    private readonly List<(CheckedMechanic<Checkable>, SingleActorChecker)> _subMechanics = [];

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

    internal CheckedMechanic<Checkable> WithSubMechanic(CheckedMechanic<Checkable> mechanic, SingleActorChecker actorChecker)
    {
        mechanic.IsASubMechanic = true;
        _subMechanics.Add((
            mechanic,
            actorChecker
        ));
        return this;
    }

    internal CheckedMechanic<Checkable> WithStabilitySubMechanic(CheckedMechanic<Checkable> stabMechanic, bool stabPresent)
    {
        if (stabPresent)
        {
            return WithSubMechanic(stabMechanic, (time, actor, log) =>
            {
                return actor.HasBuff(log, SkillIDs.Stability, time - ParserHelper.ServerDelayConstant);
            });
        }
        return WithSubMechanic(stabMechanic, (time, actor, log) =>
        {
            return !actor.HasBuff(log, SkillIDs.Stability, time - ParserHelper.ServerDelayConstant);
        });
    }

    public override IReadOnlyList<Mechanic> GetMechanics()
    {
        var res = new List<Mechanic>(1 +  _subMechanics.Count);
        foreach (var subMechanics in _subMechanics)
        {
            res.Add(subMechanics.Item1);
        }
        return res;
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
            if (actor.AgentItem.IsEnglobingAgent)
            {
                actor = log.FindActor(actor.AgentItem.FindEnglobedAgentItem(time));
            }
            mechanicLogs[this].Add(new MechanicEvent(timeToUse, this, actor));
            foreach (var subMechanics in _subMechanics)
            {
                if (subMechanics.Item2(time, actor, log))
                {
                    mechanicLogs[subMechanics.Item1].Add(new MechanicEvent(timeToUse, this, actor));
                }
            }
        }
    }

    protected virtual bool Keep(Checkable checkable, ParsedEvtcLog log)
    {
        return Checkers.All(checker => checker(checkable, log));
    }

}
