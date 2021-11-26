using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class WillbenderHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffApproximateDamageModifier(62509, "Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue, DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }),
            new BuffApproximateDamageModifier(62509, "Tyrant's Lethal Tempo", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue, DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }),
            /*
            new BuffApproximateDamageModifier(62509, "Lethal Tempo", "1% per stack", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue, DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }),
            new BuffApproximateDamageModifier(62509, "Tyrant's Lethal Tempo", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue, DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }),
            new BuffApproximateDamageModifier(62509, "Lethal Tempo", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 6000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 6000;
                }
                return false;
            }),
            new BuffApproximateDamageModifier(62509, "Tyrant's Lethal Tempo", "4% per stack", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.Willbender, ByStack, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(62509).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 4000) < ServerDelayConstant && bae.By == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 4000;
                }
                return false;
            }),
            */
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                //virtues
                new Buff("Rushing Justice", 62529, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/74/Rushing_Justice.png", 118697, ulong.MaxValue),
                new Buff("Flowing Resolve", 62632, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/be/Flowing_Resolve.png", 118697, ulong.MaxValue),
                new Buff("Crashing Courage", 62615, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/95/Crashing_Courage.png", 118697, ulong.MaxValue),
                //
                new Buff("Repose", 62638, Source.Willbender, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/80/Repose.png", 118697, ulong.MaxValue),
                new Buff("Lethal Tempo", 62509, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/10/Lethal_Tempo.png", 118697, ulong.MaxValue),
                //new Buff("Tyrant's Lethal Tempo", 62657, Source.Willbender, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Tyrant%27s_Momentum.png", 118697, ulong.MaxValue),
        };
    }
}
