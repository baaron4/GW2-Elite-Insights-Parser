using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class HeraldHelper : RevenantHelper
    {
        internal static readonly List<InstantCastFinder> HeraldInstantCastFinders = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(28085, 27732, InstantCastFinder.DefaultICD), // Legendary Dragon Stance
            new BuffGainCastFinder(29371, 29275, InstantCastFinder.DefaultICD), // Facet of Nature
            new BuffGainCastFinder(28379, 28036, InstantCastFinder.DefaultICD), // Facet of Darkness
            new BuffGainCastFinder(27014, 28243, InstantCastFinder.DefaultICD), // Facet of Elements
            new BuffGainCastFinder(26644, 27376, InstantCastFinder.DefaultICD), // Facet of Strength
            new BuffGainCastFinder(27760, 27983, InstantCastFinder.DefaultICD), // Facet of Chaos
        };

        internal static readonly List<Buff> HeraldBuffs = new List<Buff>
        {         
                //skills
                new Buff("Crystal Hibernation", 29303, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
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
                new Buff("Legendary Dragon Stance",27732, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
                new Buff("Hardening Persistence",28957, ParserHelper.Source.Herald, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Buff("Soothing Bastion",34136, ParserHelper.Source.Herald, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
        };
    }
}
