using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffOnActorDamageModifier : DamageModifierDescriptor
    {
        internal delegate double DamageGainAdjuster(AbstractHealthDamageEvent dl, ParsedEvtcLog log);

        internal BuffsTracker Tracker { get; }
        internal DamageGainAdjuster GainAdjuster { get; private set; }

        internal BuffOnActorDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        internal BuffOnActorDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        internal virtual DamageModifierDescriptor UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
        {
            GainAdjuster = gainAdjuster;
            return this;
        }

        private double ComputeAdjustedGain(AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            return GainAdjuster != null ? GainAdjuster(dl, log) * GainPerStack : GainPerStack;
        }

        protected override bool ComputeGain(IReadOnlyDictionary<long, BuffsGraphModel> bgms, AbstractHealthDamageEvent dl, ParsedEvtcLog log, out double gain)
        {
            int stack = Tracker.GetStack(bgms, dl.Time);
            gain = GainComputer.ComputeGain(ComputeAdjustedGain(dl, log), stack);
            return gain != 0;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
        {
            IReadOnlyDictionary<long, BuffsGraphModel> bgms = actor.GetBuffGraphs(log);
            if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
            {
                return new List<DamageModifierEvent>();
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageModifier.GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                if (ComputeGain(bgms, evt, log, out double gain) && CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
            return res;
        }
    }
}
