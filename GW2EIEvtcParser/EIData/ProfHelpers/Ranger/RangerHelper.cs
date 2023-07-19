using System;
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
    internal static class RangerHelper
    {

        private static HashSet<int> NonSpiritMinions = new HashSet<int>()
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
            (int)MinionID.JuvenileHyena
        };

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(12573,12573), // Hunter's Shot
            //new DamageCastFinder(12507,12507), // Crippling Shot
            new BuffGainWithMinionsCastFinder(SicEmSkill, SicEmBuff),
            new BuffGainWithMinionsCastFinder(SicEmSkill, SicEmPvPBuff),
            new BuffGainCastFinder(SignetOfStone, SignetOfStoneActive).UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant), // Signet of Stone
            new BuffGainCastFinder(LesserSignetOfStone, SignetOfStoneActive).UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 5000) < ServerDelayConstant).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait), // Lesser Signet of Stone
            new BuffGainCastFinder(SharpeningStonesSkill, SharpeningStonesBuff),
            new BuffGainCastFinder(QuickDraw, QuickDraw).UsingAfterWeaponSwap(true).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTHealingCastFinder(WindborneNotes, WindborneNotes).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTHealingCastFinder(InvigoratingBond, InvigoratingBond).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new EXTBarrierCastFinder(ProtectMe, ProtectMe),
            new BuffGiveCastFinder(GuardSkill, GuardBuff).UsingChecker(((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant)),
            new BuffGiveCastFinder(LesserGuardSkill, GuardBuff).UsingChecker(((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 4000) < ServerDelayConstant)).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new BuffGiveCastFinder(SearchAndRescueSkill, SearchAndRescueBuff).UsingICD(1100).UsingNotAccurate(true),
            new EffectCastFinder(LightningReflexes, EffectGUIDs.RangerLightningReflexes).UsingSrcBaseSpecChecker(Spec.Ranger),
            new EffectCastFinderByDst(QuickeningZephyr, EffectGUIDs.RangerQuickeningZephyr).UsingDstBaseSpecChecker(Spec.Ranger),
            new EffectCastFinderByDst(SignetOfRenewalSkill, EffectGUIDs.RangerSignetOfRenewal).UsingDstBaseSpecChecker(Spec.Ranger),
            new EffectCastFinderByDst(SignetOfTheHuntSkill, EffectGUIDs.RangerSignetOfTheHunt).UsingDstBaseSpecChecker(Spec.Ranger),
            new MinionSpawnCastFinder(RangerPetSpawned, NonSpiritMinions.ToList()).UsingNotAccurate(true),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Skills
            new BuffDamageModifier(SicEmBuff, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.SicEm, DamageModifierMode.PvE).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(SicEmBuff).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(SicEmBuff, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.SicEm, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(SicEmBuff).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(SicEmBuff, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.SicEm, DamageModifierMode.All).UsingChecker( (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(SicEmBuff).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
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
            }, ByPresence, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.July2018Balance, GW2Builds.June2023Balance),
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
            }, ByPresence, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
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
            }, ByPresence, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
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
            }, ByPresence, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.July2018Balance, GW2Builds.June2023Balance),
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
            }, ByPresence, DamageModifierMode.PvE).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
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
            }, ByPresence, DamageModifierMode.sPvPWvW).UsingApproximate(true).WithBuilds(GW2Builds.June2023Balance),
            new BuffDamageModifierTarget(new long[] { Stun, Taunt, Daze, Crippled, Fear, Immobile, Chilled }, "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.PredatorsOnslaught, DamageModifierMode.All).UsingApproximate(true),
            // Skirmishing
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking , ByPresence, DamageModifierMode.All).WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2022Balance),
            new DamageLogDamageModifier("Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking , ByPresence, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.June2022Balance),
            new DamageLogDamageModifier("Hunter's Tactics", "15% while flanking", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking , ByPresence, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2022Balance, GW2Builds.June2023Balance),
            new DamageLogDamageModifier("Hunter's Tactics", "15% while flanking or against defiant", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, BuffImages.HuntersTactics, (x, log) => x.IsFlanking || x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None , ByPresence, DamageModifierMode.PvE).WithBuilds(GW2Builds.June2023Balance),
            new BuffDamageModifier(LightOnYourFeet, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, BuffImages.LightOnYourFeet, DamageModifierMode.All),
            // Nature Magic
            // We can't check buffs on minions yet
            new BuffDamageModifier(NumberOfBoons, "Bountiful Hunter", "1% per boon", DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Ranger, ByStack, BuffImages.BountifulHunter, DamageModifierMode.All),
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
            new Buff("Guard!", GuardBuff, Source.Ranger, BuffClassification.Other, BuffImages.Guard),
            new Buff("Clarion Bond", ClarionBond, Source.Ranger, BuffClassification.Other, BuffImages.ClarionBond),
            new Buff("Search and Rescue!", SearchAndRescueBuff, Source.Ranger, BuffClassification.Support, BuffImages.SearchAndRescue),
            new Buff("Ancestral Grace", AncestralGraceBuff, Source.Ranger, BuffClassification.Other, BuffImages.AncestralGrace).WithBuilds(GW2Builds.SOTOBetaAndSilentSurfNM),
            // Traits
            new Buff("Spotter", Spotter, Source.Ranger, BuffClassification.Offensive, BuffImages.Spotter).WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
            new Buff("Opening Strike", OpeningStrike, Source.Ranger, BuffClassification.Other, BuffImages.OpeningStrike),
            new Buff("Quick Draw", QuickDraw, Source.Ranger, BuffClassification.Other, BuffImages.QuickDraw),
            new Buff("Light on your Feet", LightOnYourFeet, Source.Ranger, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.LightOnYourFeet),
            new Buff("Poison Master", PoisonMasterBuff, Source.Ranger, BuffClassification.Other, BuffImages.PoisonMaster),
        };

        public static IReadOnlyList<AnimatedCastEvent> ComputeAncestralGraceCastEvents(Player player, CombatData combatData, SkillData skillData, AgentData agentData)
        {
            var res = new List<AnimatedCastEvent>();
            SkillItem skill = skillData.Get(AncestralGraceSkill);
            var applies = combatData.GetBuffData(AncestralGraceBuff).OfType<BuffApplyEvent>().Where(x => x.To == player.AgentItem).ToList();
            var removals = combatData.GetBuffData(AncestralGraceBuff).OfType<BuffRemoveAllEvent>().Where(x => x.To == player.AgentItem).ToList();
            for (int i = 0; i < applies.Count && i < removals.Count; i++)
            {
                res.Add(new AnimatedCastEvent(player.AgentItem, skill, applies[i].Time, removals[i].Time - applies[i].Time));
            }
            return res;
        }

        public static void ProcessGadgets(IReadOnlyList<Player> players, CombatData combatData)
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

        private static readonly HashSet<int> SpiritIDs = new HashSet<int>()
        {
            (int)MinionID.FrostSpirit,
            (int)MinionID.StoneSpirit,
            (int)MinionID.StormSpirit,
            (int)MinionID.SunSpirit,
            (int)MinionID.WaterSpirit,
            (int)MinionID.SpiritOfNatureRenewal,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return NonSpiritMinions.Contains(id) || SpiritIDs.Contains(id);
        }

    }
}
