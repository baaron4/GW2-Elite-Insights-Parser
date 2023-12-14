using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class WeaverHelper
    {
        private const long extraOrbHammerDelay = 520;
        private static readonly IReadOnlyList<long> _weaverAtunements = new List<long>
        {
            DualFireAttunement, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, WaterFireAttunement, DualWaterAttunement, WaterAirAttunement, WaterEarthAttunement, AirFireAttunement, AirWaterAttunement, DualAirAttunement, AirEarthAttunement, EarthFireAttunement, EarthWaterAttunement, EarthAirAttunement, DualEarthAttunement
        };

        private static long GetLastAttunement(AgentItem agent, long time, CombatData combatData)
        {
            time = Math.Max(time, ServerDelayConstant);
            var list = new List<AbstractBuffEvent>();
            foreach (long attunement in _weaverAtunements)
            {
                list.AddRange(combatData.GetBuffData(attunement).Where(x => x is BuffApplyEvent && x.To == agent && x.Time <= time + ServerDelayConstant));
            }
            if (list.Any())
            {
                return list.MaxBy(x => x.Time).BuffID;
            }
            return Unknown;
        }

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(PrimordialStanceSkill, PrimordialStanceBuff),
            new BuffGainCastFinder(StoneResonanceSkill, StoneResonanceBuff)
                .UsingICD(500),
            new BuffGainCastFinder(UnravelSkill, UnravelBuff),
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
            // Hammer 
            new BuffGainCastFinder(FlameWheelSkill, FlameWheelBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData) == DualFireAttunement)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitFireAndWater, FlameWheelBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == FireWaterAttunement || last == WaterFireAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitFireAndAir, FlameWheelBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == FireAirAttunement || last == AirFireAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitFireAndEarth, FlameWheelBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting( GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == FireEarthAttunement || last == EarthFireAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            //
            new BuffGainCastFinder(IcyCoilSkill, IcyCoilBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting( GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData) == DualWaterAttunement)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitFireAndWater, IcyCoilBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(FlameWheelBuff, ba.To, ba.Time - extraOrbHammerDelay))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == FireWaterAttunement || last == WaterFireAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM)
                .UsingTimeOffset(-extraOrbHammerDelay),
            new BuffGainCastFinder(DualOrbitWaterAndAir, IcyCoilBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == WaterAirAttunement || last == AirWaterAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitWaterAndEarth, IcyCoilBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == WaterEarthAttunement || last == EarthWaterAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            //
            new BuffGainCastFinder(CrescentWindSkill, CrescentWindBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData) == DualAirAttunement)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitFireAndAir, CrescentWindBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(FlameWheelBuff, ba.To, ba.Time - extraOrbHammerDelay))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == FireAirAttunement || last == AirFireAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM)
                .UsingTimeOffset(-extraOrbHammerDelay),
            new BuffGainCastFinder(DualOrbitWaterAndAir, CrescentWindBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(IcyCoilBuff, ba.To, ba.Time - extraOrbHammerDelay))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == WaterAirAttunement || last == AirWaterAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM)
                .UsingTimeOffset(-extraOrbHammerDelay),
            new BuffGainCastFinder(DualOrbitAirAndEarth, CrescentWindBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == AirEarthAttunement || last == EarthAirAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            //
            new BuffGainCastFinder(RockyLoopSkill, RockyLoopBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting( GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData) == DualEarthAttunement)
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new BuffGainCastFinder(DualOrbitFireAndEarth, RockyLoopBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff( FlameWheelBuff, ba.To, ba.Time - extraOrbHammerDelay))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == FireEarthAttunement || last == EarthWaterAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM)
                .UsingTimeOffset(-extraOrbHammerDelay),
            new BuffGainCastFinder(DualOrbitWaterAndEarth, RockyLoopBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff( IcyCoilBuff, ba.To, ba.Time - extraOrbHammerDelay))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == WaterEarthAttunement || last == EarthWaterAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM)
                .UsingTimeOffset(-extraOrbHammerDelay),
            new BuffGainCastFinder(DualOrbitAirAndEarth, RockyLoopBuff)
                .UsingChecker((ba, combatData, agentData, skillData) => ba.To.Spec == Spec.Weaver)
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
                .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(CrescentWindBuff, ba.To, ba.Time - extraOrbHammerDelay))
                .UsingChecker((ba, combatData, agentData, skillData) => {
                        var last = GetLastAttunement(ba.To, ba.Time - extraOrbHammerDelay, combatData);
                        return last == AirEarthAttunement || last == EarthAirAttunement;
                    }
                )
                .WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM)
                .UsingTimeOffset(-extraOrbHammerDelay),
        };


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(WeaversProwess, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, BuffImages.WeaversProwess, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(WeaversProwess, "Weaver's Prowess", "5% cDam (8s) after switching element",  DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, BuffImages.WeaversProwess, DamageModifierMode.PvE).WithBuilds(GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(WeaversProwess, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, BuffImages.WeaversProwess, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(ElementsOfRage, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.ElementsOfRage, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(ElementsOfRage, "Elements of Rage", "5% (8s) after double attuning", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, BuffImages.ElementsOfRage, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.November2022Balance),
            new BuffOnActorDamageModifier(ElementsOfRage, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, BuffImages.ElementsOfRage, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.November2022Balance, GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(ElementsOfRage, "Elements of Rage", "7% (8s) after double attuning", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, BuffImages.ElementsOfRage, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(ElementsOfRage, "Elements of Rage", "5% (8s) after double attuning", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, BuffImages.ElementsOfRage, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.November2022Balance),
            new BuffOnActorDamageModifier(WovenFire, "Woven Fire", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, BuffImages.WovenFire, DamageModifierMode.All),
            new BuffOnActorDamageModifier(WovenAir, "Wover Air", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.WovenAir, DamageModifierMode.All).WithBuilds(GW2Builds.February2023Balance),
            new BuffOnActorDamageModifier(PerfectWeave, "Perfect Weave (Condition)", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, BuffImages.WeaveSelf, DamageModifierMode.All),
            new BuffOnActorDamageModifier(PerfectWeave, "Perfect Weave (Strike)", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.WeaveSelf, DamageModifierMode.All)
                .WithBuilds(GW2Builds.February2023Balance),
            new BuffOnActorDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.SwiftRevenge, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new BuffOnActorDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.SwiftRevenge, DamageModifierMode.All)
                .WithBuilds(GW2Builds.July2019Balance, GW2Builds.November2022Balance),
            new BuffOnActorDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "15% under swiftness/superspeed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.SwiftRevenge, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.November2022Balance, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.SwiftRevenge, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.November2022Balance, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(new long[] { Swiftness, Superspeed}, "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.SwiftRevenge, DamageModifierMode.All)
                .WithBuilds(GW2Builds.SOTOReleaseAndBalance)
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(WovenEarth, "Woven Earth", "-20% damage", DamageSource.NoPets, -20, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.WovenEarth, DamageModifierMode.All),
            new BuffOnActorDamageModifier(PerfectWeave, "Perfect Weave", "-20% damage", DamageSource.NoPets, -20, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.WeaveSelf, DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Dual Fire Attunement", DualFireAttunement, Source.Weaver, BuffClassification.Other, BuffImages.FireAttunement),
            new Buff("Fire Water Attunement", FireWaterAttunement, Source.Weaver, BuffClassification.Other, BuffImages.FireWaterAttunement),
            new Buff("Fire Air Attunement", FireAirAttunement, Source.Weaver, BuffClassification.Other, BuffImages.FireAirAttunement),
            new Buff("Fire Earth Attunement", FireEarthAttunement, Source.Weaver, BuffClassification.Other, BuffImages.FireEarthAttunement),
            new Buff("Dual Water Attunement", DualWaterAttunement, Source.Weaver, BuffClassification.Other, BuffImages.WaterAttunement),
            new Buff("Water Fire Attunement", WaterFireAttunement, Source.Weaver, BuffClassification.Other, BuffImages.WaterFireAttunement),
            new Buff("Water Air Attunement", WaterAirAttunement, Source.Weaver, BuffClassification.Other, BuffImages.WaterAirAttunement),
            new Buff("Water Earth Attunement", WaterEarthAttunement, Source.Weaver, BuffClassification.Other, BuffImages.WaterEarthAttunement),
            new Buff("Dual Air Attunement", DualAirAttunement, Source.Weaver, BuffClassification.Other, BuffImages.AirAttunement),
            new Buff("Air Fire Attunement", AirFireAttunement, Source.Weaver, BuffClassification.Other, BuffImages.AirFireAttunement),
            new Buff("Air Water Attunement", AirWaterAttunement, Source.Weaver, BuffClassification.Other, BuffImages.AirWaterAttunement),
            new Buff("Air Earth Attunement", AirEarthAttunement, Source.Weaver, BuffClassification.Other, BuffImages.AirEarthAttunement),
            new Buff("Dual Earth Attunement", DualEarthAttunement, Source.Weaver, BuffClassification.Other, BuffImages.EarthAttunement),
            new Buff("Earth Fire Attunement", EarthFireAttunement, Source.Weaver, BuffClassification.Other, BuffImages.EarthFireAttunement),
            new Buff("Earth Water Attunement", EarthWaterAttunement, Source.Weaver, BuffClassification.Other, BuffImages.EarthWaterAttunement),
            new Buff("Earth Air Attunement", EarthAirAttunement, Source.Weaver, BuffClassification.Other, BuffImages.EarthAirAttunement),
            new Buff("Primordial Stance", PrimordialStanceBuff, Source.Weaver, BuffClassification.Other, BuffImages.PrimordialStance),
            new Buff("Unravel", UnravelBuff, Source.Weaver, BuffClassification.Other, BuffImages.Unravel),
            new Buff("Weave Self", WeaveSelf, Source.Weaver, BuffClassification.Other, BuffImages.WeaveSelf),
            new Buff("Woven Air", WovenAir, Source.Weaver, BuffClassification.Other, BuffImages.WovenAir),
            new Buff("Woven Fire", WovenFire, Source.Weaver, BuffClassification.Other, BuffImages.WovenFire),
            new Buff("Woven Earth", WovenEarth, Source.Weaver, BuffClassification.Other, BuffImages.WovenEarth),
            new Buff("Woven Water", WovenWater, Source.Weaver, BuffClassification.Other, BuffImages.WovenWater),
            new Buff("Perfect Weave", PerfectWeave, Source.Weaver, BuffClassification.Other, BuffImages.WeaveSelf),
            new Buff("Molten Armor", MoltenArmor, Source.Weaver, BuffClassification.Other, BuffImages.LavaSkin),
            new Buff("Weaver's Prowess", WeaversProwess, Source.Weaver, BuffClassification.Other, BuffImages.WeaversProwess),
            new Buff("Elements of Rage", ElementsOfRage, Source.Weaver, BuffClassification.Other, BuffImages.ElementsOfRage),
            new Buff("Stone Resonance", StoneResonanceBuff, Source.Weaver, BuffClassification.Other, BuffImages.StoneResonance),
            new Buff("Grinding Stones", GrindingStones, Source.Weaver, BuffClassification.Other, BuffImages.GrindingStones),
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
                FireAttunementBuff,
                WaterAttunementBuff,
                AirAttunementBuff,
                EarthAttunementBuff
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
                res.Add(new BuffApplyEvent(a, a, pair.Key, int.MaxValue, skillData.Get(curID), IFF.Friend, curInstanceID, true));
                if (prevID != 0)
                {
                    res.Add(new BuffRemoveManualEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID), IFF.Friend));
                    res.Add(new BuffRemoveAllEvent(a, a, pair.Key, int.MaxValue, skillData.Get(prevID), IFF.Friend, 1, int.MaxValue));
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
