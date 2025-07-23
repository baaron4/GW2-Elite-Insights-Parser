using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class BuffOnActorDamageModifier : DamageModifierDescriptor
{
    internal delegate double DamageGainAdjuster(HealthDamageEvent dl, ParsedEvtcLog log);

    internal BuffsTracker Tracker;
    internal DamageGainAdjuster? GainAdjuster;

    internal BuffOnActorDamageModifier(int id, long buffID, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
    {
        Tracker = new BuffsTrackerSingle(buffID);
    }

    internal BuffOnActorDamageModifier(int id, HashSet<long> buffIDs, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
    {
        Tracker = new BuffsTrackerMulti(buffIDs);
    }

    internal virtual DamageModifierDescriptor UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
    {
        GainAdjuster = gainAdjuster;
        return this;
    }

    protected double ComputeAdjustedGain(HealthDamageEvent dl, ParsedEvtcLog log)
    {
        return GainAdjuster != null ? GainAdjuster(dl, log) * GainPerStack : GainPerStack;
    }

    protected override bool ComputeGain(IReadOnlyDictionary<long, BuffGraph> bgms, HealthDamageEvent dl, ParsedEvtcLog log, out double gain)
    {
        int stack = Tracker.GetStack(bgms, dl.Time);
        gain = GainComputer.ComputeGain(ComputeAdjustedGain(dl, log), stack);
        return gain != 0;
    }

    protected static bool Skip(BuffsTracker tracker, IReadOnlyDictionary<long, BuffGraph> bgms, GainComputer gainComputer)
    {
        return (gainComputer != ByAbsence && tracker.IsEmpty(bgms)) || (gainComputer == ByAbsence && tracker.IsFull(bgms));
    }

    internal override List<DamageModifierEvent> ComputeDamageModifier(SingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
    {
        var res = new List<DamageModifierEvent>();
        if (CheckEarlyExit(actor, log))
        {
            return res;
        }
        var typeHits = damageModifier.GetHitDamageEvents(actor, log, null);
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
                if (ComputeGain(bgms, evt, log, out double gain) && CheckCondition(evt, log))
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
                if (ComputeGain(bgms, evt, log, out double gain) && CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
        }
        return res;

    }
}
