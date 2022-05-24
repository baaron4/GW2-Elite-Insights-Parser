using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class RangerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(12573,12573,EIData.InstantCastFinder.DefaultICD), // Hunter's Shot
            //new DamageCastFinder(12507,12507,EIData.InstantCastFinder.DefaultICD), // Crippling Shot
            new BuffGainWithMinionsCastFinder(SicEmSkill,SicEmEffect,EIData.InstantCastFinder.DefaultICD), // "Sic 'Em!"
            new BuffGainWithMinionsCastFinder(SicEmSkill,SicEmEffectPvP,EIData.InstantCastFinder.DefaultICD), // "Sic 'Em!" PvP
            new BuffGainCastFinder(SignetOfStone,SignetOfStoneActive,EIData.InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant), // Signet of Stone
            new BuffGainCastFinder(LesserSignetOfStone,SignetOfStoneActive,EIData.InstantCastFinder.DefaultICD, (evt, combatData) => Math.Abs(evt.AppliedDuration - 5000) < ServerDelayConstant), // Lesser Signet of Stone
            new BuffGainCastFinder(SharpeningStonesSkill,SharpeningStonesEffect,EIData.InstantCastFinder.DefaultICD), // Sharpening Stone
            new EXTHealingCastFinder(WindbornNotes, WindbornNotes, EIData.InstantCastFinder.DefaultICD), // Windborne Notes
            new EXTBarrierCastFinder(ProtectMe, ProtectMe, EIData.InstantCastFinder.DefaultICD), // Protect Me!
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Skills
            new BuffDamageModifier(SicEmEffect, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", 0, GW2Builds.May2021Balance, DamageModifierMode.PvE, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(SicEmEffect).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
            new BuffDamageModifier(SicEmEffect, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", 0, GW2Builds.May2021Balance, DamageModifierMode.sPvPWvW, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(SicEmEffect).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
            new BuffDamageModifier(SicEmEffect, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png", GW2Builds.May2021Balance, GW2Builds.EndOfLife, DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(SicEmEffect).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
            // Marksmanship
            new DamageLogDamageModifier("Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, "https://wiki.guildwars2.com/images/2/2f/Steady_Focus.png", (x, log) => {
                if (!x.Skill.IsWeaponSkill)
                {
                    return false;
                }
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600.0;
            }, ByPresence, GW2Builds.July2018Balance, GW2Builds.EndOfLife, DamageModifierMode.All).UsingApproximate(true),
            new DamageLogDamageModifier("Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, "https://wiki.guildwars2.com/images/2/2f/Steady_Focus.png", (x, log) => {
                if (!x.Skill.IsWeaponSkill)
                {
                    return false;
                }
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) > 600.0;
            }, ByPresence, GW2Builds.July2018Balance, GW2Builds.EndOfLife, DamageModifierMode.All).UsingApproximate(true),
            new BuffDamageModifierTarget(new long[] { 872, 833, 721, 727, 791, 722, 27705}, "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Predator%27s_Onslaught.png", DamageModifierMode.All),
            // Skirmishing
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger,"https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png", (x, log) => x.IsFlanking , ByPresence, GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.All),
            new BuffDamageModifier(LightOnYourFeet, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png", DamageModifierMode.All),
            // Nature Magic
            // We can't check buffs on minions yet
            new BuffDamageModifier(NumberOfBoons, "Bountiful Hunter", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Ranger, ByStack, "https://wiki.guildwars2.com/images/2/25/Bountiful_Hunter.png", DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Counterattack",Counterattack, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c1/Counterattack.png"),
                //signets
                new Buff("Signet of Renewal",SignetOfRenewal, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png"),
                new Buff("Signet of Stone (Passive)",SignetOfStonePassive, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Passive)",SignetOfTheHuntPassive, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                new Buff("Signet of the Wild",SignetOfTheWild, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of the Wild (Pet)",SignetOfTheWildPet, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/23/Signet_of_the_Wild.png"),
                new Buff("Signet of Stone (Active)",SignetOfStoneActive, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/63/Signet_of_Stone.png"),
                new Buff("Signet of the Hunt (Active)",SignetOfTheHuntActive, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Signet_of_the_Hunt.png"),
                //spirits
                // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png"),
                new Buff("Frost Spirit", FrostSpiritOld, Source.Ranger, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", 0, GW2Builds.May2018Balance),
                new Buff("Sun Spirit", SunSpiritOld, Source.Ranger, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", 0, GW2Builds.May2018Balance),
                new Buff("Stone Spirit", StoneSpiritOld, Source.Ranger, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", 0, GW2Builds.May2018Balance),
                //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png"),
                //reworked
                new Buff("Water Spirit", WaterSpirit, Source.Ranger, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/thumb/0/06/Water_Spirit.png/33px-Water_Spirit.png", GW2Builds.May2018Balance, GW2Builds.EndOfLife),
                new Buff("Frost Spirit", FrostSpirit, Source.Ranger, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/thumb/c/c6/Frost_Spirit.png/33px-Frost_Spirit.png", GW2Builds.May2018Balance, GW2Builds.EndOfLife),
                new Buff("Sun Spirit", SunSpirit, Source.Ranger, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/thumb/d/dd/Sun_Spirit.png/33px-Sun_Spirit.png", GW2Builds.May2018Balance, GW2Builds.EndOfLife),
                new Buff("Stone Spirit", StoneSpirit, Source.Ranger, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/thumb/3/35/Stone_Spirit.png/20px-Stone_Spirit.png", GW2Builds.May2018Balance, GW2Builds.EndOfLife),
                new Buff("Storm Spirit", StormSpirit, Source.Ranger, BuffClassification.Support, "https://wiki.guildwars2.com/images/thumb/2/25/Storm_Spirit.png/30px-Storm_Spirit.png", GW2Builds.May2018Balance, GW2Builds.EndOfLife),
                //skills
                new Buff("Attack of Opportunity",AttackOfOpportunity, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/47/Moment_of_Clarity.png"),
                new Buff("Call of the Wild",CallOfTheWild, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",0 , GW2Builds.July2019Balance),
                new Buff("Call of the Wild",CallOfTheWild, Source.Ranger, BuffStackType.Stacking, 3, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8d/Call_of_the_Wild.png",GW2Builds.July2019Balance , GW2Builds.February2020Balance),
                new Buff("Strength of the Pack!",StrengthOfThePack, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/4b/%22Strength_of_the_Pack%21%22.png"),
                new Buff("Sic 'Em!",SicEmEffect, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sic 'Em! (PvP)",SicEmEffectPvP, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9d/%22Sic_%27Em%21%22.png"),
                new Buff("Sharpening Stones",SharpeningStonesEffect, Source.Ranger, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/af/Sharpening_Stone.png"),
                new Buff("Sharpen Spines",SharpenSpines, Source.Ranger, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/95/Sharpen_Spines.png"),
                //traits
                new Buff("Spotter", Spotter, Source.Ranger, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/b/b0/Spotter.png"),
                new Buff("Opening Strike",OpeningStrike, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/44/Opening_Strike_%28effect%29.png"),
                new Buff("Quick Draw",QuickDraw, Source.Ranger, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/39/Quick_Draw.png"),
                new Buff("Light on your Feet",LightOnYourFeet, Source.Ranger, BuffStackType.Queue, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/22/Light_on_your_Feet.png"),
        };


        public static void AttachMasterToRangerGadgets(IReadOnlyList<Player> players, CombatData combatData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbraces = ProfHelper.GetOffensiveGadgetAgents(combatData, 1286, playerAgents);
            HashSet<AgentItem> blackHoles = ProfHelper.GetOffensiveGadgetAgents(combatData, 31436, playerAgents);
            var rangers = players.Where(x => x.BaseSpec == Spec.Ranger).ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                ProfHelper.SetGadgetMaster(jacarandaEmbraces, ranger.AgentItem);
                ProfHelper.SetGadgetMaster(blackHoles, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                ProfHelper.AttachMasterToGadgetByCastData(combatData, jacarandaEmbraces, new List<long> { 44980 }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, blackHoles, new List<long> { 31503 }, 1000);
            }
        }

        private static readonly HashSet<long> SpiritIDs = new HashSet<long>()
        {
            (int)MinionID.FrostSpirit,
            (int)MinionID.StoneSpirit,
            (int)MinionID.StormSpirit,
            (int)MinionID.SunSpirit,
            (int)MinionID.SpiritOfNatureRenewal,
        };

        private static HashSet<long> NonSpiritMinions = new HashSet<long>()
        {
            (int)MinionID.JuvenileAlpineWolf,
            (int)MinionID.JuvenileArctodus,
            (int)MinionID.JuvenileArmorFish,
            (int)MinionID.JuvenileBlackBear,
            (int)MinionID.JuvenileBlackMoa,
            (int)MinionID.JuvenileBlackWidowSpider,
            (int)MinionID.JuvenileBlueJellyfish,
            (int)MinionID.JuvenileBlueMoa,
            (int)MinionID.JuvenileBoar,
            (int)MinionID.JuvenileBristleback,
            (int)MinionID.JuvenileBrownBear,
            (int)MinionID.JuvenileCheetah,
            (int)MinionID.JuvenileEagle,
            (int)MinionID.JuvenileEletricWywern,
            (int)MinionID.JuvenileFangedIboga,
            (int)MinionID.JuvenileFernHound,
            (int)MinionID.JuvenileFireWywern,
            (int)MinionID.JuvenileForestSpider,
            (int)MinionID.JuvenileIceDrake,
            (int)MinionID.JuvenileJacaranda,
            (int)MinionID.JuvenileJungleSpider,
            (int)MinionID.JuvenileJungleStalker,
            (int)MinionID.JuvenileKrytanDrakehound,
            (int)MinionID.JuvenileLashtailDevourer,
            (int)MinionID.JuvenileLynx,
            (int)MinionID.JuvenileMarshDrake,
            (int)MinionID.JuvenileOwl,
            (int)MinionID.JuvenilePig,
            (int)MinionID.JuvenilePinkMoa,
            (int)MinionID.JuvenileRainbowJellyfish,
            (int)MinionID.JuvenileRedJellyfish,
            (int)MinionID.JuvenileRedMoa,
            (int)MinionID.JuvenileRiverDrake,
            (int)MinionID.JuvenileRockGazelle,
            (int)MinionID.JuvenileSalamanderDraker,
            (int)MinionID.JuvenileSandLion,
            (int)MinionID.JuvenileShark,
            (int)MinionID.JuvenileSmokescale,
            (int)MinionID.JuvenileTiger,
            (int)MinionID.JuvenileWarthog,
            (int)MinionID.JuvenileWhiptailDevourer,
            (int)MinionID.JuvenileWolf,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return NonSpiritMinions.Contains(id) || SpiritIDs.Contains(id);
        }

    }
}
