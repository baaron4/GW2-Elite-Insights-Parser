using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class DamageLogDamageModifier : DamageModifierDescriptor
{

    internal DamageLogDamageModifier(int id, string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, Source src, string icon, DamageLogChecker checker, DamageModifierMode mode) : base(id, name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, ByPresence, mode)
    {
        base.UsingChecker(checker);
    }

    protected override bool ComputeGain(IReadOnlyDictionary<long, BuffGraph>? bgms, HealthDamageEvent? dl, ParsedEvtcLog log, out double gain)
    {
        gain = GainComputer.ComputeGain(GainPerStack, 1);
        return true;
    }

    internal override List<DamageModifierEvent> ComputeDamageModifier(SingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
    {
        if (CheckEarlyExit(actor, log))
        {
            return [];
        }
        var res = new List<DamageModifierEvent>();
        if (ComputeGain(null, null, log, out double gain))
        {

            var typeHits = damageModifier.GetHitDamageEvents(actor, log, null);
            foreach (HealthDamageEvent evt in typeHits)
            {
                if (CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, gain * evt.HealthDamage));
                }
            }
        }
        return res;
    }
}
