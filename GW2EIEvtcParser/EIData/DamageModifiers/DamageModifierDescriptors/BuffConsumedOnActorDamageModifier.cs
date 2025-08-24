using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class BuffConsumedOnActorDamageModifier : BuffOnActorDamageModifier
{
    private readonly long BuffID;
    internal BuffConsumedOnActorDamageModifier(int id, long buffID, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, Source src, string icon, DamageModifierMode mode) : base(id, buffID, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, ByPresence, icon, mode)
    {
        BuffID = buffID;
    }

    protected override bool ComputeGain(IReadOnlyDictionary<long, BuffGraph> bgms, HealthDamageEvent dl, ParsedEvtcLog log, out double gain)
    {
        gain = GainComputer.ComputeGain(ComputeAdjustedGain(dl, log), 1);
        return gain != 0;
    }

    private (IReadOnlyList<AbstractBuffRemoveEvent>, int currentIndex) GetBuffRemovesWithCurrentIndex(Dictionary<AgentItem, (IReadOnlyList<AbstractBuffRemoveEvent> buffRemoves, int currentIndex)> cache, ParsedEvtcLog log, AgentItem agent)
    {
        if (!cache.TryGetValue(agent, out var buffRemoveAllsWithCurrentIndex))
        {
            buffRemoveAllsWithCurrentIndex = (log.CombatData.GetBuffDataByIDByDst(BuffID, agent).OfType<AbstractBuffRemoveEvent>().Where(x => x is BuffRemoveSingleEvent || x is BuffRemoveAllEvent).ToList(), 0);
            cache[agent] = buffRemoveAllsWithCurrentIndex;
        }
        return buffRemoveAllsWithCurrentIndex;
    }

    private bool CheckConditionWithBuffRemove(Dictionary<AgentItem, (IReadOnlyList<AbstractBuffRemoveEvent> buffRemoves, int currentIndex)> cache, ParsedEvtcLog log, HealthDamageEvent evt)
    {
        if (!CheckCondition(evt, log))
        {
            return false;
        }
        (IReadOnlyList<AbstractBuffRemoveEvent> buffRemoves, int currentIndex) = GetBuffRemovesWithCurrentIndex(cache, log, evt.From);
        for (int i = currentIndex; i < buffRemoves.Count; i++)
        {
            var currentBuffRemove = buffRemoves[i];
            if (currentBuffRemove.Time > evt.Time)
            {
                break;
            }
            else if (evt.Time - currentBuffRemove.Time < ServerDelayConstant)
            {
                cache[evt.From] = (buffRemoves, i + 1);
                return true;
            }
        }
        return false;
    }

    internal override List<DamageModifierEvent> ComputeDamageModifier(SingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
    {
        var res = new List<DamageModifierEvent>();
        if (CheckEarlyExit(actor, log))
        {
            return res;
        }
        var typeHits = damageModifier.GetHitDamageEvents(actor, log, null);
        var buffRemovesByDstWithCurrentIndex = new Dictionary<AgentItem, (IReadOnlyList<AbstractBuffRemoveEvent>, int currentIndex)>();
        if (damageModifier.NeedsMinions)
        {
            var ignoredSources = new HashSet<SingleActor>();
            foreach (HealthDamageEvent evt in typeHits)
            {
                var singleActor = log.FindActor(evt.From);
                if (ignoredSources.Contains(singleActor))
                {
                    continue;
                }
                var bgms = singleActor.GetBuffGraphs(log);
                if (Skip(Tracker, bgms, GainComputer))
                {
                    ignoredSources.Add(singleActor);
                    continue;
                }
                if (ComputeGain(bgms, evt, log, out double gain) && CheckConditionWithBuffRemove(buffRemovesByDstWithCurrentIndex, log, evt))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
        } 
        else
        {
            IReadOnlyDictionary<long, BuffGraph> bgms = actor.GetBuffGraphs(log);
            if (Skip(Tracker, bgms, GainComputer))
            {
                return res;
            }
            foreach (HealthDamageEvent evt in typeHits)
            {
                if (ComputeGain(bgms, evt, log, out double gain) && CheckConditionWithBuffRemove(buffRemovesByDstWithCurrentIndex, log, evt))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
        }
        return res;

    }
}
