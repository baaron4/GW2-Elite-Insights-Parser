using GW2EIEvtcParser.LogLogic;
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

internal static class GuardianHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(ShieldOfWrathSkill, ShieldOfWrathBuff),
        new BuffGainCastFinder(ZealotsFlameSkill, ZealotsFlameBuff)
            .UsingICD(0),
        //new BuffLossCastFinder(9115,9114,InstantCastFinder.DefaultICD), // Virtue of Justice
        //new BuffLossCastFinder(9120,9119,InstantCastFinder.DefaultICD), // Virtue of Resolve
        //new BuffLossCastFinder(9118,9113,InstantCastFinder.DefaultICD), // Virtue of Courage

        // TODO: lesser symbols and symbol of blades

        // Meditations
        new DamageCastFinder(JudgesIntervention, JudgesIntervention)
            .UsingDisableWithEffectData(),
        new BuffGainCastFinder(JudgesIntervention, MercifulAndJudgesInterventionSelfBuff)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedEffectDst(EffectGUIDs.GuardianGenericTeleport2, evt.To, evt.Time + 120))
            .UsingNotAccurate(),
        new BuffGainCastFinder(MercifulInterventionSkill, MercifulAndJudgesInterventionSelfBuff)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedEffectDst(EffectGUIDs.GuardianMercifulIntervention, evt.To, evt.Time + 200))
            .UsingNotAccurate(),
        new EffectCastFinderByDst(ContemplationOfPurity, EffectGUIDs.GuardianContemplationOfPurity1)
            .UsingDstBaseSpecChecker(Spec.Guardian),
        new DamageCastFinder(SmiteCondition, SmiteCondition),
        new DamageCastFinder(LesserSmiteCondition, LesserSmiteCondition)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        
        // Shouts
        new EffectCastFinderByDst(SaveYourselves, EffectGUIDs.GuardianSaveYourselves)
            .UsingDstBaseSpecChecker(Spec.Guardian)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.GuardianShout),
        // distinguish by boons, check duration/stacks to counteract pure of voice
        new EffectCastFinderByDst(Advance, EffectGUIDs.GuardianShout)
            .UsingDstBaseSpecChecker(Spec.Guardian)
            .UsingChecker((evt, combatData, agentData, skillData) =>
            {
                return CombatData.FindRelatedEvents(combatData.GetBuffApplyDataByIDBySrc(Aegis, evt.Dst), evt.Time)
                    .Any(apply => apply.To.Is(evt.Dst) && apply.AppliedDuration + ServerDelayConstant >= 20000 && apply.AppliedDuration - ServerDelayConstant <= 40000);
            }) // identify advance by self-applied 20s to 40s aegis
            .UsingNotAccurate(),
        new EffectCastFinderByDst(StandYourGround, EffectGUIDs.GuardianShout)
            .UsingDstBaseSpecChecker(Spec.Guardian)
            .UsingChecker((evt, combatData, agentData, skillData) =>
            {
                return 5 <= CombatData.FindRelatedEvents(combatData.GetBuffApplyDataByIDBySrc(Stability, evt.Dst), evt.Time)
                    .Count(apply => apply.To.Is(evt.Dst));
            }) // identify stand your ground by self-applied 5+ stacks of stability
            .UsingNotAccurate(),
        // hold the line boons may overlap with save yourselves/pure of voice

        // Signets
        new EffectCastFinderByDst(SignetOfJudgmentSkill, EffectGUIDs.GuardianSignetOfJudgement2)
            .UsingDstBaseSpecChecker(Spec.Guardian),
        new DamageCastFinder(LesserSignetOfWrath, LesserSignetOfWrath)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        
        //new DamageCastFinder(9097,9097), // Symbol of Blades
        new DamageCastFinder(GlacialHeart, GlacialHeart)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.March2024BalanceAndCerusLegendary),
        new EXTHealingCastFinder(GlacialHeartHeal, GlacialHeartHeal)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        new DamageCastFinder(ShatteredAegis, ShatteredAegis)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EXTHealingCastFinder(SelflessDaring, SelflessDaring)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        // Pistol    
        new EffectCastFinder(DetonateJurisdiction, EffectGUIDs.GuardianDetonateJurisdictionLevel1)
            .UsingSrcBaseSpecChecker(Spec.Guardian),
        new EffectCastFinder(DetonateJurisdiction, EffectGUIDs.GuardianDetonateJurisdictionLevel2)
            .UsingSrcBaseSpecChecker(Spec.Guardian),
        new EffectCastFinder(DetonateJurisdiction, EffectGUIDs.GuardianDetonateJurisdictionLevel3)
            .UsingSrcBaseSpecChecker(Spec.Guardian),
    ];


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Zeal
        // - Fiery Wrath
        new BuffOnFoeDamageModifier(Mod_FieryWrath, Burning, "Fiery Wrath", "7% on burning target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.FieryWrath, DamageModifierMode.All),
        // - Symbolic Exposure
        new BuffOnFoeDamageModifier(Mod_SymbolicExposure, Vulnerability, "Symbolic Exposure", "5% on vuln target", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.SymbolicExposure, DamageModifierMode.All),
        // - Symbolic Avenger
        new BuffOnActorDamageModifier(Mod_SymbolicAvenger, SymbolicAvenger, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, TraitImages.SymbolicAvenger, DamageModifierMode.All)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.January2026Balance),
        new BuffOnActorDamageModifier(Mod_SymbolicAvenger, SymbolicAvenger, "Symbolic Avenger", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, TraitImages.SymbolicAvenger_Jan2026, DamageModifierMode.All)
            .WithBuilds(GW2Builds.January2026Balance),
        // - Furious Focus
        new BuffOnActorDamageModifier(Mod_FuriousFocus, Fury, "Furious Focus", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.FuriousFocus, DamageModifierMode.All)
            .WithBuilds(GW2Builds.January2026Balance),
        
        // Radiance
        // - Retribution
        new BuffOnActorDamageModifier(Mod_Retribution, Retaliation, "Retribution", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.RetributionTrait, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_Retribution, Resolution, "Retribution", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.RetributionTrait, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
        
        // Virtues
        // - Unscathed Contender
        new BuffOnActorDamageModifier(Mod_UnscathedContenderAegis, Aegis, "Unscathed Contender", "20% under aegis", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.UnscathedContender, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2023Balance),
        new BuffOnActorDamageModifier(Mod_UnscathedContenderAegis, Aegis, "Unscathed Contender (Aegis)", "7% under aegis", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.UnscathedContender, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2023Balance),
        new DamageLogDamageModifier(Mod_UnscathedContenderHP, "Unscathed Contender (HP)", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Guardian, TraitImages.UnscathedContender, (x, log) => x.IsOverNinety, DamageModifierMode.All)
            .WithBuilds( GW2Builds.February2023Balance),
        // - Power of the Virtuous
        new BuffOnActorDamageModifier(Mod_PowerOfTheVirtuous, NumberOfBoons, "Power of the Virtuous", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, TraitImages.PowerOfTheVirtuous,  DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        // - Inspiring Virtue
        new BuffOnActorDamageModifier(Mod_InspiredVirtue, NumberOfBoons, "Inspired Virtue", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Guardian, ByStack, TraitImages.InspiredVirtue, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_InspiringVirtue, InspiringVirtue, "Inspiring Virtue", "10% (6s) after activating a virtue ", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.VirtuousSolace, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2020Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Skills
        // - Signet of Judgment
        new BuffOnActorDamageModifier(Mod_SignetOfJudgment, SignetOfJudgmentBuff, "Signet of Judgment", "-10%", DamageSource.Incoming, -10, DamageType.StrikeAndCondition, DamageType.All, Source.Guardian, ByPresence, SkillImages.SignetOfJudgment, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_SignetOfJudgmentPI, SignetOfJudgmentPI, "Signet of Judgment (PI)", "-12%", DamageSource.Incoming, -12, DamageType.StrikeAndCondition, DamageType.All, Source.Guardian, ByPresence, SkillImages.SignetOfJudgment, DamageModifierMode.All),
        // - Renewed Focus
        new CounterOnActorDamageModifier(Mod_RenewedFocus, RenewedFocus, "Renewed Focus", "Invulnerable", DamageSource.Incoming, DamageType.All, DamageType.All, Source.Guardian, SkillImages.RenewedFocus, DamageModifierMode.All)
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [        
        // Skills
        new Buff("Zealot's Flame", ZealotsFlameBuff, Source.Guardian, BuffStackType.Queue, 25, BuffClassification.Other, SkillImages.ZealotsFlame),
        new Buff("Purging Flames", PurgingFlames, Source.Guardian, BuffClassification.Other, SkillImages.PurgingFlames),
        new Buff("Litany of Wrath", LitanyOfWrath, Source.Guardian, BuffClassification.Other, SkillImages.LitanyOfWrath),//.WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2023Balance),
        //new Buff("Litany of Wrath", LitanyOfWrath , Source.Berserker, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.BloodReckoning).WithBuilds(GW2Builds.November2023Balance),// TBC: looks like a similar change they made to Blood Reckning when they reworked Dead or Alive durint October2022Balance
        new Buff("Renewed Focus", RenewedFocus, Source.Guardian, BuffClassification.Other, SkillImages.RenewedFocus),
        new Buff("Shield of Wrath", ShieldOfWrathBuff, Source.Guardian, BuffStackType.Stacking, 3, BuffClassification.Other, SkillImages.ShieldOfWrath),
        new Buff("Binding Blade (Self)", BindingBladeSelf, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.BindingBlade),
        new Buff("Binding Blade", BindingBlade, Source.Guardian, BuffClassification.Other, SkillImages.BindingBlade)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2025Balance),
        new Buff("Binding Blade", BindingBlade, Source.Guardian, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.BindingBlade)
            .WithBuilds(GW2Builds.June2025Balance),
        new Buff("Banished", Banished, Source.Guardian, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.Banish),
        new Buff("Merciful Intervention (Self)", MercifulAndJudgesInterventionSelfBuff, Source.Guardian, BuffClassification.Support, SkillImages.MercifulIntervention),
        new Buff("Merciful Intervention (Target)", MercifulInterventionTargetBuff, Source.Guardian, BuffClassification.Support, SkillImages.MercifulIntervention),
        // Signets
        new Buff("Signet of Resolve", SignetOfResolve, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfResolve),
        new Buff("Signet of Resolve (Shared)", SignetOfResolveShared, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, SkillImages.SignetOfResolve)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Signet of Resolve (PI)", SignetOfResolvePI, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfResolve)
            .WithBuilds(GW2Builds.June2022Balance),
        new Buff("Bane Signet", BaneSignet, Source.Guardian, BuffClassification.Other, SkillImages.BaneSignet),
        new Buff("Bane Signet (PI)", BaneSignetPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Offensive, SkillImages.BaneSignet)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Bane Signet (PI)", BaneSignetPI, Source.Guardian, BuffClassification.Other, SkillImages.BaneSignet)
            .WithBuilds(GW2Builds.June2022Balance),
        new Buff("Signet of Judgment", SignetOfJudgmentBuff, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfJudgment),
        new Buff("Signet of Judgment (PI)", SignetOfJudgmentPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, SkillImages.SignetOfJudgment)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Signet of Judgment (PI)", SignetOfJudgmentPI, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfJudgment)
            .WithBuilds(GW2Builds.June2022Balance),
        new Buff("Signet of Mercy", SignetOfMercyBuff, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfMercy),
        new Buff("Signet of Mercy (PI)", SignetOfMercyPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, SkillImages.SignetOfMercy)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Signet of Mercy (PI)", SignetOfMercyPI, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfMercy)
            .WithBuilds(GW2Builds.June2022Balance),
        new Buff("Signet of Wrath", SignetOfWrath, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfWrath),
        new Buff("Signet of Wrath (PI)", SignetOfWrathPI, Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Offensive, SkillImages.SignetOfWrath)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Signet of Wrath (PI)", SignetOfWrathPI, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfWrath)
            .WithBuilds(GW2Builds.June2022Balance),
        new Buff("Signet of Courage", SignetOfCourage, Source.Guardian, BuffClassification.Other, SkillImages.SignetOfCourage),
        new Buff("Signet of Courage (Shared)", SignetOfCourageShared , Source.Guardian, BuffStackType.Stacking, 25, BuffClassification.Defensive, SkillImages.SignetOfCourage)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Signet of Courage (PI)", SignetOfCouragePI , Source.Guardian, BuffClassification.Other, SkillImages.SignetOfCourage)
            .WithBuilds(GW2Builds.June2022Balance),
        // Virtues
        new Buff("Virtue of Justice", VirtueOfJustice, Source.Guardian, BuffClassification.Other, SkillImages.VirtueOfJustice),
        new Buff("Virtue of Courage", VirtueOfCourage, Source.Guardian, BuffClassification.Other, SkillImages.VirtueOfCourage),
        new Buff("Virtue of Resolve", VirtueOfResolve, Source.Guardian, BuffClassification.Other, SkillImages.VirtueOfResolve),
        new Buff("Justice", Justice, Source.Guardian, BuffClassification.Other, SkillImages.VirtueOfJustice),
        // Traits
        new Buff("Strength in Numbers", StrengthinNumbers, Source.Guardian, BuffClassification.Defensive, TraitImages.StrengthInNumbers)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Invigorated Bulwark", InvigoratedBulwark, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.InvigoratedBulwark),
        new Buff("Virtue of Resolve (Battle Presence)", VirtueOfResolveBattlePresence, Source.Guardian, BuffStackType.Queue, 2, BuffClassification.Defensive, TraitImages.BattlePresence),
        new Buff("Virtue of Resolve (Battle Presence - Absolute Resolve)", VirtueOfResolveBattlePresenceAbsoluteResolve, Source.Guardian, BuffStackType.Queue, 2, BuffClassification.Defensive, SkillImages.VirtueOfResolve),
        new Buff("Symbolic Avenger", SymbolicAvenger, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.SymbolicAvenger)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.January2026Balance),
        new Buff("Symbolic Avenger", SymbolicAvenger, Source.Guardian, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.SymbolicAvenger_Jan2026)
            .WithBuilds(GW2Builds.January2026Balance),
        new Buff("Inspiring Virtue", InspiringVirtue, Source.Guardian, BuffStackType.Queue, 99, BuffClassification.Other, TraitImages.VirtuousSolace)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.February2020Balance2),
        new Buff("Inspiring Virtue", InspiringVirtue, Source.Guardian, BuffClassification.Other, TraitImages.VirtuousSolace)
            .WithBuilds(GW2Builds.February2020Balance2),
        new Buff("Force of Will", ForceOfWill, Source.Guardian, BuffClassification.Other, TraitImages.ForceOfWill),
        // Spear
        new Buff("Symbol of Luminance", SymbolOfLuminanceBuff, Source.Guardian, BuffClassification.Other, SkillImages.SymbolOfLuminance),
        new Buff("Illuminated", Illuminated, Source.Guardian, BuffClassification.Other, BuffImages.Illuminated),
    ];

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.BowOfTruth,
        (int)MinionID.HammerOfWisdom,
        (int)MinionID.ShieldOfTheAvenger,
        (int)MinionID.SwordOfJustice,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Guardian;

        // Ring of Warding (Hammer 5)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianRingOfWarding, out var ringOfWardings))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, RingOfWarding, SkillModeCategory.CC);
            foreach (EffectEvent effect in ringOfWardings)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectRingOfWarding);
            }
        }
        // Line of Warding (Staff 5)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianLineOfWarding, out var lineOfWardings))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, LineOfWarding, SkillModeCategory.CC);
            foreach (EffectEvent effect in lineOfWardings)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = new PositionConnector(effect.Position);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                replay.Decorations.Add(new RectangleDecoration(500, 70, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectLineOfWarding, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }
        // Wall of Reflection
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianWallOfReflection, out var wallOfReflections))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, WallOfReflection, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in wallOfReflections)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 10000); // 10s with trait
                var connector = new PositionConnector(effect.Position);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                replay.Decorations.Add(new RectangleDecoration(500, 70, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectWallOfReflection, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }
        // Sanctuary
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSanctuary, out var sanctuaries))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SanctuaryGuardian, SkillModeCategory.CC | SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in sanctuaries)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 7000); // 7s with trait
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectSanctuary);
            }
        }
        // Shield of the Avenger
        if (log.CombatData.TryGetEffectEventsByMasterWithGUID(player.AgentItem, EffectGUIDs.GuardianShieldOfTheAvenger, out var shieldOfTheAvengers))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, ShieldOfTheAvenger, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in shieldOfTheAvengers)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectShieldOfTheAvenger);
            }
        }

        // Signet of Mercy
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSignetOfMercyLightTray, out var signetOfMercy))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SignetOfMercySkill, SkillModeCategory.Heal);
            foreach (EffectEvent effect in signetOfMercy)
            {
                // Only displays if fully channeled.
                (long start, long end) lifespan = effect.ComputeLifespanWithSecondaryEffectAndPosition(log, EffectGUIDs.GuardianGenericLightEffect, effect.Duration);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSignetOfMercy);
            }
        }

        // Hunter's Ward
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DragonhunterHuntersWardCage, out var huntersWards))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, HuntersWard, SkillModeCategory.CC);
            foreach (EffectEvent effect in huntersWards)
            {
                long duration = log.LogData.Logic.SkillMode == LogLogic.LogLogic.SkillModeEnum.WvW ? 3000 : 5000;
                (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, duration);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 140, EffectImages.EffectHuntersWard); // radius approximation
            }
        }

        // Symbol of Energy
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DragonhunterSymbolOfEnergy, out var symbolsOfEnergy))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfEnergy);
            foreach (EffectEvent effect in symbolsOfEnergy)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSymbolOfEnergy);
            }
        }

        // Symbol of Vengeance
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandSymbolOfVengeance1, out var symbolsOfVengeance))
        {
            var skillCC = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfVengeance, SkillModeCategory.CC); // CC when traited
            var skillDamage = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfVengeance);
            foreach (EffectEvent effect in symbolsOfVengeance)
            {
                (long start, long end) = effect.ComputeLifespan(log, 4000);
                (long start, long end) lifespanCC = (start, start + 1000);
                (long start, long end) lifespanDamage = (lifespanCC.end, end);
                // CC on initial strike
                AddCircleSkillDecoration(replay, effect, color, skillCC, lifespanCC, 180, EffectImages.EffectSymbolOfVengeance);
                AddCircleSkillDecoration(replay, effect, color, skillDamage, lifespanDamage, 180, EffectImages.EffectSymbolOfVengeance);
            }
        }

        // Symbol of Punishment
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSymbolOfPunishment1, out var symbolsOfPunishment))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfPunishment);
            foreach (EffectEvent effect in symbolsOfPunishment)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSymbolOfPunishment);
            }
        }

        // Symbol of Blades
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSymbolOfBlades, out var symbolsOfBlades))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfBlades);
            foreach (EffectEvent effect in symbolsOfBlades)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSymbolOfBlades);
            }
        }

        // Symbol of Resolution
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSymbolOfResolution, out var symbolsOfResolution))
        {
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfWrath_SymbolOfResolution);
            foreach (EffectEvent effect in symbolsOfResolution)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSymbolOfResolution);
            }
        }

        // Solar Storm
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSolarStormAerealEffect, out var solarStorms))
        {
            // Skill definition has radius of 360, each hit has a radius of 180.
            var skill = new SkillModeDescriptor(player, Spec.Guardian, SolarStorm);
            foreach (EffectEvent effect in solarStorms)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 2000); // 2000 apromixated duration
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectSolarStorm);
            }
            // Spear Impact
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSolarStormSpearImpact, out var spearImpacts))
            {
                foreach (EffectEvent effect in spearImpacts)
                {
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 500); // 500 as a visual display
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(180, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                }
            }
        }

        // Symbol of Luminance
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.GuardianSymbolOfLuminance3, out var symbolsOfLuminance))
        {
            var skillCC = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfLuminanceSkill, SkillModeCategory.CC);
            var skillDamage = new SkillModeDescriptor(player, Spec.Guardian, SymbolOfLuminanceSkill);
            foreach (EffectEvent effect in symbolsOfLuminance)
            {
                (long start, long end) = effect.ComputeLifespan(log, 4000);
                (long start, long end) lifespanCC = (start, start + 1000);
                (long start, long end) lifespanDamage = (lifespanCC.end, end);
                // CC on initial strike
                AddCircleSkillDecoration(replay, effect, color, skillCC, lifespanCC, 180, EffectImages.EffectSymbolOfLuminance);
                AddCircleSkillDecoration(replay, effect, color, skillDamage, lifespanDamage, 180, EffectImages.EffectSymbolOfLuminance);
            }
        }
    }
}
