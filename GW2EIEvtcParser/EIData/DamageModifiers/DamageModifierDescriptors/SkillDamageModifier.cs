using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class SkillDamageModifier : DamageModifierDescriptor
{
    private class GainComputerBySkill : GainComputer
    {
        public GainComputerBySkill()
        {
            Multiplier = false;
            SkillBased = true;
        }

        public override double ComputeGain(double gainPerStack, int stack)
        {
            throw new InvalidOperationException("No Compute Gain on GainComputerBySkill");
        }
    }

    private readonly long _skillID;

    private static readonly GainComputerBySkill skillGainComputer = new();

    internal SkillDamageModifier(int id, string name, string tooltip, long skillID, DamageSource damageSource, DamageType srctype, DamageType compareType, Source src, string icon, DamageModifierMode mode) : base(id, name, tooltip, damageSource, int.MaxValue, srctype, compareType, src, icon, skillGainComputer, mode)
    {
        _skillID = skillID;
    }

    internal override List<DamageModifierEvent> ComputeDamageModifier(SingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
    {
        if (CheckEarlyExit(actor, log))
        {
            return [];
        }
        var res = new List<DamageModifierEvent>();
        var typeHits = damageModifier.GetHitDamageEvents(actor, log, null);
        foreach (HealthDamageEvent evt in typeHits)
        {
            if (ComputeGain(null, evt, log, out _) && CheckCondition(evt, log))
            {
                res.Add(new DamageModifierEvent(evt, damageModifier, evt.HealthDamage));
            }
        }
        return res;
    }

    protected override bool ComputeGain(IReadOnlyDictionary<long, BuffGraph>? bgms, HealthDamageEvent dl, ParsedEvtcLog log, out double gain)
    {
        gain = 0;
        if (dl.SkillID != _skillID)
        {
            return false;
        }
        gain = 1;
        return true;
    }
}
