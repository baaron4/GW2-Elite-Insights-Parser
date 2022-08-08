using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class HeraldHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryDragonStanceSkill, LegendaryDragonStanceEffect), // Legendary Dragon Stance
            new BuffGainCastFinder(FacetOfNatureSkill, FacetOfNatureEffect), // Facet of Nature
            new BuffGainCastFinder(FacetOfDarknessSkill, FacetOfDarknessUW), // Facet of Darkness
            new BuffGainCastFinder(FacetOfElementsSkill, FacetOfElementsEffect), // Facet of Elements
            new BuffGainCastFinder(FacetOfStrengthSkill, FacetOfStrengthEffect), // Facet of Strength
            new BuffGainCastFinder(FacetOfChaosSkill, FacetOfChaosEffect), // Facet of Chaos
            new DamageCastFinder(CallOfTheDragon, CallOfTheDragon), // Call of the Dragon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(NumberOfBoons, "Reinforced Potency", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Herald, ByStack, "https://wiki.guildwars2.com/images/0/0a/Envoy_of_Sustenance.png", DamageModifierMode.All),
            new BuffDamageModifier(BurstOfStrength , "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.July2019Balance),
            new BuffDamageModifier(BurstOfStrength , "Burst of Strength", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.July2019Balance),
            new BuffDamageModifier(BurstOfStrength , "Burst of Strength", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
            new BuffDamageModifier(BurstOfStrength , "Burst of Strength", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png", DamageModifierMode.sPvPWvW).WithBuilds( GW2Builds.February2020Balance),
            // 
            new BuffDamageModifier(new long[] { FacetOfChaosEffect, FacetOfDarknessUW, FacetOfElementsEffect, FacetOfNatureEffect, FacetOfStrengthEffect, FacetOfLight}, "Forceful Persistence (Facets)", "4% per active Facet", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.August2018Balance, GW2Builds.June2022Balance),
            new BuffDamageModifier(new long[] { FacetOfChaosEffect, FacetOfDarknessUW, FacetOfElementsEffect, FacetOfNatureEffect, FacetOfStrengthEffect, FacetOfLight}, "Forceful Persistence (Facets)", "3% per active Facet", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2018Balance),
            new BuffDamageModifier(new long[] { FacetOfChaosEffect, FacetOfDarknessUW, FacetOfElementsEffect, FacetOfNatureEffect, FacetOfStrengthEffect, FacetOfLight}, "Forceful Persistence (Facets)", "5% per active Facet", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Herald, ByMultiPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.June2022Balance),
            //new BuffDamageModifier(new long[] { 27273, 27581, 28001}, "Forceful Persistence", "13% if active upkeep", DamageSource.NoPets, 13.0, DamageType.Power, DamageType.All, Source.Herald, ByPresence, "https://wiki.guildwars2.com/images/5/5f/Forceful_Persistence.png", GW2Builds.August2018Balance, GW2Builds.EndOfLife, DamageModifierMode.All), // Hammers, Embrace, Impossible Odds but how to track Protective Solace?
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {         
                //skills
                new Buff("Crystal Hibernation", CrystalHibernation, Source.Herald, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/4a/Crystal_Hibernation.png"),
                //facets
                new Buff("Facet of Light",FacetOfLight, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"),
                new Buff("Facet of Light (Traited)",FacetOfLightTraited, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/18/Facet_of_Light.png"), //Lingering buff with Draconic Echo trait
                new Buff("Infuse Light",InfuseLight, Source.Herald, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/6/60/Infuse_Light.png"),
                new Buff("Facet of Darkness",FacetOfDarknessUW, Source.Herald, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),
                new Buff("Facet of Darkness (Traited)",FacetOfDarknessTraited, Source.Herald, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/e4/Facet_of_Darkness.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Elements",FacetOfElementsEffect, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),
                new Buff("Facet of Elements (Traited)",FacetOfElementsTraited, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/ce/Facet_of_Elements.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Strength",FacetOfStrengthEffect, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),
                new Buff("Facet of Strength (Traited)",FacetOfStrengthTraited, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a8/Facet_of_Strength.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Chaos",FacetOfChaosEffect, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Chaos (Traited)",FacetOfChaosTraited, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c7/Facet_of_Chaos.png"),
                new Buff("Facet of Nature",FacetOfNatureEffect, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Facet of Nature (Traited)",FacetOfNatureTraited, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),//Lingering buff with Draconic Echo trait
                new Buff("Facet of Nature-Assassin",FacetOfNatureAssassin, Source.Herald, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/c/cd/Facet_of_Nature%E2%80%95Assassin.png"),
                new Buff("Facet of Nature-Dragon",FacetOfNatureDragon, Source.Herald, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/4/47/Facet_of_Nature%E2%80%95Dragon.png"),
                new Buff("Facet of Nature-Demon",FacetOfNatureDemon, Source.Herald, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/f/ff/Facet_of_Nature%E2%80%95Demon.png"),
                new Buff("Facet of Nature-Dwarf",FacetOfNatureDwarf, Source.Herald, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/4/4c/Facet_of_Nature%E2%80%95Dwarf.png"),
                new Buff("Facet of Nature-Centaur",FacetOfNatureCentaur, Source.Herald, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/7/74/Facet_of_Nature%E2%80%95Centaur.png"),
                new Buff("Naturalistic Resonance", NaturalisticResonance, Source.Herald, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/e/e9/Facet_of_Nature.png"),
                new Buff("Legendary Dragon Stance",LegendaryDragonStanceEffect, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d5/Legendary_Dragon_Stance.png"),
                new Buff("Hardening Persistence",HardeningPersistence, Source.Herald, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/0f/Hardening_Persistence.png"),
                new Buff("Soothing Bastion",SoothingBastion, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/04/Soothing_Bastion.png"),
                new Buff("Burst of Strength",BurstOfStrength, Source.Herald, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7b/Burst_of_Strength.png"),
                new Buff("Rising Momentum",RisingMomentum, Source.Herald, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8c/Rising_Momentum.png"),
        };
    }
}
