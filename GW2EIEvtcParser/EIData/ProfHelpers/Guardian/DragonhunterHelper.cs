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
    internal static class DragonhunterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(TestOfFaith, EffectGUIDs.DragonhunterTestOfFaith)
                .UsingSrcSpecChecker(Spec.Dragonhunter),
            new EffectCastFinder(FragmentsOfFaith, EffectGUIDs.DragonhunterFragmentsOfFaith)
                .UsingSrcSpecChecker(Spec.Dragonhunter),
        };

        private static bool CheckTether(ParsedEvtcLog log, AgentItem src, AgentItem dst, long time)
        {
            if (!log.CombatData.GetBuffData(JusticeDragonhunter).Any(x => x is BuffApplyEvent bae && bae.By == src && Math.Abs(bae.AppliedDuration - 6000) > ServerDelayConstant))
            {
                return false;
            }
            return log.FindActor(dst).HasBuff(log, log.FindActor(src), JusticeDragonhunter, time);
        }

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(Crippled, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.ZealotsAggression, DamageModifierMode.All),
            // Pur of Sight unclear. Max is very likely to be 1200, as it is the maximum tooltip range for a DH but what is the distance at witch the minimum is reached? Is the scaling linear?
            new BuffOnFoeDamageModifier(JusticeDragonhunter, "Big Game Hunter", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return CheckTether(log, src, dst, x.Time);
            }),
            new BuffOnFoeDamageModifier(JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return CheckTether(log, src, dst, x.Time);
            }),
            new BuffOnFoeDamageModifier(JusticeDragonhunter, "Big Game Hunter", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2023Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return CheckTether(log, src, dst, x.Time);
            }),
            new BuffOnFoeDamageModifier(JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, BuffImages.BigGameHunter, DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.May2023Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return CheckTether(log, src, dst, x.Time);
            }),
            //         
            new BuffOnFoeDamageModifier(new long[] { Stun, Daze, Knockdown, Fear, Taunt }, "Heavy Light (Disabled)", "15% to disabled foes", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByPresence, BuffImages.HeavyLight, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.February2020Balance),
            new BuffOnFoeDamageModifier(new long[] { Stun, Daze, Knockdown, Fear, Taunt }, "Heavy Light (Defiant)", "10% to defiant non disabled foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.Strike, Source.Dragonhunter, ByAbsence, BuffImages.HeavyLight, DamageModifierMode.All)
                .UsingChecker((x, log) => x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None).UsingApproximate(true).WithBuilds(GW2Builds.November2022Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(NumberOfConditions, "Hunter's Fortification", "-10%", DamageSource.NoPets, -10, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByAbsence, BuffImages.HuntersFortification, DamageModifierMode.All),
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
