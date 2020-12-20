using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal static class WeaverHelper
    {
        private const long FireMajor = 40926;
        private const long FireMinor = 42811;
        private const long WaterMajor = 43236;
        private const long WaterMinor = 43370;
        private const long AirMajor = 41692;
        private const long AirMinor = 43229;
        private const long EarthMajor = 43740;
        private const long EarthMinor = 44822;

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(40183, 42086, EIData.InstantCastFinder.DefaultICD), // Primordial Stance
            new BuffGainCastFinder(44926, 45097, 500), // Stone Resonance
            new BuffGainCastFinder(44612, 42683, EIData.InstantCastFinder.DefaultICD), // Unravel
            // Fire       
            new BuffGainCastFinder(FireDual, FireDual, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(FireWater, FireWater, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(FireAir, FireAir, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(FireEarth, FireEarth, EIData.InstantCastFinder.DefaultICD),
            // Water
            new BuffGainCastFinder(WaterFire, WaterFire, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(WaterDual, WaterDual, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(WaterAir, WaterAir, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(WaterEarth, WaterEarth, EIData.InstantCastFinder.DefaultICD),
            // Air
            new BuffGainCastFinder(AirFire, AirFire, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(AirWater, AirWater, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(AirDual, AirDual, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(AirEarth, AirEarth, EIData.InstantCastFinder.DefaultICD),
            // Earth
            new BuffGainCastFinder(EarthFire, EarthFire, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(EarthWater, EarthWater, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(EarthAir, EarthAir, EIData.InstantCastFinder.DefaultICD),
            new BuffGainCastFinder(EarthDual, EarthDual, EIData.InstantCastFinder.DefaultICD),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(42061, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ParserHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png", DamageModifierMode.All),
            new BuffDamageModifier(42416, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png", DamageModifierMode.All),
            new BuffDamageModifier(45110, "Woven Fire", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, ParserHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png", DamageModifierMode.All),
            new BuffDamageModifier(45267, "Perfect Weave", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, ParserHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png", DamageModifierMode.All),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 0, 97950, DamageModifierMode.PvE),
            new BuffDamageModifier(new long[] { 719, 5974}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Weaver, ByPresence, "https://wiki.guildwars2.com/images/9/94/Swift_Revenge.png", 97950, ulong.MaxValue, DamageModifierMode.All)
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Dual Fire Attunement", FireDual, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png"),
                new Buff("Fire Water Attunement", FireWater, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/ihqKuUJ.png"),
                new Buff("Fire Air Attunement", FireAir, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/kKFJ8cT.png"),
                new Buff("Fire Earth Attunement", FireEarth, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/T4187h0.png"),
                new Buff("Dual Water Attunement", WaterDual, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png"),
                new Buff("Water Fire Attunement", WaterFire, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/vMUkzxH.png"),
                new Buff("Water Air Attunement", WaterAir, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/5G5OFud.png"),
                new Buff("Water Earth Attunement", WaterEarth, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/QKEtF2P.png"),
                new Buff("Dual Air Attunement", AirDual, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png"),
                new Buff("Air Fire Attunement", AirFire, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/vf68GJm.png"),
                new Buff("Air Water Attunement", AirWater, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/Tuj5Sro.png"),
                new Buff("Air Earth Attunement", AirEarth, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/lHcOSwk.png"),
                new Buff("Dual Earth Attunement", EarthDual, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png"),
                new Buff("Earth Fire Attunement", EarthFire, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/aJWvE0I.png"),
                new Buff("Earth Water Attunement", EarthWater, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/jtjj2TG.png"),
                new Buff("Earth Air Attunement", EarthAir, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://i.imgur.com/4Eti7Pb.png"),
                new Buff("Primordial Stance",42086, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3a/Primordial_Stance.png"),
                new Buff("Unravel",42683, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4b/Unravel.png"),
                new Buff("Weave Self",42951, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png"),
                new Buff("Woven Air",43038, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bc/Woven_Air.png"),
                new Buff("Woven Fire",45110, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b1/Woven_Fire.png"),
                new Buff("Woven Earth",45662, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Woven_Earth.png"),
                new Buff("Woven Water",43499, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a6/Woven_Water.png"),
                new Buff("Perfect Weave",45267, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/2a/Weave_Self.png"),
                new Buff("Molten Armor",43586, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/71/Lava_Skin.png"),
                new Buff("Weaver's Prowess",42061, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/75/Weaver%27s_Prowess.png"),
                new Buff("Elements of Rage",42416, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a2/Elements_of_Rage.png"),
                new Buff("Stone Resonance",45097, ParserHelper.Source.Weaver, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/57/Stone_Resonance.png"),
        };


        private static readonly Dictionary<long, HashSet<long>> _minorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMinor, new HashSet<long> { WaterFire, AirFire, EarthFire, FireDual }},
            { WaterMinor, new HashSet<long> { FireWater, AirWater, EarthWater, WaterDual }},
            { AirMinor, new HashSet<long> { FireAir, WaterAir, EarthAir, AirDual }},
            { EarthMinor, new HashSet<long> { FireEarth, WaterEarth, AirEarth, EarthDual }},
        };

        private static readonly Dictionary<long, HashSet<long>> _majorsTranslation = new Dictionary<long, HashSet<long>>
        {
            { FireMajor, new HashSet<long> { FireWater, FireAir, FireEarth, FireDual }},
            { WaterMajor, new HashSet<long> { WaterFire, WaterAir, WaterEarth, WaterDual }},
            { AirMajor, new HashSet<long> { AirFire, AirWater, AirEarth, AirDual }},
            { EarthMajor, new HashSet<long> { EarthFire, EarthWater, EarthAir, EarthDual }},
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
                FireDual,
                WaterDual,
                AirDual,
                EarthDual
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
                5585,
                5586,
                5575,
                5580
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
               FireMajor,
                FireMinor,
                WaterMajor,
                WaterMinor,
                AirMajor,
                AirMinor,
                EarthMajor,
                EarthMinor,

                FireDual,
                WaterDual,
                AirDual,
                EarthDual,

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
            var groupByTime = new Dictionary<long, List<AbstractBuffEvent>>();
            foreach (AbstractBuffEvent c in weaverAttuns)
            {
                long key = groupByTime.Keys.FirstOrDefault(x => Math.Abs(x - c.Time) < ParserHelper.ServerDelayConstant);
                if (key != 0)
                {
                    groupByTime[key].Add(c);
                }
                else
                {
                    groupByTime[c.Time] = new List<AbstractBuffEvent>
                            {
                                c
                            };
                }
            }
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
