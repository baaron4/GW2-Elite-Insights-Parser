using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class VindicatorHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(LegendaryAllianceStanceSkill, LegendaryAllianceStanceBuff),
        //new BuffGainCastFinder(LegendaryAllianceStanceUWSkill, LegendaryAllianceStanceEffect),
        new DamageCastFinder(CallOfTheAlliance, CallOfTheAlliance),
        new BuffGainCastFinder(UrnOfSaintViktorSkill, UrnOfSaintViktorBuff),
    ];

    private static readonly HashSet<long> _dodges =
    [
        DeathDropDodge, ImperialImpactDodge, SaintsShieldDodge
    ];

    public static bool IsVindicatorDodge(long id)
    {
        return _dodges.Contains(id);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_ForerunnerOfDeath, ForerunnerOfDeath, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, TraitImages.ForerunnerOfDeath, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_ForerunnerOfDeath, ForerunnerOfDeath, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, TraitImages.ForerunnerOfDeath, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_ForerunnerOfDeath, ForerunnerOfDeath, "Forerunner of Death", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, TraitImages.ForerunnerOfDeath, DamageModifierMode.PvE).WithBuilds(GW2Builds.September2023Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_UrnOfSaintViktor, UrnOfSaintViktorBuff, "Urn of Saint Viktor", "-50%", DamageSource.NoPets, -50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Vindicator, ByPresence, SkillImages.UrnOfSaintViktor, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2024LonelyTowerFractalRelease),
        new BuffOnActorDamageModifier(Mod_UrnOfSaintViktor, UrnOfSaintViktorBuff, "Urn of Saint Viktor", "-33%", DamageSource.NoPets, -33.0, DamageType.StrikeAndCondition, DamageType.All, Source.Vindicator, ByPresence, SkillImages.UrnOfSaintViktor, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.May2024LonelyTowerFractalRelease),
        new BuffOnActorDamageModifier(Mod_UrnOfSaintViktor, UrnOfSaintViktorBuff, "Urn of Saint Viktor", "-50%", DamageSource.NoPets, -50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Vindicator, ByPresence, SkillImages.UrnOfSaintViktor, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.May2024LonelyTowerFractalRelease),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Legendary Alliance Stance", LegendaryAllianceStanceBuff, Source.Revenant, BuffClassification.Other, SkillImages.LegendaryAllianceStance),
        new Buff("Urn of Saint Viktor", UrnOfSaintViktorBuff, Source.Vindicator, BuffClassification.Other, SkillImages.UrnOfSaintViktor),
        new Buff("Saint of zu Heltzer", SaintOfzuHeltzer, Source.Vindicator, BuffClassification.Other, TraitImages.SaintOfZuHeltzer),
        new Buff("Forerunner of Death", ForerunnerOfDeath, Source.Vindicator, BuffClassification.Other, TraitImages.ForerunnerOfDeath),
        new Buff("Imperial Guard", ImperialGuard, Source.Vindicator, BuffStackType.Stacking, 5, BuffClassification.Other, SkillImages.ImperialGuard).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
    ];
}
