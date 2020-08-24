using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class RevenantHelper : ProfHelper
    {
        internal static readonly List<InstantCastFinder> RevenantInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(28134, 27890, InstantCastFinder.DefaultICD), // Legendary Assassin Stance
            new BuffGainCastFinder(28494, 27928, InstantCastFinder.DefaultICD), // Legendary Demon Stance
            new BuffGainCastFinder(28419, 27205, InstantCastFinder.DefaultICD), // Legendary Dwarf Stance
            new BuffGainCastFinder(28195, 27972, InstantCastFinder.DefaultICD), // Legendary Centaur Stance
            new BuffGainCastFinder(27107, 27581, 500), // Impossible Odds
            new BuffLossCastFinder(28382, 27581, 500), // Relinquish Power
            new BuffGainCastFinder(26557, 27273, InstantCastFinder.DefaultICD), // Vengeful Hammers
            new BuffLossCastFinder(26956, 27273, InstantCastFinder.DefaultICD), // Release Hammers
        };

        internal static readonly List<Buff> RevenantBuffs = new List<Buff>
        {         
                //skills
                new Buff("Crystal Hibernation", 29303, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
                new Buff("Vengeful Hammers", 27273, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/c/c8/Vengeful_Hammers.png"),
                new Buff("Rite of the Great Dwarf", 26596, ParserHelper.Source.Revenant, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/69/Rite_of_the_Great_Dwarf.png"),
                new Buff("Embrace the Darkness", 28001, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Embrace_the_Darkness.png"),
                new Buff("Enchanted Daggers", 28557, ParserHelper.Source.Revenant, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fa/Enchanted_Daggers.png"),
                new Buff("Phase Traversal", 28395, ParserHelper.Source.Revenant, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f2/Phase_Traversal.png"),
                new Buff("Impossible Odds", 27581, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/87/Impossible_Odds.png"),
                //facets
                new Buff("Facet of Light",27336, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"),
                new Buff("Facet of Light (Traited)",51690, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"), //Lingering buff with Draconic Echo trait
                new Buff("Infuse Light",27737, ParserHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Buff("Facet of Darkness",28036, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),
                new Buff("Facet of Darkness (Traited)",51695, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Elements",28243, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),
                new Buff("Facet of Elements (Traited)",51706, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Strength",27376, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),
                new Buff("Facet of Strength (Traited)",51700, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Chaos",27983, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Nature",29275, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Facet of Nature (Traited)",51681, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Nature-Assassin",51692, ParserHelper.Source.Herald, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/c/cd/Facet_of_Nature%E2%80%95Assassin.png"),
                new Buff("Facet of Nature-Dragon",51674, ParserHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/47/Facet_of_Nature%E2%80%95Dragon.png"),
                new Buff("Facet of Nature-Demon",51704, ParserHelper.Source.Herald, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/f/ff/Facet_of_Nature%E2%80%95Demon.png"),
                new Buff("Facet of Nature-Dwarf",51677, ParserHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/4/4c/Facet_of_Nature%E2%80%95Dwarf.png"),
                new Buff("Facet of Nature-Centaur",51699, ParserHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/74/Facet_of_Nature%E2%80%95Centaur.png"),
                new Buff("Naturalistic Resonance", 29379, ParserHelper.Source.Herald, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                //legends
                new Buff("Legendary Centaur Stance",27972, ParserHelper.Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8a/Legendary_Centaur_Stance.png"),
                new Buff("Legendary Dragon Stance",27732, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
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
                //new Boon("Selfless Amplification",29025, BoonSource.Revenant, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/23/Selfless_Amplification.png"),
                new Buff("Hardening Persistence",28957, ParserHelper.Source.Herald, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Buff("Soothing Bastion",34136, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
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
