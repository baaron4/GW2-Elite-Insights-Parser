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

namespace GW2EIEvtcParser.EIData;

internal static class FirebrandHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        // Mantra of Solace
        new EXTHealingCastFinder(MantraOfSolace, MantraOfSolace)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(MantraOfSolace, EffectGUIDs.FirebrandMantraOfSolaceSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
        new EffectCastFinderByDst(RestoringReprieveOrRejunevatingRespite, EffectGUIDs.FirebrandMantraOfSolaceSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.February2023Balance),

        // Mantra of Flame
        new DamageCastFinder(FlameRushOld, FlameRushOld)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new DamageCastFinder(FlameRush, FlameRush)
            .WithBuilds(GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(FlameRush, EffectGUIDs.FirebrandMantraOfFlameSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedHit(FlameRush, evt.Dst, evt.Time))
            .WithBuilds(GW2Builds.February2023Balance),

        new DamageCastFinder(FlameSurgeOld, FlameSurgeOld)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new DamageCastFinder(FlameSurge, FlameSurge)
            .WithBuilds(GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(FlameSurge, EffectGUIDs.FirebrandMantraOfFlameSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedHit(FlameSurge, evt.Dst, evt.Time))
            .WithBuilds(GW2Builds.February2023Balance),

        new EffectCastFinderByDst(FlameRushOrFlameSurge, EffectGUIDs.FirebrandMantraOfFlameSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasRelatedHit(FlameSurge, evt.Dst, evt.Time) && !combatData.HasRelatedHit(FlameRush, evt.Dst, evt.Time))
            .WithBuilds(GW2Builds.February2023Balance),

        new DamageCastFinder(MantraOfFlameCast, MantraOfFlameDamage)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(MantraOfFlameCast, EffectGUIDs.FirebrandMantraOfFlameSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),

        // Mantra of Lore
        new EffectCastFinderByDst(MantraOfLore, EffectGUIDs.FirebrandMantraOfLoreSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
        new EffectCastFinderByDst(OpeningPassageOrClarifiedConclusion, EffectGUIDs.FirebrandMantraOfLoreSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.February2023Balance),

        // Mantra of Potence
        new EffectCastFinderByDst(MantraOfPotence, EffectGUIDs.FirebrandMantraOfPotenceSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
        new EffectCastFinderByDst(PotentHasteOrOverwhelmingCelerity, EffectGUIDs.FirebrandMantraOfPotenceSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.February2023Balance),

        // Mantra of Truth
        new DamageCastFinder(EchoOfTruth, EchoOfTruth)
            .WithBuilds(GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(EchoOfTruth, EffectGUIDs.FirebrandMantraOfTruthSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedHit(EchoOfTruth, evt.Dst, evt.Time))
            .WithBuilds(GW2Builds.February2023Balance),

        new DamageCastFinder(VoiceOfTruth, VoiceOfTruth)
            .WithBuilds(GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(VoiceOfTruth, EffectGUIDs.FirebrandMantraOfTruthSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .UsingChecker((evt, combatData, agentData, skillData) => combatData.HasRelatedHit(VoiceOfTruth, evt.Dst, evt.Time))
            .WithBuilds(GW2Builds.February2023Balance),

        new EffectCastFinderByDst(EchoOfTruthOrVoiceOfTruth, EffectGUIDs.FirebrandMantraOfTruthSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .UsingChecker((evt, combatData, agentData, skillData) => !combatData.HasRelatedHit(VoiceOfTruth, evt.Dst, evt.Time) && !combatData.HasRelatedHit(EchoOfTruth, evt.Dst, evt.Time))
            .WithBuilds(GW2Builds.February2023Balance),

        new DamageCastFinder(MantraOfTruthCast, MantraOfTruthDamage)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(MantraOfTruthCast, EffectGUIDs.FirebrandMantraOfTruthSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),

        // Mantra of Liberation
        new EffectCastFinderByDst(MantraOfLiberation, EffectGUIDs.FirebrandMantraOfLiberationSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.February2023Balance),
        new EffectCastFinderByDst(PortentOfFreedomOrUnhinderedDelivery, EffectGUIDs.FirebrandMantraOfLiberationSymbol)
            .UsingDstSpecChecker(Spec.Firebrand)
            .WithBuilds( GW2Builds.February2023Balance),

        // Tomes - Opening
        new BuffGainCastFinder(TomeOfJusticeSkill, TomeOfJusticeOpen)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingBeforeWeaponSwap(),
        new BuffGainCastFinder(TomeOfResolveSkill, TomeOfResolveOpen)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingBeforeWeaponSwap(),
        new BuffGainCastFinder(TomeOfCourageSkill, TomeOfCourageOpen)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingBeforeWeaponSwap(),

        // Tomes - Closing
        new BuffLossCastFinder(StowTome, TomeOfJusticeOpen)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingBeforeWeaponSwap(),
        new BuffLossCastFinder(StowTome, TomeOfResolveOpen)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingBeforeWeaponSwap(),
        new BuffLossCastFinder(StowTome, TomeOfCourageOpen)
            .WithBuilds(GW2Builds.November2022Balance)
            .UsingBeforeWeaponSwap(),
    ];

    private static readonly HashSet<long> _firebrandTomes =
    [
        TomeOfJusticeSkill,
        TomeOfResolveSkill,
        TomeOfCourageSkill,
        StowTome,
    ];

    public static bool IsFirebrandTome(long id)
    {
        return _firebrandTomes.Contains(id);
    }

    private static readonly HashSet<long> _firebrandTomeAAs =
    [
        Chapter1SearingSpell,
        Chapter1DesertBloomHeal,
        Chapter1UnflinchingCharge
    ];

    public static bool IsAutoAttack(ParsedEvtcLog log, long id)
    {
        var build = log.CombatData.GetGW2BuildEvent().Build;
        return build >= GW2Builds.September2017PathOfFireRelease && _firebrandTomeAAs.Contains(id);
    }

    // TODO: imbued haste placeholder
    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [
        new BuffOnActorDamageModifier(Mod_ImbuedHaste, Quickness, "Imbued Haste", "10% under Quickness", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Guardian, ByPresence, TraitImages.ImbuedHaste, DamageModifierMode.All)
            .WithBuilds( GW2Builds.November2026Balance),
     ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [

        new BuffOnActorDamageModifier(Mod_ImbuedHaste, Quickness, "Imbued Haste", "-10% under Quickness", DamageSource.Incoming, 10.0, DamageType.Strike, DamageType.All, Source.Guardian, ByPresence, TraitImages.ImbuedHaste, DamageModifierMode.All)
            .WithBuilds( GW2Builds.November2026Balance),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Ashes of the Just", AshesOfTheJust, Source.Firebrand, BuffStackType.Stacking, 25, BuffClassification.Offensive, SkillImages.EpilogueAshesOfTheJust),
        new Buff("Eternal Oasis", EternalOasis, Source.Firebrand, BuffClassification.Defensive, SkillImages.EpilogueEternalOasis),
        new Buff("Unbroken Lines", UnbrokenLines, Source.Firebrand, BuffStackType.Stacking, 3, BuffClassification.Defensive, SkillImages.EpilogueUnbrokenLines),
        new Buff("Tome of Justice", TomeOfJusticeBuff, Source.Firebrand, BuffClassification.Other, SkillImages.TomeOfJustice),
        new Buff("Tome of Courage", TomeOfCourageBuff, Source.Firebrand, BuffClassification.Other, SkillImages.TomeOfCourage),
        new Buff("Tome of Resolve", TomeOfResolveBuff, Source.Firebrand, BuffClassification.Other, SkillImages.TomeOfResolve),
        new Buff("Tome of Justice Open", TomeOfJusticeOpen, Source.Firebrand, BuffClassification.Hidden, SkillImages.TomeOfJustice),
        new Buff("Tome of Courage Open", TomeOfCourageOpen, Source.Firebrand, BuffClassification.Hidden, SkillImages.TomeOfCourage),
        new Buff("Tome of Resolve Open", TomeOfResolveOpen, Source.Firebrand, BuffClassification.Hidden, SkillImages.TomeOfResolve),
        new Buff("Quickfire", Quickfire, Source.Firebrand, BuffClassification.Other, TraitImages.Quickfire),
        new Buff("Dormant Justice", DormantJustice, Source.Firebrand, BuffClassification.Other, SkillImages.DormantJustice),
        new Buff("Dormant Courage", DormantCourage, Source.Firebrand, BuffClassification.Other, SkillImages.DormantCourage),
        new Buff("Dormant Resolve", DormantResolve, Source.Firebrand, BuffClassification.Other, SkillImages.DormantResolve),
    ];

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Guardian;

        // Valiant Bulwark
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandValiantBulwark, out var valiantBulwarks))
        {
            var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter3ValiantBulwark, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in valiantBulwarks)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectValiantBulwark);
            }
        }

        // Stalwart Stand
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandStalwartStand1, out var stalwartStands))
        {
            var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter4StalwartStand, SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in stalwartStands)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectStalwartStand);
            }
        }

        // Shining River
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandShiningRiver1, out var shiningRiver))
        {
            var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter4ShiningRiver, SkillModeCategory.Heal);
            foreach (EffectEvent effect in shiningRiver)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectShiningRiver);
            }
        }

        // Scorched Aftermath
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.FirebrandScorchedAftermath1, out var scorchedAftermath))
        {
            var skill = new SkillModeDescriptor(player, Spec.Firebrand, Chapter4ScorchedAftermath);
            foreach (EffectEvent effect in scorchedAftermath)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectScorchedAftermath);
            }
        }
    }
}
