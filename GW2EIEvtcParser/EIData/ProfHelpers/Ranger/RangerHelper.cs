using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class RangerHelper
    {

        private static readonly HashSet<int> SpiritIDs = new HashSet<int>()
        {
            (int)MinionID.FrostSpirit,
            (int)MinionID.StoneSpirit,
            (int)MinionID.StormSpirit,
            (int)MinionID.SunSpirit,
            (int)MinionID.WaterSpirit,
            (int)MinionID.SpiritOfNatureRenewal,
        };

        private static HashSet<int> JuvenilePetIDs = new HashSet<int>()
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
            (int)MinionID.JuvenileCarrionDevourer,
            (int)MinionID.JuvenileCaveSpider,
            (int)MinionID.JuvenileCheetah,
            (int)MinionID.JuvenileEagle,
            (int)MinionID.JuvenileEletricWywern,
            (int)MinionID.JuvenileFangedIboga,
            (int)MinionID.JuvenileFernHound,
            (int)MinionID.JuvenileFireWywern,
            (int)MinionID.JuvenileForestSpider,
            (int)MinionID.JuvenileHawk,
            (int)MinionID.JuvenileIceDrake,
            (int)MinionID.JuvenileJacaranda,
            (int)MinionID.JuvenileJaguar,
            (int)MinionID.JuvenileJungleSpider,
            (int)MinionID.JuvenileJungleStalker,
            (int)MinionID.JuvenileKrytanDrakehound,
            (int)MinionID.JuvenileLashtailDevourer,
            (int)MinionID.JuvenileLynx,
            (int)MinionID.JuvenileMarshDrake,
            (int)MinionID.JuvenileMurellow,
            (int)MinionID.JuvenileOwl,
            (int)MinionID.JuvenilePhoenix,
            (int)MinionID.JuvenilePig,
            (int)MinionID.JuvenilePinkMoa,
            (int)MinionID.JuvenilePolarBear,
            (int)MinionID.JuvenileRainbowJellyfish,
            (int)MinionID.JuvenileRaven,
            (int)MinionID.JuvenileRedJellyfish,
            (int)MinionID.JuvenileRedMoa,
            (int)MinionID.JuvenileReefDrake,
            (int)MinionID.JuvenileRiverDrake,
            (int)MinionID.JuvenileRockGazelle,
            (int)MinionID.JuvenileSalamanderDrake,
            (int)MinionID.JuvenileSandLion,
            (int)MinionID.JuvenileShark,
            (int)MinionID.JuvenileSiamoth,
            (int)MinionID.JuvenileSiegeTurtle,
            (int)MinionID.JuvenileSmokescale,
            (int)MinionID.JuvenileSnowLeopard,
            (int)MinionID.JuvenileTiger,
            (int)MinionID.JuvenileWallow,
            (int)MinionID.JuvenileWarthog,
            (int)MinionID.JuvenileWhiptailDevourer,
            (int)MinionID.JuvenileWhiteMoa,
            (int)MinionID.JuvenileWhiteRaven,
            (int)MinionID.JuvenileWhiteTiger,
            (int)MinionID.JuvenileWolf,
            (int)MinionID.JuvenileHyena,
            (int)MinionID.JuvenileAetherHunter,
            (int)MinionID.JuvenileSkyChakStriker,
            (int)MinionID.JuvenileSpinegazer,
        };

        private static bool IsJuvenilePetID(int id)
        {
            return JuvenilePetIDs.Contains(id);
        }

        internal static bool IsJuvenilePet(AgentItem agentItem)
        {
            if (agentItem.Type == AgentItem.AgentType.Gadget)
            {
                return false;
            }
            return IsJuvenilePetID(agentItem.ID);
        }

        internal static bool IsKnownMinionID(int id)
        {
            return IsJuvenilePetID(id) || SpiritIDs.Contains(id);
        }

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(12573,12573), // Hunter's Shot
            //new DamageCastFinder(12507,12507), // Crippling Shot
            new BuffGainCastFinder(SicEmSkill, SicEmBuff).WithMinions(true),
            new BuffGainCastFinder(SicEmSkill, SicEmPvPBuff).WithMinions(true),
            new BuffGainCastFinder(SignetOfStone, SignetOfStoneActive).UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant), // Signet of Stone
            new BuffGainCastFinder(LesserSignetOfStone, SignetOfStoneActive).UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 5000) < ServerDelayConstant).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait), // Lesser Signet of Stone
            new BuffGainCastFinder(SharpeningStonesSkill, SharpeningStonesBuff),
            new BuffGainCastFinder(QuickDraw, QuickDraw).UsingAfterWeaponSwap(true).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTHealingCastFinder(WindborneNotes, WindborneNotes).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTHealingCastFinder(InvigoratingBond, InvigoratingBond).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            // TODO evasive purity
            new EXTBarrierCastFinder(ProtectMe, ProtectMe),
            new BuffGiveCastFinder(GuardSkill, GuardBuff).UsingChecker(((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant)),
            new BuffGiveCastFinder(LesserGuardSkill, GuardBuff).UsingChecker(((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 4000) < ServerDelayConstant)).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new BuffGiveCastFinder(SearchAndRescueSkill, SearchAndRescueBuff).UsingICD(1100).UsingNotAccurate(true),
            new EffectCastFinder(LightningReflexes, EffectGUIDs.RangerLightningReflexes).UsingSrcBaseSpecChecker(Spec.Ranger),
            new EffectCastFinderByDst(QuickeningZephyr, EffectGUIDs.RangerQuickeningZephyr).UsingDstBaseSpecChecker(Spec.Ranger),
            new EffectCastFinderByDst(SignetOfRenewalSkill, EffectGUIDs.RangerSignetOfRenewal).UsingDstBaseSpecChecker(Spec.Ranger),
            new EffectCastFinderByDst(SignetOfTheHuntSkill, EffectGUIDs.RangerSignetOfTheHunt).UsingDstBaseSpecChecker(Spec.Ranger),
            new MinionSpawnCastFinder(RangerPetSpawned, JuvenilePetIDs.ToList()).UsingNotAccurate(true),
        };


        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Skills
            new BuffOnActorDamageModifier(SicEmBuff, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.SicEm, DamageModifierMode.PvE).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffDataByIDByDst(SicEmBuff, src).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(SicEmBuff, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.SicEm, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffDataByIDByDst(SicEmBuff, src).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(SicEmBuff, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.SicEm, DamageModifierMode.All).UsingChecker( (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffDataByIDByDst(SicEmBuff, src).Where(y => y is BuffApplyEvent).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.May2021Balance),
            // Marksmanship
            new DamageLogDamageModifier("Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SteadyFocus, (x, log) => {
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
            }, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.July2018Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SteadyFocus, (x, log) => {
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
            }, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Farsighted (<= 600)", "10% with weapon skills below 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SteadyFocus, (x, log) => {
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
            }, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SteadyFocus, (x, log) => {
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
            }, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.July2018Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Farsighted (> 600)", "15% with weapon skills above 600 range", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SteadyFocus, (x, log) => {
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
            }, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SteadyFocus, (x, log) => {
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
            }, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new BuffOnFoeDamageModifier(new long[] { Stun, Taunt, Daze, Crippled, Fear, Immobile, Chilled }, "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.PredatorsOnslaught, DamageModifierMode.All).UsingApproximate(true),
            // Skirmishing
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking , DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2022Balance),
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking , DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.June2022Balance),
            new DamageLogDamageModifier("Hunter's Tactics", "15% while flanking", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking , DamageModifierMode.PvE).WithBuilds(GW2Builds.June2022Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Hunter's Tactics", "15% while flanking or against defiant", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking || x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None , DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023Balance),
            new BuffOnActorDamageModifier(LightOnYourFeet, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.LightOnYourFeet, DamageModifierMode.All),
            // Nature Magic
            // We can't check buffs on minions yet
            new BuffOnActorDamageModifier(NumberOfBoons, "Bountiful Hunter", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Ranger, ByStack, BuffImages.BountifulHunter, DamageModifierMode.All),
            new BuffOnActorDamageModifier(FrostSpiritBuff, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.FrostSpirit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new DamageLogDamageModifier("Survival Instincts (Outgoing)","10% if hp >=50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SurvivalInstincts, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary)
                .UsingApproximate(true),
            new BuffOnActorDamageModifier(ForceOfNature, "Force of Nature", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.ForceOfNature, DamageModifierMode.All)
                .WithBuilds(GW2Builds.February2024NewWeapons, GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(ForceOfNature, "Force of Nature", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.ForceOfNature, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary, GW2Builds.May2024LonelyTowerFractalRelease),
            new BuffOnActorDamageModifier(ForceOfNature, "Force of Nature", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.ForceOfNature, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary, GW2Builds.May2024LonelyTowerFractalRelease),
            new BuffOnActorDamageModifier(ForceOfNature, "Force of Nature", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.ForceOfNature, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2024LonelyTowerFractalRelease),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(Regeneration, "Oakheart Salve", "-5% under regeneration", DamageSource.NoPets, -5.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.OakheartSalve, DamageModifierMode.All),
            new DamageLogDamageModifier("Survival Instincts (Incoming)","10% if hp <= 50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.SurvivalInstincts, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Counterattack", Counterattack, Source.Ranger, BuffClassification.Other, BuffImages.Counterattack),
            // Signets
            new Buff("Signet of Renewal", SignetOfRenewalBuff, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfRenewal),
            new Buff("Signet of Stone", SignetOfStoneBuff, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfStone),
            new Buff("Signet of Stone (Pet)", SignetOfStonePetBuff, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfStone), // not present even on soulbeast?
            new Buff("Signet of Stone (Active)", SignetOfStoneActive, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfStone),
            new Buff("Signet of the Wild", SignetOfTheWild, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfTheWild),
            new Buff("Signet of the Wild (Pet)", SignetOfTheWildPet, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfTheWild),
            new Buff("Signet of the Hunt", SignetOfTheHuntBuff, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfTheHunt),
            new Buff("Signet of the Hunt (Pet)", SignetOfTheHuntPetBuff, Source.Ranger, BuffClassification.Other, BuffImages.SignetOfTheHunt),
            // Spirits
            // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, BuffImages.WaterSpirit),
            new Buff("Frost Spirit", FrostSpiritOld, Source.Ranger, BuffClassification.Offensive, BuffImages.FrostSpirit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2018Balance),
            new Buff("Sun Spirit", SunSpiritOld, Source.Ranger, BuffClassification.Offensive, BuffImages.SunSpirit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2018Balance),
            new Buff("Stone Spirit", StoneSpiritOld, Source.Ranger, BuffClassification.Defensive, BuffImages.StoneSpirit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2018Balance),
            //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, BuffImages.StormSpirit),
            // Spirits reworked
            new Buff("Water Spirit", WaterSpiritBuff, Source.Ranger, BuffClassification.Defensive, BuffImages.WaterSpirit).WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new Buff("Frost Spirit", FrostSpiritBuff, Source.Ranger, BuffClassification.Offensive, BuffImages.FrostSpirit).WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new Buff("Sun Spirit", SunSpiritBuff, Source.Ranger, BuffClassification.Offensive, BuffImages.SunSpirit).WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new Buff("Stone Spirit", StoneSpiritBuff, Source.Ranger, BuffClassification.Defensive, BuffImages.StoneSpirit).WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new Buff("Storm Spirit", StormSpiritBuff, Source.Ranger, BuffClassification.Support, BuffImages.StormSpirit).WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            // Skills
            new Buff("Attack of Opportunity", AttackOfOpportunity, Source.Ranger, BuffClassification.Other, BuffImages.MomentOfClarity),
            new Buff("Call of the Wild", CallOfTheWild, Source.Ranger, BuffClassification.Other, BuffImages.CallOfTheWild).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new Buff("Call of the Wild", CallOfTheWild, Source.Ranger, BuffStackType.Stacking, 3, BuffClassification.Other, BuffImages.CallOfTheWild).WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
            new Buff("Strength of the Pack!", StrengthOfThePack, Source.Ranger, BuffClassification.Other, BuffImages.StrengthOfThePack),
            new Buff("Sic 'Em!", SicEmBuff, Source.Ranger, BuffClassification.Other, BuffImages.SicEm),
            new Buff("Sic 'Em! (PvP)", SicEmPvPBuff, Source.Ranger, BuffClassification.Other, BuffImages.SicEm),
            new Buff("Sharpening Stones", SharpeningStonesBuff, Source.Ranger, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SharpeningStone),
            new Buff("Sharpen Spines", SharpenSpinesBuff, Source.Ranger, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SharpenSpines),
            new Buff("Guard!", GuardBuff, Source.Ranger, BuffClassification.Defensive, BuffImages.Guard),
            new Buff("Clarion Bond", ClarionBond, Source.Ranger, BuffClassification.Other, BuffImages.ClarionBond),
            new Buff("Search and Rescue!", SearchAndRescueBuff, Source.Ranger, BuffClassification.Support, BuffImages.SearchAndRescue),
            new Buff("Ancestral Grace", AncestralGraceBuff, Source.Ranger, BuffClassification.Other, BuffImages.AncestralGrace).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            // Traits
            new Buff("Spotter", Spotter, Source.Ranger, BuffClassification.Offensive, BuffImages.Spotter).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Opening Strike", OpeningStrike, Source.Ranger, BuffClassification.Other, BuffImages.OpeningStrike),
            new Buff("Quick Draw", QuickDraw, Source.Ranger, BuffClassification.Other, BuffImages.QuickDraw),
            new Buff("Light on your Feet", LightOnYourFeet, Source.Ranger, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.LightOnYourFeet),
            new Buff("Poison Master", PoisonMasterBuff, Source.Ranger, BuffClassification.Other, BuffImages.PoisonMaster),
            // Mace
            new Buff("Force of Nature", ForceOfNature, Source.Ranger, BuffClassification.Other, BuffImages.ForceOfNature)
                .WithBuilds(GW2Builds.February2024NewWeapons),
            new Buff("Nature's Strength", NaturesStrength, Source.Ranger, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.NaturesStrength)
                .WithBuilds(GW2Builds.February2024NewWeapons),
            new Buff("Tapped Out", TappedOut, Source.Ranger, BuffClassification.Other, BuffImages.TappedOut)
                .WithBuilds(GW2Builds.February2024NewWeapons),
        };

        public static void ProcessGadgets(IReadOnlyList<Player> players, CombatData combatData)
        {
            var playerAgents = new HashSet<AgentItem>(players.Select(x => x.AgentItem));
            // entangle works fine already
            HashSet<AgentItem> jacarandaEmbraces = GetOffensiveGadgetAgents(combatData, JacarandasEmbraceMinion, playerAgents);
            HashSet<AgentItem> blackHoles = GetOffensiveGadgetAgents(combatData, BlackHoleMinion, playerAgents);
            var rangers = players.Where(x => x.BaseSpec == Spec.Ranger).ToList();
            // if only one ranger, could only be that one
            if (rangers.Count == 1)
            {
                Player ranger = rangers[0];
                SetGadgetMaster(jacarandaEmbraces, ranger.AgentItem);
                SetGadgetMaster(blackHoles, ranger.AgentItem);
            }
            else if (rangers.Count > 1)
            {
                AttachMasterToGadgetByCastData(combatData, jacarandaEmbraces, new List<long> { JacarandasEmbraceSkill }, 1000);
                AttachMasterToGadgetByCastData(combatData, blackHoles, new List<long> { BlackHoleSkill }, 1000);
            }
        }

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Ranger;

            // Siege Turtle Hunker Down
            if (log.CombatData.TryGetEffectEventsByMasterWithGUID(player.AgentItem, EffectGUIDs.RangerHunkerDown, out IReadOnlyList<EffectEvent> hunkerDowns))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, HunkerDownPetTurtle, SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in hunkerDowns)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectHunkerDown);
                }
            }
            // Barrage
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerBarrage1, out IReadOnlyList<EffectEvent> barrages))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, Barrage, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in barrages)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 600); // ~600ms interval
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, ParserIcons.EffectBarrage);
                }
            }
            // Bonfire
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerBonfire, out IReadOnlyList<EffectEvent> bonfires))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, Bonfire, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in bonfires)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 8000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectBonfire);
                }
            }
            // Frost Trap
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerFrostTrap, out IReadOnlyList<EffectEvent> frostTraps))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, FrostTrap, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in frostTraps)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectFrostTrap);
                }
            }
            // Flame Trap
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerFlameTrap, out IReadOnlyList<EffectEvent> flameTraps))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, FlameTrap, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in flameTraps)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectFlameTrap);
                }
            }
            // Viper's Nest
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerVipersNest, out IReadOnlyList<EffectEvent> vipersNests))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, VipersNest, SkillModeCategory.ShowOnSelect);
                foreach (EffectEvent effect in vipersNests)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectVipersNest);
                }
            }
            // Spike Trap
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerSpikeTrap, out IReadOnlyList<EffectEvent> spikeTraps))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, SpikeTrap, SkillModeCategory.ShowOnSelect | SkillModeCategory.CC);
                foreach (EffectEvent effect in spikeTraps)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 2000); // roughly time displayed ingame
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, ParserIcons.EffectSpikeTrap);
                }
            }
            // Sublime Conversion
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DruidSublimeConversion2, out IReadOnlyList<EffectEvent> sublimeConversions2))
            {
                var skill = new SkillModeDescriptor(player, Spec.Ranger, SublimeConversion, SkillModeCategory.ProjectileManagement);
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DruidSublimeConversion1, out IReadOnlyList<EffectEvent> sublimeConversions1))
                {
                    foreach (EffectEvent effect in sublimeConversions1)
                    {
                        if (sublimeConversions2.Any(x => Math.Abs(x.Time - effect.Time) < ServerDelayConstant))
                        {
                            (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                            var connector = new PositionConnector(effect.Position);
                            var rotationConnector = new AngleConnector(effect.Rotation.Z);
                            replay.Decorations.Add(new RectangleDecoration(400, 60, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector).UsingSkillMode(skill));
                            replay.Decorations.Add(new IconDecoration(ParserIcons.EffectSublimeConversion, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                        }
                    }
                }
            }
        }
    }
}
