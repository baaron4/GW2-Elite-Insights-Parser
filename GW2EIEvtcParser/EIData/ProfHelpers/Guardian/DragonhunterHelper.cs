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
    internal static class DragonhunterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(TestOfFaith, EffectGUIDs.DragonhunterTestOfFaith).UsingChecker((evt, log) => evt.Src.Spec == Spec.Dragonhunter),
            new EffectCastFinder(FragmentsOfFaith, EffectGUIDs.DragonhunterFragmentsOfFaith).UsingChecker((evt, log) => evt.Src.Spec == Spec.Dragonhunter),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Crippled, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/7e/Zealot%27s_Aggression.png", DamageModifierMode.All),
            // Pur of Sight unclear. Max is very likely to be 1200, as it is the maximum tooltip range for a DH but what is the distance at witch the minimum is reached? Is the scaling linear?
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "20% to tethered target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 8000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 8000;
                }
                return false;
            }),
            new BuffDamageModifierTarget(JusticeDragonhunter, "Big Game Hunter", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/76/Big_Game_Hunter.png", DamageModifierMode.PvEInstanceOnly).WithBuilds(GW2Builds.February2020Balance).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(JusticeDragonhunter).Where(y => y is BuffApplyEvent bae && Math.Abs(bae.AppliedDuration - 10000) < ServerDelayConstant && bae.By == src && bae.To == dst).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                   return x.Time - effectApply.Time < 10000;
                }
                return false;
            }),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Justice (Dragonhunter)",JusticeDragonhunter, Source.Dragonhunter, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b0/Spear_of_Light.png"),
                new Buff("Shield of Courage (Active)", ShieldOfCourageActive, Source.Dragonhunter, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Spear of Justice", SpearOfJustice, Source.Dragonhunter, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f1/Spear_of_Justice.png"),
                new Buff("Shield of Courage", ShieldOfCourage, Source.Dragonhunter, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Wings of Resolve", WingsOfResolveEffect, Source.Dragonhunter, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/cb/Wings_of_Resolve.png"),
                new Buff("Hunter's Mark", HuntersMark, Source.Dragonhunter, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e6/Hunter%27s_Ward.png"),
        };

    }
}
