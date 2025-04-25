using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ReaperHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(EnterReaperShroud, ReapersShroud)
            .UsingBeforeWeaponSwap(true),
        new BuffLossCastFinder(ExitReaperShroud, ReapersShroud)
            .UsingBeforeWeaponSwap(true),
        new BuffGainCastFinder(InfusingTerrorSkill, InfusingTerrorBuff),
        new DamageCastFinder(YouAreAllWeaklings, YouAreAllWeaklings)
            .UsingDisableWithEffectData(),
        new EffectCastFinder(YouAreAllWeaklings, EffectGUIDs.ReaperYouAreAllWeaklings1)
            .UsingSrcSpecChecker(Spec.Reaper)
            .UsingSecondaryEffectChecker(EffectGUIDs.ReaperYouAreAllWeaklings2)
            .UsingSecondaryEffectChecker(EffectGUIDs.ReaperYouAreAllWeaklings3),
        new DamageCastFinder(Suffer, Suffer)
            .UsingDisableWithEffectData(),
        new EffectCastFinder(Suffer, EffectGUIDs.ReaperSuffer)
            .UsingSrcSpecChecker(Spec.Reaper),
        new MinionSpawnCastFinder(Rise, (int)MinionID.ShamblingHorror)
            .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.GetFinalMaster().Spec == Spec.Reaper),
        new DamageCastFinder(ChillingNova, ChillingNova)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Cold Shoulder
        new BuffOnFoeDamageModifier(Mod_ColdShoulder, Chilled, "Cold Shoulder", "15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, TraitImages.ColdShoulder, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2019Balance),
        new BuffOnFoeDamageModifier(Mod_ColdShoulder, Chilled, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, TraitImages.ColdShoulder, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2019Balance),
        new BuffOnFoeDamageModifier(Mod_ColdShoulder, Chilled, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, TraitImages.ColdShoulder, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.March2019Balance),
        // Soul Eater
        new DamageLogDamageModifier(Mod_SoulEater, "Soul Eater", "10% to foes within 300 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, TraitImages.SoulEater, (x, log) => TargetWithinRangeChecker(x, log, 300, false), DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.July2019Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Reaper's Shroud
        new BuffOnActorDamageModifier(Mod_ReapersShroud, ReapersShroud, "Reaper's Shroud", "-33%", DamageSource.Incoming, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, SkillImages.ReapersShroud, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ReapersShroud, ReapersShroud, "Reaper's Shroud", "-50%", DamageSource.Incoming, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, SkillImages.ReapersShroud, DamageModifierMode.sPvPWvW),
        // Infusing Terror
        new BuffOnActorDamageModifier(Mod_InfusingTerror, InfusingTerrorBuff, "Infusing Terror", "-20%", DamageSource.Incoming, -20, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, SkillImages.InfusingTerror, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2023Balance),
        new BuffOnActorDamageModifier(Mod_InfusingTerror, InfusingTerrorBuff, "Infusing Terror", "-20%", DamageSource.Incoming, -20, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, SkillImages.InfusingTerror, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.May2023Balance),
        new BuffOnActorDamageModifier(Mod_InfusingTerror, InfusingTerrorBuff, "Infusing Terror", "-66%", DamageSource.Incoming, -66, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, SkillImages.InfusingTerror, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.May2023Balance), 
        // Rise is unclear, I don't see any inc damage reducing fact for Dark Bond
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Reaper's Shroud", ReapersShroud, Source.Reaper, BuffClassification.Other, SkillImages.ReapersShroud),
        new Buff("Infusing Terror", InfusingTerrorBuff, Source.Reaper, BuffClassification.Other, SkillImages.InfusingTerror),
        new Buff("Dark Bond", DarkBond, Source.Reaper, BuffClassification.Other, SkillImages.Rise),
        new Buff("Reaper's Frost (Chilled to the Bone!)", ReapersFrostChilledToTheBone, Source.Reaper, BuffClassification.Other, SkillImages.ChilledToTheBone),
        new Buff("Reaper's Frost (Executioner's Scythe)", ReapersFrostExecutionersScythe, Source.Reaper, BuffClassification.Other, SkillImages.ChilledToTheBone),
    ];

    private static readonly HashSet<long> _reaperShroudTransform =
    [
        EnterReaperShroud, ExitReaperShroud,
    ];

    public static bool IsReaperShroudTransform(long id)
    {
        return _reaperShroudTransform.Contains(id);
    }

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.ShamblingHorror,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }
}
