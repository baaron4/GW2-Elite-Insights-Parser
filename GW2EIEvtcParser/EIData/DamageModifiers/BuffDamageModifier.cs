using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class BuffDamageModifier : DamageModifier
    {

        internal BuffsTracker Tracker { get; }

        internal BuffDamageModifier(long id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            Tracker = new BuffsTrackerSingle(id);
        }

        internal BuffDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            Tracker = new BuffsTrackerMulti(new List<long>(ids));
        }

        protected double ComputeGain(int stack, AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            if (DLChecker != null && !DLChecker(dl, log))
            {
                return -1.0;
            }
            // When gain per stack is 0, we only count hits done under the buff or in its absence
            double gain = GainComputer.ComputeGain(GainPerStack == 0.0 ? 1.0 : GainPerStack, stack);
            return gain > 0.0 ? (GainPerStack == 0.0 ? 0.0 : gain * dl.HealthDamage) : -1.0;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            Dictionary<long, BuffsGraphModel> bgms = actor.GetBuffGraphs(log);
            if (!Tracker.Has(bgms) && GainComputer != ByAbsence)
            {
                return new List<DamageModifierEvent>();
            }
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, 0, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                res.Add(new DamageModifierEvent(evt, this, ComputeGain(Tracker.GetStack(bgms, evt.Time), evt, log)));
            }
            res.RemoveAll(x => x.DamageGain == -1.0);
            return res;
        }
    }
}
