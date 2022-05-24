using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class WillbenderHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        public static IReadOnlyList<AnimatedCastEvent> ComputeFlowingResolveCastEvents(Player willbender, CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<AnimatedCastEvent>();
            SkillItem skill = skillData.Get(FlowingResolveSkill);
            var applies = combatData.GetBuffData(FlowingResolveEffect).OfType<BuffApplyEvent>().Where(x => x.To == willbender.AgentItem).ToList();
            foreach (BuffApplyEvent bae in applies)
            {
                res.Add(new AnimatedCastEvent(willbender.AgentItem, skill, bae.Time - 440, 500));
            }
            return res;
        }


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(LethalTempo, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", GW2Builds.EODBeta1, GW2Builds.EODBeta4, DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", GW2Builds.EODBeta1, GW2Builds.EODBeta4, DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Lethal Tempo", "1% per stack", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", GW2Builds.EODBeta4, GW2Builds.EndOfLife, DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", GW2Builds.EODBeta4, GW2Builds.EndOfLife, DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Lethal Tempo", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", GW2Builds.EODBeta4, GW2Builds.March2022Balance2, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "4% per stack", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", GW2Builds.EODBeta4, GW2Builds.March2022Balance2, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", GW2Builds.March2022Balance2, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(LethalTempo, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", GW2Builds.March2022Balance2, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(LethalTempo).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }).UsingApproximate(true),
            new BuffDamageModifier(62529, "Rushing Justice", "Applies burning on consecutive hits", DamageSource.NoPets, 0, DamageType.Strike, DamageType.Strike, Source.Willbender, ByPresence, "https://wiki.guildwars2.com/images/7/74/Rushing_Justice.png", GW2Builds.EODBeta1, GW2Builds.EndOfLife, DamageModifierMode.All)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                //virtues
                new Buff("Rushing Justice", RushingJustice, Source.Willbender, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/74/Rushing_Justice.png"),
                new Buff("Flowing Resolve", FlowingResolveEffect, Source.Willbender, BuffStackType.Queue, 9, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/be/Flowing_Resolve.png"),
                new Buff("Crashing Courage", CrashingCourage, Source.Willbender, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/95/Crashing_Courage.png"),
                //
                new Buff("Repose", Repose, Source.Willbender, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/80/Repose.png"),
                new Buff("Lethal Tempo", LethalTempo, Source.Willbender, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png"),
                //new Buff("Tyrant's Lethal Tempo", 62657, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png"),
        };
    }
}
