using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

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
            new BuffDamageModifierTarget(NumberOfBoonsID, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Spellbreaker, ByPresence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", DamageModifierMode.All, (dEvt) => dEvt.HasCrit),
            new BuffDamageModifierTarget(NumberOfBoonsID, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Power, DamageType.All, ParserHelper.Source.Spellbreaker, ByAbsence, "https://wiki.guildwars2.com/images/7/76/Pure_Strike_%28trait%29.png", DamageModifierMode.All, (dEvt) => dEvt.HasCrit),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sight beyond Sight",40616, ParserHelper.Source.Spellbreaker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                new Buff("Full Counter",43949, ParserHelper.Source.Spellbreaker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Buff("Attacker's Insight",41963, ParserHelper.Source.Spellbreaker, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
        };


    }
}
