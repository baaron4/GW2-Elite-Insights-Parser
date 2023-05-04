using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDamageModifier : DamageModifier
    {
        internal delegate double DamageGainAdjuster(AbstractHealthDamageEvent dl, ParsedEvtcLog log);

        internal BuffsTracker Tracker { get; }
        internal DamageGainAdjuster GainAdjuster { get; private set; }

        internal BuffDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        internal BuffDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        internal DamageModifier UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
        {
            GainAdjuster = gainAdjuster;
            return this;
        }

        private double ComputeAdjustedGain(AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            return GainAdjuster != null ? GainAdjuster(dl, log) * GainPerStack : GainPerStack;
        }

        protected double ComputeGain(IReadOnlyDictionary<long, BuffsGraphModel> bgms, AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            int stack = Tracker.GetStack(bgms, dl.Time);
            // When gain per stack is 0, we only count hits done under the buff or in its absence
            double gain = GainComputer.ComputeGain(GainPerStack == 0.0 ? 1.0 : ComputeAdjustedGain(dl, log), stack);
            return gain > 0.0 ? (GainPerStack == 0.0 ? 0.0 : gain * dl.HealthDamage) : -1.0;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            IReadOnlyDictionary<long, BuffsGraphModel> bgms = actor.GetBuffGraphs(log);
            if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
            {
                return new List<DamageModifierEvent>();
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                if (CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, this, ComputeGain(bgms, evt, log)));
                }
            }
            res.RemoveAll(x => x.DamageGain == -1.0);
            return res;
        }
    }
}
