using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class WarriorHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new DamageCastFinder(RecklessImpact, RecklessImpact).WithBuilds(GW2Builds.December2017Balance).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new BuffGainCastFinder(BerserkersStanceSkill, BerserkersStanceBuff),
        new BuffGainCastFinder(BalancedStanceSill, BalancedStanceBuff),
        new BuffGainCastFinder(EndurePainSkill, EnduringPainBuff),
        new BuffGainCastFinder(SignetOfFurySkill, SignetOfFuryActive),
        new EffectCastFinderByDst(SignetOfMightSkill, EffectGUIDs.WarriorSignetOfMight).UsingDstBaseSpecChecker(Spec.Warrior),
        new EffectCastFinderByDst(SignetOfStaminaSkill, EffectGUIDs.WarriorSignetOfStamina).UsingDstBaseSpecChecker(Spec.Warrior),
        new EffectCastFinderByDst(DolyakSignetSkill, EffectGUIDs.WarriorDolyakSignet).UsingDstBaseSpecChecker(Spec.Warrior),
        new EXTHealingCastFinder(MendingMight, MendingMight).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
    ];

    private static HashSet<AgentItem> GetBannerAgents(CombatData combatData, long id, HashSet<AgentItem> playerAgents)
    {
        return new HashSet<AgentItem>(combatData.GetBuffData(id).Where(x => x is BuffApplyEvent && x.CreditedBy.Type == AgentItem.AgentType.Gadget && x.CreditedBy.Master == null && playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.CreditedBy));
    }


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Peak Performance
        new BuffOnActorDamageModifier(Mod_PeakPerformance, PeakPerformance, "Peak Performance", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.PeakPerformace, DamageModifierMode.PvE).WithBuilds(GW2Builds.July2018Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_PeakPerformance, PeakPerformance, "Peak Performance", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.PeakPerformace, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_PeakPerformance, PeakPerformance, "Peak Performance", "7%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.PeakPerformace, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2018Balance),
        new BuffOnActorDamageModifier(Mod_PeakPerformance, PeakPerformance, "Peak Performance", "33%", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.PeakPerformace, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
        // Berserker's Power
        new BuffOnActorDamageModifier(Mod_BerserkersPower, BerserkersPower, "Berserker's Power", "7% per stack", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByStack, TraitImages.BerserkersPower, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
        new BuffOnActorDamageModifier(Mod_BerserkersPower, BerserkersPower, "Berserker's Power", "5.25% per stack", DamageSource.NoPets, 5.25, DamageType.Strike, DamageType.All, Source.Warrior, ByStack, TraitImages.BerserkersPower, DamageModifierMode.All).WithBuilds(GW2Builds.October2022Balance),
        // 
        new BuffOnActorDamageModifier(Mod_StalwartStrength, Stability, "Stalwart Strength", "10%", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.BerserkersPower, DamageModifierMode.All).WithBuilds(GW2Builds.October2022Balance),
        // Can merciless hammer conditions be tracked reliably?
        // Cull the Weak
        new BuffOnFoeDamageModifier(Mod_CullTheWeak, Weakness, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.CullTheWeak, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_CullTheWeak, Weakness, "Cull the Weak", "7% on weakened target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.CullTheWeak, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_CullTheWeak, Weakness, "Cull the Weak", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.CullTheWeak, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
        // Leg Specialist
        new BuffOnFoeDamageModifier(Mod_LegSpecialist, [Crippled, Immobile, Chilled], "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.LegSpecialist, DamageModifierMode.All).WithBuilds(GW2Builds.October2019Balance, GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_LegSpecialist, [Crippled, Immobile, Chilled], "Leg Specialist", "7% to movement-impaired foes", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.LegSpecialist, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_LegSpecialist, [Crippled, Immobile, Chilled], "Leg Specialist", "10% to movement-impaired foes", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.LegSpecialist, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
        //
        new BuffOnActorDamageModifier(Mod_Empowered, NumberOfBoons, "Empowered", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Warrior, ByStack, TraitImages.Empowered, DamageModifierMode.All),
        // Warrior's Cunning (Barrier)
        new DamageLogDamageModifier(Mod_WarriorsCunningBarrier, "Warrior's Cunning (Barrier)", "50% against barrier", DamageSource.NoPets, 50.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage > 0 , DamageModifierMode.PvEWvW).WithBuilds(GW2Builds.December2019Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_WarriorsCunningBarrier, "Warrior's Cunning (Barrier)", "50% against barrier", DamageSource.NoPets, 50.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage > 0 , DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_WarriorsCunningBarrier, "Warrior's Cunning (Barrier)", "10% against barrier", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage > 0 , DamageModifierMode.sPvP).WithBuilds(GW2Builds.December2019Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_WarriorsCunningBarrier, "Warrior's Cunning (Barrier)", "10% against barrier", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage > 0 , DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Warrior's Cunning (High HP, no Barrier)
        new DamageLogDamageModifier(Mod_WarriorsCunningNoBarrier, "Warrior's Cunning (High HP, no Barrier)", "25% if foe hp >=90%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage == 0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 90.0, DamageModifierMode.PvEWvW).UsingApproximate(true).WithBuilds(GW2Builds.December2019Balance, GW2Builds.May2021Balance),
        new DamageLogDamageModifier(Mod_WarriorsCunningNoBarrier, "Warrior's Cunning (High HP, no Barrier)", "25% if foe hp >=80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage == 0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, DamageModifierMode.PvEWvW).UsingApproximate(true).WithBuilds(GW2Builds.May2021Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_WarriorsCunningNoBarrier, "Warrior's Cunning (High HP, no Barrier)", "25% if foe hp >=80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage == 0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_WarriorsCunningNoBarrier, "Warrior's Cunning (High HP, no Barrier)", "7% if foe hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) => x.ShieldDamage == 0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 90.0, DamageModifierMode.sPvP).UsingApproximate(true).WithBuilds(GW2Builds.December2019Balance, GW2Builds.May2021Balance),
        new DamageLogDamageModifier(Mod_WarriorsCunningNoBarrier, "Warrior's Cunning (High HP, no Barrier)", "7% if foe hp >=80%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) =>x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, DamageModifierMode.sPvP).UsingApproximate(true).WithBuilds(GW2Builds.May2021Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_WarriorsCunningNoBarrier, "Warrior's Cunning (High HP, no Barrier)", "7% if foe hp >=80%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, TraitImages.WarriorsCunning, (x, log) =>x.To.GetCurrentBarrierPercent(log, x.Time) == 0.0 && x.To.GetCurrentHealthPercent(log, x.Time) >= 80.0, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Warrior's Sprint
        new BuffOnActorDamageModifier(Mod_WarriorsSprint, Swiftness, "Warrior's Sprint", "7% under swiftness", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.WarriorsSprint, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2018Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_WarriorsSprint, Swiftness, "Warrior's Sprint", "3% under swiftness", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.WarriorsSprint, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2018Balance),
        new BuffOnActorDamageModifier(Mod_WarriorsSprint, Swiftness, "Warrior's Sprint", "10% under swiftness", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.WarriorsSprint, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
        //
        new BuffOnFoeDamageModifier(Mod_DestructionOfTheEmpowered, NumberOfBoons, "Destruction of the Empowered", "3% per target boon", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Warrior, ByMultipliyingStack, TraitImages.DestructionOfTheEmpowered, DamageModifierMode.All),

    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new CounterOnActorDamageModifier(Mod_EndurePain, EnduringPainBuff, "Endure Pain", "-100%", DamageSource.NoPets, DamageType.Strike, DamageType.All, Source.Warrior, SkillImages.EndurePain, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_HardenedArmor, Resolution, "Hardened Armor", "-10% under resolution", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, TraitImages.HardenedArmor, DamageModifierMode.All).WithBuilds(GW2Builds.March2020Balance),
        new BuffOnActorDamageModifier(Mod_Rampage, Rampage, "Rampage", "-25%", DamageSource.NoPets, -25.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, SkillImages.Rampage, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_Rampage, Rampage, "Rampage", "-50%", DamageSource.NoPets, -50.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, SkillImages.Rampage, DamageModifierMode.All).WithBuilds(GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_DolyakSignet, DolyakSignetBuff, "Dolyak Signet", "-10%", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Warrior, ByPresence, SkillImages.DolyakSignet, DamageModifierMode.All).WithBuilds(GW2Builds.October2024Balance),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        // Skills
        new Buff("Riposte", Riposte, Source.Warrior, BuffClassification.Other, SkillImages.Riposte),
        new Buff("Impaled", Impaled, Source.Warrior, BuffClassification.Debuff, SkillImages.ImpaleWarriorSword),
        new Buff("Flames of War", FlamesOfWar, Source.Warrior, BuffClassification.Other, SkillImages.FlamesOfWarWarrior).WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Signets
        new Buff("Healing Signet", HealingSignet, Source.Warrior, BuffClassification.Other, SkillImages.HealingSignet),
        new Buff("Dolyak Signet", DolyakSignetBuff, Source.Warrior, BuffClassification.Other, SkillImages.DolyakSignet),
        new Buff("Signet of Fury", SignetOfFuryBuff, Source.Warrior, BuffClassification.Other, SkillImages.SignetOfFury),
        new Buff("Signet of Fury (Active)", SignetOfFuryActive, Source.Warrior, BuffClassification.Other, SkillImages.SignetOfFury),
        new Buff("Signet of Might", SignetOfMightBuff, Source.Warrior, BuffClassification.Other, SkillImages.SignetOfMight),
        new Buff("Signet of Stamina", SignetOfStaminaBuff, Source.Warrior, BuffClassification.Other, SkillImages.SignetOfStamina),
        new Buff("Signet of Rage", SignetOfRage, Source.Warrior, BuffClassification.Other, SkillImages.SignetOfRage),
        // Banners
        new Buff("Banner of Strength", BannerOfStrengthBuff, Source.Warrior, BuffClassification.Offensive, SkillImages.BannerOfStrength).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Banner of Discipline", BannerOfDisciplineBuff, Source.Warrior, BuffClassification.Offensive, SkillImages.BannerOfDiscipline).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Banner of Tactics", BannerOfTacticsBuff, Source.Warrior, BuffClassification.Support, SkillImages.BannerOfTactics).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Banner of Defense", BannerOfDefenseBuff, Source.Warrior, BuffClassification.Defensive, SkillImages.BannerOfDefense).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        // Stances
        new Buff("Shield Stance", ShieldStance, Source.Warrior, BuffClassification.Other, SkillImages.ShieldStance),
        new Buff("Berserker's Stance", BerserkersStanceBuff, Source.Warrior, BuffClassification.Other, SkillImages.BerserkerStance),
        new Buff("Enduring Pain", EnduringPainBuff, Source.Warrior, BuffStackType.Queue, 25, BuffClassification.Other, SkillImages.EndurePain),
        new Buff("Balanced Stance", BalancedStanceBuff, Source.Warrior, BuffClassification.Other, SkillImages.BalancedStance),
        new Buff("Defiant Stance", DefiantStance, Source.Warrior, BuffClassification.Other, SkillImages.DefiantStance),
        new Buff("Rampage", Rampage, Source.Warrior, BuffClassification.Other, SkillImages.Rampage),
        // Traits
        new Buff("Soldier's Focus", SoldiersFocus, Source.Warrior, BuffClassification.Other, TraitImages.SoldiersFocus).WithBuilds(GW2Builds.October2019Balance),
        new Buff("Brave Stride", BraveStride, Source.Warrior, BuffClassification.Other, TraitImages.DeathFromAbove),
        new Buff("Empower Allies", EmpowerAllies, Source.Warrior, BuffClassification.Offensive, TraitImages.EmpowerAllies).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Peak Performance", PeakPerformance, Source.Warrior, BuffClassification.Other, TraitImages.PeakPerformace),
        new Buff("Furious Surge", FuriousSurge, Source.Warrior, BuffStackType.Stacking, 25, BuffClassification.Other, TraitImages.Furious),
        //new Boon("Health Gain per Adrenaline bar Spent",-1, BoonSource.Warrior, BoonType.Intensity, 3, BoonEnum.GraphOnlyBuff,RemoveType.Normal),
        new Buff("Rousing Resilience", RousingResilience, Source.Warrior, BuffClassification.Other, TraitImages.RousingResilience),
        new Buff("Berserker's Power" ,BerserkersPower, Source.Warrior, BuffStackType.Stacking, 3, BuffClassification.Other, TraitImages.BerserkersPower).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
        new Buff("Berserker's Power", BerserkersPower, Source.Warrior, BuffStackType.Stacking, 4, BuffClassification.Other, TraitImages.BerserkersPower).WithBuilds(GW2Builds.October2022Balance),
        new Buff("Signet of Ferocity", SignetOfFerocity, Source.Warrior, BuffStackType.Stacking, 5, BuffClassification.Other, SkillImages.SignetMastery),
        new Buff("Adrenal Health", AdrenalHealth, Source.Warrior, BuffStackType.Stacking, 3, BuffClassification.Other, TraitImages.AdrenalHealth).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2022Balance),
        new Buff("Adrenal Health", AdrenalHealth, Source.Warrior, BuffStackType.Stacking, 4, BuffClassification.Other, TraitImages.AdrenalHealth).WithBuilds(GW2Builds.October2022Balance),
    ];


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

    public static void ProcessGadgets(IReadOnlyList<AgentItem> players, CombatData combatData)
    {
        var playerAgents = new HashSet<AgentItem>(players);
        HashSet<AgentItem> strBanners = GetBannerAgents(combatData, BannerOfStrengthBuff, playerAgents),
            defBanners = GetBannerAgents(combatData, BannerOfDefenseBuff, playerAgents),
            disBanners = GetBannerAgents(combatData, BannerOfDisciplineBuff, playerAgents),
            tacBanners = GetBannerAgents(combatData, BannerOfTacticsBuff, playerAgents);
        //battleBanner = FindBattleStandards(buffData, playerAgents);
        var warriors = players.Where(x => x.BaseSpec == Spec.Warrior);
        var warriorsCount = warriors.Count();
        // if only one warrior, could only be that one
        if (warriorsCount == 1)
        {
            AgentItem warrior = warriors.First();
            ProfHelper.SetGadgetMaster(strBanners, warrior);
            ProfHelper.SetGadgetMaster(disBanners, warrior);
            ProfHelper.SetGadgetMaster(tacBanners, warrior);
            ProfHelper.SetGadgetMaster(defBanners, warrior);
            //SetBannerMaster(battleBanner, warrior.AgentItem);
        }
        else if (warriorsCount > 1)
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
