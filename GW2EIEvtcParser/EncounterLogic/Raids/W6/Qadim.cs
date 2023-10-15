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

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Qadim : MythwrightGambit
    {

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
                (int)TrashID.WyvernMatriarch,
                (int)TrashID.WyvernPatriarch,
                (int)TrashID.ApocalypseBringer,
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

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<AgentItem> pyres = agentData.GetNPCsByID(TrashID.PyreGuardian);
            // Lamps
            var lampAgents = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 202).ToList();
            foreach (AgentItem lamp in lampAgents)
            {
                lamp.OverrideType(AgentItem.AgentType.NPC);
                lamp.OverrideID(TrashID.QadimLamp);
            }
            bool refresh = lampAgents.Count > 0;
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
            ComputeFightTargets(agentData, combatData, extensions);
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
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(BurningCrucible, BurningCrucible), // Burning Crucible
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
                    var pyres = new List<ArcDPSEnums.TrashID>
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

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
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
            AddPlatformsToCombatReplay(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Qadim)), log, EnvironmentDecorations);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            int ccRadius = 200;
            switch (target.ID)
            {
                case (int)TargetID.Qadim:
                    //CC
                    var breakbar = cls.Where(x => x.SkillId == QadimCC).ToList();
                    foreach (AbstractCastEvent c in breakbar)
                    {
                        int radius = ccRadius;
                        replay.Decorations.Add(new CircleDecoration(true, 0, ccRadius, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Riposte
                    var riposte = cls.Where(x => x.SkillId == QadimRiposte).ToList();
                    foreach (AbstractCastEvent c in riposte)
                    {
                        int radius = 2200;
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, ((int)c.Time, (int)c.EndTime), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    }
                    //Big Hit
                    var maceShockwave = cls.Where(x => x.SkillId == BigHit && x.Status != AbstractCastEvent.AnimationStatus.Interrupted).ToList();
                    foreach (AbstractCastEvent c in maceShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 2230;
                        int duration = 2680;
                        int radius = 2000;
                        int impactRadius = 40;
                        int spellCenterDistance = 300;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D targetPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && targetPosition != null)
                        {
                            var position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z);
                            replay.Decorations.Add(new CircleDecoration(true, 0, impactRadius, (start, start + delay), "rgba(255, 100, 0, 0.2)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, impactRadius, (start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.7)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(false, start + delay + duration, radius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.7)", new PositionConnector(position)));
                        }
                    }
                    break;
                case (int)TrashID.AncientInvokedHydra:
                    //CC
                    var fieryMeteor = cls.Where(x => x.SkillId == FieryMeteor).ToList();
                    foreach (AbstractCastEvent c in fieryMeteor)
                    {
                        int radius = ccRadius;
                        replay.Decorations.Add(new CircleDecoration(true, 0, ccRadius, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    var eleBreath = cls.Where(x => x.SkillId == ElementalBreath).ToList();
                    foreach (AbstractCastEvent c in eleBreath)
                    {
                        int start = (int)c.Time;
                        int radius = 1300;
                        int delay = 2600;
                        int duration = 1000;
                        int openingAngle = 70;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (start + delay, start + delay + duration), "rgba(255, 180, 0, 0.3)", new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
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
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        int range = 2800;
                        int span = 2400;
                        if (facing != null)
                        {
                            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(range / 2, 0), true);
                            var rotationConnextor = new AngleConnector(facing);
                            replay.Decorations.Add(new RectangleDecoration(true, 0, range, span, (start, start + preCast), "rgba(0,100,255,0.2)", positionConnector).UsingRotationConnector(rotationConnextor));
                            replay.Decorations.Add(new RectangleDecoration(true, 0, range, span, (start + preCast, start + duration), "rgba(0,100,255,0.5)", positionConnector).UsingRotationConnector(rotationConnextor));
                        }
                    }
                    //Breath
                    var matBreath = cls.Where(x => x.SkillId == FireBreath).ToList();
                    foreach (AbstractCastEvent c in matBreath)
                    {
                        int start = (int)c.Time;
                        int radius = 1000;
                        int delay = 1600;
                        int duration = 3000;
                        int openingAngle = 70;
                        int fieldDuration = 10000;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D pos = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && pos != null)
                        {
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.3)", new AgentConnector(target)).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (start + delay + duration, start + delay + fieldDuration), "rgba(255, 50, 0, 0.3)", new PositionConnector(pos)).UsingRotationConnector(rotationConnector));
                        }
                    }
                    //Tail Swipe
                    var matSwipe = cls.Where(x => x.SkillId == TailSwipe).ToList();
                    foreach (AbstractCastEvent c in matSwipe)
                    {
                        int start = (int)c.Time;
                        int maxRadius = 700;
                        int radiusDecrement = 100;
                        int delay = 1435;
                        int openingAngle = 59;
                        int angleIncrement = 60;
                        int coneAmount = 4;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            float initialAngle = Point3D.GetRotationFromFacing(facing);
                            var connector = new AgentConnector(target);
                            for (int i = 0; i < coneAmount; i++)
                            {
                                var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                replay.Decorations.Add(new PieDecoration(false, 0, maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), "rgba(255, 255, 0, 0.6)", connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(true, 0, maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), "rgba(255, 180, 0, 0.3)", connector).UsingRotationConnector(rotationConnector));

                            }
                        }
                    }
                    break;
                case (int)TrashID.WyvernPatriarch:
                    //CC
                    var patCC = cls.Where(x => x.SkillId == PatriarchCC).ToList();
                    foreach (AbstractCastEvent c in patCC)
                    {
                        int radius = ccRadius;
                        replay.Decorations.Add(new CircleDecoration(true, 0, ccRadius, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Breath
                    var patBreath = cls.Where(x => x.SkillId == FireBreath).ToList();
                    foreach (AbstractCastEvent c in patBreath)
                    {
                        int start = (int)c.Time;
                        int radius = 1000;
                        int delay = 1600;
                        int duration = 3000;
                        int openingAngle = 60;
                        int fieldDuration = 10000;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D pos = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && pos != null)
                        {
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.3)", new AgentConnector(target)).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, openingAngle, (start + delay + duration, start + delay + fieldDuration), "rgba(255, 50, 0, 0.3)", new PositionConnector(pos)).UsingRotationConnector(rotationConnector));
                        }
                    }
                    //Tail Swipe
                    var patSwipe = cls.Where(x => x.SkillId == TailSwipe).ToList();
                    foreach (AbstractCastEvent c in patSwipe)
                    {
                        int start = (int)c.Time;
                        int maxRadius = 700;
                        int radiusDecrement = 100;
                        int delay = 1435;
                        int openingAngle = 59;
                        int angleIncrement = 60;
                        int coneAmount = 4;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            float initialAngle = Point3D.GetRotationFromFacing(facing);
                            var connector = new AgentConnector(target);
                            for (int i = 0; i < coneAmount; i++)
                            {
                                var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                replay.Decorations.Add(new PieDecoration(false, 0, maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), "rgba(255, 255, 0, 0.6)", connector).UsingRotationConnector(rotationConnector));
                                replay.Decorations.Add(new PieDecoration(true, 0, maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), "rgba(255, 180, 0, 0.3)", connector).UsingRotationConnector(rotationConnector));
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
                        int maxRadius = 2000;
                        replay.Decorations.Add(new CircleDecoration(false, start + delay + duration, maxRadius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(target)));
                    }
                    var stompShockwave = cls.Where(x => x.SkillId == SeismicStomp).ToList();
                    foreach (AbstractCastEvent c in stompShockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 1600;
                        int duration = 3500;
                        int maxRadius = 2000;
                        int impactRadius = 500;
                        int spellCenterDistance = 270; //hitbox radius
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D targetPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && targetPosition != null)
                        {
                            var position = new Point3D(targetPosition.X + facing.X * spellCenterDistance, targetPosition.Y + facing.Y * spellCenterDistance, targetPosition.Z);
                            replay.Decorations.Add(new CircleDecoration(true, 0, impactRadius, (start, start + delay), "rgba(255, 100, 0, 0.1)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, impactRadius, (start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(false, start + delay + duration, maxRadius, (start + delay, start + delay + duration), "rgba(255, 200, 0, 0.5)", new PositionConnector(position)));
                        }
                    }
                    //CC
                    var summon = cls.Where(x => x.SkillId == SummonDestroyer).ToList();
                    foreach (AbstractCastEvent c in summon)
                    {
                        int radius = ccRadius;
                        replay.Decorations.Add(new CircleDecoration(true, 0, ccRadius, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Pizza
                    var forceWave = cls.Where(x => x.SkillId == WaveOfForce).ToList();
                    foreach (AbstractCastEvent c in forceWave)
                    {
                        int start = (int)c.Time;
                        int maxRadius = 1000;
                        int radiusDecrement = 200;
                        int delay = 1560;
                        int openingAngle = 44;
                        int angleIncrement = 45;
                        int coneAmount = 3;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            float initialAngle = Point3D.GetRotationFromFacing(facing);
                            var connector = new AgentConnector(target);
                            for (int i = 0; i < coneAmount; i++)
                            {
                                var rotationConnector = new AngleConnector(initialAngle - (i * angleIncrement));
                                replay.Decorations.Add(new PieDecoration(false, 0, maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), "rgba(255, 255, 0, 0.6)", connector));
                                replay.Decorations.Add(new PieDecoration(true, 0, maxRadius - (i * radiusDecrement), openingAngle, (start, start + delay), "rgba(255, 180, 0, 0.3)", connector).UsingRotationConnector(rotationConnector));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
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

        private static void AddPlatformsToCombatReplay(AbstractSingleActor qadim, ParsedEvtcLog log, List<GenericDecoration> decorations)
        {
            // We later use the target to find out the timing of the last move
            Debug.Assert(qadim.IsSpecies(TargetID.Qadim));

            // These values were all calculated by hand.
            // It would be way nicer to calculate them here, but we don't have a nice vector library
            // and it would double the amount of work.

            const string platformImageUrl = "https://i.imgur.com/DbXr5Fo.png";
            const double hiddenOpacity = 0.2;

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
                if (log.CombatData.GetBuffData(AchievementEligibilityTakingTurns).Any()) { CheckAchievementBuff(log, AchievementEligibilityTakingTurns); }
                if (log.CombatData.GetBuffData(AchievementEligibilityManipulateTheManipulator).Any()) { CheckAchievementBuff(log, AchievementEligibilityManipulateTheManipulator); }
            }
        }

        private void CheckAchievementBuff(ParsedEvtcLog log, long achievement)
        {
            foreach (Player p in log.PlayerList)
            {
                if (p.HasBuff(log, achievement, log.FightData.FightEnd - ServerDelayConstant))
                {
                    InstanceBuffs.Add((log.Buffs.BuffsByIds[achievement], 1));
                    break;
                }
            }
        }
    }
}
