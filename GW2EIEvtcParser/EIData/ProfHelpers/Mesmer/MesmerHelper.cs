using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class MesmerHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffLossCastFinder(SignetOfMidnightSkill, SignetOfMidnightBuff)
            .UsingChecker((brae, combatData, agentData, skillData) => combatData.HasGainedBuff(HideInShadows, brae.To, brae.Time, 2000, brae.To))
            .UsingNotAccurate() // HideInShadows may not be applied if the Mesmer has a full stack of HideInShadows already
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(SignetOfMidnightSkill, EffectGUIDs.MesmerSignetOfMidnight)
            .UsingDstBaseSpecChecker(Spec.Mesmer),
        new BuffGainCastFinder(PortalEntre, PortalWeaving),
        new BuffGainCastFinder(PortalExeunt, PortalUses),
        new DamageCastFinder(LesserPhantasmalDefender, LesserPhantasmalDefender)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinder(Feedback, EffectGUIDs.MesmerFeedback)
            .UsingSrcBaseSpecChecker(Spec.Mesmer),
        // identify swap by buff remove
        // identify phase retreat by spawned staff clone
        // fallback to blink or phase retreat
        new EffectCastFinderByDst(Swap, EffectGUIDs.MesmerTeleport)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasLostBuffStack(IllusionaryLeapBuff, evt.Dst, evt.Time, 30))
            .UsingNotAccurate(),
        new EffectCastFinderByDst(PhaseRetreat, EffectGUIDs.MesmerTeleport)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasLostBuffStack(IllusionaryLeapBuff, evt.Dst, evt.Time, 30))
            .UsingChecker((evt, combatData, agentData, skillData) => agentData.HasSpawnedMinion(MinionID.CloneStaff, evt.Dst, evt.Time, 30))
            .UsingNotAccurate(),
        new EffectCastFinderByDst(BlinkOrPhaseRetreat, EffectGUIDs.MesmerTeleport)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasLostBuffStack(IllusionaryLeapBuff, evt.Dst, evt.Time, 30))
            .UsingChecker((evt, combatData, agentData, skillData) => !agentData.HasSpawnedMinion(MinionID.CloneStaff, evt.Dst, evt.Time, 30))
            .UsingNotAccurate(),
        // Shatters
        new EffectCastFinder(MindWrack, EffectGUIDs.MesmerDistortionOrMindWrack)
            .UsingSrcSpecsChecker([Spec.Mirage, Spec.Mesmer])
            .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasGainedBuff(DistortionBuff, evt.Src, evt.Time)),
        new EffectCastFinder(CryOfFrustration, EffectGUIDs.MesmerCryOfFrustration)
            .UsingSrcSpecsChecker([Spec.Mirage, Spec.Mesmer]),
        new EffectCastFinder(Diversion, EffectGUIDs.MesmerDiversion)
            .UsingSrcSpecsChecker([Spec.Mirage, Spec.Mesmer]),
        new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortionOrMindWrack)
            .UsingSrcSpecsChecker([Spec.Mirage, Spec.Mesmer])
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasGainedBuff(DistortionBuff, evt.Src, evt.Time))
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
        new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortionOrMindWrack)
            .UsingSrcSpecsChecker([Spec.Mirage, Spec.Mesmer, Spec.Chronomancer])
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasGainedBuff(DistortionBuff, evt.Src, evt.Time))
            .WithBuilds(GW2Builds.October2022Balance),
        // Mantras        
        new DamageCastFinder(PowerSpike, PowerSpike)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new DamageCastFinder(MantraOfPain, MantraOfPain)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
        new DamageCastFinder(PowerSpike, PowerSpike)
            .WithBuilds(GW2Builds.February2023Balance),
        new EXTHealingCastFinder(MantraOfRecovery, MantraOfRecovery)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
        new EffectCastFinderByDst(PowerReturn, EffectGUIDs.MesmerPowerReturn)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .WithBuilds(GW2Builds.February2023Balance),
        new EffectCastFinder(MantraOfResolve, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse)
            .UsingSrcBaseSpecChecker(Spec.Mesmer)
            .WithBuilds(GW2Builds.StartOfLife ,GW2Builds.February2023Balance),
        new EffectCastFinder(PowerCleanse, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse)
            .UsingSrcBaseSpecChecker(Spec.Mesmer)
            .WithBuilds(GW2Builds.February2023Balance, GW2Builds.February2024NewWeapons),
        new EffectCastFinderByDst(PowerCleanse, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse2)
            .UsingSrcNotBaseSpecChecker(Spec.Mesmer)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .UsingSecondaryEffectInvertedSrcChecker(EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new EffectCastFinderByDst(MantraOfConcentration, EffectGUIDs.MesmerMantraOfConcentrationAndPowerBreak)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2023Balance),
        new EffectCastFinderByDst(PowerBreak, EffectGUIDs.MesmerMantraOfConcentrationAndPowerBreak)
            .UsingDstBaseSpecChecker(Spec.Mesmer)
            .WithBuilds(GW2Builds.February2023Balance),
        // Rifle
        new BuffGiveCastFinder(DimensionalApertureSkill, DimensionalAperturePortalBuff),
        new EffectCastFinder(Abstraction, EffectGUIDs.MesmerRifleAbstraction)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.MesmerRifleAbstraction2)
            .UsingSrcBaseSpecChecker(Spec.Mesmer),
    ];

    internal static bool IllusionsWithMesmerChecker(DamageEvent x, ParsedEvtcLog log)
    {
        return x.From.Is(x.CreditedFrom) || IsIllusion(x.From);
    }

    internal static bool IllusionsChecker(DamageEvent x, ParsedEvtcLog log)
    {
        return IsIllusion(x.From);
    }

    internal static bool PhantasmsChecker(DamageEvent x, ParsedEvtcLog log)
    {
        return IsPhantasm(x.From);
    }

    private static bool SuperiorityComplexBonusChecker(HealthDamageEvent x, ParsedEvtcLog log)
    {
        // Damage event must be a critical hit
        if (!x.HasCrit)
        {
            return false;
        }

        bool isCC = false;
        bool hasCCBuffs = false;

        // If the target doesn't have a defiance bar, it can be CC'd
        if (x.To.GetCurrentBreakbarState(log, x.Time) == BreakbarState.None)
        {
            var ccEvents = log.CombatData.GetIncomingCrowdControlData(x.To);
            isCC = ccEvents.Any(cc => x.Time >= cc.Time && x.Time <= cc.Time + cc.Duration);
        }
        // Otherwise check for these buffs being applied
        else
        {
            long[] buffs = [Stun, Daze, Fear, Taunt];
            hasCCBuffs = x.To.HasAnyBuff(log, buffs, x.Time);
        }

        // Conditions are: Must be a crowd control event or below 50% HP.
        return isCC || hasCCBuffs || x.AgainstUnderFifty;
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Domination
        // - Empowered Illusions
        new DamageLogDamageModifier(Mod_EmpoweredIllusions, "Empowered Illusions", "15% for Illusions", DamageSource.PetsOnly, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.EmpoweredIllusions, IllusionsChecker, DamageModifierMode.All)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => IsIllusion(x.ReferenceAgentItem))),
        // - Vicious Expression
        new BuffOnFoeDamageModifier(Mod_ViciousExpressionWithIllusions, NumberOfBoons, "Vicious Expression", "25% on boonless target", DamageSource.All, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, TraitImages.ConfoundingSuggestions, DamageModifierMode.PvE)
            .UsingChecker(IllusionsWithMesmerChecker)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.February2020Balance2),
        new BuffOnFoeDamageModifier(Mod_ViciousExpressionWithIllusions, NumberOfBoons, "Vicious Expression", "15% on boonless target", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, TraitImages.ConfoundingSuggestions, DamageModifierMode.All)
            .UsingChecker(IllusionsWithMesmerChecker)
            .WithBuilds(GW2Builds.February2020Balance2),
        // - Egotism
        new DamageLogDamageModifier(Mod_Egotism, "Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.TemporalEnchanter, FromHigherThanToHPChecker, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2023Balance)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_Egotism, "Egotism", "5% if target hp% lower than self hp%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.TemporalEnchanter, FromHigherThanToHPChecker, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2023Balance)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_Egotism, "Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.TemporalEnchanter, FromHigherThanToHPChecker, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2023Balance)
            .UsingApproximate(),
        // - Fragility
        new BuffOnFoeDamageModifier(Mod_Fragility, Vulnerability, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, TraitImages.Fragility, DamageModifierMode.All),
        // - Power block
        new BuffOnActorDamageModifier(Mod_PowerBlock, PowerBlockBuff, "Power Block", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.PowerBlock, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2025Balance),
        new BuffOnActorDamageModifier(Mod_PowerBlock, PowerBlockBuff, "Power Block", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.PowerBlock, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.June2025Balance),
        // Dueling
        // - Superiority Complex
        new DamageLogDamageModifier(Mod_SuperiorityComplex, "Superiority Complex", "15% on crit", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.SuperiorityComplex, (x, log) => x.HasCrit && !SuperiorityComplexBonusChecker(x, log), DamageModifierMode.PvEInstanceOnly)
            .WithEvtcBuilds(ArcDPSBuilds.StartOfLife, ArcDPSBuilds.WeaponSwapValueIsPrevious_CrowdControlEvents_GliderEvents)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_SuperiorityComplex, "Superiority Complex", "15% on crit", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.SuperiorityComplex, (x, log) => x.HasCrit && !SuperiorityComplexBonusChecker(x, log), DamageModifierMode.PvEInstanceOnly)
            .WithEvtcBuilds(ArcDPSBuilds.WeaponSwapValueIsPrevious_CrowdControlEvents_GliderEvents),
        new DamageLogDamageModifier(Mod_SuperiorityComplexBonus, "Superiority Complex", "25% against disabled foes or below 50% hp", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.SuperiorityComplex, SuperiorityComplexBonusChecker, DamageModifierMode.PvEInstanceOnly)
            .WithEvtcBuilds(ArcDPSBuilds.StartOfLife, ArcDPSBuilds.WeaponSwapValueIsPrevious_CrowdControlEvents_GliderEvents)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_SuperiorityComplexBonus, "Superiority Complex", "25% against disabled foes or below 50% hp", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, TraitImages.SuperiorityComplex, SuperiorityComplexBonusChecker, DamageModifierMode.PvEInstanceOnly)
            .WithEvtcBuilds(ArcDPSBuilds.WeaponSwapValueIsPrevious_CrowdControlEvents_GliderEvents),
        
        // Illusions
        // - Compounding Power
        new BuffOnActorDamageModifier(Mod_CompoundingPower, CompoundingPower, "Compounding Power", "2% per stack after creating an illusion", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, TraitImages.CompoundingPower, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2023Balance),
        new BuffOnActorDamageModifier(Mod_CompoundingPower, CompoundingPower, "Compounding Power", "2% per stack after creating an illusion", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Mesmer, ByStack, TraitImages.CompoundingPower, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2023Balance),
        // - Phantasmal Force
        new BuffOnActorDamageModifier(Mod_PhantasmalForce, PhantasmalForce, "Phantasmal Force", "1% per stack of might when creating an illusion", DamageSource.PetsOnly, 1.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, TraitImages.PhantasmalForce_Mistrust, DamageModifierMode.PvE)
            .UsingEarlyExit((a, log) => !a.GetMinions(log).Any(x => IsPhantasm(x.ReferenceAgentItem)))
            .UsingChecker(PhantasmsChecker),
        
        // Chaos
        // - Illusionary Membrane
        new BuffOnActorDamageModifier(Mod_IllusionaryMembrane, Regeneration, "Illusionary Membrane", "10% under regeneration", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.IllusionaryMembrane, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.November2023Balance),
        new BuffOnActorDamageModifier(Mod_IllusionaryMembrane, ChaosAura, "Illusionary Membrane", "10% under chaos aura", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.IllusionaryMembrane, DamageModifierMode.All)
            .WithBuilds(GW2Builds.November2023Balance, GW2Builds.January2024Balance),
        new BuffOnActorDamageModifier(Mod_IllusionaryMembrane, ChaosAura, "Illusionary Membrane", "10% under chaos aura", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.IllusionaryMembrane, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.January2024Balance, GW2Builds.April2025Balance),
        new BuffOnActorDamageModifier(Mod_IllusionaryMembrane, ChaosAura, "Illusionary Membrane", "7% under chaos aura", DamageSource.NoPets, 7.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.IllusionaryMembrane, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.January2024Balance, GW2Builds.April2025Balance),
        new BuffOnActorDamageModifier(Mod_IllusionaryMembrane, IllusionaryMembrane, "Illusionary Membrane", "10%", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.IllusionaryMembrane, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.April2025Balance),
        new BuffOnActorDamageModifier(Mod_IllusionaryMembrane, IllusionaryMembrane, "Illusionary Membrane", "7%", DamageSource.NoPets, 7.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, TraitImages.IllusionaryMembrane, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.April2025Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Distortion
        new CounterOnActorDamageModifier(Mod_Distortion, DistortionBuff, "Distortion", "Invulnerable", DamageSource.Incoming, DamageType.All, DamageType.All, Source.Mesmer, SkillImages.Distortion, DamageModifierMode.All)   
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.OctoberVoERelease)
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        // Signets
        new Buff("Signet of the Ether", SignetOfTheEther, Source.Mesmer, BuffClassification.Other, SkillImages.SignetOfTheEther),
        new Buff("Signet of Domination", SignetOfDomination, Source.Mesmer, BuffClassification.Other, SkillImages.SignetOfDomination),
        new Buff("Signet of Illusions", SignetOfIllusions, Source.Mesmer, BuffClassification.Other, SkillImages.SignetOfIllusions),
        new Buff("Signet of Inspiration", SignetOfInspirationBuff, Source.Mesmer, BuffClassification.Other, SkillImages.SignetOfInspiration),
        new Buff("Signet of Midnight", SignetOfMidnightBuff, Source.Mesmer, BuffClassification.Other, SkillImages.SignetOfMidnight),
        new Buff("Signet of Humility", SignetOfHumility, Source.Mesmer, BuffClassification.Other, SkillImages.SignetOfHumility),
        // Skills
        new Buff("Distortion", DistortionBuff, Source.Mesmer, BuffStackType.Queue, 25, BuffClassification.Other, SkillImages.Distortion)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.OctoberVoERelease),
        new Buff("Distortion", DistortionBuff, Source.Mesmer, BuffStackType.Queue, 25, BuffClassification.Defensive, SkillImages.Distortion)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new Buff("Blur", Blur, Source.Mesmer, BuffClassification.Other, SkillImages.Distortion)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.OctoberVoERelease),
        new Buff("Mirror", Mirror, Source.Mesmer, BuffClassification.Other, SkillImages.Mirror),
        new Buff("Echo", Echo, Source.Mesmer, BuffClassification.Other, BuffImages.Echo),
        new Buff("Illusionary Counter", IllusionaryCounterBuff, Source.Mesmer, BuffClassification.Other, SkillImages.IllusionaryCounter),
        new Buff("Illusionary Riposte", IllusionaryRiposteBuff, Source.Mesmer, BuffClassification.Other, SkillImages.IllusionaryRiposte),
        new Buff("Illusionary Leap", IllusionaryLeapBuff, Source.Mesmer, BuffClassification.Other, SkillImages.IllusionaryLeap),
        new Buff("Portal Weaving", PortalWeaving, Source.Mesmer, BuffClassification.Other, SkillImages.PortalEnter),
        new Buff("Portal Uses", PortalUses, Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.PortalEnter),
        new Buff("Illusion of Life", IllusionOfLifeBuff, Source.Mesmer, BuffClassification.Support, SkillImages.IllusionOfLife),
        new Buff("Time Echo", TimeEcho, Source.Mesmer, BuffClassification.Other, SkillImages.DejaVu)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Dimensional Aperture", DimensionalAperturePortalBuff, Source.Mesmer, BuffClassification.Other, SkillImages.DimensionalAperture),
        // Traits
        new Buff("Fencer's Finesse", FencersFinesse , Source.Mesmer, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.FencersFinesse),
        new Buff("Illusionary Defense", IllusionaryDefense, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.IllusionaryDefense),
        new Buff("Compounding Power", CompoundingPower, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.CompoundingPower),
        new Buff("Phantasmal Force", PhantasmalForce, Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, TraitImages.PhantasmalForce_Mistrust),
        new Buff("Reflection", Reflection, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.ArcaneShield),
        new Buff("Reflection 2", Reflection2, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.ArcaneShield),
        new Buff("Illusionary Membrane", IllusionaryMembrane, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.IllusionaryMembrane),
        new Buff("Power Block", PowerBlockBuff, Source.Mesmer, BuffClassification.Other, TraitImages.PowerBlock),
        // Transformations
        new Buff("Morphed (Polymorph Moa)", MorphedPolymorphMoa, Source.Mesmer, BuffClassification.Debuff, SkillImages.MorphedPolymorphMoa),
        new Buff("Morphed (Polymorph Tuna)", MorphedPolymorphTuna, Source.Mesmer, BuffClassification.Debuff, SkillImages.MorphedPolymorphTuna),
        // Spear
        new Buff("Clarity", Clarity, Source.Mesmer, BuffClassification.Other, BuffImages.Clarity),
    ];

    private static readonly HashSet<int> _clones =
    [
        (int)MinionID.CloneSword,
        (int)MinionID.CloneScepter,
        (int)MinionID.CloneAxe,
        (int)MinionID.CloneGreatsword,
        (int)MinionID.CloneStaff,
        (int)MinionID.CloneTrident,
        (int)MinionID.CloneSpear,
        (int)MinionID.CloneDagger,
        (int)MinionID.CloneDownstate,
        (int)MinionID.CloneRifle,
        (int)MinionID.CloneUnknown,
        (int)MinionID.CloneSwordTorch,
        (int)MinionID.CloneSwordFocus,
        (int)MinionID.CloneSwordSword,
        (int)MinionID.CloneSwordShield,
        (int)MinionID.CloneSwordPistol,
        (int)MinionID.CloneIllusionaryLeap,
        (int)MinionID.CloneIllusionaryLeapFocus,
        (int)MinionID.CloneIllusionaryLeapShield,
        (int)MinionID.CloneIllusionaryLeapSword,
        (int)MinionID.CloneIllusionaryLeapPistol,
        (int)MinionID.CloneIllusionaryLeapTorch,
        (int)MinionID.CloneScepterTorch,
        (int)MinionID.CloneScepterShield,
        (int)MinionID.CloneScepterPistol,
        (int)MinionID.CloneScepterFocus,
        (int)MinionID.CloneScepterSword,
        (int)MinionID.CloneAxeTorch,
        (int)MinionID.CloneAxePistol,
        (int)MinionID.CloneAxeSword,
        (int)MinionID.CloneAxeFocus,
        (int)MinionID.CloneAxeShield,
        (int)MinionID.CloneDaggerShield,
        (int)MinionID.CloneDaggerPistol,
        (int)MinionID.CloneDaggerFocus,
        (int)MinionID.CloneDaggerTorch,
        (int)MinionID.CloneDaggerSword,
    ];

    internal static void AdjustMinionName(AgentItem minion)
    {
        switch (minion.ID)
        {
            case (int)MinionID.CloneSpear:
                minion.OverrideName("Spear " + minion.Name);
                break;
            case (int)MinionID.CloneGreatsword:
                minion.OverrideName("Greatsword " + minion.Name);
                break;
            case (int)MinionID.CloneStaff:
                minion.OverrideName("Staff " + minion.Name);
                break;
            case (int)MinionID.CloneTrident:
                minion.OverrideName("Trident " + minion.Name);
                break;
            case (int)MinionID.CloneDownstate:
                minion.OverrideName("Downstate " + minion.Name);
                break;
            case (int)MinionID.CloneRifle:
                minion.OverrideName("Rifle " + minion.Name);
                break;
            case (int)MinionID.CloneSword:
            case (int)MinionID.CloneSwordPistol:
            case (int)MinionID.CloneSwordTorch:
            case (int)MinionID.CloneSwordFocus:
            case (int)MinionID.CloneSwordSword:
            case (int)MinionID.CloneSwordShield:
            case (int)MinionID.CloneIllusionaryLeap:
            case (int)MinionID.CloneIllusionaryLeapFocus:
            case (int)MinionID.CloneIllusionaryLeapShield:
            case (int)MinionID.CloneIllusionaryLeapSword:
            case (int)MinionID.CloneIllusionaryLeapPistol:
            case (int)MinionID.CloneIllusionaryLeapTorch:
                minion.OverrideName("Sword " + minion.Name);
                break;
            case (int)MinionID.CloneScepter:
            case (int)MinionID.CloneScepterTorch:
            case (int)MinionID.CloneScepterShield:
            case (int)MinionID.CloneScepterPistol:
            case (int)MinionID.CloneScepterFocus:
            case (int)MinionID.CloneScepterSword:
                minion.OverrideName("Scepter " + minion.Name);
                break;
            case (int)MinionID.CloneAxe:
            case (int)MinionID.CloneAxeTorch:
            case (int)MinionID.CloneAxePistol:
            case (int)MinionID.CloneAxeSword:
            case (int)MinionID.CloneAxeFocus:
            case (int)MinionID.CloneAxeShield:
                minion.OverrideName("Axe " + minion.Name);
                break;
            case (int)MinionID.CloneDagger:
            case (int)MinionID.CloneDaggerShield:
            case (int)MinionID.CloneDaggerPistol:
            case (int)MinionID.CloneDaggerFocus:
            case (int)MinionID.CloneDaggerTorch:
            case (int)MinionID.CloneDaggerSword:
                minion.OverrideName("Dagger " + minion.Name);
                break;
            default:
                break;
        }
    }

    internal static bool IsClone(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return _clones.Contains(agentItem.ID);
    }

    private static readonly HashSet<int> _phantasms =
    [
        (int)MinionID.IllusionaryWarlock,
        (int)MinionID.IllusionaryWarden,
        (int)MinionID.IllusionarySwordsman,
        (int)MinionID.IllusionaryMage,
        (int)MinionID.IllusionaryDuelist,
        (int)MinionID.IllusionaryBerserker,
        (int)MinionID.IllusionaryDisenchanter,
        (int)MinionID.IllusionaryRogue,
        (int)MinionID.IllusionaryDefender,
        (int)MinionID.IllusionaryMariner,
        (int)MinionID.IllusionaryWhaler,
        (int)MinionID.IllusionaryAvenger,
        (int)MinionID.IllusionarySharpShooter,
        (int)MinionID.IllusionaryLancer,
    ];

    internal static bool IsPhantasm(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return _phantasms.Contains(agentItem.ID);
    }

    internal static bool IsIllusion(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return _clones.Contains(agentItem.ID) || _phantasms.Contains(agentItem.ID);
    }

    internal static bool IsKnownMinionID(int id)
    {
        return _phantasms.Contains(id) || _clones.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Mesmer;
        ulong gw2Build = log.CombatData.GetGW2BuildEvent().Build;
        // Portal locations
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerPortalInactive, out var portalInactives))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, PortalEntre);
            foreach (EffectEvent effect in portalInactives)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 60000, player.AgentItem, PortalWeaving);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.3, connector).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.PortalMesmerEntre, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }

        if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerPortalActive, out var portalActives))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, PortalExeunt, SkillModeCategory.Portal);
            foreach (var group in portalActives)
            {
                AttachedDecoration? first = null;
                foreach (EffectEvent effect in group)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 10000, player.AgentItem, PortalUses);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.5, connector).UsingSkillMode(skill));
                    AttachedDecoration icon = new IconDecoration(EffectImages.PortalMesmerExeunt, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                    if (first == null)
                    {
                        first = icon;
                    }
                    else
                    {
                        replay.Decorations.Add(first.LineTo(icon, color, 0.5).UsingSkillMode(skill));
                    }
                    replay.Decorations.Add(icon);
                }
            }
        }

        // Feedback
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerFeedback, out var feedbacks))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, Feedback, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in feedbacks)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary ? 6000 : 7000); // 7s with trait pre March 2024
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectFeedback);
            }
        }
        // Veil
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerVeil, out var veils))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, Veil, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in veils)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary ? 6000 : 7000); // 7s with trait pre March 2024
                var connector = new PositionConnector(effect.Position);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                replay.Decorations.Add(new RectangleDecoration(500, 70, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectVeil, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }
        // Null Field
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerNullField, out var nullFields))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, NullField, SkillModeCategory.Strip | SkillModeCategory.Cleanse);
            foreach (EffectEvent effect in nullFields)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary ? 5000 : 6000); // 6s with trait pre March 2024
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectNullField);
            }
        }

        // Illusion of Life
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerIllusionOfLife1, out var illusionOfLife))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, IllusionOfLifeSkill, SkillModeCategory.Heal);
            foreach (EffectEvent effect in illusionOfLife)
            {
                (long, long) lifespan = (effect.Time, effect.Time + 500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectIllusionOfLife);
            }
        }

        // Dimensional Aperture
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerDimensionalAperturePortal, out var dimensionalApertures))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, DimensionalApertureSkill, SkillModeCategory.Portal);
            var applies = log.CombatData.GetBuffData(DimensionalAperturePortalBuff).Where(x => x.CreditedBy.Is(player.AgentItem));
            foreach (EffectEvent effect in dimensionalApertures)
            {
                // The buff can be quite delayed
                var buffApply = applies.FirstOrDefault(x => x.Time >= effect.Time - ServerDelayConstant && x.Time <= effect.Time + 100);
                // Security
                if (buffApply != null)
                {
                    AgentItem portal = buffApply.To;
                    DespawnEvent? despawn = log.CombatData.GetDespawnEvents(portal).FirstOrDefault();
                    // Security
                    if (despawn != null)
                    {
                        uint radius = portal.HitboxWidth / 2;
                        // Security
                        if (radius == 0)
                        {
                            // 120 / 2
                            radius = 60;
                        }
                        (long start, long end) lifespan = (buffApply.Time, despawn.Time);
                        var connector = new PositionConnector(effect.Position);

                        // Portal location
                        replay.Decorations.Add(new CircleDecoration(radius, lifespan, color, 0.3, connector).UsingSkillMode(skill));
                        replay.Decorations.Add(new IconDecoration(EffectImages.PortalDimensionalAperture, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                        // Tether between the portal and the player
                        replay.Decorations.Add(new LineDecoration(lifespan, color, 0.3, new AgentConnector(player.AgentItem), connector));
                    }
                }
            }
        }

        // Unstable Bladestorm
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.VirtuosoUnstableBladestorm, out var unstableBladestorm))
        {
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, UnstableBladestorm);
            foreach (EffectEvent effect in unstableBladestorm)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 6000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectUnstableBladestorm);
            }
        }

        // Mental Collapse
        var mentalCollapseEffects = new[]
        {
            EffectGUIDs.MesmerMentalCollapse120Radius,
            EffectGUIDs.MesmerMentalCollapse240Radius,
            EffectGUIDs.MesmerMentalCollapse360Radius,
        };
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, mentalCollapseEffects, out var mentalCollapses))
        {
            var mapping = new Dictionary<GUID, (long duration, uint radius)>
            {
                { EffectGUIDs.MesmerMentalCollapse120Radius, (280, 120) },
                { EffectGUIDs.MesmerMentalCollapse240Radius, (280, 240) },
                { EffectGUIDs.MesmerMentalCollapse360Radius, (1280, 360) }
            };
            var skill = new SkillModeDescriptor(player, Spec.Mesmer, MentalCollapse);
            foreach (EffectEvent effect in mentalCollapses)
            {
                long duration = 0; // Overriding logged duration of 0
                uint radius = 0;

                if (mapping.TryGetValue(effect.GUIDEvent.ContentGUID, out (long duration, uint radius) values))
                {
                    duration = values.duration;
                    radius = values.radius;
                }

                (long, long) lifespan = (effect.Time, effect.Time + duration);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, radius, EffectImages.EffectMentalCollapse);
            }
        }

        // Chaos Storm
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerChaosStorm1, out var chaosStorms))
        {
            var skillCC = new SkillModeDescriptor(player, Spec.Mesmer, ChaosStorm, SkillModeCategory.CC);
            var skillDamage = new SkillModeDescriptor(player, Spec.Mesmer, ChaosStorm);
            foreach (EffectEvent effect in chaosStorms)
            {
                (long start, long end) = effect.ComputeLifespan(log, 5000);
                (long start, long end) lifespanCC = (start, start + 1000);
                (long start, long end) lifespanDamage = (lifespanCC.end, end);
                AddCircleSkillDecoration(replay, effect, color, skillCC, lifespanCC, 240, EffectImages.EffectChaosStorm);
                AddCircleSkillDecoration(replay, effect, color, skillDamage, lifespanDamage, 240, EffectImages.EffectChaosStorm);
            }
        }
    }
}
