using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class RevenantHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryAssassinStanceSkill, LegendaryAssassinStanceBuff),
            new BuffGainCastFinder(LegendaryDemonStanceSkill, LegendaryDemonStanceBuff),
            new BuffGainCastFinder(LegendaryDwarfStanceSkill, LegendaryDwarfStanceBuff), 
            new BuffGainCastFinder(LegendaryCentaurStanceSkill, LegendaryCentaurStanceBuff), 
            new BuffGainCastFinder(ImpossibleOddsSkill, ImpossibleOddsBuff).UsingICD(500), 
            new BuffLossCastFinder(RelinquishPower, ImpossibleOddsBuff).UsingICD(500),
            new BuffGainCastFinder(VengefulHammersSkill, VengefulHammersBuff),
            new BuffLossCastFinder(ReleaseHammers, VengefulHammersBuff), 
            new BuffLossCastFinder(ResistTheDarkness, EmbraceTheDarkness),
            new DamageCastFinder(InvokingTorment, InvokingTorment)
                .WithBuilds(GW2Builds.February2020Balance)
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(CallOfTheAssassin, CallOfTheAssassin),
            new DamageCastFinder(CallOfTheDwarf, CallOfTheDwarf),
            new DamageCastFinder(CallOfTheDemon, CallOfTheDemon),
            new DamageCastFinder(LesserBanishEnchantment, LesserBanishEnchantment)
                .WithBuilds(GW2Builds.December2018Balance, GW2Builds.February2020Balance)
                .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTHealingCastFinder(CallOfTheCentaur, CallOfTheCentaur),
            new EffectCastFinder(ProjectTranquility, EffectGUIDs.RevenantTabletAutoHeal).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinderByDstFromMinion(VentarisWill, EffectGUIDs.RevenantTabletVentarisWill).UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinderByDstFromMinion(NaturalHarmony, EffectGUIDs.RevenantNaturalHarmony).UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinderFromMinion(PurifyingEssence, EffectGUIDs.RevenantPurifyingEssence).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinderFromMinion(EnergyExpulsion, EffectGUIDs.RevenantEnergyExpulsion).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinder(ProtectiveSolace, EffectGUIDs.RevenantProtectiveSolace).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == Spec.Revenant && evt.IsAroundDst && evt.Dst.IsSpecies(MinionID.VentariTablet)),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Retribution
            new BuffDamageModifierTarget(Weakness, "Dwarven Battle Training", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DwarvenBattleTraining, DamageModifierMode.All).WithBuilds(GW2Builds.December2018Balance),
            new BuffDamageModifier(Retaliation, "Vicious Reprisal", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.ViciousReprisal, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(Resolution, "Vicious Reprisal", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, BuffImages.ViciousReprisal, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            // Invocation
            new BuffDamageModifier(Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance, GW2Builds.August2022Balance),
            new BuffDamageModifier(Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifier(Fury, "Ferocious Aggression", "10% under fury", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Rising Tide", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Rising Tide", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Rising Tide", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.IsOverNinety, ByPresence, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance, GW2Builds.November2022Balance),
            new DamageLogDamageModifier("Rising Tide", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, ByPresence, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance).UsingApproximate(true),
            // Devastation
            new BuffDamageModifier(ViciousLacerations, "Vicious Lacerations", "3% per Stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, BuffImages.ViciousLacerations, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance),
            new BuffDamageModifier(ViciousLacerations, "Vicious Lacerations", "2% per Stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, BuffImages.ViciousLacerations, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
            new DamageLogDamageModifier("Unsuspecting Strikes", "25% if target hp > 80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.ViciousLacerations, (x,log) =>
            {
                double foeHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (foeHP < 0.0)
                {
                    return false;
                }
                return foeHP > 80.0;
            }, ByPresence, DamageModifierMode.PvE ).UsingApproximate(true).WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021BalanceHotFix),
            new DamageLogDamageModifier("Unsuspecting Strikes", "20% if target hp > 80%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.ViciousLacerations, (x,log) =>
            {
                double foeHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (foeHP < 0.0)
                {
                    return false;
                }
                return foeHP > 80.0;
            }, ByPresence, DamageModifierMode.PvE ).UsingApproximate(true).WithBuilds(GW2Builds.May2021BalanceHotFix),
            new DamageLogDamageModifier("Unsuspecting Strikes", "10% if target hp > 80%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.ViciousLacerations, (x,log) =>
            {
                double foeHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (foeHP < 0.0)
                {
                    return false;
                }
                return foeHP > 80.0;
            }, ByPresence, DamageModifierMode.sPvPWvW ).UsingApproximate(true).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Vulnerability, "Targeted Destruction", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, BuffImages.TargetedDestruction, DamageModifierMode.All).WithBuilds(GW2Builds.March2019Balance),
            new BuffDamageModifierTarget(Vulnerability, "Targeted Destruction", "10.0% if vuln", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.TargetedDestruction, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance, GW2Builds.March2019Balance),
            new BuffDamageModifierTarget(Vulnerability, "Targeted Destruction", "7.0% if vuln", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.TargetedDestruction, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
            new DamageLogDamageModifier("Swift Termination", "20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.SwiftTermination, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Vengeful Hammers", VengefulHammersBuff, Source.Revenant, BuffClassification.Other, BuffImages.VengefulHammers),
            new Buff("Rite of the Great Dwarf", RiteOfTheGreatDwarf, Source.Revenant, BuffClassification.Defensive, BuffImages.RiteOfTheGreatDwarf),
            new Buff("Rite of the Great Dwarf (Traited)", RiteOfTheGreatDwarfTraited, Source.Revenant, BuffClassification.Defensive, BuffImages.RiteOfTheGreatDwarf),
            new Buff("Embrace the Darkness", EmbraceTheDarkness, Source.Revenant, BuffClassification.Other, BuffImages.EmbraceTheDarkness),
            new Buff("Enchanted Daggers", EnchantedDaggers, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.EnchantedDaggers),
            new Buff("Phase Traversal", PhaseTraversal, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PhaseTraversal),
            new Buff("Tranquil", Tranquil, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.ProjectTranquility),
            new Buff("Impossible Odds", ImpossibleOddsBuff, Source.Revenant, BuffClassification.Other, BuffImages.ImpossibleOdds),
            new Buff("Jade", Jade, Source.Revenant, BuffClassification.Other, BuffImages.Stun),
            new Buff("Legendary Centaur Stance", LegendaryCentaurStanceBuff, Source.Revenant, BuffClassification.Other, BuffImages.LegendaryCentaurStance),
            new Buff("Legendary Dwarf Stance", LegendaryDwarfStanceBuff, Source.Revenant, BuffClassification.Other, BuffImages.LegendaryDwarfStance),
            new Buff("Legendary Demon Stance", LegendaryDemonStanceBuff, Source.Revenant, BuffClassification.Other, BuffImages.LegendaryDemonStance),
            new Buff("Legendary Assassin Stance", LegendaryAssassinStanceBuff, Source.Revenant, BuffClassification.Other, BuffImages.LegendaryAssassinStance),
            new Buff("Crystal Hibernation", CrystalHibernation, Source.Revenant, BuffClassification.Other, BuffImages.CrystalHibernation).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Imperial Guard", ImperialGuard, Source.Revenant, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.ImperialGuard).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            // Traits
            new Buff("Vicious Lacerations", ViciousLacerations, Source.Revenant, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.ViciousLacerations).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new Buff("Assassin's Presence", AssassinsPresence, Source.Revenant, BuffClassification.Offensive, BuffImages.AssassinsPresence).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Expose Defenses", ExposeDefenses, Source.Revenant, BuffClassification.Other, BuffImages.MutilateDefenses),
            new Buff("Invoking Harmony", InvokingHarmony, Source.Revenant, BuffClassification.Other, BuffImages.InvokingHarmony),
            new Buff("Unyielding Devotion", UnyieldingDevotion, Source.Revenant, BuffClassification.Other, BuffImages.UnyieldingDevotion).WithBuilds(GW2Builds.April2019Balance, GW2Builds.EndOfLife),
            new Buff("Selfless Amplification", SelflessAmplification, Source.Revenant, BuffClassification.Other, BuffImages.SelflessAmplification),
            new Buff("Battle Scars", BattleScars, Source.Revenant, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.ThrillOfCombat).WithBuilds(GW2Builds.February2020Balance, GW2Builds.EndOfLife),
            new Buff("Steadfast Rejuvenation", SteadfastRejuvenation, Source.Revenant, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.SteadfastRejuvenation),
        };

        private static readonly HashSet<long> _legendSwaps = new HashSet<long>
        {
            LegendaryAssassinStanceSkill, // Assassin
            LegendaryDemonStanceSkill, // Demon
            LegendaryDwarfStanceSkill, // Dwarf
            LegendaryCentaurStanceSkill, // Centaur
            LegendaryDragonStanceSkill, // Dragon
            LegendaryRenegadeStanceSkill, // Renegade
            LegendaryAllianceStanceSkill, // Alliance
            //LegendaryAllianceStanceUWSkill, // Alliance (UW)
        };

        public static bool IsLegendSwap(long id)
        {
            return _legendSwaps.Contains(id);
        }

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.VentariTablet
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

        public static void ProcessGadgets(IReadOnlyList<Player> players, CombatData combatData, AgentData agentData)
        {
            if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantTabletAutoHeal, out IReadOnlyList<EffectEvent> tabletHealEffectEvents))
            {
                var allTablets = new HashSet<AgentItem>(tabletHealEffectEvents.Select(x => x.Src));
                foreach (AgentItem tablet in allTablets)
                {
                    tablet.OverrideType(AgentItem.AgentType.NPC);
                    tablet.OverrideID(MinionID.VentariTablet);
                }
                if (allTablets.Any())
                {
                    agentData.Refresh();
                }
            }
        }

    }
}
