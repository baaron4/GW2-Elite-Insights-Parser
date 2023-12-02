using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class CounterOnFoeDamageModifier : BuffOnFoeDamageModifier
    {
        private class GainComputerCounter : GainComputer
        {
            public GainComputerCounter()
            {
                Multiplier = false;
            }

            public override double ComputeGain(double gainPerStack, int stack)
            {
                throw new InvalidOperationException("No Compute Gain on GainComputerCounter");
            }
        }

        private static readonly GainComputerCounter counterGainComputer = new GainComputerCounter();

        internal CounterOnFoeDamageModifier(long id, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, int.MaxValue, srctype, compareType, src, counterGainComputer, icon, mode)
        {
        }

        internal CounterOnFoeDamageModifier(long[] ids, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, string icon, DamageModifierMode mode) : base(ids, name, tooltip, damageSource, int.MaxValue, srctype, compareType, src, counterGainComputer, icon, mode)
        {
        }
        internal override DamageModifierDescriptor UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
        {
            throw new InvalidOperationException("Not Possible to adjust gain for counter damage modifiers");
        }

        protected override bool ComputeGain(IReadOnlyDictionary<long, BuffsGraphModel> bgms, AbstractHealthDamageEvent dl, ParsedEvtcLog log, out double gain)
        {
            gain = 0;
            int stack = Tracker.GetStack(bgms, dl.Time);
            return stack > 0.0;
        }
    }
}
