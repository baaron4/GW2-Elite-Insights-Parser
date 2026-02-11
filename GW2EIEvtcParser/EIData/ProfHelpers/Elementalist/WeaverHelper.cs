using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class WeaverHelper
{
    private const long ExtraOrbHammerDelay = 520;
    private static readonly IReadOnlyList<long> _weaverAttunements = new List<long>
    {
        DualFireAttunement, FireWaterAttunement, FireAirAttunement, FireEarthAttunement, WaterFireAttunement, DualWaterAttunement, WaterAirAttunement, WaterEarthAttunement, AirFireAttunement, AirWaterAttunement, DualAirAttunement, AirEarthAttunement, EarthFireAttunement, EarthWaterAttunement, EarthAirAttunement, DualEarthAttunement
    };

    public static bool IsAttunementSwap(long id)
    {
        return _weaverAttunements.Contains(id);
    }

    private static long GetLastAttunement(AgentItem agent, long time, CombatData combatData)
    {
        time = Math.Max(time, ServerDelayConstant);
        var list = new List<BuffEvent>();
        foreach (long attunement in _weaverAttunements)
        {
            list.AddRange(combatData.GetBuffDataByIDByDst(attunement, agent).Where(x => x is BuffApplyEvent && x.Time <= time + ServerDelayConstant));
        }
        if (list.Count != 0)
        {
            return list.MaxBy(x => x.Time).BuffID;
        }
        return Unknown;
    }

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
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
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData) == DualFireAttunement)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitFireAndWater, FlameWheelBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == FireWaterAttunement || last == WaterFireAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitFireAndAir, FlameWheelBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == FireAirAttunement || last == AirFireAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitFireAndEarth, FlameWheelBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting( GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == FireEarthAttunement || last == EarthFireAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        //
        new BuffGainCastFinder(IcyCoilSkill, IcyCoilBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting( GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData) == DualWaterAttunement)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitFireAndWater, IcyCoilBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(FlameWheelBuff, ba.To, ba.Time - ExtraOrbHammerDelay))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == FireWaterAttunement || last == WaterFireAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM)
            .UsingTimeOffset(-ExtraOrbHammerDelay),
        new BuffGainCastFinder(DualOrbitWaterAndAir, IcyCoilBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == WaterAirAttunement || last == AirWaterAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitWaterAndEarth, IcyCoilBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == WaterEarthAttunement || last == EarthWaterAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        //
        new BuffGainCastFinder(CrescentWindSkill, CrescentWindBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData) == DualAirAttunement)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitFireAndAir, CrescentWindBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(FlameWheelBuff, ba.To, ba.Time - ExtraOrbHammerDelay))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == FireAirAttunement || last == AirFireAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM)
            .UsingTimeOffset(-ExtraOrbHammerDelay),
        new BuffGainCastFinder(DualOrbitWaterAndAir, CrescentWindBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(IcyCoilBuff, ba.To, ba.Time - ExtraOrbHammerDelay))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == WaterAirAttunement || last == AirWaterAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM)
            .UsingTimeOffset(-ExtraOrbHammerDelay),
        new BuffGainCastFinder(DualOrbitAirAndEarth, CrescentWindBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == AirEarthAttunement || last == EarthAirAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        //
        new BuffGainCastFinder(RockyLoopSkill, RockyLoopBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting( GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData) == DualEarthAttunement)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new BuffGainCastFinder(DualOrbitFireAndEarth, RockyLoopBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff( FlameWheelBuff, ba.To, ba.Time - ExtraOrbHammerDelay))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == FireEarthAttunement || last == EarthWaterAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM)
            .UsingTimeOffset(-ExtraOrbHammerDelay),
        new BuffGainCastFinder(DualOrbitWaterAndEarth, RockyLoopBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff( IcyCoilBuff, ba.To, ba.Time - ExtraOrbHammerDelay))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == WaterEarthAttunement || last == EarthWaterAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM)
            .UsingTimeOffset(-ExtraOrbHammerDelay),
        new BuffGainCastFinder(DualOrbitAirAndEarth, RockyLoopBuff)
            .UsingToSpecChecker(Spec.Weaver)
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.IsCasting(GrandFinale, ba.To, ba.Time))
            .UsingChecker((ba, combatData, agentData, skillData) => !combatData.HasGainedBuff(CrescentWindBuff, ba.To, ba.Time - ExtraOrbHammerDelay))
            .UsingChecker((ba, combatData, agentData, skillData) => {
                    var last = GetLastAttunement(ba.To, ba.Time - ExtraOrbHammerDelay, combatData);
                    return last == AirEarthAttunement || last == EarthAirAttunement;
                }
            )
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM)
            .UsingTimeOffset(-ExtraOrbHammerDelay),
        // Spear
        new BuffGainCastFinder(FrostfireWardSkill, FrostfireWardBuff)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.January2026Balance),
        new BuffGainCastFinder(FrostfireWardSkill, FrostfireWardSecondaryAttackBuff)
            .WithBuilds(GW2Builds.January2026Balance),

        new BuffGainCastFinder(GalvanizeSkill, GalvanizeBuff)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.January2026Balance),
        new BuffGainCastFinder(GalvanizeSkill, GalvanizeSecondaryAttackBuff)
            .WithBuilds( GW2Builds.January2026Balance),

        new BuffGainCastFinder(FieryImpactSkill, FieryImpactBuff)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.January2026Balance),
        new BuffGainCastFinder(FieryImpactSkill, FieryImpactSecondaryAttackBuff)
            .WithBuilds(GW2Builds.January2026Balance),

        new BuffGainCastFinder(ElutriateSkill, ElutriateBuff)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.January2026Balance),
        new BuffGainCastFinder(ElutriateSkill, ElutriateSecondaryAttackBuff)
            .WithBuilds( GW2Builds.January2026Balance),

        new BuffGainCastFinder(ShaleStormSkill, ShaleStormBuff)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.January2026Balance),
        new BuffGainCastFinder(ShaleStormSkill, ShaleStormSecondaryAttackBuff)
            .WithBuilds(GW2Builds.January2026Balance),

        new BuffGainCastFinder(SoothingBurst, SoothingBurstSecondaryAttackBuff)
            .WithBuilds(GW2Builds.January2026Balance),
    ];


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Weaver's Prowess
        new BuffOnActorDamageModifier(Mod_WeaversProwess, WeaversProwess, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, TraitImages.WeaversProwess, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_WeaversProwess, WeaversProwess, "Weaver's Prowess", "5% cDam (8s) after switching element",  DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, TraitImages.WeaversProwess, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_WeaversProwess, WeaversProwess, "Weaver's Prowess", "10% cDam (8s) after switching element",  DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, TraitImages.WeaversProwess, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.September2023Balance),
        // Elements of Rage
        new BuffOnActorDamageModifier(Mod_ElementsOfRage, ElementsOfRage, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, TraitImages.ElementsOfRage, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_ElementsOfRage, ElementsOfRage, "Elements of Rage", "5% (8s) after double attuning", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, TraitImages.ElementsOfRage, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_ElementsOfRage, ElementsOfRage, "Elements of Rage", "10% (8s) after double attuning", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, TraitImages.ElementsOfRage, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_ElementsOfRage, ElementsOfRage, "Elements of Rage", "7% (8s) after double attuning", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, TraitImages.ElementsOfRage, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_ElementsOfRage, ElementsOfRage, "Elements of Rage", "5% (8s) after double attuning", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Weaver, ByPresence, TraitImages.ElementsOfRage, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.November2022Balance),
        // Woven Fire
        new BuffOnActorDamageModifier(Mod_WovenFire, WovenFire, "Woven Fire", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, BuffImages.WovenFire, DamageModifierMode.All),
        // Woven Air
        new BuffOnActorDamageModifier(Mod_WovenAir, WovenAir, "Wover Air", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.WovenAir, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2023Balance),
        // Perfect Weave
        new BuffOnActorDamageModifier(Mod_PerfectWeaveCondition, PerfectWeave, "Perfect Weave (Condition)", "20%", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Weaver, ByPresence, SkillImages.WeaveSelf, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_PerfectWeaveStrike, PerfectWeave, "Perfect Weave (Strike)", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, SkillImages.WeaveSelf, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2023Balance),
        // Swift Revenge
        new BuffOnActorDamageModifier(Mod_SwiftRevenge, [Swiftness, Superspeed], "Swift Revenge", "7% under swiftness/superspeed", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, TraitImages.SwiftRevenge, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_SwiftRevenge, [Swiftness, Superspeed], "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, TraitImages.SwiftRevenge, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_SwiftRevenge, [Swiftness, Superspeed], "Swift Revenge", "15% under swiftness/superspeed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, TraitImages.SwiftRevenge, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_SwiftRevenge, [Swiftness, Superspeed], "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, TraitImages.SwiftRevenge, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_SwiftRevenge, [Swiftness, Superspeed], "Swift Revenge", "10% under swiftness/superspeed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, TraitImages.SwiftRevenge, DamageModifierMode.All)
            .WithBuilds(GW2Builds.SOTOReleaseAndBalance)
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Woven Earth
        new BuffOnActorDamageModifier(Mod_WovenEarth, WovenEarth, "Woven Earth", "-20% damage", DamageSource.Incoming, -20, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, BuffImages.WovenEarth, DamageModifierMode.All),
        // Perfect Wave
        new BuffOnActorDamageModifier(Mod_PerfectWeaveStrike, PerfectWeave, "Perfect Weave", "-20% damage", DamageSource.Incoming, -20, DamageType.Strike, DamageType.All, Source.Weaver, ByPresence, SkillImages.WeaveSelf, DamageModifierMode.All),
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Dual Fire Attunement", DualFireAttunement, Source.Weaver, BuffClassification.Other, SkillImages.FireAttunement),
        new Buff("Fire Water Attunement", FireWaterAttunement, Source.Weaver, BuffClassification.Other, SkillImages.FireWaterAttunement),
        new Buff("Fire Air Attunement", FireAirAttunement, Source.Weaver, BuffClassification.Other, SkillImages.FireAirAttunement),
        new Buff("Fire Earth Attunement", FireEarthAttunement, Source.Weaver, BuffClassification.Other, SkillImages.FireEarthAttunement),
        new Buff("Dual Water Attunement", DualWaterAttunement, Source.Weaver, BuffClassification.Other, SkillImages.WaterAttunement),
        new Buff("Water Fire Attunement", WaterFireAttunement, Source.Weaver, BuffClassification.Other, SkillImages.WaterFireAttunement),
        new Buff("Water Air Attunement", WaterAirAttunement, Source.Weaver, BuffClassification.Other, SkillImages.WaterAirAttunement),
        new Buff("Water Earth Attunement", WaterEarthAttunement, Source.Weaver, BuffClassification.Other, SkillImages.WaterEarthAttunement),
        new Buff("Dual Air Attunement", DualAirAttunement, Source.Weaver, BuffClassification.Other, SkillImages.AirAttunement),
        new Buff("Air Fire Attunement", AirFireAttunement, Source.Weaver, BuffClassification.Other, SkillImages.AirFireAttunement),
        new Buff("Air Water Attunement", AirWaterAttunement, Source.Weaver, BuffClassification.Other, SkillImages.AirWaterAttunement),
        new Buff("Air Earth Attunement", AirEarthAttunement, Source.Weaver, BuffClassification.Other, SkillImages.AirEarthAttunement),
        new Buff("Dual Earth Attunement", DualEarthAttunement, Source.Weaver, BuffClassification.Other, SkillImages.EarthAttunement),
        new Buff("Earth Fire Attunement", EarthFireAttunement, Source.Weaver, BuffClassification.Other, SkillImages.EarthFireAttunement),
        new Buff("Earth Water Attunement", EarthWaterAttunement, Source.Weaver, BuffClassification.Other, SkillImages.EarthWaterAttunement),
        new Buff("Earth Air Attunement", EarthAirAttunement, Source.Weaver, BuffClassification.Other, SkillImages.EarthAirAttunement),
        new Buff("Primordial Stance", PrimordialStanceBuff, Source.Weaver, BuffClassification.Other, SkillImages.PrimordialStance),
        new Buff("Unravel", UnravelBuff, Source.Weaver, BuffClassification.Other, SkillImages.Unravel),
        new Buff("Weave Self", WeaveSelf, Source.Weaver, BuffClassification.Other, SkillImages.WeaveSelf),
        new Buff("Woven Air", WovenAir, Source.Weaver, BuffClassification.Other, BuffImages.WovenAir),
        new Buff("Woven Fire", WovenFire, Source.Weaver, BuffClassification.Other, BuffImages.WovenFire),
        new Buff("Woven Earth", WovenEarth, Source.Weaver, BuffClassification.Other, BuffImages.WovenEarth),
        new Buff("Woven Water", WovenWater, Source.Weaver, BuffClassification.Other, BuffImages.WovenWater),
        new Buff("Perfect Weave", PerfectWeave, Source.Weaver, BuffClassification.Other, SkillImages.WeaveSelf),
        new Buff("Molten Armor", MoltenArmor, Source.Weaver, BuffClassification.Other, SkillImages.LavaSkin),
        new Buff("Weaver's Prowess", WeaversProwess, Source.Weaver, BuffClassification.Other, TraitImages.WeaversProwess),
        new Buff("Elements of Rage", ElementsOfRage, Source.Weaver, BuffClassification.Other, TraitImages.ElementsOfRage),
        new Buff("Stone Resonance", StoneResonanceBuff, Source.Weaver, BuffClassification.Other, SkillImages.StoneResonance),
        new Buff("Grinding Stones", GrindingStones, Source.Weaver, BuffClassification.Other, SkillImages.GrindingStones),
        // Spear
        new Buff("Frostfire Ward", FrostfireWardBuff, Source.Weaver, BuffClassification.Hidden, SkillImages.FrostfireWard),
        new Buff("Galvanize", GalvanizeBuff, Source.Weaver, BuffClassification.Hidden, SkillImages.Galvanize),
        new Buff("Fiery Impact", FieryImpactBuff, Source.Weaver, BuffClassification.Hidden, SkillImages.FieryImpact),
        new Buff("Elutriate", ElutriateBuff, Source.Weaver, BuffClassification.Hidden, SkillImages.Elutriate),
        new Buff("Shale Storm", ShaleStormBuff, Source.Weaver, BuffClassification.Hidden, SkillImages.ShaleStorm),
        // Spear additional strikes
        new Buff("Fiery Impact (Additional Strike)", FieryImpactSecondaryAttackBuff, Source.Elementalist, BuffClassification.Other, SkillImages.FieryImpact),
        new Buff("Galvanize (Additional Strike)", GalvanizeSecondaryAttackBuff, Source.Elementalist, BuffClassification.Other, SkillImages.Galvanize),
        new Buff("Frostfire Ward (Additional Strike)", FrostfireWardBuff, Source.Elementalist, BuffClassification.Other, SkillImages.FrostfireWard),
        new Buff("Elutriate (Additional Strike)", ElutriateSecondaryAttackBuff, Source.Elementalist, BuffClassification.Other, SkillImages.Elutriate),
        new Buff("Soothing Burst (Additional Strike)", SoothingBurstSecondaryAttack, Source.Elementalist, BuffClassification.Other, SkillImages.SoothingBurst),
        new Buff("Shale Storm (Additional Strike)", ShaleStormSecondaryAttackBuff, Source.Elementalist, BuffClassification.Other, SkillImages.ShaleStorm),
    ];


    private static readonly Dictionary<long, HashSet<long>> _minorsTranslation = new()
    {
        { FireMinorAttunement, [WaterFireAttunement, AirFireAttunement, EarthFireAttunement, DualFireAttunement] },
        { WaterMinorAttunement, [FireWaterAttunement, AirWaterAttunement, EarthWaterAttunement, DualWaterAttunement] },
        { AirMinorAttunement, [FireAirAttunement, WaterAirAttunement, EarthAirAttunement, DualAirAttunement] },
        { EarthMinorAttunement, [FireEarthAttunement, WaterEarthAttunement, AirEarthAttunement, DualEarthAttunement] },
    };

    private static readonly Dictionary<long, HashSet<long>> _majorsTranslation = new()
    {
        { FireMajorAttunement, [FireWaterAttunement, FireAirAttunement, FireEarthAttunement, DualFireAttunement] },
        { WaterMajorAttunement, [WaterFireAttunement, WaterAirAttunement, WaterEarthAttunement, DualWaterAttunement] },
        { AirMajorAttunement, [AirFireAttunement, AirWaterAttunement, AirEarthAttunement, DualAirAttunement] },
        { EarthMajorAttunement, [EarthFireAttunement, EarthWaterAttunement, EarthAirAttunement, DualEarthAttunement] },
    };

    private static long TranslateWeaverAttunement(IEnumerable<BuffApplyEvent> buffApplies)
    {
        // check if more than 3 ids are present
        // Seems to happen when the attunement bug happens
        // removed the throw
        /*if (buffApplies.Select(x => x.BuffID).Distinct().Count() > 3)
        {
            throw new EIException("Too much buff apply events in TranslateWeaverAttunement");
        }*/
        HashSet<long> duals =
        [
            DualFireAttunement,
            DualWaterAttunement,
            DualAirAttunement,
            DualEarthAttunement,
        ];
        HashSet<long>? major = null;
        HashSet<long>? minor = null;
        foreach (BuffApplyEvent c in buffApplies)
        {
            if (duals.Contains(c.BuffID))
            {
                return c.BuffID;
            }
            if (_majorsTranslation.TryGetValue(c.BuffID, out var potentialMajors))
            {
                major = potentialMajors;
            }
            else if (_minorsTranslation.TryGetValue(c.BuffID, out var potentialMinors))
            {
                minor = potentialMinors;
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

    public static List<BuffEvent> TransformWeaverAttunements(IReadOnlyList<BuffEvent> buffs, Dictionary<long, List<BuffEvent>> buffsByID, AgentItem a, SkillData skillData)
    {
        List<BuffEvent> res = [];
        HashSet<long> attunements =
        [
            FireAttunementBuff,
            WaterAttunementBuff,
            AirAttunementBuff,
            EarthAttunementBuff
        ];

        // not useful for us
        /*const long fireAir = 45162;
        const long fireEarth = 42756;
        const long fireWater = 45502;
        const long waterAir = 46418;
        const long waterEarth = 42792;
        const long airEarth = 45683;*/

        HashSet<long> weaverAttunements =
        [
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
        ];
        // first we get rid of standard attunements
        HashSet<long> toClean = [];
        var attuns = buffs.Where(x => attunements.Contains(x.BuffID));
        foreach (BuffEvent c in attuns)
        {
            toClean.Add(c.BuffID);
            c.Invalidate(skillData);
        }
        // get all weaver attunements ids and group them by time
        var weaverAttuns = buffs.Where(x => weaverAttunements.Contains(x.BuffID));
        if (!weaverAttuns.Any())
        {
            return res;
        }
        Dictionary<long, List<BuffEvent>> groupByTime = GroupByTime(weaverAttuns);
        long prevID = 0;
        foreach (KeyValuePair<long, List<BuffEvent>> pair in groupByTime)
        {
            var applies = pair.Value.OfType<BuffApplyEvent>();
            long curID = TranslateWeaverAttunement(applies);
            foreach (BuffEvent c in pair.Value)
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
