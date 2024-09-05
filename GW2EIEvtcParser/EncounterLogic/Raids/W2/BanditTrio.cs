using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class BanditTrio : SalvationPass
    {
        private static readonly Point3D ChestOfPrisonCampPosition = new Point3D(-903.703f, -9450.76f, -126.277008f);
        public BanditTrio(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new PlayerDstBuffApplyMechanic(ShellShocked, "Shell-Shocked", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Launched", "Shell-Shocked (Launched from pad)", "Shell-Shocked", 0),
            new PlayerDstBuffApplyMechanic(Blind, "Blinded", new MechanicPlotlySetting(Symbols.X, Colors.White), "Blinded", "Blinded by Zane", "Blinded", 0).UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Zane)),
            new PlayerDstBuffApplyMechanic(Burning, "Burning", new MechanicPlotlySetting(Symbols.StarOpen, Colors.Red), "Burning", "Burned by Narella", "Burning", 0).UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.Narella)),
            new PlayerDstBuffApplyMechanic(SlowBurn, "Slow Burn", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.LightPurple), "SlowBurn.A", "Received Slow Burn", "Slow Burn Application", 0),
            new PlayerSrcBuffApplyMechanic(Targeted, "Targeted", new MechanicPlotlySetting(Symbols.StarSquare, Colors.Pink), "Targeted.B", "Applied Targeted Buff (Berg)", "Targeted Application (Berg)", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TargetID.Berg)),
            new PlayerSrcBuffApplyMechanic(Targeted, "Targeted", new MechanicPlotlySetting(Symbols.StarSquare, Colors.Purple), "Targeted.A", "Applied Targeted Buff (Any)", "Targeted Application (Any)", 0),
            new PlayerSrcBuffApplyMechanic(SapperBombDamageBuff, "Sapper Bomb", new MechanicPlotlySetting(Symbols.CircleCross, Colors.Green), "Hit Cage", "Hit Cage with Sapper Bomb", "Hit Cage (Sapper Bomb)", 0).UsingChecker((bae, log) => bae.To.IsSpecies(TrashID.Cage)),
            new PlayerCastStartMechanic(ThrowOilKeg, "Throw", new MechanicPlotlySetting(Symbols.Hourglass, Colors.LightRed), "OilKeg.T", "Threw Oil Keg", "Oil Keg Throw", 0),
            new PlayerCastStartMechanic(Beehive, "Beehive", new MechanicPlotlySetting(Symbols.Pentagon, Colors.Yellow), "Beehive.T", "Threw Beehive", "Beehive Throw", 0),
            new PlayerSrcHitMechanic(Beehive, "Beehive", new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.Yellow), "Beehive.H.B", "Beehive Hits (Berg)", "Beehive Hit (Berg)", 0).UsingChecker((ahde, log) => ahde.To.IsSpecies(TargetID.Berg)),
            new PlayerSrcHitMechanic(Beehive, "Beehive", new MechanicPlotlySetting(Symbols.PentagonOpen, Colors.LightOrange), "Beehive.H.A", "Beehive Hits (Any)", "Beehive Hit (Any)", 0),
            new PlayerDstHitMechanic(OverheadSmashBerg, "Overhead Smash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Smash","Overhead Smash (CC Attack Berg)", "CC Smash",0),
            new PlayerDstHitMechanic(HailOfBulletsZane, "Hail of Bullets", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "Zane Cone","Hail of Bullets (Zane Cone Shot)", "Hail of Bullets",0),
            new PlayerDstHitMechanic(FieryVortexNarella, "Fiery Vortex", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            new PlayerDstHitMechanic(FlakShotNarella, "Flak SHot", new MechanicPlotlySetting(Symbols.Diamond, Colors.LightRed), "Flak", "Flak Shot (Narella)", "Flak Shot Hit", 0),
            });
            Extension = "trio";
            GenericFallBackMethod = FallBackMethod.ChestGadget;
            ChestID = ChestID.ChestOfPrisonCamp;
            Icon = EncounterIconBanditTrio;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000002;
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                (int)TargetID.Narella
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.Berg,
                (int)TargetID.Zane,
                (int)TargetID.Narella
            };
        }

        protected override List<int> GetFriendlyNPCIDs()
        {
            return new List<int>
            {
                (int)TrashID.Cage
            };
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayBanditTrio,
                            (1000, 913),
                            (-2900, -12251, 2561, -7265)/*,
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210)*/);
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                startToUse = long.MaxValue;
                if (!agentData.TryGetFirstAgentItem(TargetID.Berg, out AgentItem berg))
                {
                    throw new MissingKeyActorsException("Berg not found");
                }
                startToUse = Math.Min(berg.FirstAware, startToUse);
                if (!agentData.TryGetFirstAgentItem(TargetID.Zane, out AgentItem zane))
                {
                    throw new MissingKeyActorsException("Zane not found");
                }
                startToUse = Math.Min(zane.FirstAware, startToUse);
                if (!agentData.TryGetFirstAgentItem(TargetID.Narella, out AgentItem narella))
                {
                    throw new MissingKeyActorsException("Narella not found");
                }
                startToUse = Math.Min(narella.FirstAware, startToUse);
                // Thrash mob start check
                var boxStart = new Point2D(-2200, -11300);
                var boxEnd = new Point2D(1000, -7200);
                var trashMobsToCheck = new HashSet<TrashID>()
                {
                    TrashID.BanditAssassin,
                    TrashID.BanditAssassin2,
                    TrashID.BanditSapperTrio,
                    TrashID.BanditDeathsayer,
                    TrashID.BanditDeathsayer2,
                    TrashID.BanditBrawler,
                    TrashID.BanditBrawler2,
                    TrashID.BanditBattlemage,
                    TrashID.BanditBattlemage2,
                    TrashID.BanditCleric,
                    TrashID.BanditCleric2,
                    TrashID.BanditBombardier,
                    TrashID.BanditSniper,
                };
                var banditSniperPositions = combatData.Where(x => x.IsStateChange == StateChange.Position && agentData.GetAgent(x.SrcAgent, x.Time).IsAnySpecies(trashMobsToCheck)).Select(x => new PositionEvent(x, agentData)).ToList();
                var banditsInBox = new HashSet<AgentItem>(banditSniperPositions.Where(x => x.Time < startToUse + 10000 && Point3D.IsInBox2D(x.GetPoint3D(), boxStart, boxEnd)).Select(x => x.Src));
                if (banditsInBox.Count > 0)
                {
                    startToUse = Math.Min(banditsInBox.Min(x => x.FirstAware), startToUse);
                }
            }
            return startToUse;
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Cage
            AgentItem cage = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 224100 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 238 && x.HitboxHeight == 300).FirstOrDefault();
            if (cage != null)
            {
                cage.OverrideType(AgentItem.AgentType.NPC);
                cage.OverrideID(TrashID.Cage);
            }

            // Bombs
            var bombs = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 0 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 240).ToList();
            foreach (AgentItem bomb in bombs)
            {
                bomb.OverrideType(AgentItem.AgentType.NPC);
                bomb.OverrideID(TrashID.Bombs);
            }

            // Reward Chest
            FindChestGadget(ChestID, agentData, combatData, ChestOfPrisonCampPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));

            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (TargetHPPercentUnderThreshold(TargetID.Berg, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            if (agentData.TryGetFirstAgentItem(TargetID.Berg, out AgentItem berg) && combatData.GetLogNPCUpdateEvents().Count > 0)
            {
                var movements = combatData.GetMovementData(berg).Where(x => x.Time > berg.FirstAware + MinimumInCombatDuration).ToList();
                if (movements.Count != 0)
                {
                    AbstractMovementEvent firstMove = movements.First();
                    // two minutes
                    if (firstMove.Time < 120000)
                    {
                        return FightData.EncounterStartStatus.Late;
                    }
                }
            }
            return FightData.EncounterStartStatus.Normal;
        }

        private static void SetPhasePerTarget(AbstractSingleActor target, List<PhaseData> phases, ParsedEvtcLog log)
        {
            EnterCombatEvent phaseStart = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
            if (phaseStart != null)
            {
                long start = phaseStart.Time;
                DeadEvent phaseEnd = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                long end = log.FightData.FightEnd;
                if (phaseEnd != null)
                {
                    end = phaseEnd.Time;
                }
                var phase = new PhaseData(start, Math.Min(end, log.FightData.FightEnd));
                phase.AddTarget(target);
                switch (target.ID)
                {
                    case (int)TargetID.Narella:
                        phase.Name = "Narella";
                        break;
                    case (int)TargetID.Berg:
                        phase.Name = "Berg";
                        break;
                    case (int)TargetID.Zane:
                        phase.Name = "Zane";
                        break;
                    default:
                        throw new MissingKeyActorsException("Unknown target in Bandit Trio");
                }
                phases.Add(phase);
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)TargetID.Berg,
                (int)TargetID.Zane,
                (int)TargetID.Narella
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor berg = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Berg)) ?? throw new MissingKeyActorsException("Berg not found");
            AbstractSingleActor zane = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Zane)) ?? throw new MissingKeyActorsException("Zane not found");
            AbstractSingleActor narella = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Narella)) ?? throw new MissingKeyActorsException("Narella not found");
            phases[0].AddTargets(Targets);
            if (!requirePhases)
            {
                return phases;
            }
            foreach (AbstractSingleActor target in Targets)
            {
                SetPhasePerTarget(target, phases, log);
            }
            return phases;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                TrashID.BanditSaboteur,
                TrashID.Warg,
                TrashID.VeteranTorturedWarg,
                TrashID.BanditAssassin,
                TrashID.BanditAssassin2,
                TrashID.BanditSapperTrio,
                TrashID.BanditDeathsayer,
                TrashID.BanditDeathsayer2,
                TrashID.BanditBrawler,
                TrashID.BanditBrawler2,
                TrashID.BanditBattlemage,
                TrashID.BanditBattlemage2,
                TrashID.BanditCleric,
                TrashID.BanditCleric2,
                TrashID.BanditBombardier,
                TrashID.BanditSniper,
                TrashID.NarellaTornado,
                TrashID.OilSlick,
                TrashID.Prisoner1,
                TrashID.Prisoner2,
                TrashID.InsectSwarm,
                TrashID.Bombs,
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Bandit Trio";
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)TargetID.Berg:
                    var overheadSmash = cls.Where(x => x.SkillId == OverheadSmashBerg).ToList();
                    foreach (AbstractCastEvent c in overheadSmash)
                    {
                        uint radius = 550;
                        int angle = 80;
                        (long, long) lifespan = (c.Time, c.Time + c.ActualDuration);
                        Point3D facing = target.GetCurrentRotation(log, lifespan.Item1 + 600, lifespan.Item2);
                        if (facing != null)
                        {
                            var rotationConnector = new AngleConnector(facing);
                            var agentConnector = new AgentConnector(target);
                            var cone = (PieDecoration)new PieDecoration(radius, angle, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(rotationConnector);
                            replay.AddDecorationWithGrowing(cone, lifespan.Item2);
                        }
                    }
                    break;
                case (int)TargetID.Zane:
                    var bulletHail = cls.Where(x => x.SkillId == HailOfBulletsZane).ToList();
                    foreach (AbstractCastEvent c in bulletHail)
                    {
                        long start = c.Time;
                        long firstConeStart = start;
                        long secondConeStart = start + 800;
                        long thirdConeStart = start + 1600;
                        long firstConeEnd = firstConeStart + 400;
                        long secondConeEnd = secondConeStart + 400;
                        long thirdConeEnd = thirdConeStart + 400;
                        uint radius = 1500;
                        Point3D facing = target.GetCurrentRotation(log, start);
                        if (facing != null)
                        {
                            var connector = new AgentConnector(target);
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new PieDecoration(radius, 28, (firstConeStart, firstConeEnd), Colors.Yellow, 0.3, connector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(radius, 54, (secondConeStart, secondConeEnd), Colors.Yellow, 0.3, connector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(radius, 81, (thirdConeStart, thirdConeEnd), Colors.Yellow, 0.3, connector).UsingRotationConnector(rotationConnector));
                        }
                    }
                    break;

                case (int)TargetID.Narella:
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
            // Sapper bombs
            var sapperBombs = player.GetBuffStatus(log, SapperBombBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in sapperBombs)
            {
                var circle = new CircleDecoration(180, seg, "rgba(200, 255, 100, 0.5)", new AgentConnector(player));
                replay.AddDecorationWithFilledWithGrowing(circle.UsingFilled(false), true, seg.Start + 5000);
                replay.AddOverheadIcon(seg, player, ParserIcons.BombOverhead);
            }
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && log.CombatData.GetBuffData(EnvironmentallyFriendly).Any())
            {
                InstanceBuffs.AddRange(GetOnPlayerCustomInstanceBuff(log, EnvironmentallyFriendly));
            }
        }
    }
}
