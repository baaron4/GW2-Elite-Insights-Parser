using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class WillbenderHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(LethalTempo, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, BuffImages.LethalTempo, DamageModifierMode.All)
                .UsingChecker( (x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 6000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffOnActorDamageModifier(LethalTempo, "Lethal Tempo", "1% per stack", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.LethalTempo, DamageModifierMode.PvE)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 6000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.EODBeta4, GW2Builds.November2022Balance),
            new BuffOnActorDamageModifier(LethalTempo, "Lethal Tempo", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.LethalTempo, DamageModifierMode.PvE)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 6000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.November2022Balance),
            //
            new BuffOnActorDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, BuffImages.TyrantsMomentum, DamageModifierMode.All)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 4000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.EODBeta1, GW2Builds.EODBeta4),
            new BuffOnActorDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.TyrantsMomentum, DamageModifierMode.PvE)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 4000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.EODBeta4, GW2Builds.November2022Balance),
            new BuffOnActorDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.TyrantsMomentum, DamageModifierMode.PvE)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 4000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.November2022Balance),
            //
            new BuffOnActorDamageModifier(LethalTempo, "Lethal Tempo", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.LethalTempo, DamageModifierMode.sPvPWvW)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 6000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.EODBeta4, GW2Builds.March2022Balance2),
            new BuffOnActorDamageModifier(LethalTempo, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.LethalTempo, DamageModifierMode.sPvPWvW)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 6000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.March2022Balance2),
            //
            new BuffOnActorDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "4% per stack", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.TyrantsMomentum, DamageModifierMode.sPvPWvW)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 4000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.EODBeta4, GW2Builds.March2022Balance2),
            new BuffOnActorDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, BuffImages.TyrantsMomentum, DamageModifierMode.sPvPWvW)
                .UsingChecker((x, log) => {
                    AgentItem src = x.From;
                    AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                    if (effectApply != null)
                    {
                       return x.Time - effectApply.Time < 4000;
                    }
                    return false;
                })
                .UsingApproximate(true)
                .WithBuilds(GW2Builds.March2022Balance2),
            //
            new CounterOnActorDamageModifier(RushingJusticeBuff, "Rushing Justice", "Applies burning on consecutive hits", DamageSource.NoPets, DamageType.Strike, DamageType.Strike, Source.Willbender, BuffImages.RushingJustice, DamageModifierMode.All)
                .WithBuilds(GW2Builds.EODBeta1)
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(CrashingCourage, "Deathless Courage", "50%", DamageSource.NoPets, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByPresence, BuffImages.DeathlessCourage, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(CrashingCourage, "Deathless Courage", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByPresence, BuffImages.DeathlessCourage, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // Virtues
            new Buff("Rushing Justice", RushingJusticeBuff, Source.Willbender, BuffClassification.Other, BuffImages.RushingJustice),
            new Buff("Flowing Resolve", FlowingResolveBuff, Source.Willbender, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.FlowingResolve),
            new Buff("Crashing Courage", CrashingCourage, Source.Willbender, BuffClassification.Other, BuffImages.CrashingCourage),
            //
            new Buff("Repose", Repose, Source.Willbender, BuffClassification.Other, BuffImages.Repose),
            new Buff("Lethal Tempo", LethalTempo, Source.Willbender, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.LethalTempo),
            //new Buff("Tyrant's Lethal Tempo", 62657, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, BuffImages.TyrantsMomentum),
        };
    }
}
