using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal abstract class BuffCastFinder<Event> : CheckedCastFinder<Event> where Event : BuffEvent
{
    protected bool Minions = false;
    protected bool Target = false;
    protected readonly long BuffID;
    protected BuffCastFinder(long skillID, long buffID) : base(skillID)
    {
        BuffID = buffID;
    }
    protected AgentItem GetCasterAgent(Event evt)
    {
        return Minions ? GetKeyAgent(evt).GetFinalMaster() : GetKeyAgent(evt);
    }

    protected abstract AgentItem GetKeyAgent(Event evt);
    protected abstract AgentItem GetOtherKeyAgent(Event evt);

    public virtual BuffCastFinder<Event> WithMinions()
    {
        Minions = true;
        return this;
    }

    public virtual BuffCastFinder<Event> WithTarget()
    {
        Target = true;
        return this;
    }

    internal BuffCastFinder<Event> UsingByBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.By.GetBaseSpecAtTime(evt.Time) == spec);
        return this;
    }

    internal BuffCastFinder<Event> UsingToBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.To.GetBaseSpecAtTime(evt.Time) == spec);
        return this;
    }

    internal BuffCastFinder<Event> UsingBySpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.By.GetSpecAtTime(evt.Time) == spec);
        return this;
    }

    internal BuffCastFinder<Event> UsingByNotSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.By.GetSpecAtTime(evt.Time) != spec);
        return this;
    }

    internal BuffCastFinder<Event> UsingBySpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => specs.Contains(evt.By.GetSpecAtTime(evt.Time)));
        return this;
    }
    internal BuffCastFinder<Event> UsingByNotSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => !specs.Contains(evt.By.GetSpecAtTime(evt.Time)));
        return this;
    }

    internal BuffCastFinder<Event> UsingToSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.To.GetSpecAtTime(evt.Time) == spec);
        return this;
    }

    internal BuffCastFinder<Event> UsingToNotSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.To.GetSpecAtTime(evt.Time) != spec);
        return this;
    }

    internal BuffCastFinder<Event> UsingToSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => specs.Contains(evt.To.GetSpecAtTime(evt.Time)));
        return this;
    }
    internal BuffCastFinder<Event> UsingToNotSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => !specs.Contains(evt.To.GetSpecAtTime(evt.Time)));
        return this;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var applies = combatData.GetBuffData(BuffID).OfType<Event>().GroupBy(x => GetCasterAgent(x));
        foreach (var group in applies)
        {
            long lastTime = int.MinValue;
            foreach (Event evt in group)
            {
                if (CheckCondition(evt, combatData, agentData, skillData))
                {
                    if (evt.Time - lastTime < ICD)
                    {
                        lastTime = evt.Time;
                        continue;
                    }
                    lastTime = evt.Time;
                    var instant = new InstantCastEvent(GetTime(evt, GetKeyAgent(evt), combatData), skillData.Get(SkillID), group.Key);
                    if (Target)
                    {
                        instant.OverrideTarget(GetOtherKeyAgent(evt));
                    }
                    res.Add(instant);
                }
            }
        }

        return res;
    }
}
