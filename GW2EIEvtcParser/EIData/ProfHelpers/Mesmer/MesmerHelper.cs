using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MesmerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffLossCastFinder(SignetOfMidnightSkill, SignetOfMidnightBuff)
                .UsingChecker((brae, combatData, agentData, skillData) => {
                     return combatData.HasGainedBuff(HideInShadows, brae.To, brae.Time, 2000, brae.To);
                })
                .UsingNotAccurate(true) // HideInShadows may not be applied if the Mesmer has a full stack of HideInShadows already
                .UsingDisableWithEffectData(),
            new EffectCastFinderByDst(SignetOfMidnightSkill, EffectGUIDs.MesmerSignetOfMidnight).UsingDstBaseSpecChecker(Spec.Mesmer),
            new BuffGainCastFinder(PortalEntre, PortalWeaving),
            new BuffGainCastFinder(PortalExeunt, PortalUses),
            new DamageCastFinder(LesserPhantasmalDefender, LesserPhantasmalDefender).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EffectCastFinder(Feedback, EffectGUIDs.MesmerFeedback).UsingSrcBaseSpecChecker(Spec.Mesmer),
            // identify swap by buff remove
            // identify phase retreat by spawned staff clone
            // fallback to blink or phase retreat
            new EffectCastFinderByDst(Swap, EffectGUIDs.MesmerTeleport)
                .UsingDstBaseSpecChecker(Spec.Mesmer)
                .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasLostBuffStack(IllusionaryLeapBuff, evt.Dst, evt.Time, 30))
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(PhaseRetreat, EffectGUIDs.MesmerTeleport)
                .UsingDstBaseSpecChecker(Spec.Mesmer)
                .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasLostBuffStack(IllusionaryLeapBuff, evt.Dst, evt.Time, 30))
                .UsingChecker((evt, combatData, agentData, skillData) => agentData.HasSpawnedMinion(MinionID.CloneStaff, evt.Dst, evt.Time, 30))
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(BlinkOrPhaseRetreat, EffectGUIDs.MesmerTeleport)
                .UsingDstBaseSpecChecker(Spec.Mesmer)
                .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasLostBuffStack(IllusionaryLeapBuff, evt.Dst, evt.Time, 30))
                .UsingChecker((evt, combatData, agentData, skillData) => !agentData.HasSpawnedMinion(MinionID.CloneStaff, evt.Dst, evt.Time, 30))
                .UsingNotAccurate(true),
            // Shatters
            new EffectCastFinder(MindWrack, EffectGUIDs.MesmerDistortionOrMindWrack)
                .UsingSrcSpecsChecker(new HashSet<Spec> { Spec.Mirage, Spec.Mesmer})
                .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasGainedBuff(DistortionBuff, evt.Src, evt.Time)),
            new EffectCastFinder(CryOfFrustration, EffectGUIDs.MesmerCryOfFrustration)
                .UsingSrcSpecsChecker(new HashSet<Spec> { Spec.Mirage, Spec.Mesmer}),
            new EffectCastFinder(Diversion, EffectGUIDs.MesmerDiversion)
                .UsingSrcSpecsChecker(new HashSet<Spec> { Spec.Mirage, Spec.Mesmer}),
            new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortionOrMindWrack)
                .UsingSrcSpecsChecker(new HashSet<Spec> { Spec.Mirage, Spec.Mesmer})
                .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasGainedBuff(DistortionBuff, evt.Src, evt.Time))
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortionOrMindWrack)
                .UsingSrcSpecsChecker(new HashSet<Spec> { Spec.Mirage, Spec.Mesmer, Spec.Chronomancer})
                .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasGainedBuff(DistortionBuff, evt.Src, evt.Time))
                .WithBuilds(GW2Builds.October2022Balance),
            // Mantras        
            new DamageCastFinder(PowerSpike, PowerSpike).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new DamageCastFinder(MantraOfPain, MantraOfPain).WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new DamageCastFinder(PowerSpike, PowerSpike).WithBuilds(GW2Builds.February2023Balance),
            new EXTHealingCastFinder(MantraOfRecovery, MantraOfRecovery).WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(PowerReturn, EffectGUIDs.MesmerPowerReturn).UsingDstBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.February2023Balance),
            new EffectCastFinder(MantraOfResolve, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse).UsingSrcBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.StartOfLife ,GW2Builds.February2023Balance),
            new EffectCastFinder(PowerCleanse, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse).UsingSrcBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.February2023Balance, GW2Builds.February2024NewWeapons),
            new EffectCastFinder(PowerCleanse, EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse)
                .UsingSrcBaseSpecChecker(Spec.Mesmer)
                .UsingSecondaryEffectChecker(EffectGUIDs.MesmerMantraOfResolveAndPowerCleanse2)
                .WithBuilds(GW2Builds.February2024NewWeapons),
            new EffectCastFinderByDst(MantraOfConcentration, EffectGUIDs.MesmerMantraOfConcentrationAndPowerBreak).UsingDstBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2023Balance),
            new EffectCastFinderByDst(PowerBreak, EffectGUIDs.MesmerMantraOfConcentrationAndPowerBreak).UsingDstBaseSpecChecker(Spec.Mesmer).WithBuilds(GW2Builds.February2023Balance),
            // Rifle
            new BuffGiveCastFinder(DimensionalApertureSkill, DimensionalAperturePortalBuff),
            new EffectCastFinder(Abstraction, EffectGUIDs.MesmerRifleAbstraction)
                .UsingSrcBaseSpecChecker(Spec.Mesmer),
        };


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Domination
            // Empowered illusions require knowing all illusion species ID
            // We need illusion species ID to enable Vicious Expression on All
            new BuffOnFoeDamageModifier(NumberOfBoons, "Vicious Expression", "25% on boonless target",  DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, BuffImages.ConfoundingSuggestions, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.February2020Balance2),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Vicious Expression", "15% on boonless target",  DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, BuffImages.ConfoundingSuggestions, DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance2),
            //
            new DamageLogDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, BuffImages.TemporalEnchanter, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2023Balance).UsingApproximate(true),
            new DamageLogDamageModifier("Egotism", "5% if target hp% lower than self hp%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Mesmer, BuffImages.TemporalEnchanter, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2023Balance).UsingApproximate(true),
            new DamageLogDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, BuffImages.TemporalEnchanter, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, DamageModifierMode.All).WithBuilds(GW2Builds.February2023Balance).UsingApproximate(true),
            //
            new BuffOnFoeDamageModifier(Vulnerability, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, BuffImages.Fragility, DamageModifierMode.All),
            // Dueling
            // Superiority Complex can all the conditions be tracked?
            // Illusions
            new BuffOnActorDamageModifier(CompoundingPower, "Compounding Power", "2% per stack after creating an illusion", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, BuffImages.CompoundingPower, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2023Balance),
            new BuffOnActorDamageModifier(CompoundingPower, "Compounding Power", "2% per stack after creating an illusion", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Mesmer, ByStack, BuffImages.CompoundingPower, DamageModifierMode.All).WithBuilds(GW2Builds.November2023Balance),
            // Phantasmal Force: the current infrastructure is not capable of checking buffs on minions, once we have that, this does not require knowing illusion species id
            // Chaos       
            new BuffOnActorDamageModifier(Regeneration, "Illusionary Membrane", "10% under regeneration", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, BuffImages.IllusionaryMembrane, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance, GW2Builds.November2023Balance),
            new BuffOnActorDamageModifier(ChaosAura, "Illusionary Membrane", "10% under chaos aura", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, BuffImages.IllusionaryMembrane, DamageModifierMode.All).WithBuilds(GW2Builds.November2023Balance, GW2Builds.January2024Balance),
            new BuffOnActorDamageModifier(ChaosAura, "Illusionary Membrane", "10% under chaos aura", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, BuffImages.IllusionaryMembrane, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.January2024Balance),
            new BuffOnActorDamageModifier(ChaosAura, "Illusionary Membrane", "7% under chaos aura", DamageSource.NoPets, 7.0, DamageType.Condition, DamageType.All, Source.Mesmer, ByPresence, BuffImages.IllusionaryMembrane, DamageModifierMode.PvE).WithBuilds(GW2Builds.January2024Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new CounterOnActorDamageModifier(DistortionBuff, "Distortion", "Invulnerable", DamageSource.NoPets, DamageType.All, DamageType.All, Source.Mesmer, BuffImages.Distortion, DamageModifierMode.All)
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // Signets
            new Buff("Signet of the Ether", SignetOfTheEther, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfTheEther),
            new Buff("Signet of Domination", SignetOfDomination, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfDomination),
            new Buff("Signet of Illusions", SignetOfIllusions, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfIllusions),
            new Buff("Signet of Inspiration", SignetOfInspirationBuff, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfInspiration),
            new Buff("Signet of Midnight", SignetOfMidnightBuff, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfMidnight),
            new Buff("Signet of Humility", SignetOfHumility, Source.Mesmer, BuffClassification.Other, BuffImages.SignetOfHumility),
            // Skills
            new Buff("Distortion", DistortionBuff, Source.Mesmer, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.Distortion),
            new Buff("Blur", Blur, Source.Mesmer, BuffClassification.Other, BuffImages.Distortion),
            new Buff("Mirror", Mirror, Source.Mesmer, BuffClassification.Other, BuffImages.Mirror),
            new Buff("Echo", Echo, Source.Mesmer, BuffClassification.Other, BuffImages.Echo),
            new Buff("Illusionary Counter", IllusionaryCounterBuff, Source.Mesmer, BuffClassification.Other, BuffImages.IllusionaryCounter),
            new Buff("Illusionary Riposte", IllusionaryRiposteBuff, Source.Mesmer, BuffClassification.Other, BuffImages.IllusionaryRiposte),
            new Buff("Illusionary Leap", IllusionaryLeapBuff, Source.Mesmer, BuffClassification.Other, BuffImages.IllusionaryLeap),
            new Buff("Portal Weaving", PortalWeaving, Source.Mesmer, BuffClassification.Other, BuffImages.PortalEnter),
            new Buff("Portal Uses", PortalUses, Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PortalEnter),
            new Buff("Illusion of Life", IllusionOfLifeBuff, Source.Mesmer, BuffClassification.Support, BuffImages.IllusionOfLife),
            new Buff("Time Echo", TimeEcho, Source.Mesmer, BuffClassification.Other, BuffImages.DejaVu).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Dimensional Aperture", DimensionalAperturePortalBuff, Source.Mesmer, BuffClassification.Other, BuffImages.DimensionalAperture),
            // Traits
            new Buff("Fencer's Finesse", FencersFinesse , Source.Mesmer, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.FencersFinesse),
            new Buff("Illusionary Defense", IllusionaryDefense, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.IllusionaryDefense),
            new Buff("Compounding Power", CompoundingPower, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.CompoundingPower),
            new Buff("Phantasmal Force", PhantasmalForce, Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Mistrust),
            new Buff("Reflection", Reflection, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ArcaneShield),
            new Buff("Reflection 2", Reflection2, Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ArcaneShield),
            // Transformations
            new Buff("Morphed (Polymorph Moa)", MorphedPolymorphMoa, Source.Mesmer, BuffClassification.Debuff, BuffImages.MorphedPolymorphMoa),
            new Buff("Morphed (Polymorph Tuna)", MorphedPolymorphTuna, Source.Mesmer, BuffClassification.Debuff, BuffImages.MorphedPolymorphTuna),
            // Spear
            new Buff("Clarity", Clarity, Source.Mesmer, BuffClassification.Other, BuffImages.MonsterSkill),
        };

        private static readonly HashSet<int> _cloneIDs = new HashSet<int>()
        {
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
        };

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
            return _cloneIDs.Contains(agentItem.ID);
        }

        private static bool IsClone(int id)
        {
            return _cloneIDs.Contains(id);
        }

        private static HashSet<int> NonCloneMinions = new HashSet<int>()
        {
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
        };

        internal static bool IsKnownMinionID(int id)
        {
            return NonCloneMinions.Contains(id) || IsClone(id);
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Mesmer;
            ulong gw2Build = log.CombatData.GetGW2BuildEvent().Build;
            // Portal locations
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerPortalInactive, out IReadOnlyList<EffectEvent> portalInactives))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, PortalEntre);
                foreach (EffectEvent effect in portalInactives)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 60000, player.AgentItem, PortalWeaving);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.3, connector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.PortalMesmerEntre, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }

            if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerPortalActive, out IReadOnlyList<IReadOnlyList<EffectEvent>> portalActives))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, PortalExeunt, SkillModeCategory.Portal);
                foreach (IReadOnlyList<EffectEvent> group in portalActives)
                {
                    GenericAttachedDecoration first = null;
                    foreach (EffectEvent effect in group)
                    {
                        (long, long) lifespan = effect.ComputeLifespan(log, 10000, player.AgentItem, PortalUses);
                        var connector = new PositionConnector(effect.Position);
                        replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.5, connector).UsingSkillMode(skill));
                        GenericAttachedDecoration icon = new IconDecoration(ParserIcons.PortalMesmerExeunt, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
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
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerFeedback, out IReadOnlyList<EffectEvent> feedbacks))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, Feedback, SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in feedbacks)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary ? 6000 : 7000); // 7s with trait pre March 2024
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectFeedback);
                }
            }
            // Veil
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerVeil, out IReadOnlyList<EffectEvent> veils))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, Veil, SkillModeCategory.ImportantBuffs);
                foreach (EffectEvent effect in veils)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary ? 6000 : 7000); // 7s with trait pre March 2024
                    var connector = new PositionConnector(effect.Position);
                    var rotationConnector = new AngleConnector(effect.Rotation.Z);
                    replay.Decorations.Add(new RectangleDecoration(500, 70, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector).UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectVeil, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                }
            }
            // Null Field
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerNullField, out IReadOnlyList<EffectEvent> nullFields))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, NullField, SkillModeCategory.Strip | SkillModeCategory.Cleanse);
                foreach (EffectEvent effect in nullFields)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, gw2Build >= GW2Builds.March2024BalanceAndCerusLegendary ? 5000 : 6000); // 6s with trait pre March 2024
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectNullField);
                }
            }

            // Illusion of Life
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerIllusionOfLife1, out IReadOnlyList<EffectEvent> illusionOfLife))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, IllusionOfLifeSkill, SkillModeCategory.Heal);
                foreach (EffectEvent effect in illusionOfLife)
                {
                    (long, long) lifespan = (effect.Time, effect.Time + 500);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectIllusionOfLife);
                }
            }

            // Dimensional Aperture
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MesmerDimensionalAperturePortal, out IReadOnlyList<EffectEvent> dimensionalApertures))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, DimensionalApertureSkill, SkillModeCategory.Portal);
                var applies = log.CombatData.GetBuffData(DimensionalAperturePortalBuff).Where(x => x.CreditedBy == player.AgentItem).ToList();
                foreach (EffectEvent effect in dimensionalApertures)
                {
                    // The buff can be quite delayed
                    AbstractBuffEvent buffApply = applies.Where(x => x.Time >= effect.Time - ServerDelayConstant && x.Time <= effect.Time + 100).FirstOrDefault();
                    // Security
                    if (buffApply != null)
                    {
                        AgentItem portal = buffApply.To;
                        DespawnEvent despawn = log.CombatData.GetDespawnEvents(portal).FirstOrDefault();
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
                            replay.Decorations.Add(new IconDecoration(ParserIcons.PortalDimensionalAperture, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));

                            // Tether between the portal and the player
                            replay.Decorations.Add(new LineDecoration(lifespan, color, 0.3, new AgentConnector(player.AgentItem), connector));
                        }
                    }
                }
            }

            // Unstable Bladestorm
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.VirtuosoUnstableBladestorm, out IReadOnlyList<EffectEvent> unstableBladestorm))
            {
                var skill = new SkillModeDescriptor(player, Spec.Mesmer, UnstableBladestorm);
                foreach (EffectEvent effect in unstableBladestorm)
                {
                    (long, long) lifespan = effect.ComputeDynamicLifespan(log, 6000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, ParserIcons.EffectUnstableBladestorm);
                }
            }
        }
    }
}
