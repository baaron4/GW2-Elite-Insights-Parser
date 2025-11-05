using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class BuffOnActorDamageModifier : DamageModifierDescriptor
{
    internal delegate double DamageGainAdjuster(HealthDamageEvent dl, ParsedEvtcLog log);

    protected bool FromDst = false;

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

    internal virtual DamageModifierDescriptor WithBuffOnActorFromFoe()
    {
        if (GainComputer == ByAbsence)
        {
            throw new InvalidOperationException("Unsupported mode when using ByAbsence");
        }
        FromDst = true;
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
                var minionOrActor = log.FindActor(damageModifier.GetActor(evt));
                if (ignoredSources.Contains(minionOrActor))
                {
                    continue;
                }
                var bgms = minionOrActor.GetBuffGraphs(log);
                if (Skip(Tracker, bgms, GainComputer))
                {
                    ignoredSources.Add(minionOrActor);
                    continue;
                }
                if (FromDst)
                {
                    bgms = minionOrActor.GetBuffGraphs(log, damageModifier.GetFoe(evt).GetMainSingleActorWhenAttackTarget(log));
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
                if (FromDst)
                {
                    bgms = actor.GetBuffGraphs(log, damageModifier.GetFoe(evt).GetMainSingleActorWhenAttackTarget(log));
                }
                if (ComputeGain(bgms, evt, log, out double gain) && CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
        }
        return res;

    }
}
