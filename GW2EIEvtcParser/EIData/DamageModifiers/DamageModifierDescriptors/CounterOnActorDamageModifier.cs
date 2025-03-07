using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class CounterOnActorDamageModifier : BuffOnActorDamageModifier
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

    private static readonly GainComputerCounter counterGainComputer = new();

    internal CounterOnActorDamageModifier(int id, long buffID, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, string icon, DamageModifierMode mode) : base(id, buffID, name, tooltip, damageSource, int.MaxValue, srctype, compareType, src, counterGainComputer, icon, mode)
    {
    }

    internal CounterOnActorDamageModifier(int id, HashSet<long> buffIDs, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, string icon, DamageModifierMode mode) : base(id,buffIDs, name, tooltip, damageSource, int.MaxValue, srctype, compareType, src, counterGainComputer, icon, mode)
    {
    }
    internal override DamageModifierDescriptor UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
    {
        throw new InvalidOperationException("Not Possible to adjust gain for counter damage modifiers");
    }

    protected override bool ComputeGain(IReadOnlyDictionary<long, BuffGraph> bgms, HealthDamageEvent dl, ParsedEvtcLog log, out double gain)
    {
        gain = 0;
        int stack = Tracker.GetStack(bgms, dl.Time);
        return stack > 0.0;
    }
}
