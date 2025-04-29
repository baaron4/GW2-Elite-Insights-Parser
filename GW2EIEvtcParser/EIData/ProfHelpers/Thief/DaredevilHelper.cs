using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.InstantCastFinder;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class DaredevilHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(Bound, BoundingDodger)
            .UsingOrigin(InstantCastOrigin.Trait),
        new BuffGainCastFinder(ImpalingLotus, LotusTraining)
            .UsingOrigin(InstantCastOrigin.Trait),
        new BuffGainCastFinder(Dash, UnhinderedCombatant)
            .UsingOrigin(InstantCastOrigin.Trait),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Lotus Training
        new BuffOnActorDamageModifier(Mod_LotusTraining, LotusTraining, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, TraitImages.LotusTraining, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_LotusTraining, LotusTraining, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, TraitImages.LotusTraining, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2021Balance),
        new BuffOnActorDamageModifier(Mod_LotusTraining, LotusTraining, "Lotus Training", "15% cDam (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, TraitImages.LotusTraining, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2021Balance),
        new BuffOnActorDamageModifier(Mod_LotusTraining, LotusTraining, "Lotus Training", "15% cDam (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, TraitImages.LotusTraining, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2021Balance),
        // Bounding Dodger
        new BuffOnActorDamageModifier(Mod_BoundingDodger, BoundingDodger, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.BoundingDodger, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_BoundingDodger, BoundingDodger, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.BoundingDodger, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_BoundingDodger, BoundingDodger, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.BoundingDodger, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_BoundingDodger, BoundingDodger, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.BoundingDodger, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2022Balance),
        // Weakening Strikes
        new BuffOnFoeDamageModifier(Mod_WeakeningStrikes, Weakness, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.WeakeningStrikes, DamageModifierMode.All)
            .WithBuilds(GW2Builds.April2019Balance, GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_WeakeningStrikes, Weakness, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.WeakeningStrikes, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2022Balance),
        new BuffOnFoeDamageModifier(Mod_WeakeningStrikes, Weakness, "Weakening Strikes", "10% if weakness on target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.WeakeningStrikes, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2022Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Weakening Strikes
        new BuffOnFoeDamageModifier(Mod_WeakeningStrikes, Weakness, "Weakening Strikes", "-10% if weakness on foe", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, TraitImages.WeakeningStrikes, DamageModifierMode.All),
        // Unhindered Combatant
        new BuffOnActorDamageModifier(Mod_UnhideredCombatant, UnhinderedCombatant, "Unhindered Combatant", "-10%", DamageSource.Incoming, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Daredevil, ByPresence, TraitImages.UnhinderedCombatant, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Palm Strike", PalmStrike, Source.Daredevil, BuffClassification.Other, SkillImages.PalmStrike),
        new Buff("Pulmonary Impact", PulmonaryImpactBuff, Source.Daredevil, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.PalmStrike),
        new Buff("Lotus Training", LotusTraining, Source.Daredevil, BuffClassification.Other, TraitImages.LotusTraining),
        new Buff("Unhindered Combatant", UnhinderedCombatant, Source.Daredevil, BuffClassification.Other, TraitImages.UnhinderedCombatant),
        new Buff("Bounding Dodger", BoundingDodger, Source.Daredevil, BuffClassification.Other, TraitImages.BoundingDodger),
        new Buff("Weakening Strikes", WeakeningStrikes, Source.Daredevil, BuffClassification.Other, TraitImages.WeakeningStrikes)
            .WithBuilds(GW2Builds.April2019Balance),
    ];

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.DaredevilSylvari1,
        (int)MinionID.DaredevilAsura1,
        (int)MinionID.DaredevilHuman1,
        (int)MinionID.DaredevilAsura2,
        (int)MinionID.DaredevilNorn1,
        (int)MinionID.DaredevilNorn2,
        (int)MinionID.DaredevilCharr1,
        (int)MinionID.DaredevilSylvari2,
        (int)MinionID.DaredevilHuman2,
        (int)MinionID.DaredevilCharr2,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

}
