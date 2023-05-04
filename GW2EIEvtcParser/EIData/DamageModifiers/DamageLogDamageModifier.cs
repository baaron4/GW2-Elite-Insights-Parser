using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class DamageLogDamageModifier : DamageModifier
    {

        internal DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ParserHelper.Source src, string icon, DamageLogChecker checker, GainComputer gainComputer, DamageModifierMode mode) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, mode)
        {
            base.UsingChecker(checker);
        }

        internal override List<DamageModifierEvent> ComputeDamageModifier(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            var res = new List<DamageModifierEvent>();
            double gain = GainComputer.ComputeGain(GainPerStack, 1);
            IReadOnlyList<AbstractHealthDamageEvent> typeHits = GetHitDamageEvents(actor, log, null, log.FightData.FightStart, log.FightData.FightEnd);
            foreach (AbstractHealthDamageEvent evt in typeHits)
            {
                if (CheckCondition(evt, log))
                {
                    res.Add(new DamageModifierEvent(evt, this, gain * evt.HealthDamage));
                }
            }
            return res;
        }

        internal override DamageModifier UsingChecker(DamageLogChecker dlChecker)
        {
            throw new InvalidOperationException("Checker is already required for DamageLog modifiers");
        }
    }
}
