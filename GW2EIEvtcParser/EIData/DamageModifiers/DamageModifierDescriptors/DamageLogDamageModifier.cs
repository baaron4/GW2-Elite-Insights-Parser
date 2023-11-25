using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class DamageLogDamageModifier : DamageModifierDescriptor
    {

        internal DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, ByPresence, mode)
        {
            base.UsingChecker(checker);
        }

        protected override bool ComputeGain(IReadOnlyDictionary<long, BuffsGraphModel> bgms, AbstractHealthDamageEvent dl, ParsedEvtcLog log, out double gain)
        {
            gain = GainComputer.ComputeGain(GainPerStack, 1);
            return true;
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log, DamageModifier damageModifier)
        {
            var res = new List<DamageModifierEvent>();
            if (ComputeGain(null, null, log, out double gain)) {

                IReadOnlyList<AbstractHealthDamageEvent> typeHits = damageModifier.GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
                foreach (AbstractHealthDamageEvent evt in typeHits)
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
}
