using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ElementalistHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(FireAttunementSkill, FireAttunementBuff),
            new BuffGainCastFinder(WaterAttunementSkill, WaterAttunementBuff),
            new BuffGainCastFinder(AirAttunementSkill, AirAttunementBuff),
            new BuffGainCastFinder(EarthAttunementSkill, EarthAttunementBuff),

            new BuffGainCastFinder(GlyphOfElementalPowerFireSkill, GlyphOfElementalPowerFireBuff),
            new BuffGainCastFinder(GlyphOfElementalPowerWaterSkill, GlyphOfElementalPowerWaterBuff),
            new BuffGainCastFinder(GlyphOfElementalPowerAirSkill, GlyphOfElementalPowerAirBuff),
            new BuffGainCastFinder(GlyphOfElementalPowerEarthSkill, GlyphOfElementalPowerEarthBuff),
            new DamageCastFinder(ArcaneBlast, ArcaneBlast),
            new BuffGiveCastFinder(ArcanePowerSkill, ArcanePowerBuff),
            new BuffGainCastFinder(ArcaneShieldSkill, ArcaneShieldBuff),
            new DamageCastFinder(ArcaneWave, ArcaneWave),
            new BuffGainCastFinder(MistForm, MistForm),
            new DamageCastFinder(SignetOfAirSkill, SignetOfAirSkill)
                .UsingDisableWithEffectData(),
            new EffectCastFinderByDst(SignetOfAirSkill, EffectGUIDs.ElementalistSignetOfAir)
                .UsingDstBaseSpecChecker(Spec.Elementalist),
            new DamageCastFinder(Sunspot, Sunspot)
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(EarthenBlast, EarthenBlast)
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(LightningStrike, LightningStrike),
            new DamageCastFinder(LightningRod, LightningRod)
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(LightningFlash, LightningFlash)/*.UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasEffectData)*/,
            new EffectCastFinderByDst(ArmorOfEarth, EffectGUIDs.ElementalistArmorOfEarth1)
                .UsingDstBaseSpecChecker(Spec.Elementalist),
            //new EffectCastFinderByDst(CleansingFire, EffectGUIDs.ElementalistCleansingFire).UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.BaseSpec == Spec.Elementalist && evt.Src == evt.Dst),
            //new EffectCastFinder(LightningFlash, EffectGUIDs.ElementalistLightningFlash).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == Spec.Elementalist && evt.Src == evt.Dst)

            // Elementals
            new MinionCommandCastFinder(FireElementalFlameBarrage, (int) MinionID.FireElemental),
            new MinionCommandCastFinder(WaterElementalCrashingWaves, (int) MinionID.IceElemental),
            new MinionCommandCastFinder(AirElementalShockingBolt, (int) MinionID.AirElemental),
            new MinionCommandCastFinder(EarthElementalStomp, (int) MinionID.EarthElemental),
            // Hammer
            new BuffGainCastFinder(FlameWheelSkill, FlameWheelBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec != Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(IcyCoilSkill, IcyCoilBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec != Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(CrescentWindSkill, CrescentWindBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec != Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(RockyLoopSkill, RockyLoopBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec != Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Fire
            new BuffDamageModifier(PersistingFlames, "Persisting Flames", "1% per stack", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, BuffImages.PersistingFlames, DamageModifierMode.All)
                .WithBuilds(GW2Builds.July2020Balance, GW2Builds.EndOfLife),
            new BuffDamageModifier(new long[] { FireAttunementBuff, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, DualFireAttunement }, "Pyromancer's Training", "10% while fire attuned", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PyromancersTraining, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new BuffDamageModifierTarget(Burning, "Burning Rage", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.BurningRage, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new BuffDamageModifierTarget(Burning, "Pyromancer's Training", "10% on burning target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PyromancersTraining, DamageModifierMode.All)
                .WithBuilds(GW2Builds.July2019Balance),
            // Air
            new DamageLogDamageModifier("Bolt to the Heart", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Elementalist, BuffImages.BoltToTheHeart, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Earth
            new BuffDamageModifierTarget(Bleeding, "Serrated Stones", "5% to bleeding target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.SerratedStones, DamageModifierMode.All),
            // Water
            new DamageLogDamageModifier("Aquamancer's Training", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, BuffImages.AquamancersTraining, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new BuffDamageModifierTarget(Vulnerability, "Piercing Shards w/ Water", "20% on vuln target while on water", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PiercingShards, DamageModifierMode.PvE)
                .UsingSourceActivator(new long[] { WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, ByPresence)
                .WithBuilds(GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            new BuffDamageModifierTarget(Vulnerability, "Piercing Shards", "10% on vuln target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PiercingShards, DamageModifierMode.PvE)
                .UsingSourceActivator(new long[] { WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, ByAbsence)
                .WithBuilds(GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            new BuffDamageModifierTarget(Vulnerability, "Piercing Shards w/ Water", "10% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PiercingShards, DamageModifierMode.sPvPWvW)
                .UsingSourceActivator(new long[] { WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, ByPresence)
                .WithBuilds(GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            new BuffDamageModifierTarget(Vulnerability, "Piercing Shards", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PiercingShards, DamageModifierMode.sPvPWvW)
                .UsingSourceActivator(new long[] { WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, ByAbsence)
                .WithBuilds(GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            new BuffDamageModifierTarget(Vulnerability, "Piercing Shards", "20% on vuln target while on water", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByPresence, BuffImages.PiercingShards, DamageModifierMode.PvE)
                .UsingSourceActivator(new long[] { WaterAttunementBuff, WaterAirAttunement, WaterEarthAttunement, WaterFireAttunement, DualWaterAttunement }, ByPresence)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new DamageLogDamageModifier("Flow like Water", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, BuffImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, DamageModifierMode.All)
                .WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance)
                .UsingApproximate(true),
            new DamageLogDamageModifier("Flow like Water", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Elementalist, BuffImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.February2020Balance, GW2Builds.EndOfLife)
                .UsingApproximate(true),
            new DamageLogDamageModifier("Flow like Water", "5% if hp >=75%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Elementalist, BuffImages.FlowLikeWater, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.February2020Balance, GW2Builds.EndOfLife)
                .UsingApproximate(true),
            //new DamageLogDamageModifier("Flow like Water", "10% over 75% HP", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParseHelper.Source.Elementalist, BuffImages.FlowLikeWater, x => x.IsOverNinety, ByPresence, GW2Builds.July2019Balance, GW2Builds.EndOfLife),
            // Arcane
            new BuffDamageModifier(NumberOfBoons, "Bountiful Power", "2% per boon", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Elementalist, ByStack, BuffImages.BountifulPower, DamageModifierMode.All),
            new BuffDamageModifierTarget(new long[] { Stun, Daze, Knockdown, Fear, Taunt }, "Stormsoul", "10% to disabled foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Elementalist, ByPresence, BuffImages.Stormsoul, DamageModifierMode.All)
                .WithBuilds(GW2Builds.December2018Balance)
                .UsingApproximate(true),
            new BuffDamageModifier(FlameWheelBuff, "Flame Wheel", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Elementalist, ByPresence, BuffImages.FlameWheel, DamageModifierMode.All)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {       
            // Signets
            new Buff("Signet of Restoration", SignetOfRestoration, Source.Elementalist, BuffClassification.Other, BuffImages.SignetOfRestoration),
            new Buff("Signet of Air", SignetOfAirBuff, Source.Elementalist, BuffClassification.Other, BuffImages.SignetOfAir),
            new Buff("Signet of Earth", SignetOfEarth, Source.Elementalist, BuffClassification.Other, BuffImages.SignetOfEarth),
            new Buff("Signet of Fire", SignetOfFire, Source.Elementalist, BuffClassification.Other, BuffImages.SignetOfFire),
            new Buff("Signet of Water", SignetOfWater, Source.Elementalist, BuffClassification.Other, BuffImages.SignetOfWater),
            // Attunements
            // Fire
            new Buff("Fire Attunement", FireAttunementBuff, Source.Elementalist, BuffClassification.Other, BuffImages.FireAttunement),
            // Water
            new Buff("Water Attunement", WaterAttunementBuff, Source.Elementalist, BuffClassification.Other, BuffImages.WaterAttunement),
            // Air
            new Buff("Air Attunement", AirAttunementBuff, Source.Elementalist, BuffClassification.Other, BuffImages.AirAttunement),
            // Earth
            new Buff("Earth Attunement", EarthAttunementBuff, Source.Elementalist, BuffClassification.Other, BuffImages.EarthAttunement),
            // Forms
            new Buff("Mist Form", MistForm, Source.Elementalist, BuffClassification.Other, BuffImages.MistForm),
            new Buff("Mist Form 2", MistForm2, Source.Elementalist, BuffClassification.Other, BuffImages.MistForm),
            new Buff("Ride the Lightning",RideTheLightningBuff, Source.Elementalist, BuffClassification.Other, BuffImages.RideTheLightning),
            new Buff("Vapor Form", VaporForm, Source.Elementalist, BuffClassification.Other, BuffImages.VaporForm),
            new Buff("Tornado", Tornado, Source.Elementalist, BuffClassification.Other, BuffImages.Tornado),
            new Buff("Whirlpool", Whirlpool, Source.Elementalist, BuffClassification.Other, BuffImages.Whirlpool),
            new Buff("Electrified Tornado", ElectrifiedTornado, Source.Elementalist, BuffClassification.Other, BuffImages.ChainLightning),
            new Buff("Arcane Lightning", ArcaneLightning, Source.Elementalist, BuffClassification.Other, BuffImages.ElementalSurge),
            // Conjures
            new Buff("Conjure Earth Shield", ConjureEarthShield, Source.Elementalist, BuffClassification.Support, BuffImages.ConjureEarthShield),
            new Buff("Conjure Flame Axe", ConjureFlameAxe, Source.Elementalist, BuffClassification.Support, BuffImages.ConjureFlameAxe),
            new Buff("Conjure Frost Bow", ConjureFrostBow, Source.Elementalist, BuffClassification.Support, BuffImages.ConjureFrostBow),
            new Buff("Conjure Lightning Hammer", ConjureLightningHammer, Source.Elementalist, BuffClassification.Support, BuffImages.ConjureLightningHammer),
            new Buff("Conjure Fiery Greatsword", ConjureFieryGreatsword, Source.Elementalist, BuffClassification.Support, BuffImages.ConjureFieryGreatsword),
            new Buff("Freeze 1", Freeze1, Source.Elementalist, BuffClassification.Other, BuffImages.Stun),
            new Buff("Freeze 2", Freeze2, Source.Elementalist, BuffClassification.Other, BuffImages.Stun),
            // Summons
            new Buff("Lesser Air Elemental Summoned", LesserAirElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfLesserElementalsAir),
            new Buff("Air Elemental Summoned", AirElementalSummoned, Source.Elementalist, BuffClassification.Other, BuffImages.GlyphOfElementalsAir),
            new Buff("Lesser Water Elemental Summoned", LesserWaterElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfLesserElementalsWater),
            new Buff("Water Elemental Summoned", WaterElementalSummoned, Source.Elementalist, BuffClassification.Other, BuffImages.GlyphOfElementalsWater),
            new Buff("Lesser Fire Elemental Summoned", LesserFireElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfLesserElementalsFire),
            new Buff("Fire Elemental Summoned", FireElementalSummoned, Source.Elementalist, BuffClassification.Other, BuffImages.GlyphOfElementalsFire),
            new Buff("Lesser Earth Elemental Summoned", LesserEarthElementalSummoned, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfLesserElementalsEarth),
            new Buff("Earth Elemental Summoned", EarthElementalSummoned, Source.Elementalist, BuffClassification.Other, BuffImages.GlyphOfElementalsEarth),
            // Skills
            new Buff("Arcane Power", ArcanePowerBuff, Source.Elementalist, BuffStackType.Stacking, 6, BuffClassification.Other, BuffImages.ArcanePower),
            new Buff("Arcane Shield", ArcaneShieldBuff, Source.Elementalist, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.ArcaneShield),
            new Buff("Renewal of Fire", RenewalOfFire, Source.Elementalist, BuffClassification.Other, BuffImages.RenewalOfFire),
            new Buff("Rock Barrier", RockBarrier, Source.Elementalist, BuffClassification.Other, BuffImages.RockBarrier),//750?
            new Buff("Magnetic Wave", MagneticWave, Source.Elementalist, BuffClassification.Other, BuffImages.MagneticWave),
            new Buff("Obsidian Flesh", ObsidianFlesh, Source.Elementalist, BuffClassification.Other, BuffImages.ObsidianFlesh),
            new Buff("Persisting Flames", PersistingFlames, Source.Elementalist, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.PersistingFlames)
                .WithBuilds(GW2Builds.July2020Balance, GW2Builds.EndOfLife),
            new Buff("Fresh Air", FreshAir, Source.Elementalist, BuffClassification.Other, BuffImages.FreshAir),
            new Buff("Soothing Mist", SoothingMist, Source.Elementalist, BuffClassification.Defensive, BuffImages.SoothingMist)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2023Balance),
            new Buff("Soothing Mist", SoothingMist, Source.Elementalist, BuffStackType.Queue, 9, BuffClassification.Defensive, BuffImages.SoothingMist)
                .WithBuilds(GW2Builds.May2023Balance),
            new Buff("Stone Heart", StoneHeart, Source.Elementalist, BuffClassification.Defensive, BuffImages.StoneHeart),
            new Buff("Glyph of Elemental Power (Fire)", GlyphOfElementalPowerFireBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfElementalPowerFire),
            new Buff("Glyph of Elemental Power (Air)", GlyphOfElementalPowerAirBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfElementalPowerAir),
            new Buff("Glyph of Elemental Power (Water)", GlyphOfElementalPowerWaterBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfElementalPowerWater),
            new Buff("Glyph of Elemental Power (Earth)", GlyphOfElementalPowerEarthBuff, Source.Elementalist, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.GlyphOfElementalPowerEarth),
            // Hammer
            new Buff("Flame Wheel", FlameWheelBuff, Source.Elementalist, BuffClassification.Other, BuffImages.FlameWheel)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Icy Coil", IcyCoilBuff, Source.Elementalist, BuffClassification.Other, BuffImages.IcyCoil)
                .WithBuilds( GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Crescent Wind", CrescentWindBuff, Source.Elementalist, BuffClassification.Other, BuffImages.CrescentWind)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Rocky Loop", RockyLoopBuff, Source.Elementalist, BuffClassification.Other, BuffImages.RockyLoop)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
        };


        private static readonly HashSet<long> _elementalSwaps = new HashSet<long>
        {
            FireAttunementSkill,WaterAttunementSkill,AirAttunementSkill, EarthAttunementSkill, DualFireAttunement, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, WaterFireAttunement, DualWaterAttunement, WaterAirAttunement, WaterEarthAttunement, AirFireAttunement, AirWaterAttunement, DualAirAttunement, AirEarthAttunement, EarthFireAttunement, EarthWaterAttunement, EarthAirAttunement, DualEarthAttunement
        };

        public static bool IsElementalSwap(long id)
        {
            return _elementalSwaps.Contains(id);
        }

        public static void RemoveDualBuffs(IReadOnlyList<AbstractBuffEvent> buffsPerDst, Dictionary<long, List<AbstractBuffEvent>> buffsByID, SkillData skillData)
        {
            var duals = new HashSet<long>
            {
                DualFireAttunement,
                DualWaterAttunement,
                DualAirAttunement,
                DualEarthAttunement,
            };
            var toClean = new HashSet<long>();
            foreach (AbstractBuffEvent c in buffsPerDst.Where(x => duals.Contains(x.BuffID)))
            {
                toClean.Add(c.BuffID);
                c.Invalidate(skillData);
            }
            foreach (long buffID in toClean)
            {
                buffsByID[buffID].RemoveAll(x => x.BuffID == NoBuff);
            }
        }

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.LesserAirElemental,
            (int)MinionID.LesserEarthElemental,
            (int)MinionID.LesserFireElemental,
            (int)MinionID.LesserIceElemental,
            (int)MinionID.AirElemental,
            (int)MinionID.EarthElemental,
            (int)MinionID.FireElemental,
            (int)MinionID.IceElemental,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        public static IReadOnlyList<AnimatedCastEvent> ComputeUpdraftCastEvents(Player player, CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<AnimatedCastEvent>();
            SkillItem skill = skillData.Get(Updraft);
            if (combatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistUpdraft2, out IReadOnlyList<EffectEvent> updrafts))
            {
                foreach(EffectEvent effect in updrafts)
                {
                    res.Add(new AnimatedCastEvent(player.AgentItem, skill, effect.Time, 1000));
                }
            }
            return res;
        }

        public static IReadOnlyList<AnimatedCastEvent> ComputeRideTheLightningCastEvents(Player player, CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<AnimatedCastEvent>();
            SkillItem skill = skillData.Get(RideTheLightningSkill);
            var applies = combatData.GetBuffData(RideTheLightningBuff).OfType<BuffApplyEvent>().Where(x => x.To == player.AgentItem).ToList();
            var removals = combatData.GetBuffData(RideTheLightningBuff).OfType<BuffRemoveAllEvent>().Where(x => x.To == player.AgentItem).ToList();
            for (int i = 0; i < applies.Count && i < removals.Count; i++)
            {
                res.Add(new AnimatedCastEvent(player.AgentItem, skill, applies[i].Time, removals[i].Time - applies[i].Time));
            }
            return res;
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Elementalist;

            // Meteor Shower - Outer circle
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistMeteorShowerCircle, out IReadOnlyList<EffectEvent> meteorShowerCircles))
            {
                var skill = new SkillModeDescriptor(player, Spec.Elementalist, MeteorShower);
                foreach (EffectEvent effect in meteorShowerCircles)
                {
                    (long, long) lifespan = ProfHelper.ComputeDynamicEffectLifespan(log, effect, 9000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(360, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectMeteorShower, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
                // The meteors
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistMeteorShowerMeteor, out IReadOnlyList<EffectEvent> meteorShowersMeteors))
                {
                    foreach (EffectEvent effect in meteorShowersMeteors)
                    {
                        (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 1330);
                        var connector = new PositionConnector(effect.Position);
                        // -750 to make the decoration faster than in game
                        replay.Decorations.Add(new CircleDecoration(180, (lifespan.Item2 - 750, lifespan.Item2), color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                        replay.Decorations.Add(new CircleDecoration(180, (lifespan.Item2 - 750, lifespan.Item2), color.WithAlpha(0.2f).ToString(), connector).UsingFilled(false).UsingGrowingEnd(lifespan.Item2, true).UsingSkillMode(skill));
                    }
                }
            }

            // Static Field (Staff)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistStaticFieldStaff, out IReadOnlyList<EffectEvent> staticFieldsStaff))
            {
                var skill = new SkillModeDescriptor(player, Spec.Elementalist, StaticFieldStaff);
                foreach (EffectEvent effect in staticFieldsStaff)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 4000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(180, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectStaticField, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }

            // Static Field (Lightning Hammer)
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistStaticFieldLightningHammer, out IReadOnlyList<EffectEvent> staticFieldsHammer))
            {
                var skill = new SkillModeDescriptor(player, Spec.Elementalist, StaticFieldLightingHammer);
                foreach (EffectEvent effect in staticFieldsHammer)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 4000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectStaticField, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }

            // Updraft
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistUpdraft2, out IReadOnlyList<EffectEvent> updrafts))
            {
                var skill = new SkillModeDescriptor(player, Spec.Elementalist, Updraft);
                foreach (EffectEvent effect in updrafts)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 750);
                    var connector = new PositionConnector(effect.Position);
                    var circle = (CircleDecoration)new CircleDecoration(240, lifespan, color.WithAlpha(0.3f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill);
                    replay.AddDecorationWithGrowing(circle, lifespan.Item2, true);
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectUpdraft, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }

            // Firestorm
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistFirestorm, out IReadOnlyList<EffectEvent> firestorms))
            {
                var skill = new SkillModeDescriptor(player, Spec.Elementalist, FirestormGlyphOfStormsOrFieryGreatsword);
                foreach (EffectEvent effect in firestorms)
                {
                    (long, long) lifespan = ProfHelper.ComputeDynamicEffectLifespan(log, effect, 10000);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectFirestormGlyphOrFieryGreatsword, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }

            // Geyser
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistGeyser, out IReadOnlyList<EffectEvent> geysers))
            {
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ElementalistGeyserSplash, out IReadOnlyList<EffectEvent> geyserSplash))
                {
                    var skill = new SkillModeDescriptor(player, Spec.Elementalist, GeyserStaffElementalist);
                    foreach (EffectEvent effect in geysers)
                    {
                        if (!geyserSplash.Any(x => Math.Abs(x.Time - effect.Time) < ServerDelayConstant))
                        {
                            continue;
                        }
                        (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 4000);
                        var connector = new PositionConnector(effect.Position);
                        replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.5f).ToString(), connector).UsingFilled(false).UsingSkillMode(skill));
                        replay.Decorations.Add(new IconDecoration(ParserIcons.EffectGeyser, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                    }
                }
                
            }
        }
    }
}
