using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Adina : TheKeyOfAhdashim
    {
        public Adina(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerDstBuffApplyMechanic(RadiantBlindness, "Radiant Blindness", new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerDstBuffApplyMechanic(ErodingCurse, "Eroding Curse", new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
                new PlayerDstHitMechanic(BoulderBarrage, "Boulder Barrage", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Red), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
                new PlayerDstHitMechanic(PerilousPulse, "Perilous Pulse", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Pink), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0).UsingChecker( (de, log) => !de.To.HasBuff(log, Stability, de.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(StalagmitesDetonation, "Stalagmites", new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Mines", "Hit by mines", "Mines", 0),
                new PlayerDstHitMechanic(DiamondPalisadeEye, "Diamond Palisade", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Pink), "Eye", "Looked at Eye", "Looked at Eye", 0),
                new PlayerDstSkillMechanic(new long[] { DoubleRotatingEarthRays, TripleRotatingEarthRays }, "Quantum Quake", new MechanicPlotlySetting(Symbols.Hourglass,Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0).UsingChecker((de, log) => de.HasKilled),
            });
            Extension = "adina";
            Icon = EncounterIconAdina;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(SeismicSuffering, SeismicSuffering), // Seismic Suffering
            };
        }

        internal override FightLogic AdjustLogic(AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogNPCUpdate);
            // Handle potentially wrongly associated logs
            if (logStartNPCUpdate != null)
            {
                if (agentData.GetNPCsByID(ArcDPSEnums.TargetID.Sabir).Any(sabir => combatData.Any(evt => evt.IsDamagingDamage() && evt.DstMatchesAgent(sabir) && agentData.GetAgent(evt.SrcAgent, evt.Time).GetFinalMaster().IsPlayer)))
                {
                    return new Sabir((int)ArcDPSEnums.TargetID.Sabir);
                }
            }
            return base.AdjustLogic(agentData, combatData);
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var attackTargetEvents = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).Select(x => new AttackTargetEvent(x, agentData)).ToList();
            var targetableEvents = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable).Select(x => new TargetableEvent(x, agentData)).Where(x => x.Src.Type == AgentItem.AgentType.Gadget).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
            attackTargetEvents.RemoveAll(x =>
            {
                AgentItem atAgent = x.AttackTarget;
                if (targetableEvents.TryGetValue(atAgent, out List<TargetableEvent> targetables))
                {
                    return !targetables.Any(y => y.Targetable);
                }
                return true;
            });
            long final = fightData.FightEnd;
            var handOfEruptionPositions = new List<Point3D> { new Point3D(15570.5f, -693.117f), new Point3D(14277.2f, -2202.52f) };
            var processedAttackTargets = new HashSet<AgentItem>();
            foreach (AttackTargetEvent attackTargetEvent in attackTargetEvents)
            {
                AgentItem atAgent = attackTargetEvent.AttackTarget;
                if (processedAttackTargets.Contains(atAgent) || !targetableEvents.TryGetValue(atAgent, out List<TargetableEvent> targetables))
                {
                    continue;
                }
                processedAttackTargets.Add(atAgent);
                AgentItem hand = attackTargetEvent.Src;
                var copyEventsFrom = new List<AgentItem>() { hand };
                var attackOns = targetables.Where(x => x.Targetable).ToList();
                var attackOffs = targetables.Where(x => !x.Targetable).ToList();
                CombatItem posEvt = combatData.FirstOrDefault(x => x.SrcMatchesAgent(hand) && x.IsStateChange == ArcDPSEnums.StateChange.Position);
                ArcDPSEnums.TrashID id = ArcDPSEnums.TrashID.HandOfErosion;
                if (posEvt != null)
                {
                    Point3D pos = AbstractMovementEvent.GetPoint3D(posEvt);
                    if (handOfEruptionPositions.Any(x => x.Distance2DToPoint(pos) < InchDistanceThreshold))
                    {
                        id = ArcDPSEnums.TrashID.HandOfEruption;
                    }
                }
                foreach (TargetableEvent targetableEvent in attackOns)
                {
                    long start = targetableEvent.Time;
                    long end = final;
                    TargetableEvent attackOff = attackOffs.FirstOrDefault(x => x.Time > start);
                    if (attackOff != null)
                    {
                        end = attackOff.Time;
                    }
                    AgentItem extra = agentData.AddCustomNPCAgent(start, end, hand.Name, hand.Spec, id, false, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                    RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, hand, copyEventsFrom, extra, true);
                }
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Adina,
                (int)ArcDPSEnums.TrashID.HandOfErosion,
                (int)ArcDPSEnums.TrashID.HandOfEruption
            };
        }

        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                { (int)ArcDPSEnums.TargetID.Adina, 0 },
                { (int)ArcDPSEnums.TrashID.HandOfErosion, 1 },
                { (int)ArcDPSEnums.TrashID.HandOfEruption, 1 },
            };
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            var radiantBlindnesses = p.GetBuffStatus(log, RadiantBlindness, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in radiantBlindnesses)
            {
                replay.Decorations.Add(new CircleDecoration(90, seg, "rgba(200, 0, 200, 0.3)", new AgentConnector(p)));
            }
        }


        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Adina:
                    var doubleQuantumQuakes = cls.Where(x => x.SkillId == DoubleRotatingEarthRays).ToList();
                    foreach (AbstractCastEvent c in doubleQuantumQuakes)
                    {
                        long start = c.Time;
                        int preCastTime = 2990; // casttime 0
                        int duration = c.ActualDuration;
                        uint width = 1100; uint height = 60;
                        foreach (int angle in new List<int> { 90, 270 })
                        {
                            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(width / 2, 0), true);
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start, start + preCastTime), Colors.Orange, 0.2, positionConnector).UsingRotationConnector(new AngleConnector(angle)));
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start + preCastTime, start + duration), Colors.Red, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(angle, 360)));
                        }
                    }
                    //
                    var tripleQuantumQuakes = cls.Where(x => x.SkillId == TripleRotatingEarthRays).ToList();
                    foreach (AbstractCastEvent c in tripleQuantumQuakes)
                    {
                        long start = c.Time;
                        int preCastTime = 2990; // casttime 0
                        int duration = c.ActualDuration;
                        uint width = 1100; uint height = 60;
                        foreach (int angle in new List<int> { 30, 150, 270 })
                        {
                            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(width / 2, 0), true);
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start, start + preCastTime), Colors.Orange, 0.2, positionConnector).UsingRotationConnector(new AngleConnector(angle)));
                            replay.Decorations.Add(new RectangleDecoration(width, height, (start + preCastTime, start + duration), Colors.Red, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(angle, 360)));
                        }

                    }
                    //
                    var terraforms = cls.Where(x => x.SkillId == Terraform).ToList();
                    foreach (AbstractCastEvent c in terraforms)
                    {
                        long start = c.Time;
                        int delay = 2000; // casttime 0 from skill def
                        int duration = 5000;
                        uint radius = 1100;
                        (long, long) lifespan = (start + delay, start + duration);
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.AddShockwave(connector, lifespan, Colors.LightOrange, 0.6, radius);
                    }
                    //
                    var diamondPalisades = target.GetBuffStatus(log, DiamondPalisade, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in diamondPalisades)
                    {
                        replay.Decorations.Add(new CircleDecoration(90, seg, Colors.Red, 0.2, new AgentConnector(target)));
                    }
                    //
                    var boulderBarrages = cls.Where(x => x.SkillId == BoulderBarrage).ToList();
                    foreach (AbstractCastEvent c in boulderBarrages)
                    {
                        long start = c.Time;
                        int duration = 4600; // cycle 3 from skill def
                        uint radius = 1100;
                        replay.Decorations.Add(new CircleDecoration(radius, (start, start + duration), Colors.LightOrange, 0.4, new AgentConnector(target)).UsingGrowingEnd(start + duration));
                    }
                    break;
                default:
                    break;

            }
        }


        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Adina)) ?? throw new MissingKeyActorsException("Adina not found");
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Split phases
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, Determined762, mainTarget, true, true);
            long start = 0;
            var splitPhases = new List<PhaseData>();
            var splitPhaseEnds = new List<long>();
            for (int i = 0; i < invuls.Count; i++)
            {
                PhaseData splitPhase;
                AbstractBuffEvent be = invuls[i];
                if (be is BuffApplyEvent)
                {
                    start = be.Time;
                    if (i == invuls.Count - 1)
                    {
                        splitPhase = new PhaseData(start, log.FightData.FightEnd, "Split " + (i / 2 + 1));
                        splitPhaseEnds.Add(log.FightData.FightEnd);
                        AddTargetsToPhaseAndFit(splitPhase, new List<int> { (int)ArcDPSEnums.TrashID.HandOfErosion, (int)ArcDPSEnums.TrashID.HandOfEruption }, log);
                        splitPhases.Add(splitPhase);
                    }
                }
                else
                {
                    long end = be.Time;
                    splitPhase = new PhaseData(start, end, "Split " + (i / 2 + 1));
                    splitPhaseEnds.Add(end);
                    AddTargetsToPhaseAndFit(splitPhase, new List<int> { (int)ArcDPSEnums.TrashID.HandOfErosion, (int)ArcDPSEnums.TrashID.HandOfEruption }, log);
                    splitPhases.Add(splitPhase);
                }
            }
            // Main phases
            var mainPhases = new List<PhaseData>();
            var pillarApplies = log.CombatData.GetBuffDataByIDByDst(PillarPandemonium, mainTarget.AgentItem).OfType<BuffApplyEvent>().ToList();
            Dictionary<long, List<BuffApplyEvent>> pillarAppliesGroupByTime = GroupByTime(pillarApplies);
            var mainPhaseEnds = new List<long>();
            foreach (KeyValuePair<long, List<BuffApplyEvent>> pair in pillarAppliesGroupByTime)
            {
                if (pair.Value.Count == 6)
                {
                    mainPhaseEnds.Add(pair.Key);
                }
            }
            AbstractCastEvent boulderBarrage = mainTarget.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).FirstOrDefault(x => x.SkillId == BoulderBarrage && x.Time < 6000);
            start = boulderBarrage == null ? 0 : boulderBarrage.EndTime;
            if (mainPhaseEnds.Count != 0)
            {
                int phaseIndex = 1;
                foreach (long quantumQake in mainPhaseEnds)
                {
                    long curPhaseStart = splitPhaseEnds.LastOrDefault(x => x < quantumQake);
                    if (curPhaseStart == 0)
                    {
                        curPhaseStart = start;
                    }
                    long nextPhaseStart = splitPhaseEnds.FirstOrDefault(x => x > quantumQake);
                    if (nextPhaseStart != 0)
                    {
                        start = nextPhaseStart;
                        phaseIndex = splitPhaseEnds.IndexOf(start) + 1;
                    }
                    mainPhases.Add(new PhaseData(curPhaseStart, quantumQake, "Phase " + phaseIndex));
                }
                if (start != mainPhases.Last().Start)
                {
                    mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase " + (phaseIndex + 1)));
                }
            }
            else if (start > 0 && invuls.Count == 0)
            {
                // no split
                mainPhases.Add(new PhaseData(start, log.FightData.FightEnd, "Phase 1"));
            }

            foreach (PhaseData phase in mainPhases)
            {
                phase.AddTarget(mainTarget);
            }
            phases.AddRange(mainPhases);
            phases.AddRange(splitPhases);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            //
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            var map = new CombatReplayMap(CombatReplayAdinaMainPhase1,
                            (866, 1000),
                            (13840, -2698, 15971, -248)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
            //
            try
            {
                var splitPhasesMap = new List<string>()
                {
                        CombatReplayAdinaSplitPhase1,
                        CombatReplayAdinaSplitPhase2,
                        CombatReplayAdinaSplitPhase3,
                };
                var mainPhasesMap = new List<string>()
                {
                        CombatReplayAdinaMainPhase1,
                        CombatReplayAdinaMainPhase2,
                        CombatReplayAdinaMainPhase3,
                        CombatReplayAdinaMainPhase4
                };
                var crMaps = new List<string>();
                int mainPhaseIndex = 0;
                int splitPhaseIndex = 0;
                var phases = log.FightData.GetPhases(log).Where(x => !x.BreakbarPhase).ToList();
                var mainPhases = phases.Where(x => x.Name.Contains("Phase")).ToList();
                for (int i = 1; i < phases.Count; i++)
                {
                    PhaseData phaseData = phases[i];
                    if (mainPhases.Contains(phaseData))
                    {
                        if (mainPhasesMap.Contains(crMaps.LastOrDefault()))
                        {
                            splitPhaseIndex++;
                        }
                        crMaps.Add(mainPhasesMap[mainPhaseIndex++]);
                    }
                    else
                    {
                        if (splitPhasesMap.Contains(crMaps.LastOrDefault()))
                        {
                            mainPhaseIndex++;
                        }
                        crMaps.Add(splitPhasesMap[splitPhaseIndex++]);
                    }
                }
                map.MatchMapsToPhases(crMaps, phases, log.FightData.FightEnd);
            }
            catch (Exception)
            {
                log.UpdateProgressWithCancellationCheck("Parsing: Failed to associate Adina Combat Replay maps");
            }
            //
            return map;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Adina)) ?? throw new MissingKeyActorsException("Adina not found");
            return (target.GetHealth(combatData) > 23e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && log.CombatData.GetBuffData(AchievementEligibilityConserveTheLand).Any())
            {
                InstanceBuffs.AddRange(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityConserveTheLand));
            }
        }
    }
}
