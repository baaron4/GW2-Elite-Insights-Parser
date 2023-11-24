using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
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

        private static readonly GainComputerBySkill skillGainComputer = new GainComputerBySkill();

        internal SkillDamageModifier(string name, string tooltip, long skillID, DamageSource damageSource, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageModifierMode mode) : base(name, tooltip, damageSource, int.MaxValue, srctype, compareType, src, icon, skillGainComputer, mode)
        {
            base.UsingChecker((evt, log) => evt.SkillId == skillID);
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
        {
            var res = new List<DamageModifierEvent>();
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageModifier.GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                if (CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, damageModifier, evt.HealthDamage));
                }
            }
            return res;
        }
    }
}
