using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class CounterOnFoeDamageModifier : BuffOnFoeDamageModifier
{
    public override bool IsCounter => true;
    internal CounterOnFoeDamageModifier(int id, long buffID, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, buffID, name, tooltip, damageSource, 1.0, srctype, compareType, src, gainComputer, icon, mode)
    {
    }

    internal CounterOnFoeDamageModifier(int id, HashSet<long> buffIDs, string name, string tooltip, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, GainComputer gainComputer, string icon, DamageModifierMode mode) : base(id, buffIDs, name, tooltip, damageSource, 1.0, srctype, compareType, src, gainComputer, icon, mode)
    {
    }
    internal override DamageModifierDescriptor UsingGainAdjuster(DamageGainAdjuster gainAdjuster)
    {
        throw new InvalidOperationException("Not Possible to adjust gain for counter damage modifiers");
    }

    protected override bool ComputeGain(IReadOnlyDictionary<long, BuffGraph> bgms, HealthDamageEvent dl, ParsedEvtcLog log, out double gain)
    {
        base.ComputeGain(bgms, dl, log, out gain);
        var keep = gain != 0.0;
        gain = 0;
        return keep;
    }
}
