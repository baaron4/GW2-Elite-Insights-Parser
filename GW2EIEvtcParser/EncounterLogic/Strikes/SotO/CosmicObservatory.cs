using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class CosmicObservatory : SecretOfTheObscureStrike
    {
        public CosmicObservatory(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(new long [] { SpinningNebulaCentral, SpinningNebulaWithTeleport }, "Spinning Nebula", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.DarkBlue), "Spin.Neb.H", "Spining Nebula Hit (Spin Projectiles)", "Spinning Nebula Hit", 0),
                new PlayerDstHitMechanic(DemonicBlast, "Demonic Blast", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Red), "Dmn.Blst.H", "Demonic Blast Hit (Cones AoEs)", "Demonic Blast Hit", 0),
                new PlayerDstHitMechanic(SoulFeast, "Soul Feast", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Sl.Fst.H", "Soul Feat (Pulsing Orb AoEs)", "Soul Feast Hit", 0),
                new PlayerDstHitMechanic(PlanetCrashProjectileSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.StarDiamond, Colors.White), "PlnCrhProj.H", "Planet Crash (Projectiles Hits)", "Hit by Planet Crash Projectiles", 0),
                new PlayerDstBuffApplyMechanic(ShootingStarsTargetBuff, "Shooting Stars", new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Green), "StarsTarg.A", "Shooting Stars Target (Green Arrow)", "Targetted by Shooting Stars", 0),
                new PlayerDstBuffApplyMechanic(ResidualAnxiety, "Residual Anxiety", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Red), "Rsdl.Anxty", "Residual Anxiety", "Residual Anxiety", 0),
                new PlayerDstBuffApplyMechanic(CosmicObservatoryLostControlBuff, "Lost Control", new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Lst.Ctrl", "Lost Control (10 stacks of Residual Anxiety)", "Lost Control", 0),
                new PlayerDstBuffApplyMechanic(Revealed, "Revealed", new MechanicPlotlySetting(Symbols.Bowtie, Colors.Teal), "Sl.Fst.T", "Soul Feast Target", "Targetted by Soul Feast", 0).UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Dagda)),
                new PlayerDstBuffApplyMechanic(DagdaSharedDestruction, "Shared Destruction", new MechanicPlotlySetting(Symbols.Circle, Colors.Green), "Shar.Des.T", "Targetted by Shared Destruction (Greens)", "Shared Destruction Target (Green)", 0),
                new PlayerDstBuffApplyMechanic(new long [] { TargetOrder1, TargetOrder2, TargetOrder3, TargetOrder4, TargetOrder5 }, "Target Order", new MechanicPlotlySetting(Symbols.Star, Colors.LightOrange), "Targ.Ord.A", "Received Target Order", "Target Order Application", 0),
                new PlayerSrcSkillMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightBlue), "PurLight.C", "Casted Purifying Light", "Casted Purifying Light", 0),
                new PlayerSrcHitMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.LightBlue), "PurLight.Soul.C", "Casted Purifying Light (Hit Soul Feast)", "Purifying Light Hit Soul Feast", 0).UsingChecker((ahde, log) => ahde.To.IsSpecies(TrashID.SoulFeast)),
                new PlayerSrcHitMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.Blue), "PurLight.Dagda.C", "Casted Purifying Light (Hit Dagda)", "Purifying Light Hit Dagda", 0).UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.Dagda)).UsingEnable(x => x.FightData.IsCM),
                new EnemyCastStartMechanic(ShootingStars, "Shooting Stars", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "Shooting Stars", "Shooting Stars Cast", "Cast Shooting Stars", 0),
                new EnemyCastStartMechanic(PlanetCrashSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "Planet Crash", "Planet Crash Cast", "Cast Planet Crash", 0),
                new EnemyCastStartMechanic(new long [] { SpinningNebulaCentral, SpinningNebulaWithTeleport }, "Spinning Nebula", new MechanicPlotlySetting(Symbols.CircleCross, Colors.LightRed), "Spinning Nebula", "Spinning Nebula Cast", "Cast Spinning Nebula", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.LightBlue), "Planet Crash (Int)", "Interrupted Planet Crash", "Interrupted Planet Crash", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.Dagda)),
                new EnemySrcSkillMechanic(PlanetCrashSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.DarkBlue), "Planet Crash (Land)", "Planet Crash Landed", "Fully Casted Planet Crash", 1000).UsingChecker((ahde, log) => ahde.HealthDamage >= 0 && ahde.To.IsPlayer),
            }
            );
            Icon = EncounterIconCosmicObservatory;
            Extension = "cosobs";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayCosmicObservatory,
                            (1169, 1169),
                            (-1388, -779, 1991, 2610));
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);

            // Lost Control
            IEnumerable<Segment> lostControls = p.GetBuffStatus(log, CosmicObservatoryLostControlBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(lostControls, p, ParserIcons.CallTarget);

            // Shooting Stars Target Overhead
            var shootingStarsTarget = p.GetBuffStatus(log, ShootingStarsTargetBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            replay.AddOverheadIcons(shootingStarsTarget, p, ParserIcons.TargetOverhead);

            // Target Order (CM)
            replay.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder1Overhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder2, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder2Overhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder3, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder3Overhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder4, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder4Overhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, TargetOrder5, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.TargetOrder5Overhead);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // Special check since CM release, normal mode broke too, but we always trust reward events
            if (combatData.GetBuildEvent().Build >= GW2Builds.DagdaNMHPChangedAndCMRelease && combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == RewardTypes.PostEoDStrikeReward) == null)
            {
                AbstractSingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda));
                if (dagda == null)
                {
                    throw new MissingKeyActorsException("Dagda not found");
                }
                HealthUpdateEvent hpUpdate = combatData.GetHealthUpdateEvents(dagda.AgentItem).FirstOrDefault(x => x.HPPercent <= 1e-6);
                if (hpUpdate != null)
                {
                    AbstractHealthDamageEvent lastDamageEvent = combatData.GetDamageTakenData(dagda.AgentItem).LastOrDefault(x => x.HealthDamage > 0 && x.Time <= hpUpdate.Time + ServerDelayConstant);
                    if (fightData.Success)
                    {
                        fightData.SetSuccess(true, Math.Min(lastDamageEvent.Time, fightData.FightEnd));
                    } 
                    else
                    {
                        fightData.SetSuccess(true, lastDamageEvent.Time);
                    }
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Dagda not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Cast check
            var tormentedIDs = new List<TrashID>()
            {
                TrashID.TheTormented,
                TrashID.VeteranTheTormented,
                TrashID.EliteTheTormented,
                TrashID.ChampionTheTormented,
            };
            var tormentedAgents = new List<AgentItem>();
            foreach (TrashID tormentedID in tormentedIDs)
            {
                tormentedAgents.AddRange(log.AgentData.GetNPCsByID(tormentedID));
            }
            tormentedAgents = tormentedAgents.OrderBy(x => x.FirstAware).ToList();
            var tormentedGroups = new List<List<AgentItem>>();
            var processedAgents = new HashSet<AgentItem>();
            foreach (AgentItem tormentedAgent in tormentedAgents)
            {
                if (processedAgents.Contains(tormentedAgent))
                {
                    continue;
                }
                var group = new List<AgentItem>();
                AgentItem currentReferenceTormented = tormentedAgent;
                foreach (AgentItem tormentedAgentToBeGrouped in tormentedAgents)
                {
                    if (tormentedAgentToBeGrouped.FirstAware >= currentReferenceTormented.FirstAware && tormentedAgentToBeGrouped.FirstAware <= currentReferenceTormented.LastAware)
                    {
                        group.Add(tormentedAgentToBeGrouped);
                        processedAgents.Add(tormentedAgentToBeGrouped);
                        currentReferenceTormented = tormentedAgentToBeGrouped;
                    }
                }
                tormentedGroups.Add(group);
            }
            var phaseTimes = new List<(long start, long end)>();
            long previousStart = log.FightData.FightStart;
            for (int i = 0; i < tormentedGroups.Count; i++)
            {
                List<AgentItem> group = tormentedGroups[i];
                long start = Math.Max(log.FightData.FightStart, group.Min(x => x.FirstAware));
                long end = Math.Min(log.FightData.FightEnd, group.Max(x => {
                    long res = x.LastAware;
                    if (log.CombatData.GetDeadEvents(x).Any())
                    {
                        res = log.CombatData.GetDeadEvents(x).Last().Time;
                    }
                    return res;
                }));

                phaseTimes.Add((previousStart, start));
                phaseTimes.Add((start, end));
                previousStart = end;
                if (i == tormentedGroups.Count - 1)
                {
                    phaseTimes.Add((end, log.FightData.FightEnd));
                }
            }
            for (int i = 0; i < phaseTimes.Count; i++)
            {
                (long start, long end) = phaseTimes[i];
                PhaseData phase;
                if (i % 2 == 1)
                {
                    phase = new PhaseData(start, end)
                    {
                        Name = "Tormenteds " + (i + 1) / 2
                    };
                    var ids = new List<int>
                    {
                        (int)TrashID.TheTormented,
                        (int)TrashID.VeteranTheTormented,
                        (int)TrashID.EliteTheTormented,
                        (int)TrashID.ChampionTheTormented,
                    };
                    AddTargetsToPhase(phase, ids);
                }
                else
                {
                    AbstractBuffEvent phasingBuffLoss = log.CombatData.GetBuffData(DagdaDuringPhase75_50_25).FirstOrDefault(x => x.To == mainTarget.AgentItem && x.Time >= start && x.Time <= end && x is BuffRemoveAllEvent);
                    if (phasingBuffLoss != null)
                    {
                        start = phasingBuffLoss.Time;
                    }
                    phase = new PhaseData(start, end)
                    {
                        Name = "Phase " + (i + 2) / 2
                    };
                }
                phase.AddTarget(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int curTormented = 1;
            int curVeteranTormented = 1;
            int curEliteTormented = 1;
            int curChampionTormented = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                switch (target.ID)
                {
                    case (int)TrashID.TheTormented:
                        target.OverrideName(target.Character + " " + curTormented++);
                        break;
                    case (int)TrashID.VeteranTheTormented:
                        target.OverrideName("Veteran " + target.Character + " " + curVeteranTormented++);
                        break;
                    case (int)TrashID.EliteTheTormented:
                        target.OverrideName("Elite " + target.Character + " " + curEliteTormented++);
                        break;
                    case (int)TrashID.ChampionTheTormented:
                        target.OverrideName("Champion " + target.Character + " " + curChampionTormented++);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)TargetID.Dagda,
                (int)TrashID.TheTormented,
                (int)TrashID.VeteranTheTormented,
                (int)TrashID.EliteTheTormented,
                (int)TrashID.ChampionTheTormented,
            };
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            return new List<TrashID>()
            {
                TrashID.SoulFeast,
                TrashID.TormentedPhantom,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (combatData.GetBuildEvent().Build < GW2Builds.DagdaNMHPChangedAndCMRelease)
            {
                return FightData.EncounterMode.Normal;
            }
            AbstractSingleActor dagda = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dagda));
            if (dagda == null)
            {
                throw new MissingKeyActorsException("Dagda not found");
            }
            return (dagda.GetHealth(combatData) > 56e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Cosmic Observatory";
        }
    }
}
