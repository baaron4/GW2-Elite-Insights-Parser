using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class DeadeyeHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new EffectCastFinderByDst(Mercy, EffectGUIDs.DeadeyeMercy)
            .UsingDstSpecChecker(Spec.Deadeye), // Needs more testing to check for collisions
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_Premeditation, NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, TraitImages.Premeditation, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_Premeditation, NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, TraitImages.Premeditation, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),
        new BuffOnActorDamageModifier(Mod_Premeditation, NumberOfBoons, "Premeditation", "1.5% per boon",DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, TraitImages.Premeditation, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),
        new BuffOnActorDamageModifier(Mod_Premeditation, NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, TraitImages.Premeditation, DamageModifierMode.All).WithBuilds( GW2Builds.July2023BalanceAndSilentSurfCM),
        //
        new BuffOnActorDamageModifier(Mod_IronSight, DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.All).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.From).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_IronSight, DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.From).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),
        new BuffOnActorDamageModifier(Mod_IronSight, DeadeyesGaze, "Iron Sight", "15% to marked target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.PvE).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.From).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),

        new BuffOnActorDamageModifier(Mod_IronSight, DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.All).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.From).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.July2023BalanceAndSilentSurfCM),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_IronSight, DeadeyesGaze, "Iron Sight", "-10% from marked target", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.All).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.To).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.From == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_IronSight, DeadeyesGaze, "Iron Sight", "-10% from marked target", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.To).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.From == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.August2022Balance),
        new BuffOnActorDamageModifier(Mod_IronSight,DeadeyesGaze, "Iron Sight", "-15% from marked target", DamageSource.NoPets, -15.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, TraitImages.IronSight, DamageModifierMode.PvE).UsingChecker((x, log) => {
            var effectApply = log.CombatData.GetBuffDataByIDByDst(DeadeyesGaze, x.To).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
            if (effectApply != null)
            {
                return x.From == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
            }
            return false;
        }).WithBuilds(GW2Builds.August2022Balance),
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Kneeling", Kneeling, Source.Deadeye, BuffClassification.Other, SkillImages.Kneel).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
        new Buff("Deadeye's Gaze", DeadeyesGaze, Source.Deadeye, BuffClassification.Other, SkillImages.DeadeyesMark),
    ];

    private static HashSet<int> Minions =
    [
        (int)MinionID.DeadeyeSylvari1,
        (int)MinionID.DeadeyeHuman1,
        (int)MinionID.DeadeyeCharr1,
        (int)MinionID.DeadeyeSylvari2,
        (int)MinionID.DeadeyeAsura1,
        (int)MinionID.DeadeyeNorn1,
        (int)MinionID.DeadeyeNorn2,
        (int)MinionID.DeadeyeHuman2,
        (int)MinionID.DeadeyeCharr2,
        (int)MinionID.DeadeyeAsura2,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

}
