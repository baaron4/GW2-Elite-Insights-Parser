using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Qadim : MythwrightGambit
    {
        private bool _manualPlatforms = true;
        public Qadim(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new EnemyCastStartMechanic(QadimCC, "Qadim CC", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.DarkTeal), "Q.CC","Qadim CC", "Qadim CC",0),
            new EnemyCastEndMechanic(QadimCC, "Qadim CC", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.DarkGreen), "Q.CCed","Qadim Breakbar broken", "Qadim CCed",0).UsingChecker((ce, log) => ce.ActualDuration < 6500),
            new EnemyCastStartMechanic(QadimRiposte, "Riposte", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.DarkRed), "Q.CC Fail","Qadim Breakbar failed", "Qadim CC Fail",0),
            new PlayerDstHitMechanic(QadimRiposte, "Riposte", new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "NoCC Attack", "Riposte (Attack if CC on Qadim failed)", "Riposte (No CC)", 0),
            new PlayerDstHitMechanic(new long[] { FieryDance1, FieryDance2, FieryDance3, FieryDance4, }, "Fiery Dance", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Orange), "F.Dance", "Fiery Dance (Fire running along metal edges)", "Fire on Lines", 0),
            new PlayerDstHitMechanic(ShatteringImpact, "Shattering Impact", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Stun","Shattering Impact (Stunning flame bolt)", "Flame Bolt Stun",0),
            new PlayerDstHitMechanic(FlameWave, "Flame Wave", new MechanicPlotlySetting(Symbols.StarTriangleUpOpen,Colors.Pink), "KB","Flame Wave (Knockback Frontal Beam)", "KB Push",0),
            new PlayerDstHitMechanic(FireWaveQadim, "Fire Wave", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Q.Wave","Fire Wave (Shockwave after Qadim's Mace attack)", "Mace Shockwave",0),
            new PlayerDstHitMechanic(FireWaveDestroyer, "Fire Wave", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkRed), "D.Wave","Fire Wave (Shockwave after Destroyer's Jump or Stomp)", "Destroyer Shockwave",0),
            new PlayerDstHitMechanic(ElementalBreath, "Elemental Breath", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Red), "Hydra Breath","Elemental Breath (Hydra Breath)", "Hydra Breath",0),
            new PlayerDstHitMechanic(Fireball, "Fireball", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow,10), "H.FBall","Fireball (Hydra)", "Hydra Fireball",0),
            new PlayerDstHitMechanic(FieryMeteor, "Fiery Meteor", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Pink), "H.Meteor","Fiery Meteor (Hydra)", "Hydra Meteor",0),
            new EnemyCastStartMechanic(FieryMeteor, "Fiery Meteor", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "H.CC","Fiery Meteor (Hydra Breakbar)", "Hydra CC",0),
            //new Mechanic(718, "Fiery Meteor (Spawn)", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Qadim, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkRed), "H.CC.Fail","Fiery Meteor Spawned (Hydra Breakbar)", "Hydra CC Fail",0,(condition =>  condition.CombatItem.IFF == ParseEnum.IFF.Foe)),
            new EnemyCastEndMechanic(FieryMeteor, "Fiery Meteor", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "H.CCed","Fiery Meteor (Hydra Breakbar broken)", "Hydra CCed",0).UsingChecker((ce, log) => ce.ActualDuration < 12364),
            new EnemyCastEndMechanic(FieryMeteor, "Fiery Meteor", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkRed), "H.CC Fail","Fiery Meteor (Hydra Breakbar not broken)", "Hydra CC Failed",0).UsingChecker((ce,log) => ce.ActualDuration >= 12364),
            new PlayerDstHitMechanic(TeleportHydra, "Teleport", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "H.KB","Teleport Knockback (Hydra)", "Hydra TP KB",0),
            new PlayerDstHitMechanic(BigHit, "Big Hit", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Mace","Big Hit (Mace Impact)", "Mace Impact",0),
            new PlayerDstHitMechanic(Inferno, "Inferno", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Red), "Inf.","Inferno (Lava Pool drop  on long platform spokes)", "Inferno Pool",0),
            new PlayerDstHitMechanic(SlashWyvern, "Slash (Wyvern)", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Slash","Wyvern Slash (Double attack: knock into pin down)", "KB/Pin down",0),
            new PlayerDstHitMechanic(TailSwipe, "Tail Swipe", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Yellow), "W.Pizza","Wyvern Tail Swipe (Pizza attack)", "Tail Swipe",0),
            new PlayerDstHitMechanic(FireBreath, "Fire Breath", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Orange), "W.Breath","Fire Breath (Wyvern)", "Fire Breath",0),
            new PlayerDstHitMechanic(WingBuffet, "Wing Buffet", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.DarkTeal), "W.Wing","Wing Buffet (Wyvern Launching Wing Storm)", "Wing Buffet",0),
            new EnemyCastStartMechanic(PatriarchCC, "Patriarch CC", new MechanicPlotlySetting(Symbols.StarSquare,Colors.DarkTeal), "W.BB","Platform Destruction (Patriarch CC)", "Patriarch CC",0),
            new EnemyCastEndMechanic(PatriarchCC, "Patriarch CC", new MechanicPlotlySetting(Symbols.StarSquare,Colors.DarkGreen), "W.CCed","Platform Destruction (Patriarch Breakbar broken)", "Patriarch CCed",0).UsingChecker((ce, log) => ce.ActualDuration < 6500),
            new EnemyCastStartMechanic(PatriarchCCJumpInAir, "Patriarch CC (Jump into air)", new MechanicPlotlySetting(Symbols.StarSquare,Colors.DarkRed), "Wyv CC Fail","Platform Destruction (Patriarch Breakbar failed)", "Patriarch CC Fail",0),
            new PlayerDstHitMechanic(SeismicStomp, "Seismic Stomp", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Yellow), "D.Stomp","Seismic Stomp (Destroyer Stomp)", "Seismic Stomp (Destroyer)",0),
            new PlayerDstHitMechanic(ShatteredEarth, "Shattered Earth", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Red), "D.Slam","Shattered Earth (Destroyer Jump Slam)", "Jump Slam (Destroyer)",0),
            new PlayerDstHitMechanic(WaveOfForce, "Wave of Force", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Orange), "D.Pizza","Wave of Force (Destroyer Pizza)", "Destroyer Auto",0),
            new EnemyCastStartMechanic(SummonDestroyer, "Summon", new MechanicPlotlySetting(Symbols.StarTriangleDown,Colors.DarkTeal), "D.CC","Summon (Destroyer Breakbar)", "Destroyer CC",0),
            new EnemyCastEndMechanic(SummonDestroyer, "Summon", new MechanicPlotlySetting(Symbols.StarTriangleDown,Colors.DarkGreen), "D.CCed","Summon (Destroyer Breakbar broken)", "Destroyer CCed",0).UsingChecker((ce, log) => ce.ActualDuration < 8332),
            new EnemyCastEndMechanic(SummonDestroyer, "Summon", new MechanicPlotlySetting(Symbols.StarTriangleDown,Colors.DarkRed), "D.CC Fail","Summon (Destroyer Breakbar failed)", "Destroyer CC Fail",0).UsingChecker((ce,log) => ce.ActualDuration >= 8332),
            new SpawnMechanic(SummonSpawn, "Summon (Spawn)", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkRed), "D.Spwn","Summon (Destroyer Trolls summoned)", "Destroyer Summoned",0),
            new PlayerDstHitMechanic(BodyOfFlame, "Body of Flame", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Pink,10), "P.AoE","Body of Flame (Pyre Ground AoE (CM))", "Pyre Hitbox AoE",0),
            new PlayerDstHitMechanic(SeaOfFlame, "Sea of Flame", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Q.Hitbox","Sea of Flame (Stood in Qadim Hitbox)", "Qadim Hitbox AoE",0),
            new PlayerDstHitMechanic(Claw, "Claw", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.DarkTeal,10), "Claw","Claw (Reaper of Flesh attack)", "Reaper Claw",0),
            new PlayerDstHitMechanic(SwapQadim, "Swap", new MechanicPlotlySetting(Symbols.CircleCrossOpen,Colors.Magenta), "Port","Swap (Ported from below Legendary Creature to Qadim)", "Port to Qadim",0),
            new PlayerDstBuffApplyMechanic(PowerOfTheLamp, "Power of the Lamp", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.LightPurple,10), "Lamp","Power of the Lamp (Returned from the Lamp)", "Lamp Return",0),
            new PlayerStatusMechanic<DeadEvent>("Taking Turns", new MechanicPlotlySetting(Symbols.Bowtie, Colors.Black), "Taking Turns", "Achievement Eligibility: Taking Turns", "Taking Turns", 0, (log, a) => log.CombatData.GetDeadEvents(a)).UsingEnable((log) => CustomCheckTakingTurns(log)).UsingAchievementEligibility(true),
            new EnemyStatusMechanic<DeadEvent>("Pyre Guardian", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "Pyre.K","Pyre Killed", "Pyre Killed",0, (log, a) => a.IsSpecies(TrashID.PyreGuardian) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
            new EnemyStatusMechanic<DeadEvent>("Stab Pyre Guardian", new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pyre.S.K","Stab Pyre Killed", "Stab Pyre Killed",0, (log, a) => a.IsSpecies(TrashID.PyreGuardianStab) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
            new EnemyStatusMechanic<DeadEvent>("Protect Pyre Guardian", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Orange), "Pyre.P.K","Protect Pyre Killed", "Protect Pyre Killed",0, (log, a) => a.IsSpecies(TrashID.PyreGuardianProtect) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
            new EnemyStatusMechanic<DeadEvent>("Retal Pyre Guardian", new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightRed), "Pyre.R.K","Retal Pyre Killed", "Retal Pyre Killed",0, (log, a) => a.IsSpecies(TrashID.PyreGuardianRetal) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
            new EnemyStatusMechanic<DeadEvent>("Resolution Pyre Guardian", new MechanicPlotlySetting(Symbols.Bowtie,Colors.DarkRed), "Pyre.R.K","Resolution Pyre Killed", "Resolution Pyre Killed",0, (log, a) => a.IsSpecies(TrashID.PyreGuardianResolution) ? log.CombatData.GetDeadEvents(a) : new List<DeadEvent>()),
            });
            Extension = "qadim";
            Icon = EncounterIconQadim;
            GenericFallBackMethod = FallBackMethod.Death | FallBackMethod.CombatExit;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayQadim,
                            (1000, 994),
                            (-11676, 8825, -3870, 16582)/*,
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256)*/);
        }


        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.Qadim,
                (int)TrashID.AncientInvokedHydra,
                (int)TrashID.ApocalypseBringer,
                (int)TrashID.WyvernMatriarch,
                (int)TrashID.WyvernPatriarch,
                (int)TrashID.QadimLamp,
            };
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)TargetID.Qadim,
                (int)TrashID.AncientInvokedHydra,
                (int)TrashID.ApocalypseBringer,
                (int)TrashID.WyvernMatriarch,
                (int)TrashID.WyvernPatriarch
            };
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            bool refresh = false;
            if (evtcVersion >= ArcDPSBuilds.FunctionalEffect2Events)
            {
                var platformAgents = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth >= 2576 && x.HitboxWidth <= 2578).ToList();
                foreach (AgentItem platform in platformAgents)
                {
                    platform.OverrideType(AgentItem.AgentType.NPC);
                    platform.OverrideID(TrashID.QadimPlatform);
                }
                refresh = refresh || platformAgents.Any();
            }
            IReadOnlyList<AgentItem> pyres = agentData.GetNPCsByID(TrashID.PyreGuardian);
            // Lamps
            var lampAgents = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 202).ToList();
            foreach (AgentItem lamp in lampAgents)
            {
                lamp.OverrideType(AgentItem.AgentType.NPC);
                lamp.OverrideID(TrashID.QadimLamp);
            }
            refresh = refresh || lampAgents.Any();
            // Pyres
            var protectPyrePositions = new List<Point3D> { new Point3D(-8947, 14728), new Point3D(-10834, 12477) };
            var stabilityPyrePositions = new List<Point3D> { new Point3D(-4356, 12076), new Point3D(-5889, 14723), new Point3D(-7851, 13550) };
            var resolutionRetaliationPyrePositions = new List<Point3D> { new Point3D(-8951, 9429), new Point3D(-5716, 9325), new Point3D(-7846, 10612) };
            foreach (AgentItem pyre in pyres)
            {
                CombatItem positionEvt = combatData.FirstOrDefault(x => x.SrcMatchesAgent(pyre) && x.IsStateChange == StateChange.Position);
                if (positionEvt != null)
                {
                    Point3D position = AbstractMovementEvent.GetPoint3D(positionEvt.DstAgent, 0);
                    if (protectPyrePositions.Any(x => x.Distance2DToPoint(position) < InchDistanceThreshold))
                    {
                        pyre.OverrideID(TrashID.PyreGuardianProtect);
                        refresh = true;
                    }
                    else if (stabilityPyrePositions.Any(x => x.Distance2DToPoint(position) < InchDistanceThreshold))
                    {
                        pyre.OverrideID(TrashID.PyreGuardianStab);
                        refresh = true;
                    }
                    else if (resolutionRetaliationPyrePositions.Any(x => x.Distance2DToPoint(position) < InchDistanceThreshold))
                    {
                        pyre.OverrideID(gw2Build >= GW2Builds.May2021Balance ? TrashID.PyreGuardianResolution : TrashID.PyreGuardianRetal);
                        refresh = true;
                    }
                }
            }
            if (refresh)
            {
                agentData.Refresh();
            }
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            foreach (NPC target in TrashMobs)
            {
                if (target.IsSpecies(TrashID.PyreGuardianProtect))
                {
                    target.OverrideName("Protect " + target.Character);
                }
                if (target.IsSpecies(TrashID.PyreGuardianRetal))
                {
                    target.OverrideName("Retal " + target.Character);
                }
                if (target.IsSpecies(TrashID.PyreGuardianResolution))
                {
                    target.OverrideName("Resolution " + target.Character);
                }
                if (target.IsSpecies(TrashID.PyreGuardianStab))
                {
                    target.OverrideName("Stab " + target.Character);
                }
            }
            var platformNames = new List<string>()
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "00",
                "01",
                "02",
                "03",
                "04",
                "05",
                "06",
                "07",
                "08",
                "09",
                "10",
                "11",
            };
            _manualPlatforms = TrashMobs.Count(x => platformNames.Contains(x.Character)) != 12;
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AgentItem qadim = agentData.GetNPCsByID(TargetID.Qadim).FirstOrDefault();
            if (qadim == null)
            {
                throw new MissingKeyActorsException("Qadim not found");
            }
            if (combatData.HasMovementData)
            {
                var qadimAroundInitialPosition = new Point3D(-9742.406f, 12075.2627f, -4731.031f);
                var positions = combatData.GetMovementData(qadim).Where(x => x is PositionEvent pe && pe.Time < qadim.FirstAware + MinimumInCombatDuration).Select(x => x.GetParametricPoint3D()).ToList();
                if (!positions.Any(x => x.Distance2DToPoint(qadimAroundInitialPosition) < 150))
                {
                    return FightData.EncounterStartStatus.Late;
                }
            }
            if (TargetHPPercentUnderThreshold(TargetID.Qadim, fightData.FightStart, combatData, Targets) ||
                (Targets.Any(x => x.IsSpecies(TrashID.AncientInvokedHydra)) && TargetHPPercentUnderThreshold((int)TrashID.AncientInvokedHydra, fightData.FightStart, combatData, Targets)))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(BurningCrucible, BurningCrucible),
            };
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            AgentItem target = agentData.GetNPCsByID(TargetID.Qadim).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Qadim not found");
            }
            CombatItem startCast = combatData.FirstOrDefault(x => x.SkillID == QadimInitialCast && x.StartCasting());
            CombatItem sanityCheckCast = combatData.FirstOrDefault(x => (x.SkillID == FlameSlash3 || x.SkillID == FlameSlash || x.SkillID == FlameWave) && x.StartCasting());
            if (startCast == null || sanityCheckCast == null)
            {
                return fightData.LogStart;
            }
            // sanity check
            if (sanityCheckCast.Time - startCast.Time > 0)
            {
                return startCast.Time;
            }
            return GetGenericFightOffset(fightData);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            // Warning: Combat replay relies on these phases.
            // If changing phase detection, combat replay platform timings may have to be updated.

            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor qadim = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim));
            if (qadim == null)
            {
                throw new MissingKeyActorsException("Qadim not found");
            }
            phases[0].AddTarget(qadim);
            var secondaryTargetIds = new HashSet<TrashID>
                        {
                           TrashID.WyvernMatriarch,
                           TrashID.WyvernPatriarch,
                           TrashID.AncientInvokedHydra,
                           TrashID.ApocalypseBringer,
                        };
            phases[0].AddSecondaryTargets(Targets.Where(x => x.IsAnySpecies(secondaryTargetIds)));
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, QadimInvulnerable, qadim, true, false));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Qadim P" + (i) / 2;
                    var pyresFirstAware = new List<long>();
                    var pyres = new List<TrashID>
                        {
                            TrashID.PyreGuardian,
                            TrashID.PyreGuardianProtect,
                            TrashID.PyreGuardianStab,
                            TrashID.PyreGuardianRetal,
                            TrashID.PyreGuardianResolution,
                        };
                    foreach (int pyreId in pyres)
                    {
                        pyresFirstAware.AddRange(log.AgentData.GetNPCsByID(pyreId).Where(x => phase.InInterval(x.FirstAware)).Select(x => x.FirstAware));
                    }
                    if (pyresFirstAware.Count > 0 && pyresFirstAware.Max() > phase.Start)
                    {
                        phase.OverrideStart(pyresFirstAware.Max());
                    }
                    phase.AddTarget(qadim);
                }
                else
                {
                    var ids = new List<int>
                        {
                           (int) TrashID.WyvernMatriarch,
                           (int) TrashID.WyvernPatriarch,
                           (int) TrashID.AncientInvokedHydra,
                           (int) TrashID.ApocalypseBringer,
                           (int) TrashID.QadimLamp
                        };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                    if (phase.Targets.Count > 0)
                    {
                        var phaseTarIDs = new HashSet<int>(phase.Targets.Select(x => x.ID));
                        if (phaseTarIDs.Contains((int)TrashID.AncientInvokedHydra))
                        {
                            phase.Name = "Hydra";
                        }
                        else if (phaseTarIDs.Contains((int)TrashID.ApocalypseBringer))
                        {
                            phase.Name = "Apocalypse";
                        }
                        else if (phaseTarIDs.Contains((int)TrashID.WyvernPatriarch) || phaseTarIDs.Contains((int)TrashID.WyvernMatriarch))
                        {
                            phase.Name = "Wyvern";
                        }
                        else
                        {
                            phase.Name = "Unknown";
                        }
                    }
                }
            }
            phases.RemoveAll(x => x.Start >= x.End);
            return phases;
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            return new List<TrashID>()
            {
                TrashID.QadimPlatform,
                TrashID.LavaElemental1,
                TrashID.LavaElemental2,
                TrashID.IcebornHydra,
                TrashID.GreaterMagmaElemental1,
                TrashID.GreaterMagmaElemental2,
                TrashID.FireElemental,
                TrashID.FireImp,
                TrashID.PyreGuardian,
                TrashID.PyreGuardianProtect,
                TrashID.PyreGuardianRetal,
                TrashID.PyreGuardianResolution,
                TrashID.PyreGuardianStab,
                TrashID.ReaperOfFlesh,
                TrashID.DestroyerTroll,
                TrashID.IceElemental,
                TrashID.AngryZommoros,
                TrashID.AssaultCube,
                TrashID.AwakenedSoldier,
                TrashID.Basilisk,
                TrashID.BlackMoa,
                TrashID.BrandedCharr,
                TrashID.BrandedDevourer,
                TrashID.ChakDrone,
                TrashID.CrazedKarkaHatchling,
                TrashID.FireImpLamp,
                TrashID.GhostlyPirateFighter,
                TrashID.GiantBrawler,
                TrashID.GiantHunter,
                TrashID.GoldOoze,
                TrashID.GrawlBascher,
                TrashID.GrawlTrapper,
                TrashID.GuildInitiateModusSceleris,
                TrashID.IcebroodAtrocity,
                TrashID.IcebroodKodan,
                TrashID.IcebroodQuaggan,
                TrashID.Jotun,
                TrashID.JungleWurm,
                TrashID.Karka,
                TrashID.MinotaurBull,
                TrashID.ModnirrBerserker,
                TrashID.MoltenDisaggregator,
                TrashID.MoltenProtector,
                TrashID.MoltenReverberant,
                TrashID.MordremVinetooth,
                TrashID.Murellow,
                TrashID.NightmareCourtier,
                TrashID.OgreHunter,
                TrashID.PirareSkrittSentry,
                TrashID.PolarBear,
                TrashID.Rabbit,
                TrashID.ReefSkelk,
                TrashID.RisenKraitDamoss,
                TrashID.RottingAncientOakheart,
                TrashID.RottingDestroyer,
                TrashID.ShadowSkelk,
                TrashID.SpiritOfExcess,
                TrashID.TamedWarg,
                TrashID.TarElemental,
                TrashID.WindRider,
            };
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            if (_manualPlatforms)
            {
                AddManuallyAnimatedPlatformsToCombatReplay(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)), log, EnvironmentDecorations);
            }

            // Incineration Orbs - CM
            if (log.CombatData.TryGetGroupedEffectEventsByGUID(EffectGUIDs.QadimCMIncinerationOrbs, out IReadOnlyList<IReadOnlyList<EffectEvent>> cmOrbs))
            {
                foreach (IReadOnlyList<EffectEvent> orbs in cmOrbs)
                {
                    var positions = orbs.Select(x => x.Position).ToList();
                    Point3D middle = positions.FirstOrDefault(x => Point3D.IsInTriangle2D(x, positions.Where(y => y != x).ToList()));
                    EffectEvent middleEvent = orbs.FirstOrDefault(x => x.Position == middle);
                    if (middleEvent != null)
                    {
                        foreach (EffectEvent effect in orbs)
                        {
                            uint radius = (uint)(effect == middleEvent ? 540 : 180);
                            (long start, long end) lifespan = effect.ComputeLifespan(log, 2600);
                            var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                            var circle2 = new CircleDecoration(radius, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                            EnvironmentDecorations.Add(circle);
                            EnvironmentDecorations.Add(circle2.UsingGrowingEnd(lifespan.end));
                        }
                    }
                }
            }

            // Incineration Orbs - Pyres
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimPyresIncinerationOrbs, out IReadOnlyList<EffectEvent> pyreOrbs))
            {
                foreach (EffectEvent effect in pyreOrbs)
                {
                    uint radius = 240;
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 2300);
                    var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                    var circleRed = new CircleDecoration(radius, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circleRed.UsingGrowingEnd(lifespan.end));
                }
            }

            // Bouncing blue orbs
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimJumpingBlueOrbs, out IReadOnlyList<EffectEvent> blueOrbEvents))
            {
                foreach (EffectEvent effect in blueOrbEvents)
                {
                    (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, effect.Duration);
                    var circle = new CircleDecoration(100, lifespan, Colors.Blue, 0.5, new PositionConnector(effect.Position));
                    EnvironmentDecorations.Add(circle);
                }
            }

            // Inferno - Qadim's AoEs on every platform
            // ! Disabled until we have a working solution for effects on moving platforms
            /*if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.QadimInfernoAoEs, out IReadOnlyList<EffectEvent> infernoAoEs))
            {
                foreach (EffectEvent effect in infernoAoEs)
                {
                    int radius = 150;
                    (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                    var circle = new CircleDecoration(radius, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                    var circleRed = new CircleDecoration(radius, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circleRed.UsingGrowingEnd(lifespan.end));
                }
            }*/
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            uint ccRadius = 200;
            switch (target.ID)
            {
                case (int)TargetID.Qadim:
                    //CC
                    var breakbar = cls.Where(x => x.SkillId == QadimCC).ToList();
                    foreach (AbstractCastEvent c in breakbar)
                    {
                        replay.Decorations.Add(new CircleDecoration(ccRadius, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    //Riposte
                    var riposte = cls.Where(x => x.SkillId == QadimRiposte).ToList();
                    foreach (AbstractCastEvent c in riposte)
                    {
                        uint radius = 2200;
                        replay.Decorations.Add(new CircleDecoration(radius, ((int)c.Time, (int)c.EndTime), Colors.Red, 0.5, new AgentConnector(target)));
                    }
                    //Big Hit
                    var maceShockwave = cls.Where(x => x.SkillId == BigHit && x.Status != AbstractCastEvent.AnimationStatus.Interrupted).ToList();
                    foreach (AbstractCastEvent c in maceShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 2230;
                        int duration = 2680;
                        uint radius = 2000;
                        uint impactRadius = 40;
                        int spellCenterDistance = 300;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        Point3D targetPosition = target.GetCurrentPosition(log, start + 1000);
                        if (facing != null && targetPosition != null)
                        {
                            var position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z);
                            replay.Decorations.Add(new CircleDecoration(impactRadius, (start, start + delay), Colors.Orange, 0.2, new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(impactRadius, (start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.7)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(radius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.7)", new PositionConnector(position)).UsingFilled(false).UsingGrowingEnd(start + delay + duration));
                        }
                    }
                    break;
                case (int)TrashID.AncientInvokedHydra:
                    //CC
                    var fieryMeteor = cls.Where(x => x.SkillId == FieryMeteor).ToList();
                    foreach (AbstractCastEvent c in fieryMeteor)
                    {
                        replay.Decorations.Add(new CircleDecoration(ccRadius, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    var eleBreath = cls.Where(x => x.SkillId == ElementalBreath).ToList();
                    foreach (AbstractCastEvent c in eleBreath)
                    {
                        int start = (int)c.Time;
                        uint radius = 1300;
                        int delay = 2600;
                        int duration = 1000;
                        int openingAngle = 70;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing != null)
                        {
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay, start + delay + duration), Colors.LightOrange, 0.3, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
                        }
                    }
                    break;
                case (int)TrashID.WyvernMatriarch:
                    //Wing Buffet
                    var wingBuffet = cls.Where(x => x.SkillId == WingBuffet).ToList();
                    foreach (AbstractCastEvent c in wingBuffet)
                    {
                        int start = (int)c.Time;
                        int preCast = Math.Min(3500, c.ActualDuration);
                        int duration = Math.Min(6500, c.ActualDuration);
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        uint range = 2800;
                        uint span = 2400;
                        if (facing != null)
                        {
                            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(range / 2, 0), true);
                            var rotationConnextor = new AngleConnector(facing);
                            replay.Decorations.Add(new RectangleDecoration(range, span, (start, start + preCast), Colors.LightBlue, 0.2, positionConnector).UsingRotationConnector(rotationConnextor));
                            replay.Decorations.Add(new RectangleDecoration(range, span, (start + preCast, start + duration), Colors.LightBlue, 0.5, positionConnector).UsingRotationConnector(rotationConnextor));
                        }
                    }
                    //Breath
                    var matBreath = cls.Where(x => x.SkillId == FireBreath).ToList();
                    foreach (AbstractCastEvent c in matBreath)
                    {
                        int start = (int)c.Time;
                        uint radius = 1000;
                        int delay = 1600;
                        int duration = 3000;
                        int openingAngle = 70;
                        int fieldDuration = 10000;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        Point3D pos = target.GetCurrentPosition(log, start + 1000);
                        if (facing != null && pos != null)
                        {
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay, start + delay + duration), Colors.Yellow, 0.3, new AgentConnector(target)).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay + duration, start + delay + fieldDuration), Colors.Red, 0.3, new PositionConnector(pos)).UsingRotationConnector(rotationConnector));
                        }
                    }
                    //Tail Swipe
                    var matSwipe = cls.Where(x => x.SkillId == TailSwipe).ToList();
                    foreach (AbstractCastEvent c in matSwipe)
                    {
                        int start = (int)c.Time;
                        uint maxRadius = 700;
                        uint radiusDecrement = 100;
                        int delay = 1435;
                        int openingAngle = 59;
                        int angleIncrement = 60;
                        int coneAmount = 4;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing != null)
                        {
                            float initialAngle = Point3D.GetZRotationFromFacing(facing);
                            var connector = new AgentConnector(target);
                            for (uint i = 0; i < coneAmount; i++)
                            {
                                var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                replay.AddDecorationWithBorder((PieDecoration)new PieDecoration( maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), Colors.LightOrange, 0.3, connector).UsingRotationConnector(rotationConnector));

                            }
                        }
                    }
                    break;
                case (int)TrashID.WyvernPatriarch:
                    //CC
                    var patCC = cls.Where(x => x.SkillId == PatriarchCC).ToList();
                    foreach (AbstractCastEvent c in patCC)
                    {
                        replay.Decorations.Add(new CircleDecoration(ccRadius, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.4)", new AgentConnector(target)));
                    }
                    //Breath
                    var patBreath = cls.Where(x => x.SkillId == FireBreath).ToList();
                    foreach (AbstractCastEvent c in patBreath)
                    {
                        int start = (int)c.Time;
                        uint radius = 1000;
                        int delay = 1600;
                        int duration = 3000;
                        int openingAngle = 60;
                        int fieldDuration = 10000;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        Point3D pos = target.GetCurrentPosition(log, start + 1000);
                        if (facing != null && pos != null)
                        {
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay, start + delay + duration), Colors.Yellow, 0.3, new AgentConnector(target)).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start + delay + duration, start + delay + fieldDuration), Colors.Red, 0.3, new PositionConnector(pos)).UsingRotationConnector(rotationConnector));
                        }
                    }
                    //Tail Swipe
                    var patSwipe = cls.Where(x => x.SkillId == TailSwipe).ToList();
                    foreach (AbstractCastEvent c in patSwipe)
                    {
                        int start = (int)c.Time;
                        uint maxRadius = 700;
                        uint radiusDecrement = 100;
                        int delay = 1435;
                        int openingAngle = 59;
                        int angleIncrement = 60;
                        int coneAmount = 4;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing != null)
                        {
                            float initialAngle = Point3D.GetZRotationFromFacing(facing);
                            var connector = new AgentConnector(target);
                            for (uint i = 0; i < coneAmount; i++)
                            {
                                var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                replay.AddDecorationWithBorder((PieDecoration)new PieDecoration( maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), Colors.LightOrange, 0.4, connector).UsingRotationConnector(rotationConnector));
                            }
                        }
                    }
                    break;
                case (int)TrashID.ApocalypseBringer:
                    var jumpShockwave = cls.Where(x => x.SkillId == ShatteredEarth).ToList();
                    foreach (AbstractCastEvent c in jumpShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 1800;
                        int duration = 3000;
                        uint maxRadius = 2000;
                        replay.Decorations.Add(new CircleDecoration( maxRadius, (start + delay, start + delay + duration), Colors.Yellow, 0.5, new AgentConnector(target)).UsingFilled(false).UsingGrowingEnd(start + delay + duration));
                    }
                    var stompShockwave = cls.Where(x => x.SkillId == SeismicStomp).ToList();
                    foreach (AbstractCastEvent c in stompShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 1600;
                        int duration = 3500;
                        uint maxRadius = 2000;
                        uint impactRadius = 500;
                        int spellCenterDistance = 270; //hitbox radius
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        Point3D targetPosition = target.GetCurrentPosition(log, start + 1000);
                        if (facing != null && targetPosition != null)
                        {
                            var position = new Point3D(targetPosition.X + facing.X * spellCenterDistance, targetPosition.Y + facing.Y * spellCenterDistance, targetPosition.Z);
                            replay.Decorations.Add(new CircleDecoration(impactRadius, (start, start + delay), Colors.Orange, 0.1, new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(impactRadius, (start + delay - 10, start + delay + 100), Colors.Orange, 0.5, new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(maxRadius, (start + delay, start + delay + duration), Colors.Yellow, 0.5, new PositionConnector(position)).UsingFilled(false).UsingGrowingEnd(start + delay + duration));
                        }
                    }
                    //CC
                    var summon = cls.Where(x => x.SkillId == SummonDestroyer).ToList();
                    foreach (AbstractCastEvent c in summon)
                    {
                        replay.Decorations.Add(new CircleDecoration(ccRadius, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    //Pizza
                    var forceWave = cls.Where(x => x.SkillId == WaveOfForce).ToList();
                    foreach (AbstractCastEvent c in forceWave)
                    {
                        int start = (int)c.Time;
                        uint maxRadius = 1000;
                        uint radiusDecrement = 200;
                        int delay = 1560;
                        int openingAngle = 44;
                        int angleIncrement = 45;
                        int coneAmount = 3;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing != null)
                        {
                            float initialAngle = Point3D.GetZRotationFromFacing(facing);
                            var connector = new AgentConnector(target);
                            for (uint i = 0; i < coneAmount; i++)
                            {
                                var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                replay.AddDecorationWithBorder((PieDecoration)new PieDecoration( maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), Colors.LightOrange, 0.4, connector).UsingRotationConnector(rotationConnector));
                            }
                        }
                    }
                    break;
                case (int)TrashID.QadimPlatform:
                    if (_manualPlatforms)
                    {
                        return;
                    }
                    const float hiddenOpacity = 0.1f;
                    const float visibleOpacity = 1f;
                    const float noOpacity = -1f;
                    var heights = replay.Positions.Select(x => new ParametricPoint1D(x.Z, x.Time)).ToList();
                    var opacities = new List<ParametricPoint1D> { new ParametricPoint1D(visibleOpacity, target.FirstAware) };
                    int velocityIndex = 0;
                    AbstractSingleActor qadim = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim));
                    if (qadim == null)
                    {
                        throw new MissingKeyActorsException("Qadim not found");
                    }
                    HealthUpdateEvent below21Percent = log.CombatData.GetHealthUpdateEvents(qadim.AgentItem).FirstOrDefault(x => x.HPPercent < 21);
                    long finalPhasePlatformSwapTime = below21Percent != null ? below21Percent.Time + 9000 : log.FightData.LogEnd;
                    float threshold = 1f;
                    switch (target.Character)
                    {
                        case "00":
                        case "0":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(-76.52588f, 44.1894531f, 22.7294922f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0, 0, 0), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                }
                            }
                            break;
                        case "01":
                        case "1":
                            ParametricPoint3D found = replay.Velocities.FirstOrDefault(x => new Point3D(-28.3569336f, -49.2431641f, 90.90576f).DistanceToPoint(x) < threshold);
                            if (found != null)
                            {
                                opacities.Add(new ParametricPoint1D(hiddenOpacity, found.Time));
                            }
                            break;
                        case "02":
                        case "2":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(-0.122070313f, 77.88086f, 4.54101563f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(37.0361328f, -13.94043f, -22.7294922f), visibleOpacity, velocityIndex, out velocityIndex, 10000, 0, hiddenOpacity))
                                {
                                    if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(153.723145f, -110.742188f, -3.63769531f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                    }
                                }
                            }
                            break;
                        case "03":
                        case "3":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(348.474121f, -123.4375f, 10.9130859f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                            }
                            break;
                        case "04":
                        case "4":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(37.20703f, 13.94043f, 22.7294922f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity)) 
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(-0.29296875f, -59.6923828f, -13.6352539f), visibleOpacity, velocityIndex, out velocityIndex, 10000, 0, hiddenOpacity))
                                {
                                    if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(357.592773f, -294.018555f, 13.6352539f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                    }
                                }
                            }
                            break;
                        case "05":
                        case "5":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(255.712891f, -69.43359f, 2.722168f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                            }
                            break;
                        case "06":
                        case "6":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(182.8125f, -80.15137f, 22.7294922f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0, 0, 0), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.QadimJumpingBlueOrbs, out IReadOnlyList<EffectEvent> blueOrbs))
                                        {
                                            EffectEvent lastBlueOrb = blueOrbs.FirstOrDefault(x => x.Time > opacities.Last().Time);
                                            if (lastBlueOrb != null)
                                            {
                                                (long start, long end) = lastBlueOrb.ComputeDynamicLifespan(log, lastBlueOrb.Duration);
                                                if (Math.Abs(end - log.FightData.FightEnd) > 500)
                                                {
                                                    opacities.Add(new ParametricPoint1D(hiddenOpacity, end));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "07":
                        case "7":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(-98.53516f, 49.2919922f, -19.0917969f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    if(AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(46.75293f, 0, -6.35986328f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0, 0, 0), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity);
                                    }
                                }
                            }
                            break;
                        case "08":
                        case "8":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(37.20703f, -14.0136719f, 18.17627f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(15.234375f, 31.9580078f, -9.094238f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(87.25586f, -70.87402f, 4.54101563f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                        {
                                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                            {
                                                AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "09":
                        case "9":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(50.7568359f, 69.3847656f, -6.35986328f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                            }
                            break;
                        case "10":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(-0.122070313f, -77.92969f, 4.54101563f), hiddenOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), noOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                {
                                    if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                    {
                                        if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(-51.3793945f, 110.473633f, -3.63769531f), hiddenOpacity, velocityIndex, out velocityIndex, 0, 0, hiddenOpacity))
                                        {
                                            AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), visibleOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                                        }
                                    }
                                }
                            }
                            break;
                        case "11":
                            if (AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(143.493652f, 114.282227f, 17.27295f), noOpacity, 0, out velocityIndex, 0, 0, hiddenOpacity))
                            {
                                AddOpacityUsingVelocity(replay.Velocities, opacities, new Point3D(0f, 0f, 0f), noOpacity, velocityIndex, out velocityIndex, 0, finalPhasePlatformSwapTime, hiddenOpacity);
                            }
                            break;
                        default:
                            break;
                    }
                    var platformDecoration = new BackgroundIconDecoration(ParserIcons.QadimPlatform, 0, 2247, opacities, heights, (target.FirstAware, target.LastAware), new AgentConnector(target));
                    RotationConnector platformRotationConnector = new AgentFacingConnector(target, 180, AgentFacingConnector.RotationOffsetMode.AddToMaster);
                    replay.Decorations.Add(platformDecoration.UsingRotationConnector(platformRotationConnector));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns true if velocity was found
        /// </summary>
        /// <param name="velocities">Velocities of the platform</param>
        /// <param name="opacities">Opacities of the platform, will be filled</param>
        /// <param name="referenceVelocity">Velocity to find</param>
        /// <param name="opacity">Opacity to add, won't be added if <0</param>
        /// <param name="startIndex"></param>
        /// <param name="foundIndexPlusOne"></param>
        /// <param name="timeOffset">Time to be added to found velocity time</param>
        /// <param name="forceHideTime">If > 0, forces the addition of a hidden opacity at given time</param>
        /// <param name="hiddenOpacity">Hidden opacity value</param>
        /// <returns></returns>
        private static bool AddOpacityUsingVelocity(IReadOnlyList<ParametricPoint3D> velocities, List<ParametricPoint1D> opacities, Point3D referenceVelocity, float opacity, int startIndex, out int foundIndexPlusOne, long timeOffset, long forceHideTime, float hiddenOpacity)
        {
            float threshold = 1f;
            for (int velocityIndex = startIndex; velocityIndex < velocities.Count; velocityIndex++)
            {
                if (referenceVelocity.DistanceToPoint(velocities[velocityIndex]) < threshold)
                {
                    if (opacity >= 0)
                    {
                        opacities.Add(new ParametricPoint1D(opacity, velocities[velocityIndex].Time + timeOffset));
                    }
                    if (forceHideTime > 0 && opacity != hiddenOpacity)
                    {
                        opacities.Add(new ParametricPoint1D(hiddenOpacity, forceHideTime));
                    }
                    foundIndexPlusOne = velocityIndex + 1;
                    return true;
                }
            }
            foundIndexPlusOne = 0;
            return false;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim));
            if (target == null)
            {
                throw new MissingKeyActorsException("Qadim not found");
            }
            return (target.GetHealth(combatData) > 21e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        private static void AddManuallyAnimatedPlatformsToCombatReplay(AbstractSingleActor qadim, ParsedEvtcLog log, List<GenericDecoration> decorations)
        {
            // We later use the target to find out the timing of the last move
            Debug.Assert(qadim.IsSpecies(TargetID.Qadim));

            // These values were all calculated by hand.
            // It would be way nicer to calculate them here, but we don't have a nice vector library
            // and it would double the amount of work.

            const string platformImageUrl = ParserIcons.QadimPlatform;
            const double hiddenOpacity = 0.1;

            bool isCM = log.FightData.IsCM;

            const int xLeft = -7975;
            const int xLeftLeft = -8537;
            const int xLeftLeftLeft = -9661;
            const int xRight = -6851;
            const int xRightRight = -6289;
            const int xRightRightRight = -5165;
            const int yMid = 12077;
            const int yUp = 13050;
            const int yUpUp = 14023;
            const int yDown = 11104;
            const int yDownDown = 10131;
            const int xGapsLeft = -8018;
            const int xGapsLeftLeft = -8618;
            const int xGapsLeftLeftLeft = -9822;
            const int xGapsRight = -6815;
            const int xGapsRightRight = -6215;
            const int xGapsRightRightRight = -5011;
            const int yGapsUp = 13118;
            const int yGapsUpUp = 14161;
            const int yGapsDown = 11037;
            const int yGapsDownDown = 9993;

            const int xDestroyerLeftLeftLeft = -9732;
            const int xDestroyerLeftLeft = xGapsLeftLeft + 5;
            const int xDestroyerLeft = -8047;
            const int xDestroyerRight = -6778;
            const int xDestroyerRightRight = xGapsRightRight - 5;
            const int xDestroyerRightRightRight = -5101;

            (int x, int y) retaliationPyre1 = (-8951, 9429);
            (int x, int y) protectionPyre1 = (-8947, 14728);
            (int x, int y) stabilityPyre1 = (-4356, yMid);

            (int x, int y) retaliationPyre2 = (-5717, 9325);
            (int x, int y) protectionPyre2 = (-10834, 12477);
            (int x, int y) stabilityPyre2 = (-5889, 14723);

            const double wyvernPhaseMiddleRotation = 0.34;

            const int yJumpingPuzzleOffset1 = 12077 - 11073; // Easternmost two platforms
            const int yJumpingPuzzleOffset2 = 12077 - 10612; // Two platforms on each side, including pyres
            const int yJumpingPuzzleOffset3 = 12077 - 10056; // Northernmost and southernmost rotating platforms
            const int xJumpingPuzzleQadim = -10237; // Qadim's platform
            const int xJumpingPuzzlePreQadim = -8808;
            const int xJumpingPuzzlePyres = -7851;
            const int xJumpingPuzzlePrePyres = -6289;
            const int xJumpingPuzzleRotatingPrePyres = -5736;
            const int xJumpingPuzzleFirstRotating = -5736;
            const int xJumpingPuzzleFirstPlatform = -4146;

            const double jumpingPuzzleRotationRate = 2 * Math.PI / 30; // rad/sec, TODO: Not perfect, it's a bit off

            const int xFinalPlatform = -8297;
            const int qadimFinalX = -7356;
            const int qadimFinalY = 12077;

            const int zDefault = -4731;
            const int zJumpingPuzzlePyres = -4871;
            const int zJumpingPuzzlePrePyres = -4801;
            const int zJumpingPuzzlePreQadim = -4941;
            const int zJumpingPuzzleFirstPlatform = -4591; // The first platform Zommoros visits
            const int zJumpingPuzzleSecondPlatform = -4661; // The second platform Zommoros visits
            const int zFinalPlatforms = -5011;

            const int timeAfterPhase2 = 4000;
            const int timeAfterWyvernPhase = 25000;
            const int jumpingPuzzleShuffleDuration = 11000;
            const int lastPhasePreparationDuration = 13000;

            // If phase data is not calculated, only the first layout is used
            var phases = log.FightData.GetPhases(log).Where(x => !x.BreakbarPhase).ToList();

            int qadimPhase1Time = (int)(phases.Count > 1 ? phases[1].End : int.MaxValue);
            int destroyerPhaseTime = (int)(phases.Count > 2 ? phases[2].End : int.MaxValue);
            int qadimPhase2Time = (int)(phases.Count > 3 ? phases[3].End : int.MaxValue);
            int wyvernPhaseTime = (int)(phases.Count > 4 ? phases[4].End + timeAfterPhase2 : int.MaxValue);
            int jumpingPuzzleTime = (int)(phases.Count > 5 ? phases[5].End + timeAfterWyvernPhase : int.MaxValue);
            int finalPhaseTime = int.MaxValue;
            if (phases.Count > 6)
            {
                PhaseData lastPhase = phases[6];

                IReadOnlyList<ParametricPoint3D> qadimMovement = qadim.GetCombatReplayNonPolledPositions(log);

                ParametricPoint3D lastMove = qadimMovement.FirstOrDefault(
                    pt =>
                    {
                        return Math.Abs(pt.X - qadimFinalX) < 5 && Math.Abs(pt.Y - qadimFinalY) < 5;
                    });

                if (lastMove != null)
                {
                    finalPhaseTime = (int)lastMove.Time;
                }
            }

            int jumpingPuzzleDuration = finalPhaseTime - lastPhasePreparationDuration - jumpingPuzzleShuffleDuration - jumpingPuzzleTime;

            const int platformCount = 12;

            // The following monstrosity is needed to avoid the final platform rotating all the way back
            // must be an odd multiple of PI
            int finalPlatformHalfRotationCount =
                (int)Math.Round((Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate) / Math.PI);
            if (finalPlatformHalfRotationCount % 2 == 0)
            {
                finalPlatformHalfRotationCount++;
            }
            double finalPlatformRotation = Math.PI * finalPlatformHalfRotationCount;


            // Proper skipping of phases (if even possible) is not implemented.
            // Right now transitioning to another state while still moving behaves weirdly.
            // Interpolating to find the position to stop in would be necessary.
            int startOffset = -(int)log.FightData.FightStartOffset;
            (int start, int duration, (int x, int y, int z, double angle, double opacity)[] platforms)[] movements =
            {
                (
                    // Initial position, all platforms tightly packed

                    startOffset, 0, new[]
                    {
                        (xLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (xLeftLeft, yUpUp, zDefault, Math.PI, 1.0),
                        (xRightRight, yUpUp, zDefault, 0.0, 1.0),
                        (xRightRightRight, yMid, zDefault, Math.PI, 1.0),
                        (xRightRight, yDownDown, zDefault, 0.0, 1.0),
                        (xLeftLeft, yDownDown, zDefault, Math.PI, 1.0),
                        (xLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xLeft, yUp, zDefault, 0.0, 1.0),
                        (xRight, yUp, zDefault, Math.PI, 1.0),
                        (xRightRight, yMid, zDefault, 0.0, 1.0),
                        (xRight, yDown, zDefault, Math.PI, 1.0),
                        (xLeft, yDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Hydra phase, all platforms have a small gap between them
                    startOffset, 12000, new[]
                    {
                        (xGapsLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (xGapsLeftLeft, yGapsUpUp, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yGapsUpUp, zDefault, 0.0, 1.0),
                        (xGapsRightRightRight, yMid, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yGapsDownDown, zDefault, 0.0, 1.0),
                        (xGapsLeftLeft, yGapsDownDown, zDefault, Math.PI, 1.0),
                        (xGapsLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xGapsLeft, yGapsUp, zDefault, 0.0, 1.0),
                        (xGapsRight, yGapsUp, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yMid, zDefault, 0.0, 1.0),
                        (xGapsRight, yGapsDown, zDefault, Math.PI, 1.0),
                        (xGapsLeft, yGapsDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // First Qadim phase, packed together except for pyre platforms
                    qadimPhase1Time, 10000, new[]
                    {
                        (xLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (protectionPyre1.x, protectionPyre1.y, zDefault, Math.PI, 1.0),
                        (xRightRight, yUpUp, zDefault, 0.0, 1.0),
                        (stabilityPyre1.x, stabilityPyre1.y, zDefault, Math.PI, 1.0),
                        (xRightRight, yDownDown, zDefault, 0.0, 1.0),
                        (retaliationPyre1.x, retaliationPyre1.y, zDefault, Math.PI, 1.0),
                        (xLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xLeft, yUp, zDefault, 0.0, 1.0),
                        (xRight, yUp, zDefault, Math.PI, 1.0),
                        (xRightRight, yMid, zDefault, 0.0, 1.0),
                        (xRight, yDown, zDefault, Math.PI, 1.0),
                        (xLeft, yDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Destroyer phase, packed together, bigger vertical gap in the middle, 4 platforms hidden
                    destroyerPhaseTime, 15000, new[]
                    {
                        (xDestroyerLeftLeftLeft, yMid, zDefault, 0.0, 1.0),
                        (xGapsLeftLeft, yGapsUpUp, zDefault, Math.PI, hiddenOpacity), // TODO: Unknown position while hidden
                        (xGapsRightRight, yGapsUpUp, zDefault, 0.0, hiddenOpacity), // TODO: Unknown position while hidden
                        (xDestroyerRightRightRight, yMid, zDefault, Math.PI, 1.0),
                        (xGapsRightRight, yGapsDownDown, zDefault, 0.0, hiddenOpacity), // TODO: Unknown position while hidden
                        (xGapsLeftLeft, yGapsDownDown, zDefault, Math.PI, hiddenOpacity), // TODO: Unknown position while hidden
                        (xDestroyerLeftLeft, yMid, zDefault, Math.PI, 1.0),
                        (xDestroyerLeft, yUp, zDefault, 0.0, isCM ? hiddenOpacity : 1.0),
                        (xDestroyerRight, yUp, zDefault, Math.PI, 1.0),
                        (xDestroyerRightRight, yMid, zDefault, 0.0, 1.0),
                        (xDestroyerRight, yDown, zDefault, Math.PI, 1.0),
                        (xDestroyerLeft, yDown, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Second Qadim phase
                    qadimPhase2Time, 10000, new[]
                    {
                        (protectionPyre2.x, protectionPyre2.y, zDefault, 0.0, 1.0),
                        (-8540, 14222, zDefault, Math.PI, 1.0),
                        (stabilityPyre2.x, stabilityPyre2.y, zDefault, 0.0, 1.0),
                        (-5160, yMid, zDefault, Math.PI, 1.0),
                        (retaliationPyre2.x, retaliationPyre2.y, zDefault, 0.0, 1.0),
                        (-8369, 9640, zDefault, Math.PI, 1.0),
                        (protectionPyre2.x + 1939, protectionPyre2.y, zDefault, Math.PI, 1.0),
                        (-7978, 13249, zDefault, 0.0, 1.0),
                        (-6846, 13050, zDefault, Math.PI, 1.0),
                        (-6284, yMid, zDefault, 0.0, 1.0),
                        (retaliationPyre2.x - 1931 / 2, retaliationPyre2.y + 1672, zDefault, Math.PI, 1.0),
                        (-7807, 10613, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // TODO: Heights are not correct, they differ here, currently not important for the replay
                    // Wyvern phase
                    wyvernPhaseTime, 11000, new[]
                    {
                        (protectionPyre2.x, protectionPyre2.y, zDefault, 0.0, hiddenOpacity), // TODO: Unknown position while hidden
                        (-9704, 15323, zDefault, Math.PI, 1.0),
                        (-7425, 15312, zDefault, 0.0, 1.0),
                        (-5160, yMid, zDefault, Math.PI, hiddenOpacity), // TODO: Unknown position while hidden
                        (-5169, 8846, zDefault, 0.0, isCM ? hiddenOpacity : 1.0),
                        (-7414, 8846, zDefault, Math.PI, hiddenOpacity),
                        (-7728, 11535, zDefault, Math.PI + wyvernPhaseMiddleRotation, 1.0),
                        (-9108, 14335, zDefault, 0.0, 1.0),
                        (-7987, 14336, zDefault, Math.PI, 1.0),
                        (-7106, 12619, zDefault, wyvernPhaseMiddleRotation, 1.0),
                        (-5729, 9821, zDefault, Math.PI, 1.0),
                        (-6854, 9821, zDefault, 0.0, 1.0),
                    }
                ),
                (
                    // Jumping puzzle preparation, platforms hide
                    jumpingPuzzleTime - 500, 0, new[]
                    {
                        (protectionPyre2.x, protectionPyre2.y, zDefault, 0.0, hiddenOpacity),
                        (-9704, 15323, zDefault, Math.PI, hiddenOpacity),
                        (-7425, 15312, zDefault, 0.0, hiddenOpacity),
                        (-5160, yMid, zDefault, Math.PI, hiddenOpacity),
                        (-5169, 8846, zDefault, 0.0, hiddenOpacity),
                        (-7414, 8846, zDefault, Math.PI, hiddenOpacity),
                        (-7728, 11535, zDefault, Math.PI + wyvernPhaseMiddleRotation, hiddenOpacity),
                        (-9108, 14335, zDefault, 0.0, hiddenOpacity),
                        (-7987, 14336, zDefault, Math.PI, hiddenOpacity),
                        (-7106, 12619, zDefault, wyvernPhaseMiddleRotation, hiddenOpacity),
                        (-5729, 9821, zDefault, Math.PI, 1.0),
                        (-6854, 9821, zDefault, 0.0, hiddenOpacity),
                    }
                ),
                (
                    // Jumping puzzle, platforms move
                    jumpingPuzzleTime, jumpingPuzzleShuffleDuration - 1, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, hiddenOpacity),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
                (
                    // Jumping puzzle, platforms appear
                    jumpingPuzzleTime + jumpingPuzzleShuffleDuration - 1, 1, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, 1.0),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, 0.0, 1.0),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                    }
                ),
                (
                    // Jumping puzzle appears, platforms rotate...
                    // Jumping puzzle platform breaks are not shown for now because their timing is rather tricky.
                    jumpingPuzzleTime + jumpingPuzzleShuffleDuration, jumpingPuzzleDuration, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, 1.0),
                        (xJumpingPuzzlePreQadim, yMid, zJumpingPuzzlePreQadim, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, 1.0),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, 1.0),
                    }
                ),
                (
                    // Final phase preparation.
                    finalPhaseTime - lastPhasePreparationDuration, lastPhasePreparationDuration, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
                (
                    // Final phase.
                    finalPhaseTime, 0, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, 1.0),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
                (
                    // Second to last platform is destroyed
                    finalPhaseTime, 7000, new[]
                    {
                        (xJumpingPuzzleQadim, yMid, zFinalPlatforms, 0.0, hiddenOpacity),
                        (xJumpingPuzzleFirstRotating, yMid, zDefault, Math.PI + jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid + yJumpingPuzzleOffset3, zJumpingPuzzlePyres, -jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleRotatingPrePyres, yMid - yJumpingPuzzleOffset3, zJumpingPuzzlePyres, jumpingPuzzleDuration / 1000.0 * jumpingPuzzleRotationRate, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xFinalPlatform, yMid, zJumpingPuzzlePreQadim, finalPlatformRotation, 1.0),
                        (xJumpingPuzzlePrePyres, yMid + yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid + yJumpingPuzzleOffset1, zJumpingPuzzleSecondPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePyres, yMid, zJumpingPuzzlePyres, Math.PI, hiddenOpacity),
                        (xJumpingPuzzleFirstPlatform, yMid - yJumpingPuzzleOffset1, zJumpingPuzzleFirstPlatform, Math.PI, hiddenOpacity),
                        (xJumpingPuzzlePrePyres, yMid - yJumpingPuzzleOffset2, zJumpingPuzzlePrePyres, Math.PI, hiddenOpacity),
                    }
                ),
            };

            // All platforms have to have positions in all phases
            Debug.Assert(movements.All(x => x.platforms.Length == platformCount));

            var platforms = new MovingPlatformDecoration[platformCount];
            for (int i = 0; i < platformCount; i++)
            {
                platforms[i] = new MovingPlatformDecoration(platformImageUrl, 2247, 2247, (int.MinValue, int.MaxValue));
                decorations.Add(platforms[i]);
            }

            // Add movement "keyframes" on a movement end and on the start of the next one.
            // This approach requires one extra movement at the start for initial positions (should be of duration 0)
            for (int i = 0; i < movements.Length; i++)
            {
                (int start, int duration, (int x, int y, int z, double angle, double opacity)[] platforms) movement = movements[i];
                (int x, int y, int z, double angle, double opacity)[] positions = movement.platforms;

                for (int platformIndex = 0; platformIndex < platformCount; platformIndex++)
                {
                    MovingPlatformDecoration platform = platforms[platformIndex];
                    (int x, int y, int z, double angle, double opacity) = positions[platformIndex];

                    // Add a keyframe for movement end.
                    platform.AddPosition(x, y, z, angle, opacity, movement.start + movement.duration);

                    if (i != movements.Length - 1)
                    {
                        // Add a keyframe for next movement start to ensure that there is no change
                        // between the end of this movement and the start of the next one
                        platform.AddPosition(x, y, z, angle, opacity, movements[i + 1].start);
                    }
                }
            }
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success)
            {
                if (log.CombatData.GetBuffData(AchievementEligibilityManipulateTheManipulator).Any())
                {
                    InstanceBuffs.AddRange(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityManipulateTheManipulator));
                }
                else if (CustomCheckManipulateTheManipulator(log))
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityManipulateTheManipulator], 1));
                }
            }
        }

        /// <summary>
        /// Check the player positions for the achievement eligiblity.<br></br>
        /// </summary>
        /// <param name="log"></param>
        /// <returns><see langword="true"/> if eligible, otherwise <see langword="false"/>.</returns>
        private static bool CustomCheckTakingTurns(ParsedEvtcLog log)
        {
            // Z coordinates info:
            // The player in the lamp is roughly at -81
            // The death zone from falling off the platform is roughly at -2950
            // The main fight platform is at roughly at -4700

            var lamps = log.AgentData.GetNPCsByID(TrashID.QadimLamp).ToList();
            int lampLabyrinthZ = -250; // Height Threshold

            foreach (Player p in log.PlayerList)
            {
                IReadOnlyList<ParametricPoint3D> positions = p.GetCombatReplayPolledPositions(log);
                var exitBuffs = log.CombatData.GetBuffData(PowerOfTheLamp).OfType<BuffApplyEvent>().Where(x => x.To == p.AgentItem).ToList();

                // Count the times the player has entered and exited the lamp.
                // A player that has entered the lamp but never exites and remains alive is elible for the achievement.

                int entered = 0;
                int exited = 0;

                for (int i = 0; i < lamps.Count; i++)
                {
                    if (positions.Any(x => x.Z > lampLabyrinthZ && x.Time >= lamps[i].FirstAware && x.Time <= lamps[i].LastAware) && entered == exited)
                    {
                        entered++;
                    }

                    var end = i < lamps.Count - 1 ? lamps[i + 1].FirstAware : log.FightData.FightEnd;
                    var segment = new Segment(lamps[i].LastAware, end, 1);

                    if (exitBuffs.Any(x => segment.ContainsPoint(x.Time)))
                    {
                        exited++;
                    }

                    if (entered > 1) { return false; } // Failed achievement
                }
            }

            return true; // Successful achievement
        }

        /// <summary>
        /// Check the NPC positions for the achievement eligiblity.<br></br>
        /// </summary>
        /// <param name="log"></param>
        /// <returns><see langword="true"/> if eligible, otherwise <see langword="false"/>.</returns>
        private static bool CustomCheckManipulateTheManipulator(ParsedEvtcLog log)
        {
            AbstractSingleActor qadim = log.FightData.Logic.Targets.Where(x => x.IsSpecies(TargetID.Qadim)).FirstOrDefault();
            AbstractSingleActor hydra = log.FightData.Logic.Targets.Where(x => x.IsSpecies(TrashID.AncientInvokedHydra)).FirstOrDefault();
            AbstractSingleActor bringer = log.FightData.Logic.Targets.Where(x => x.IsSpecies(TrashID.ApocalypseBringer)).FirstOrDefault();
            AbstractSingleActor matriarch = log.FightData.Logic.Targets.Where(x => x.IsSpecies(TrashID.WyvernMatriarch)).FirstOrDefault();
            AbstractSingleActor patriarch = log.FightData.Logic.Targets.Where(x => x.IsSpecies(TrashID.WyvernPatriarch)).FirstOrDefault();

            if (qadim != null && hydra != null && bringer != null && matriarch != null && patriarch != null)
            {
                return !DistanceCheck(log, qadim, hydra) &&
                    !DistanceCheck(log, qadim, bringer) &&
                    !DistanceCheck(log, qadim, matriarch) &&
                    !DistanceCheck(log, qadim, patriarch);
            }
            
            return false;
        }

        /// <summary>
        /// Find out if the distance points between <paramref name="qadim"/> and an <paramref name="add"/> goes under 2000 range.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="qadim"></param>
        /// <param name="add"></param>
        /// <returns><see langword="true"/> if distance goes under 2000, otherwise <see langword="false"/>.</returns>
        private static bool DistanceCheck(ParsedEvtcLog log, AbstractSingleActor qadim, AbstractSingleActor add)
        {
            // Get positions of Ancient Invoked Hydra, Apocalypse Bringer, Wyvern Matriarch and Patriarch
            var addPositions = add.GetCombatReplayPolledPositions(log).ToList();
            if (addPositions.Count == 0)
            {
                return true;
            }
            // Get positions of Qadim during the times of the adds being present
            var qadimPositions = qadim.GetCombatReplayPolledPositions(log).Where(x => x.Time >= addPositions.First().Time && x.Time <= addPositions.Last().Time).ToList();
            if (qadimPositions.Count == 0)
            {
                return true;
            }

            // For each matching position polled, check if the distance between points is under 2000
            for (int i = 0; i < Math.Min(addPositions.Count, qadimPositions.Count); i++)
            {
                if (qadimPositions[i].DistanceToPoint(addPositions[i]) < 2000)
                {
                    return true;
                }
            }

            // Never went under 2000 range
            return false;
        }
    }
}
