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

internal static class SpellbreakerHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(SightBeyondSightSkill, SightBeyondSightBuff),
        new BuffGiveCastFinder(MagebaneTetherSkill, MagebaneTetherBuff)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new DamageCastFinder(LossAversion, LossAversion)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Pure Strike (Boons)
        new BuffOnFoeDamageModifier(Mod_PureStrikeBoons, NumberOfBoons, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, TraitImages.PureStrike, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_PureStrikeBoons, NumberOfBoons, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, TraitImages.PureStrike, DamageModifierMode.sPvPWvW)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_PureStrikeBoons, NumberOfBoons, "Pure Strike (boons)", "7.5% crit damage", DamageSource.NoPets, 7.5, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, TraitImages.PureStrike, DamageModifierMode.PvE)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.August2022Balance),
        // Pure Strike (No Boons)
        new BuffOnFoeDamageModifier(Mod_PureStrikeNoBoons, NumberOfBoons, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, TraitImages.PureStrike, DamageModifierMode.All)
            .UsingChecker( (x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_PureStrikeNoBoons, NumberOfBoons, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, TraitImages.PureStrike, DamageModifierMode.sPvPWvW)
            .UsingChecker( (x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_PureStrikeNoBoons, NumberOfBoons, "Pure Strike (no boons)", "15% crit damage", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, TraitImages.PureStrike, DamageModifierMode.PvE)
            .UsingChecker( (x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.August2022Balance),
        // Magebane Tether
        new BuffOnFoeDamageModifier(Mod_MagebaneTether, MagebaneTetherBuff, "Magebane Tether", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, TraitImages.MagebaneTether, DamageModifierMode.PvEInstanceOnly)
            .WithBuffOnFoeFromSelf()
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_MagebaneTether, MagebaneTetherBuff, "Magebane Tether", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, TraitImages.MagebaneTether, DamageModifierMode.PvEInstanceOnly)
            .WithBuffOnFoeFromSelf()
            .WithBuilds(GW2Builds.August2022Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Sight beyond Sight", SightBeyondSightBuff, Source.Spellbreaker, BuffClassification.Other, TraitImages.SightBeyondSight),
        new Buff("Full Counter", FullCounterBuff, Source.Spellbreaker, BuffClassification.Other, SkillImages.FullCounter),
        new Buff("Disenchantment", Disenchantment, Source.Spellbreaker, BuffClassification.Other, SkillImages.WindsOfDisenchantment),
        new Buff("Attacker's Insight", AttackersInsight, Source.Spellbreaker, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.AttackersInsight),
        new Buff("Magebane Tether", MagebaneTetherBuff, Source.Spellbreaker, BuffStackType.Stacking, 25, BuffClassification.Other, TraitImages.MagebaneTether),
    ];

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Warrior;

        // Winds of Disenchantment
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpellbreakerWindsOfDisenchantment, out var windsOfDisenchantments))
        {
            var skill = new SkillModeDescriptor(player, Spec.Spellbreaker, WindsOfDisenchantment, SkillModeCategory.Strip | SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in windsOfDisenchantments)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectWindsOfDisenchantment);
            }
        }
    }
}
