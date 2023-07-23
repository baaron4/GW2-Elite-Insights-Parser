using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class WarriorHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(RecklessImpact, RecklessImpact).WithBuilds(GW2Builds.December2017Balance).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new BuffGainCastFinder(BerserkersStanceSkill, BerserkersStanceBuff),
            new BuffGainCastFinder(BalancedStanceSill, BalancedStanceBuff),
            new BuffGainCastFinder(EndurePainSkill, EnduringPainBuff),
            new BuffGainCastFinder(SignetOfFurySkill, SignetOfFuryActive),
            new EffectCastFinderByDst(SignetOfMightSkill, EffectGUIDs.WarriorSignetOfMight).UsingDstBaseSpecChecker(Spec.Warrior),
            new EffectCastFinderByDst(SignetOfStaminaSkill, EffectGUIDs.WarriorSignetOfStamina).UsingDstBaseSpecChecker(Spec.Warrior),
            new EffectCastFinderByDst(DolyakSignetSkill, EffectGUIDs.WarriorDolyakSignet).UsingDstBaseSpecChecker(Spec.Warrior),
            new EXTHealingCastFinder(MendingMight, MendingMight).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        };

        private static HashSet<AgentItem> GetBannerAgents(CombatData combatData, long id, HashSet<AgentItem> playerAgents)
        {
            return new HashSet<AgentItem>(combatData.GetBuffData(id).Where(x => x is BuffApplyEvent && x.CreditedBy.Type == AgentItem.AgentType.Gadget && x.CreditedBy.Master == null && playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.CreditedBy));
        }


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Peak Performance
            new BuffDamageModifier(PeakPerformance, "Peak Performance", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.PeakPerformace, DamageModifierMode.PvE).WithBuilds(GW2Builds.July2018Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(PeakPerformance, "Peak Performance", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.PeakPerformace, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(PeakPerformance, "Peak Performance", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.PeakPerformace, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2018Balance),
            new BuffDamageModifier(PeakPerformance, "Peak Performance", "33%", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.PeakPerformace, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
            // Berserker's Power
            new BuffDamageModifier(BerserkersPower, "Berserker's Power", "7% per stack", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByStack, BuffImages.BerserkersPower, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new BuffDamageModifier(BerserkersPower, "Berserker's Power", "5.25% per stack", DamageSource.NoPets, 5.25, DamageType.Strike, DamageType.All, Source.Warrior, ByStack, BuffImages.BerserkersPower, DamageModifierMode.All).WithBuilds(GW2Builds.October2022Balance),
            // 
            new BuffDamageModifier(Stability, "Stalwart Strength", "10%", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.BerserkersPower, DamageModifierMode.All).WithBuilds(GW2Builds.October2022Balance),
            // Can merciless hammer conditions be tracked reliably?
            // Cull the Weak
            new BuffDamageModifierTarget(Weakness, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.CullTheWeak, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifierTarget(Weakness, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.CullTheWeak, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifierTarget(Weakness, "Cull the Weak", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.CullTheWeak, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            // Leg Specialist
            new BuffDamageModifierTarget(new long[] {Crippled, Immobile, Chilled}, "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.LegSpecialist, DamageModifierMode.All).WithBuilds(GW2Builds.October2019Balance, GW2Builds.May2021Balance),
            new BuffDamageModifierTarget(new long[] {Crippled, Immobile, Chilled}, "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.LegSpecialist, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifierTarget(new long[] {Crippled, Immobile, Chilled}, "Leg Specialist", "10% to movement-impaired foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.LegSpecialist, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            //
            new BuffDamageModifier(NumberOfBoons, "Empowered", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Warrior, ByStack, BuffImages.Empowered, DamageModifierMode.All),
            // Warrior's Cunning (Barrier)
            new DamageLogDamageModifier("Warrior's Cunning (Barrier)", "50% against barrier", DamageSource.NoPets, 50.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) > 0.0 , ByPresence, DamageModifierMode.PvEWvW).UsingApproximate(true).WithBuilds(GW2Builds.December2019Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Warrior's Cunning (Barrier)", "50% against barrier", DamageSource.NoPets, 50.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) > 0.0 , ByPresence, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Warrior's Cunning (Barrier)", "10% against barrier", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) > 0.0 , ByPresence, DamageModifierMode.sPvP).UsingApproximate(true).WithBuilds(GW2Builds.December2019Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Warrior's Cunning (Barrier)", "10% against barrier", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) > 0.0 , ByPresence, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            // Warrior's Cunning (High HP, no Barrier)
            new DamageLogDamageModifier("Warrior's Cunning (High HP, no Barrier)", "25% if foe hp >=90%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 90.0, ByPresence, DamageModifierMode.PvEWvW).UsingApproximate(true).WithBuilds(GW2Builds.December2019Balance, GW2Builds.May2021Balance),
            new DamageLogDamageModifier("Warrior's Cunning (High HP, no Barrier)", "25% if foe hp >=80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, ByPresence, DamageModifierMode.PvEWvW).UsingApproximate(true).WithBuilds(GW2Builds.May2021Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Warrior's Cunning (High HP, no Barrier)", "25% if foe hp >=80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) => x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, ByPresence, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Warrior's Cunning (High HP, no Barrier)", "7% if foe hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) =>x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 90.0, ByPresence, DamageModifierMode.sPvP).UsingApproximate(true).WithBuilds(GW2Builds.December2019Balance, GW2Builds.May2021Balance),
            new DamageLogDamageModifier("Warrior's Cunning (High HP, no Barrier)", "7% if foe hp >=80%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) =>x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, ByPresence, DamageModifierMode.sPvP).UsingApproximate(true).WithBuilds(GW2Builds.May2021Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Warrior's Cunning (High HP, no Barrier)", "7% if foe hp >=80%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, BuffImages.WarriorsCunning, (x, log) =>x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, ByPresence, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            // Warrior's Sprint
            new BuffDamageModifier(Swiftness, "Warrior's Sprint", "7% under swiftness", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.WarriorsSprint, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2018Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(Swiftness, "Warrior's Sprint", "3% under swiftness", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.WarriorsSprint, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2018Balance),
            new BuffDamageModifier(Swiftness, "Warrior's Sprint", "10% under swiftness", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, BuffImages.WarriorsSprint, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            //
            new BuffDamageModifierTarget(NumberOfBoons, "Destruction of the Empowered", "3% per target boon", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Warrior, ByMultipliyingStack, BuffImages.DestructionOfTheEmpowered, DamageModifierMode.All),

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // Skills
            new Buff("Riposte", Riposte, Source.Warrior, BuffClassification.Other, BuffImages.Riposte),
            new Buff("Impaled", Impaled, Source.Warrior, BuffClassification.Debuff, BuffImages.ImpaleWarriorSword),
            new Buff("Flames of War", FlamesOfWar, Source.Warrior, BuffClassification.Other, BuffImages.FlamesOfWarWarrior).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            // Signets
            new Buff("Healing Signet", HealingSignet, Source.Warrior, BuffClassification.Other, BuffImages.HealingSignet),
            new Buff("Dolyak Signet", DolyakSignetBuff, Source.Warrior, BuffClassification.Other, BuffImages.DolyakSignet),
            new Buff("Signet of Fury", SignetOfFuryBuff, Source.Warrior, BuffClassification.Other, BuffImages.SignetOfFury),
            new Buff("Signet of Might", SignetOfMightBuff, Source.Warrior, BuffClassification.Other, BuffImages.SignetOfMight),
            new Buff("Signet of Stamina", SignetOfStaminaBuff, Source.Warrior, BuffClassification.Other, BuffImages.SignetOfStamina),
            new Buff("Signet of Rage", SignetOfRage, Source.Warrior, BuffClassification.Other, BuffImages.SignetOfRage),
            // Banners
            new Buff("Banner of Strength", BannerOfStrengthBuff, Source.Warrior, BuffClassification.Offensive, BuffImages.BannerOfStrength).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Banner of Discipline", BannerOfDisciplineBuff, Source.Warrior, BuffClassification.Offensive, BuffImages.BannerOfDiscipline).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Banner of Tactics", BannerOfTacticsBuff, Source.Warrior, BuffClassification.Support, BuffImages.BannerOfTactics).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Banner of Defense", BannerOfDefenseBuff, Source.Warrior, BuffClassification.Defensive, BuffImages.BannerOfDefense).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            // Stances
            new Buff("Shield Stance", ShieldStance, Source.Warrior, BuffClassification.Other, BuffImages.ShieldStance),
            new Buff("Berserker's Stance", BerserkersStanceBuff, Source.Warrior, BuffClassification.Other, BuffImages.BerserkerStance),
            new Buff("Enduring Pain", EnduringPainBuff, Source.Warrior, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.EndurePain),
            new Buff("Balanced Stance", BalancedStanceBuff, Source.Warrior, BuffClassification.Other, BuffImages.BalancedStance),
            new Buff("Defiant Stance", DefiantStance, Source.Warrior, BuffClassification.Other, BuffImages.DefiantStance),
            new Buff("Rampage", Rampage, Source.Warrior, BuffClassification.Other, BuffImages.Rampage),
            // Traits
            new Buff("Soldier's Focus", SoldiersFocus, Source.Warrior, BuffClassification.Other, BuffImages.SoldiersFocus).WithBuilds(GW2Builds.October2019Balance, GW2Builds.EndOfLife),
            new Buff("Brave Stride", BraveStride, Source.Warrior, BuffClassification.Other, BuffImages.DeathFromAbove),
            new Buff("Empower Allies", EmpowerAllies, Source.Warrior, BuffClassification.Offensive, BuffImages.EmpowerAllies).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Peak Performance", PeakPerformance, Source.Warrior, BuffClassification.Other, BuffImages.PeakPerformace),
            new Buff("Furious Surge", FuriousSurge, Source.Warrior, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.Furious),
            //new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.Normal),
            new Buff("Rousing Resilience", RousingResilience, Source.Warrior, BuffClassification.Other, BuffImages.RousingResilience),
            new Buff("Berserker's Power" ,BerserkersPower, Source.Warrior, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.BerserkersPower).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new Buff("Berserker's Power", BerserkersPower, Source.Warrior, BuffStackType.Stacking, 4, BuffClassification.Other, BuffImages.BerserkersPower).WithBuilds(GW2Builds.October2022Balance, GW2Builds.EndOfLife),
            new Buff("Signet of Ferocity", SignetOfFerocity, Source.Warrior, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.SignetMastery),
            new Buff("Adrenal Health", AdrenalHealth, Source.Warrior, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.AdrenalHealth).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
            new Buff("Adrenal Health", AdrenalHealth, Source.Warrior, BuffStackType.Stacking, 4, BuffClassification.Other, BuffImages.AdrenalHealth).WithBuilds(GW2Builds.October2022Balance, GW2Builds.EndOfLife),
        };


        /*private static HashSet<AgentItem> FindBattleStandards(Dictionary<long, List<AbstractBuffEvent>> buffData, HashSet<AgentItem> playerAgents)
        {
            if (buffData.TryGetValue(725, out List<AbstractBuffEvent> list))
            {
                var battleBannerCandidates = new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && (playerAgents.Contains(x.To) || playerAgents.Contains(x.To.Master))).Select(x => x.By));
                if (battleBannerCandidates.Count > 0)
                {
                    if (buffData.TryGetValue(740, out list))
                    {
                        battleBannerCandidates.IntersectWith(new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && (playerAgents.Contains(x.To) || playerAgents.Contains(x.To.Master))).Select(x => x.By)));
                        if (battleBannerCandidates.Count > 0)
                        {
                            if (buffData.TryGetValue(Swiftness, out list))
                            {
                                battleBannerCandidates.IntersectWith(new HashSet<AgentItem>(list.Where(x => x is BuffApplyEvent && x.By.Type == AgentItem.AgentType.Gadget && (playerAgents.Contains(x.To) || playerAgents.Contains(x.To.Master))).Select(x => x.By)));
                                return battleBannerCandidates;
                            }
                        }
                    }
                }
            }
            return new HashSet<AgentItem>();
        }*/

        public static void ProcessGadgets(IReadOnlyList<Player> players, CombatData combatData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            HashSet<AgentItem> strBanners = GetBannerAgents(combatData, BannerOfStrengthBuff, playerAgents),
                defBanners = GetBannerAgents(combatData, BannerOfDefenseBuff, playerAgents),
                disBanners = GetBannerAgents(combatData, BannerOfDisciplineBuff, playerAgents),
                tacBanners = GetBannerAgents(combatData, BannerOfTacticsBuff, playerAgents);
            //battleBanner = FindBattleStandards(buffData, playerAgents);
            var warriors = players.Where(x => x.BaseSpec == Spec.Warrior).ToList();
            // if only one warrior, could only be that one
            if (warriors.Count == 1)
            {
                Player warrior = warriors[0];
                ProfHelper.SetGadgetMaster(strBanners, warrior.AgentItem);
                ProfHelper.SetGadgetMaster(disBanners, warrior.AgentItem);
                ProfHelper.SetGadgetMaster(tacBanners, warrior.AgentItem);
                ProfHelper.SetGadgetMaster(defBanners, warrior.AgentItem);
                //SetBannerMaster(battleBanner, warrior.AgentItem);
            }
            else if (warriors.Count > 1)
            {
                // land and under water cast ids
                ProfHelper.AttachMasterToGadgetByCastData(combatData, strBanners, new List<long> { BannerOfStrengthSkill, BannerOfStrengthSkillUW }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, defBanners, new List<long> { BannerOfDefenseSkill, BannerOfDefenseSkillUW }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, disBanners, new List<long> { BannerOfDisciplineSkill, BannerOfDisciplineSkillUW }, 1000);
                ProfHelper.AttachMasterToGadgetByCastData(combatData, tacBanners, new List<long> { BannerOfTacticsSkill, BannerOfTacticsSkillUW }, 1000);
                //AttachMasterToBanner(castData, battleBanner, 14419, 14569);
            }
        }

    }
}
