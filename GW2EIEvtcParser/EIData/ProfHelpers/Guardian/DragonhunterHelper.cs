using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class DragonhunterHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new EffectCastFinder(TestOfFaith, EffectGUIDs.DragonhunterTestOfFaith)
            .UsingSrcSpecChecker(Spec.Dragonhunter),
        new EffectCastFinder(FragmentsOfFaith, EffectGUIDs.DragonhunterFragmentsOfFaith)
            .UsingSrcSpecChecker(Spec.Dragonhunter),
    ];

    private static bool CheckTether(ParsedEvtcLog log, AgentItem src, AgentItem dst, long time)
    {
        if (!log.CombatData.GetBuffData(JusticeDragonhunter).Any(x => x is BuffApplyEvent bae && bae.By == src && Math.Abs(bae.AppliedDuration - 6000) > ServerDelayConstant))
        {
            return false;
        }
        return log.FindActor(dst).HasBuff(log, log.FindActor(src), JusticeDragonhunter, time);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnFoeDamageModifier(Mod_ZealotsAggression, Crippled, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, TraitImages.ZealotsAggression, DamageModifierMode.All),
        // Pur of Sight unclear. Max is very likely to be 1200, as it is the maximum tooltip range for a DH but what is the distance at witch the minimum is reached? Is the scaling linear?
        new BuffOnFoeDamageModifier(Mod_BigGameHunter, JusticeDragonhunter, "Big Game Hunter", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, TraitImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance)
            .UsingChecker((x, log) => {
            AgentItem src = x.From;
            AgentItem dst = x.To;
            return CheckTether(log, src, dst, x.Time);
        }),
        new BuffOnFoeDamageModifier(Mod_BigGameHunter, JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, TraitImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly)
            .WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance)
            .UsingChecker((x, log) => {
            AgentItem src = x.From;
            AgentItem dst = x.To;
            return CheckTether(log, src, dst, x.Time);
        }),
        new BuffOnFoeDamageModifier(Mod_BigGameHunter, JusticeDragonhunter, "Big Game Hunter", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, TraitImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2023Balance)
            .UsingChecker((x, log) => {
            AgentItem src = x.From;
            AgentItem dst = x.To;
            return CheckTether(log, src, dst, x.Time);
        }),
        new BuffOnFoeDamageModifier(Mod_BigGameHunter, JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, TraitImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly)
            .WithBuilds(GW2Builds.May2023Balance, GW2Builds.February2025BalancePatch)
            .UsingChecker((x, log) => {
            AgentItem src = x.From;
            AgentItem dst = x.To;
            return CheckTether(log, src, dst, x.Time);
        }),
        new BuffOnFoeDamageModifier(Mod_BigGameHunter, JusticeDragonhunter, "Big Game Hunter", "25% to tethered target", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, TraitImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly)
            .WithBuilds(GW2Builds.February2025BalancePatch)
            .UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return CheckTether(log, src, dst, x.Time);
            }),
        //         
        new BuffOnFoeDamageModifier(Mod_HeavyLightDisabled, [Stun, Daze, Knockdown, Fear, Taunt], "Heavy Light (Disabled)", "15% to disabled foes", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByPresence, TraitImages.HeavyLight, DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.February2025BalancePatch),
        new BuffOnFoeDamageModifier(Mod_HeavyLightDisabled, [Stun, Daze, Knockdown, Fear, Taunt], "Heavy Light (Disabled)", "15% to disabled foes", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByPresence, TraitImages.HeavyLight, DamageModifierMode.sPvPWvW)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnFoeDamageModifier(Mod_HeavyLightDisabled, [Stun, Daze, Knockdown, Fear, Taunt], "Heavy Light (Disabled)", "20% to disabled foes", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByPresence, TraitImages.HeavyLight, DamageModifierMode.PvE)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnFoeDamageModifier(Mod_HeavyLightDefiant, [Stun, Daze, Knockdown, Fear, Taunt], "Heavy Light (Defiant)", "10% to defiant non disabled foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByAbsence, TraitImages.HeavyLight, DamageModifierMode.All)
            .UsingChecker((x, log) => x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.February2025BalancePatch),
        new BuffOnFoeDamageModifier(Mod_HeavyLightDefiant, [Stun, Daze, Knockdown, Fear, Taunt], "Heavy Light (Defiant)", "10% to defiant non disabled foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByAbsence, TraitImages.HeavyLight, DamageModifierMode.sPvPWvW)
            .UsingChecker((x, log) => x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnFoeDamageModifier(Mod_HeavyLightDefiant, [Stun, Daze, Knockdown, Fear, Taunt], "Heavy Light (Defiant)", "15% to defiant non disabled foes", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByAbsence, TraitImages.HeavyLight, DamageModifierMode.PvE)
            .UsingChecker((x, log) => x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_HuntersFortification, NumberOfConditions, "Hunter's Fortification", "-10%", DamageSource.NoPets, -10, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByAbsence, TraitImages.HuntersFortification, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Justice (Dragonhunter)", JusticeDragonhunter, Source.Dragonhunter, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.SpearOfLight),
        new Buff("Shield of Courage (Active)", ShieldOfCourageActive, Source.Dragonhunter, BuffClassification.Defensive, SkillImages.ShieldOfCourage),
        new Buff("Spear of Justice", SpearOfJustice, Source.Dragonhunter, BuffClassification.Other, SkillImages.SpearOfJustice),
        new Buff("Shield of Courage", ShieldOfCourage, Source.Dragonhunter, BuffClassification.Other, SkillImages.ShieldOfCourage),
        new Buff("Wings of Resolve", WingsOfResolveBuff, Source.Dragonhunter, BuffClassification.Other, SkillImages.WingsOfResolve),
        new Buff("Hunter's Mark", HuntersMark, Source.Dragonhunter, BuffClassification.Other, SkillImages.HuntersWard),
    ];

}
