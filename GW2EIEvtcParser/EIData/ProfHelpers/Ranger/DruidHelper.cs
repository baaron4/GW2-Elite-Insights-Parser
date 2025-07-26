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

internal static class DruidHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(EnterCelestialAvatar, CelestialAvatar)
            .UsingBeforeWeaponSwap(),
        new BuffLossCastFinder(ExitCelestialAvatar, CelestialAvatar)
            .UsingBeforeWeaponSwap(),
        new EffectCastFinder(SeedOfLife, EffectGUIDs.DruidSeedOfLife)
            .UsingSrcSpecChecker(Spec.Druid)
            .WithBuilds(GW2Builds.October2022Balance),
        new DamageCastFinder(GlyphOfEquality, GlyphOfEquality)
            .UsingDisableWithEffectData(),
        new EffectCastFinderByDst(GlyphOfEqualityCA, EffectGUIDs.DruidGlyphOfEqualityCA)
            .UsingDstSpecChecker(Spec.Druid),
        new EffectCastFinder(GlyphOfEquality, EffectGUIDs.DruidGlyphOfEquality)
            .UsingSrcSpecChecker(Spec.Druid),
        new EffectCastFinder(BloodMoonDaze, EffectGUIDs.DruidBloodMoon)
            .UsingSrcSpecChecker(Spec.Druid)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
    ];

    private static readonly HashSet<long> _celestialAvatar =
    [
        EnterCelestialAvatar, ExitCelestialAvatar
    ];

    public static bool IsCelestialAvatarTransform(long id)
    {
        return _celestialAvatar.Contains(id);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Natural Balance
        new BuffOnActorDamageModifier(Mod_NaturalBalance, NaturalBalance, "Natural Balance", "10% after leaving or entering Celestial Avatar", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Druid, ByPresence, TraitImages.NaturalBalance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Natural Balance
        new BuffOnActorDamageModifier(Mod_NaturalBalance, NaturalBalance, "Natural Balance", "-10% after leaving or entering Celestial Avatar", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Druid, ByPresence, TraitImages.NaturalBalance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.March2024BalanceAndCerusLegendary),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Celestial Avatar", CelestialAvatar, Source.Druid, BuffClassification.Other, SkillImages.CelestialAvatar),
        new Buff("Ancestral Grace", AncestralGraceBuff, Source.Druid, BuffClassification.Other, SkillImages.AncestralGrace)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Glyph of Empowerment", GlyphOfEmpowermentBuff, Source.Druid, BuffClassification.Offensive, SkillImages.GlyphOfTheStars)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.April2019Balance),
        new Buff("Glyph of Unity", GlyphOfUnityBuff, Source.Druid, BuffClassification.Other, SkillImages.GlyphOfUnity),
        new Buff("Glyph of Unity (CA)", GlyphOfUnityCABuff, Source.Druid, BuffClassification.Other, SkillImages.GlyphOfUnityCelestialAvatar),
        new Buff("Glyph of the Stars", GlyphOfTheStars, Source.Druid, BuffClassification.Defensive, SkillImages.GlyphOfTheStars)
            .WithBuilds(GW2Builds.April2019Balance, GW2Builds.October2022Balance),
        new Buff("Glyph of the Stars (CA)", GlyphOfTheStarsCA, Source.Druid, BuffClassification.Defensive, SkillImages.GlyphOfTheStarsCelestialAvatar)
            .WithBuilds(GW2Builds.April2019Balance),
        new Buff("Natural Mender", NaturalMender, Source.Druid, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.NaturalMender)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
        new Buff("Lingering Light", LingeringLight, Source.Druid, BuffClassification.Other, TraitImages.LingeringLight)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new Buff("Natural Balance", NaturalBalance, Source.Druid, BuffClassification.Other, TraitImages.NaturalBalance)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
    ];

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Ranger;

        // Glyph of the Stars
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DruidGlyphOfTheStars, out var glyphOfTheStars))
        {
            var skill = new SkillModeDescriptor(player, Spec.Druid, GlyphOfTheStars, SkillModeCategory.Heal | SkillModeCategory.ImportantBuffs | SkillModeCategory.Cleanse);
            foreach (EffectEvent effect in glyphOfTheStars)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectGlyphOfTheStars);
            }
        }

        // Glyph of the Stars (Celestial Avatar)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DruidGlyphOfTheStarsCA, out var glyphOfTheStarsCA))
        {
            var skill = new SkillModeDescriptor(player, Spec.Druid, GlyphOfTheStarsCA, SkillModeCategory.Heal | SkillModeCategory.ImportantBuffs);
            foreach (EffectEvent effect in glyphOfTheStarsCA)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectGlyphOfTheStarsCA);
            }
        }
    }
}
