using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpellbreakerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(43745, 40616, EIData.InstantCastFinder.DefaultICD), // Sight beyond Sight
            new DamageCastFinder(45534, 45534, EIData.InstantCastFinder.DefaultICD), // Loss Aversion

        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(NumberOfBoonsID, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", DamageModifierMode.All, (x, log) => x.HasCrit),
            new BuffDamageModifierTarget(NumberOfBoonsID, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", DamageModifierMode.All, (x, log) => x.HasCrit),
            new BuffDamageModifierTarget(42428, "Magebane Tether", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(42428).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sight beyond Sight",40616, Source.Spellbreaker, BuffNature.GraphOnly,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                new Buff("Full Counter",43949, Source.Spellbreaker, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Buff("Disenchantment",44633, Source.Spellbreaker, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/e/e1/Winds_of_Disenchantment.png"),
                new Buff("Attacker's Insight",41963, Source.Spellbreaker, BuffStackType.Stacking, 5, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
                new Buff("Magebane Tether",42428, Source.Spellbreaker, BuffStackType.Stacking, 25, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/e/e5/Magebane_Tether.png"),
        };


    }
}
