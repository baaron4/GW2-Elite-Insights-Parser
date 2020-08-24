using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal class RevenantHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(28134, 27890, EIData.InstantCastFinder.DefaultICD), // Legendary Assassin Stance
            new BuffGainCastFinder(28494, 27928, EIData.InstantCastFinder.DefaultICD), // Legendary Demon Stance
            new BuffGainCastFinder(28419, 27205, EIData.InstantCastFinder.DefaultICD), // Legendary Dwarf Stance
            new BuffGainCastFinder(28195, 27972, EIData.InstantCastFinder.DefaultICD), // Legendary Centaur Stance
            new BuffGainCastFinder(27107, 27581, 500), // Impossible Odds
            new BuffLossCastFinder(28382, 27581, 500), // Relinquish Power
            new BuffGainCastFinder(26557, 27273, EIData.InstantCastFinder.DefaultICD), // Vengeful Hammers
            new BuffLossCastFinder(26956, 27273, EIData.InstantCastFinder.DefaultICD), // Release Hammers
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(29395, "Vicious Lacerations", "3% per Stack", DamageSource.NoPets, 3.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByStack, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 92715, 102321, DamageModifierMode.PvE),
            new BuffDamageModifier(29395, "Vicious Lacerations", "2% per Stack", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByStack, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 0, 92715, DamageModifierMode.PvE),
            new BuffDamageModifier(873, "Vicious Reprisal", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/c/cf/Vicious_Reprisal.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(742, "Dwarven Battle Training", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/5/50/Dwarven_Battle_Training.png", 94051, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(Buff.NumberOfBoonsID, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, ParserHelper.Source.Herald, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png", DamageModifierMode.All),
            new BuffDamageModifier(725, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ParserHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ec/Ferocious_Aggression.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByStack, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 95535, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "10.0% if vuln", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 92715, 95535, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(738, "Targeted Destruction", "7.0% if vuln", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant, ByPresence, "https://wiki.guildwars2.com/images/e/ed/Targeted_Destruction.png", 0, 92715, DamageModifierMode.PvE),
            new DamageLogDamageModifier("Swift Termination", "20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParserHelper.Source.Revenant,"https://wiki.guildwars2.com/images/b/bb/Swift_Termination.png", x => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {         
                new Buff("Vengeful Hammers", 27273, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/c8/Vengeful_Hammers.png"),
                new Buff("Rite of the Great Dwarf", 26596, ParserHelper.Source.Revenant, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Buff("Embrace the Darkness", 28001, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Embrace_the_Darkness.png"),
                new Buff("Enchanted Daggers", 28557, ParserHelper.Source.Revenant, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fa/Enchanted_Daggers.png"),
                new Buff("Phase Traversal", 28395, ParserHelper.Source.Revenant, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f2/Phase_Traversal.png"),
                new Buff("Impossible Odds", 27581, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/87/Impossible_Odds.png"),
                new Buff("Legendary Centaur Stance",27972, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8a/Legendary_Centaur_Stance.png"),
                new Buff("Legendary Dwarf Stance",27205, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b2/Legendary_Dwarf_Stance.png"),
                new Buff("Legendary Demon Stance",27928, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Legendary_Demon_Stance.png"),
                new Buff("Legendary Assassin Stance",27890, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/02/Legendary_Assassin_Stance.png"),
                //traits
                new Buff("Vicious Lacerations",29395, ParserHelper.Source.Revenant, BuffStackType.Stacking, 3, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cd/Vicious_Lacerations.png", 0, 102321),
                new Buff("Rising Momentum",51683, ParserHelper.Source.Revenant, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8c/Rising_Momentum.png"),
                new Buff("Assassin's Presence", 26854, ParserHelper.Source.Revenant, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/5/54/Assassin%27s_Presence.png"),
                new Buff("Expose Defenses", 48894, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5c/Mutilate_Defenses.png"),
                new Buff("Invoking Harmony",29025, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ec/Invoking_Harmony.png"),
                new Buff("Unyielding Devotion",55044, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4f/Unyielding_Devotion.png", 96406, ulong.MaxValue),
                new Buff("Battle Scars", 26646, ParserHelper.Source.Revenant, BuffStackType.StackingConditionalLoss, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/30/Thrill_of_Combat.png", 102321, ulong.MaxValue),
        };

        private static readonly HashSet<long> _legendSwaps = new HashSet<long>
        {
            28134, 28494, 28419, 28195, 28085, 41858
        };

        public static bool IsLegendSwap(long id)
        {
            return _legendSwaps.Contains(id);
        }
    }
}
