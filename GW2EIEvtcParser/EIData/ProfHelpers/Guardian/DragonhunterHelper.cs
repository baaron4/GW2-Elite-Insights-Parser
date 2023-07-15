using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DragonhunterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(TestOfFaith, EffectGUIDs.DragonhunterTestOfFaith).UsingSrcSpecChecker(Spec.Dragonhunter),
            new EffectCastFinder(FragmentsOfFaith, EffectGUIDs.DragonhunterFragmentsOfFaith).UsingSrcSpecChecker(Spec.Dragonhunter),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Crippled, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.ZealotsAggression, DamageModifierMode.All),
            // Pur of Sight unclear. Max is very likely to be 1200, as it is the maximum tooltip range for a DH but what is the distance at witch the minimum is reached? Is the scaling linear?
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.February2020Balance, GW2Builds.November2022Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 10000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 10000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.November2022Balance, GW2Builds.May2023Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 12000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 12000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.May2023Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 12000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 12000;
                }
                return false;
            }),
            //         
            new BuffDamageModifierTarget(new long[] { Stun, Daze, Knockdown, Fear, Taunt }, "Heavy Light", "15% to disabled foes", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.Strike, Source.Elementalist, ByPresence, BuffImages.HeavyLight, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.February2020Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Justice (Dragonhunter)", JusticeDragonhunter, Source.Dragonhunter, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SpearOfLight),
            new Buff("Shield of Courage (Active)", ShieldOfCourageActive, Source.Dragonhunter, BuffClassification.Other, BuffImages.ShieldOfCourage),
            new Buff("Spear of Justice", SpearOfJustice, Source.Dragonhunter, BuffClassification.Other, BuffImages.SpearOfJustice),
            new Buff("Shield of Courage", ShieldOfCourage, Source.Dragonhunter, BuffClassification.Other, BuffImages.ShieldOfCourage),
            new Buff("Wings of Resolve", WingsOfResolveBuff, Source.Dragonhunter, BuffClassification.Other, BuffImages.WingsOfResolve),
            new Buff("Hunter's Mark", HuntersMark, Source.Dragonhunter, BuffClassification.Other, BuffImages.HuntersWard),
        };

    }
}
