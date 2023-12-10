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
using System;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;

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
            new BuffGainCastFinder(ImpossibleOddsSkill, ImpossibleOddsBuff)
                .UsingICD(500), 
            new BuffLossCastFinder(RelinquishPower, ImpossibleOddsBuff)
                .UsingICD(500),
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
            new EffectCastFinder(ProjectTranquility, EffectGUIDs.RevenantTabletAutoHeal)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinderByDstFromMinion(VentarisWill, EffectGUIDs.RevenantTabletVentarisWill)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.IsSpecies(MinionID.VentariTablet)),
            new EffectCastFinderByDstFromMinion(NaturalHarmony, EffectGUIDs.RevenantNaturalHarmony)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.IsSpecies(MinionID.VentariTablet))
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new EffectCastFinderFromMinion(NaturalHarmony, EffectGUIDs.RevenantNaturalHarmony)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet))
                .WithBuilds( GW2Builds.June2022Balance),
            new EffectCastFinderFromMinion(PurifyingEssence, EffectGUIDs.RevenantPurifyingEssence)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet))
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new EffectCastFinder(PurifyingEssence, EffectGUIDs.RevenantPurifyingEssence)
                .UsingSrcBaseSpecChecker(Spec.Revenant)
                .WithBuilds(GW2Builds.June2022Balance),
            new EffectCastFinderFromMinion(EnergyExpulsion, EffectGUIDs.RevenantEnergyExpulsion)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.IsSpecies(MinionID.VentariTablet))
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new EffectCastFinder(EnergyExpulsion, EffectGUIDs.RevenantEnergyExpulsion)
                .UsingSrcBaseSpecChecker(Spec.Revenant)
                .WithBuilds(GW2Builds.June2022Balance),
            new EffectCastFinder(ProtectiveSolaceSkill, EffectGUIDs.RevenantProtectiveSolace)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == Spec.Revenant && evt.IsAroundDst && evt.Dst.IsSpecies(MinionID.VentariTablet)),
        };


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Retribution
            new BuffOnFoeDamageModifier(Weakness, "Dwarven Battle Training", "10% on weakened target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DwarvenBattleTraining, DamageModifierMode.All).WithBuilds(GW2Builds.December2018Balance),
            new BuffOnActorDamageModifier(Retaliation, "Vicious Reprisal", "10% under retaliation", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.ViciousReprisal, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(Resolution, "Vicious Reprisal", "10% under resolution", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, BuffImages.ViciousReprisal, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            // Invocation
            new BuffOnActorDamageModifier(Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance, GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(Fury, "Ferocious Aggression", "7% under fury", DamageSource.NoPets, 7.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(Fury, "Ferocious Aggression", "10% under fury", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Revenant, ByPresence, BuffImages.FerociousAggression, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Rising Tide", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.IsOverNinety, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Rising Tide", "7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.IsOverNinety, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new DamageLogDamageModifier("Rising Tide", "10% if hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.IsOverNinety, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance, GW2Builds.November2022Balance),
            new DamageLogDamageModifier("Rising Tide", "10% if hp >=75%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.RisingTide, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 75.0, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance).UsingApproximate(true),
            // Devastation
            new BuffOnActorDamageModifier(ViciousLacerations, "Vicious Lacerations", "3% per Stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, BuffImages.ViciousLacerations, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance, GW2Builds.February2020Balance),
            new BuffOnActorDamageModifier(ViciousLacerations, "Vicious Lacerations", "2% per Stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, BuffImages.ViciousLacerations, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
            new DamageLogDamageModifier("Unsuspecting Strikes", "25% if target hp > 80%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.ViciousLacerations, (x,log) =>
            {
                double foeHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (foeHP < 0.0)
                {
                    return false;
                }
                return foeHP > 80.0;
            }, DamageModifierMode.PvE ).UsingApproximate(true).WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021BalanceHotFix),
            new DamageLogDamageModifier("Unsuspecting Strikes", "20% if target hp > 80%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.ViciousLacerations, (x,log) =>
            {
                double foeHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (foeHP < 0.0)
                {
                    return false;
                }
                return foeHP > 80.0;
            }, DamageModifierMode.PvE ).UsingApproximate(true).WithBuilds(GW2Builds.May2021BalanceHotFix),
            new DamageLogDamageModifier("Unsuspecting Strikes", "10% if target hp > 80%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.ViciousLacerations, (x,log) =>
            {
                double foeHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (foeHP < 0.0)
                {
                    return false;
                }
                return foeHP > 80.0;
            }, DamageModifierMode.sPvPWvW ).UsingApproximate(true).WithBuilds(GW2Builds.February2020Balance),
            new BuffOnFoeDamageModifier(Vulnerability, "Targeted Destruction", "0.5% per stack vuln", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Revenant, ByStack, BuffImages.TargetedDestruction, DamageModifierMode.All).WithBuilds(GW2Builds.March2019Balance),
            new BuffOnFoeDamageModifier(Vulnerability, "Targeted Destruction", "10.0% if vuln", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.TargetedDestruction, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance, GW2Builds.March2019Balance),
            new BuffOnFoeDamageModifier(Vulnerability, "Targeted Destruction", "7.0% if vuln", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.TargetedDestruction, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
            new DamageLogDamageModifier("Swift Termination", "20% if target <50%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.SwiftTermination, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(RiteOfTheGreatDwarf, "Rite of the Great Dwarf (condition)", "-50%", DamageSource.All, -50.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
            new BuffOnActorDamageModifier(RiteOfTheGreatDwarf, "Rite of the Great Dwarf (strike)", "-50%", DamageSource.All, -50.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
            new BuffOnActorDamageModifier(RiteOfTheGreatDwarfAncientEcho, "Rite of the Great Dwarf (Ancient Echo)", "-50%", DamageSource.All, -50.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.RiteOfTheGreatDwarf, DamageModifierMode.All),
            new BuffOnActorDamageModifier(Resistance, "Demonic Resistance", "-20%", DamageSource.All, -20.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DemonicResistance, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
            new BuffOnActorDamageModifier(Resistance, "Demonic Resistance", "-20%", DamageSource.All, -20.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DemonicResistance, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new BuffOnActorDamageModifier(Resistance, "Demonic Resistance", "-10%", DamageSource.All, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DemonicResistance, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2020Balance),
            new BuffOnActorDamageModifier(Resistance, "Demonic Resistance", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DemonicResistance, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(Resistance, "Demonic Resistance", "-20%", DamageSource.All, -20.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DemonicResistance, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new DamageLogDamageModifier("Close Quarters", "10% from foes beyond 360 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Revenant, BuffImages.CloseQuarters, (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) >= 360.0;
            }, DamageModifierMode.All).UsingApproximate(true),
            new BuffOnActorDamageModifier(Stability, "Determined Resolution", "-15% under stability", DamageSource.All, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DeterminedResolution, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffOnActorDamageModifier(Vigor, "Determined Resolution", "-15 under vigor%", DamageSource.All, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DeterminedResolution, DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(Resolution, "Determined Resolution", "-15% under resolution", DamageSource.All, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DeterminedResolution, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance, GW2Builds.May2021BalanceHotFix),
            new BuffOnActorDamageModifier(Resolution, "Determined Resolution", "-10% under resolution", DamageSource.All, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.DeterminedResolution, DamageModifierMode.All).WithBuilds(GW2Builds.May2021BalanceHotFix),
            new BuffOnActorDamageModifier(UnyieldingSpirit, "Unyielding Spirit", "-15%", DamageSource.All, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.UnyieldingSpirit, DamageModifierMode.All).WithBuilds(GW2Builds.April2019Balance, GW2Builds.July2022FractalInstabilitiesRework),
            new BuffOnActorDamageModifier(UnyieldingSpirit, "Unyielding Spirit", "-15%", DamageSource.All, -15.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.UnyieldingSpirit, DamageModifierMode.PvE).WithBuilds(GW2Builds.July2022FractalInstabilitiesRework),
            new BuffOnActorDamageModifier(UnyieldingSpirit, "Unyielding Spirit", "-10%", DamageSource.All, -10.0, DamageType.Strike, DamageType.All, Source.Revenant, ByPresence, BuffImages.UnyieldingSpirit, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.July2022FractalInstabilitiesRework),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Vengeful Hammers", VengefulHammersBuff, Source.Revenant, BuffClassification.Other, BuffImages.VengefulHammers),
            new Buff("Rite of the Great Dwarf", RiteOfTheGreatDwarf, Source.Revenant, BuffClassification.Defensive, BuffImages.RiteOfTheGreatDwarf),
            new Buff("Rite of the Great Dwarf (Ancient Echo)", RiteOfTheGreatDwarfAncientEcho, Source.Revenant, BuffClassification.Defensive, BuffImages.RiteOfTheGreatDwarf),
            new Buff("Embrace the Darkness", EmbraceTheDarkness, Source.Revenant, BuffClassification.Other, BuffImages.EmbraceTheDarkness),
            new Buff("Enchanted Daggers", EnchantedDaggers, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.EnchantedDaggers),
            new Buff("Phase Traversal", PhaseTraversal, Source.Revenant, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PhaseTraversal),
            new Buff("Tranquil", Tranquil, Source.Revenant, BuffClassification.Other, BuffImages.ProjectTranquility),
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
            new Buff("Unyielding Spirit", UnyieldingSpirit, Source.Revenant, BuffClassification.Other, BuffImages.UnyieldingSpirit).WithBuilds(GW2Builds.April2019Balance, GW2Builds.EndOfLife),
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
            var allTablets = new HashSet<AgentItem>();
            if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantTabletAutoHeal, out IReadOnlyList<EffectEvent> tabletHealEffectEvents))
            {
                allTablets.UnionWith(tabletHealEffectEvents.Select(x => x.Src));        
            }
            if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantTabletVentarisWill, out IReadOnlyList<EffectEvent> ventarisWillEffectEvents))
            {
                allTablets.UnionWith(ventarisWillEffectEvents.Where(x => x.IsAroundDst).Select(x => x.Dst));
            }
            if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.RevenantProtectiveSolace, out IReadOnlyList<EffectEvent> protectiveSolaceEffectEvents))
            {
                allTablets.UnionWith(protectiveSolaceEffectEvents.Where(x => x.IsAroundDst).Select(x => x.Dst));
            }
            foreach (AgentItem tablet in allTablets)
            {
                tablet.OverrideType(AgentItem.AgentType.NPC);
                tablet.OverrideID(MinionID.VentariTablet);
                tablet.OverrideName("Ventari's Tablet");
            }
            if (allTablets.Any())
            {
                agentData.Refresh();
            }
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Revenant;

            // Inspiring Reinforcement
            var inspiringReinforcementSkill = new SkillModeDescriptor(player, Spec.Revenant, InspiringReinforcement, SkillModeCategory.ImportantBuffs);
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantInspiringReinforcementPart, out IReadOnlyList<EffectEvent> inspiringReinforcementParts))
            {
                
                foreach (EffectEvent effect in inspiringReinforcementParts)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = new PositionConnector(effect.Position);
                    var rotationConnector = new AngleConnector(effect.Rotation.Z);
                    replay.Decorations.Add(new RectangleDecoration(240, 360, lifespan, Colors.DarkTeal.WithAlpha(0.1f).ToString(), connector).UsingRotationConnector(rotationConnector).UsingSkillMode(inspiringReinforcementSkill));
                }
            }
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantInspiringReinforcement, out IReadOnlyList<EffectEvent> inspiringReinforcements))
            {
                foreach (EffectEvent effect in inspiringReinforcements)
                {
                    (long, long) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 5000);
                    var connector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new Point3D(0, -420.0f), true); // 900 units in front, 60 behind
                    var rotationConnector = new AngleConnector(effect.Rotation.Z);
                    replay.Decorations.Add(new RectangleDecoration(240, 960, lifespan, color.WithAlpha(0.5f).ToString(), connector)
                        .UsingFilled(false)
                        .UsingRotationConnector(rotationConnector)
                        .UsingSkillMode(inspiringReinforcementSkill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectInspiringReinforcement, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector)
                        .UsingRotationConnector(rotationConnector)
                        .UsingSkillMode(inspiringReinforcementSkill));
                }
            }
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RevenantProtectiveSolace, out IReadOnlyList<EffectEvent> protectiveSolaceEffectEvents))
            {
                var skill = new SkillModeDescriptor(player, Spec.Revenant, ProtectiveSolaceSkill, SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in protectiveSolaceEffectEvents.Where(x => x.IsAroundDst))
                {
                    (long, long) lifespan = ProfHelper.ComputeDynamicEffectLifespan(log, effect, 0, effect.Dst, ProtectiveSolaceTabletBuff); // manually disabled or when no more ressources
                    var connector = new AgentConnector(effect.Dst);
                    replay.Decorations.Add(new CircleDecoration(240, lifespan, color.WithAlpha(0.2f).ToString(), connector)
                        .UsingSkillMode(skill));
                    replay.Decorations.Add(new IconDecoration(ParserIcons.EffectProtectiveSolace, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector)
                        .UsingSkillMode(skill));
                }
            }
        }
    }
}
