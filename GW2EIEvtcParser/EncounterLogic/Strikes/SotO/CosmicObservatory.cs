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
                new PlayerDstBuffApplyMechanic(Revealed, "Revealed", new MechanicPlotlySetting(Symbols.Bowtie, Colors.Teal), "Sl.Fst.T", "Soul Feast Target", "Targetted by Soul Feast", 0).UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(ArcDPSEnums.TargetID.Dagda)),
                new PlayerSrcSkillMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightBlue), "PurLight.C", "Casted Purifying Light", "Casted Purifying Light", 0),
                new PlayerSrcSkillMechanic(PurifyingLight, "Purifying Light", new MechanicPlotlySetting(Symbols.HourglassOpen, Colors.LightBlue), "PurLight.Soul.C", "Casted Purifying Light (Hit Soul Feast)", "Purifying Light Hit Soul Feast", 0).UsingChecker((ahde, log) => ahde.HasHit && ahde.To.IsSpecies(ArcDPSEnums.TrashID.SoulFeast)),
                new EnemyCastStartMechanic(ShootingStars, "Shooting Stars", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Green), "Shooting Stars", "Shooting Stars Cast", "Cast Shooting Stars", 0),
                new EnemyCastStartMechanic(PlanetCrashSkill, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "Planet Crash", "Planet Crash Cast", "Cast Planet Crash", 0),
                new EnemyCastStartMechanic(new long [] { SpinningNebulaCentral, SpinningNebulaWithTeleport }, "Spinning Nebula", new MechanicPlotlySetting(Symbols.CircleCross, Colors.LightRed), "Spinning Nebula", "Spinning Nebula Cast", "Cast Spinning Nebula", 0),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Planet Crash", new MechanicPlotlySetting(Symbols.Star, Colors.LightBlue), "Planet Crash (Int)", "Interrupted Planet Crash", "Interrupted Planet Crash", 0).UsingChecker((bae, log) => bae.To.IsSpecies(ArcDPSEnums.TargetID.Dagda)),
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
            IEnumerable<Segment> lostControls = p.GetBuffStatus(log, CosmicObservatoryLostControlBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(lostControls, p, ParserIcons.CallTarget);

            // Shooting Stars Target Overhead
            var shootingStarsTarget = p.GetBuffStatus(log, ShootingStarsTargetBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            replay.AddOverheadIcons(shootingStarsTarget, p, ParserIcons.TargetOverhead);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Dagda));
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
            var tormentedIDs = new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.TheTormented1,
                ArcDPSEnums.TrashID.TheTormented2,
                ArcDPSEnums.TrashID.TheTormented3,
            };
            var tormentedAgents = new List<AgentItem>();
            foreach (ArcDPSEnums.TrashID tormentedID in tormentedIDs)
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
                var phase = new PhaseData(start, end);
                if (i % 2 == 1)
                {
                    phase.Name = "Tormenteds " + (i + 1) / 2;
                    var ids = new List<int>
                    {
                        (int)ArcDPSEnums.TrashID.TheTormented1,
                        (int)ArcDPSEnums.TrashID.TheTormented2,
                        (int)ArcDPSEnums.TrashID.TheTormented3,
                    };
                    AddTargetsToPhase(phase, ids);
                }
                else
                {
                    phase.Name = "Phase " + (i + 2) / 2;
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
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.TheTormented1) || target.IsSpecies(ArcDPSEnums.TrashID.TheTormented2) || target.IsSpecies(ArcDPSEnums.TrashID.TheTormented3))
                {
                    target.OverrideName(target.Character + " " + curTormented++);
                }
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Dagda,
                (int)ArcDPSEnums.TrashID.TheTormented1,
                (int)ArcDPSEnums.TrashID.TheTormented2,
                (int)ArcDPSEnums.TrashID.TheTormented3,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.SoulFeast
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Cosmic Observatory";
        }
    }
}
