using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class WeaverHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(PrimordialStanceSkill, PrimordialStanceEffect), // Primordial Stance
            new BuffGainCastFinder(StoneResonanceSkill, StoneResonanceEffect).UsingICD(500), // Stone Resonance
            new BuffGainCastFinder(UnravelSkill, UnravelEffect), // Unravel
            // Fire       
            new BuffGainCastFinder(DualFireAttunement, DualFireAttunement),
            new BuffGainCastFinder(FireWaterAttunement, FireWaterAttunement),
            new BuffGainCastFinder(FireAirAttunement, FireAirAttunement),
            new BuffGainCastFinder(FireEarthAttunement, FireEarthAttunement),
            // Water
            new BuffGainCastFinder(WaterFireAttunement, WaterFireAttunement),
            new BuffGainCastFinder(DualWaterAttunement, DualWaterAttunement),
            new BuffGainCastFinder(WaterAirAttunement, WaterAirAttunement),
            new BuffGainCastFinder(WaterEarthAttunement, WaterEarthAttunement),
            // Air
            new BuffGainCastFinder(AirFireAttunement, AirFireAttunement),
            new BuffGainCastFinder(AirWaterAttunement, AirWaterAttunement),
            new BuffGainCastFinder(DualAirAttunement, DualAirAttunement),
            new BuffGainCastFinder(AirEarthAttunement, AirEarthAttunement),
            // Earth
            new BuffGainCastFinder(EarthFireAttunement, EarthFireAttunement),
            new BuffGainCastFinder(EarthWaterAttunement, EarthWaterAttunement),
            new BuffGainCastFinder(EarthAirAttunement, EarthAirAttunement),
            new BuffGainCastFinder(DualEarthAttunement, DualEarthAttunement),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(WeaversProwess, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png", DamageModifierMode.All),
            new BuffDamageModifier(ElementsOfRage, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png", DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(ElementsOfRage, "Elements of Rage", "5% (8s) after double attuning", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png", DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(WovenFire, "Woven Fire", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png", DamageModifierMode.All),
            new BuffDamageModifier(PerfectWeave, "Perfect Weave", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png", DamageModifierMode.All),
            new BuffDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new BuffDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", DamageModifierMode.All).WithBuilds(GW2Builds.July2019Balance)
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Dual Fire Attunement", DualFireAttunement, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Fire Water Attunement", FireWaterAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/ar8Hn8G.png"),
                new Buff("Fire Air Attunement", FireAirAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/YU31LwG.png"),
                new Buff("Fire Earth Attunement", FireEarthAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/64g3rto.png"),
                new Buff("Dual Water Attunement", DualWaterAttunement, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Water Fire Attunement", WaterFireAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/H1peqpz.png"),
                new Buff("Water Air Attunement", WaterAirAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/Gz1XwEw.png"),
                new Buff("Water Earth Attunement", WaterEarthAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/zqX3y4c.png"),
                new Buff("Dual Air Attunement", DualAirAttunement, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Air Fire Attunement", AirFireAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/4ekncW5.png"),
                new Buff("Air Water Attunement", AirWaterAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/HIcUaXG.png"),
                new Buff("Air Earth Attunement", AirEarthAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/MCCrMls.png"),
                new Buff("Dual Earth Attunement", DualEarthAttunement, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Earth Fire Attunement", EarthFireAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/Vgu0B54.png"),
                new Buff("Earth Water Attunement", EarthWaterAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/exrTKSW.png"),
                new Buff("Earth Air Attunement", EarthAirAttunement, Source.Weaver, BuffClassification.Other, "https://i.imgur.com/Z3P8cPa.png"),
                new Buff("Primordial Stance",PrimordialStanceEffect, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3a/Primordial_Stance.png"),
                new Buff("Unravel",UnravelEffect, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4b/Unravel.png"),
                new Buff("Weave Self",WeaveSelf, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png"),
                new Buff("Woven Air",WovenAir, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/bc/Woven_Air.png"),
                new Buff("Woven Fire",WovenFire, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png"),
                new Buff("Woven Earth",WovenEarth, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7a/Woven_Earth.png"),
                new Buff("Woven Water",WovenWater, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a6/Woven_Water.png"),
                new Buff("Perfect Weave",PerfectWeave, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png"),
                new Buff("Molten Armor",MoltenArmor, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/71/Lava_Skin.png"),
                new Buff("Weaver's Prowess",WeaversProwess, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
                new Buff("Elements of Rage",ElementsOfRage, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
                new Buff("Stone Resonance",StoneResonanceEffect, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/57/Stone_Resonance.png"),
                new Buff("Grinding Stones",GrindingStones, Source.Weaver, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3d/Grinding_Stones.png"),
        };


        private static readonly Dictionary<long, HashSet<long>> _minorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMinorAttunement, new HashSet<long> { WaterFireAttunement, AirFireAttunement, EarthFireAttunement, DualFireAttunement }},
            { WaterMinorAttunement, new HashSet<long> { FireWaterAttunement, AirWaterAttunement, EarthWaterAttunement, DualWaterAttunement }},
            { AirMinorAttunement, new HashSet<long> { FireAirAttunement, WaterAirAttunement, EarthAirAttunement, DualAirAttunement }},
            { EarthMinorAttunement, new HashSet<long> { FireEarthAttunement, WaterEarthAttunement, AirEarthAttunement, DualEarthAttunement }},
        };

        private static readonly Dictionary<long, HashSet<long>> _majorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMajorAttunement, new HashSet<long> { FireWaterAttunement, FireAirAttunement, FireEarthAttunement, DualFireAttunement }},
            { WaterMajorAttunement, new HashSet<long> { WaterFireAttunement, WaterAirAttunement, WaterEarthAttunement, DualWaterAttunement }},
            { AirMajorAttunement, new HashSet<long> { AirFireAttunement, AirWaterAttunement, AirEarthAttunement, DualAirAttunement }},
            { EarthMajorAttunement, new HashSet<long> { EarthFireAttunement, EarthWaterAttunement, EarthAirAttunement, DualEarthAttunement }},
        };

        private static long TranslateWeaverAttunement(List<BuffApplyEvent> buffApplies)
        {
            // check if more than 3 ids are present
            // Seems to happen when the attunement bug happens
            // removed the throw
            /*if (buffApplies.Select(x => x.BuffID).Distinct().Count() > 3)
            {
                throw new EIException("Too much buff apply events in TranslateWeaverAttunement");
            }*/
            var duals = new HashSet<long>
            {
                DualFireAttunement,
                DualWaterAttunement,
                DualAirAttunement,
                DualEarthAttunement
            };
            HashSet<long> major = null;
            HashSet<long> minor = null;
            foreach (BuffApplyEvent c in buffApplies)
            {
                if (duals.Contains(c.BuffID))
                {
                    return c.BuffID;
                }
                if (_majorsTranslation.ContainsKey(c.BuffID))
                {
                    major = _majorsTranslation[c.BuffID];
                }
                else if (_minorsTranslation.ContainsKey(c.BuffID))
                {
                    minor = _minorsTranslation[c.BuffID];
                }
            }
            if (major == null || minor == null)
            {
                return 0;
            }
            IEnumerable<long> inter = major.Intersect(minor);
            if (inter.Count() != 1)
            {
                throw new InvalidDataException("Intersection incorrect in TranslateWeaverAttunement");
            }
            return inter.First();
        }

        public static List<AbstractBuffEvent> TransformWeaverAttunements(IReadOnlyList<AbstractBuffEvent> buffs, Dictionary<long, List<AbstractBuffEvent>> buffsByID, AgentItem a, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            var attunements = new HashSet<long>
            {
                FireAttunementEffect,
                WaterAttunementEffect,
                AirAttunementEffect,
                EarthAttunementEffect
            };

            // not useful for us
            /*const long fireAir = 45162;
            const long fireEarth = 42756;
            const long fireWater = 45502;
            const long waterAir = 46418;
            const long waterEarth = 42792;
            const long airEarth = 45683;*/

            var weaverAttunements = new HashSet<long>
            {
                FireMajorAttunement,
                FireMinorAttunement,
                WaterMajorAttunement,
                WaterMinorAttunement,
                AirMajorAttunement,
                AirMinorAttunement,
                EarthMajorAttunement,
                EarthMinorAttunement,

                DualFireAttunement,
                DualWaterAttunement,
                DualAirAttunement,
                DualEarthAttunement,

                /*fireAir,
                fireEarth,
                fireWater,
                waterAir,
                waterEarth,
                airEarth,*/
            };
            // first we get rid of standard attunements
            var toClean = new HashSet<long>();
            var attuns = buffs.Where(x => attunements.Contains(x.BuffID)).ToList();
            foreach (AbstractBuffEvent c in attuns)
            {
                toClean.Add(c.BuffID);
                c.Invalidate(skillData);
            }
            // get all weaver attunements ids and group them by time
            var weaverAttuns = buffs.Where(x => weaverAttunements.Contains(x.BuffID)).ToList();
            if (weaverAttuns.Count == 0)
            {
                return res;
            }
            Dictionary<long, List<AbstractBuffEvent>> groupByTime = GroupByTime(weaverAttuns);
            long prevID = 0;
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in groupByTime)
            {
                var applies = pair.Value.OfType<BuffApplyEvent>().ToList();
                long curID = TranslateWeaverAttunement(applies);
                foreach (AbstractBuffEvent c in pair.Value)
                {
                    toClean.Add(c.BuffID);
                    c.Invalidate(skillData);
                }
                if (curID == 0)
                {
                    continue;
                }
                uint curInstanceID = applies.First().BuffInstance;
                res.Add(new BuffApplyEvent(a, a, pair.Key, int.MaxValue, skillData.Get(curID), curInstanceID, true));
                if (prevID != 0)
                {
                    res.Add(new BuffRemoveManualEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID)));
                    res.Add(new BuffRemoveAllEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID), 1, int.MaxValue));
                }
                prevID = curID;
            }
            foreach (long buffID in toClean)
            {
                buffsByID[buffID].RemoveAll(x => x.BuffID == NoBuff);
            }
            return res;
        }
    }
}
