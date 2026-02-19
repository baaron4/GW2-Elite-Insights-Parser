using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class SkillMechanic<T> : IDBasedMechanic<T> where T : SkillEvent
{


    public delegate IEnumerable<T> CombatEventsGetter(ParsedEvtcLog log, long id);

    private readonly CombatEventsGetter _getter;
    protected bool Minions { get; private set; } = false;

     public SkillMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        _getter = getter;
    }

    public SkillMechanic<T> WithMinions()
    {
        Minions = true;
        return this;
    }



    internal SkillMechanic<T> UsingBuffChecker(long buffID, bool isPresent)
    {
        if (isPresent)
        {
            return (SkillMechanic<T>)UsingChecker((evt, log) => GetAgentItem(evt).HasBuff(log, buffID, evt.Time - ParserHelper.ServerDelayConstant));
        }
        else
        {
            return (SkillMechanic<T>)UsingChecker((evt, log) => !GetAgentItem(evt).HasBuff(log, buffID, evt.Time - ParserHelper.ServerDelayConstant));
        }
    }

    protected abstract AgentItem GetAgentItem(T evt);

    protected AgentItem GetCreditedAgentItem(T evt)
    {
        AgentItem agentItem = GetAgentItem(evt);
        if (Minions)
        {
            agentItem = agentItem.GetFinalMaster();
        }
        return agentItem;
    }
    protected abstract bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor);

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long skillID in MechanicIDs)
        {
            foreach (T evt in _getter(log, skillID))
            {
                if (TryGetActor(log, GetCreditedAgentItem(evt), regroupedMobs, out var amp) && Keep(evt, log))
                {
                    InsertMechanic(log, mechanicLogs, evt.Time, amp, evt.GetValue());
                }
            }
        }
    }

}
