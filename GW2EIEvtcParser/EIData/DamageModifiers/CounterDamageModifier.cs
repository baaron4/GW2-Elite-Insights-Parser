using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class CounterDamageModifier : BuffDamageModifier
    {
        private class GainComputerCounter : GainComputer
        {
            public GainComputerCounter()
            {
                Multiplier = false;
            }

            public override double ComputeGain(double gainPerStack, int stack)
            {
                return stack > 0 ? 1.0 : 0.0;
            }
        }

        private static readonly GainComputerCounter counterGainComputer = new GainComputerCounter();

        internal CounterDamageModifier(long id, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, 0.0, srctype, compareType, Source.Item, counterGainComputer, icon, mode)
        {
        }

        internal CounterDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, string icon, DamageModifierMode mode) : base(ids, name, tooltip, damageSource, 0.0, srctype, compareType, Source.Item, counterGainComputer, icon, mode)
        {
        }
        internal override DamageModifier UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
        {
            throw new InvalidOperationException("Not Possible to adjust gain for counter damage modifiers");
        }

        protected override double ComputeGain(IReadOnlyDictionary<long, BuffsGraphModel> bgms, AbstractHealthDamageEvent dl, ParsedEvtcLog log)
        {
            int stack = Tracker.GetStack(bgms, dl.Time);
            // When gain per stack is 0, we only count hits done under the buff or in its absence
            double gain = GainComputer.ComputeGain(0, stack);
            return gain > 0.0 ? 0.0 : -1.0;
        }
    }
}
