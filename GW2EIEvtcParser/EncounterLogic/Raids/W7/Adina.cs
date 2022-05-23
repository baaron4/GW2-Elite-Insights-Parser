using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Adina : TheKeyOfAhdashim
    {
        public Adina(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerBuffApplyMechanic(RadiantBlindness, "Radiant Blindness", new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "R.Blind", "Unremovable blindness", "Radiant Blindness", 0),
                new PlayerBuffApplyMechanic(ErodingCurse, "Eroding Curse", new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Curse", "Stacking damage debuff from Hand of Erosion", "Eroding Curse", 0),
                new HitOnPlayerMechanic(56648, "Boulder Barrage", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Red), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
                new HitOnPlayerMechanic(56390, "Perilous Pulse", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Pink), "Perilous Pulse", "Perilous Pulse", "Perilous Pulse", 0, (de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
                new HitOnPlayerMechanic(56141, "Stalagmites", new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Mines", "Hit by mines", "Mines", 0),
                new HitOnPlayerMechanic(56114, "Diamond Palisade", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Pink), "Eye", "Looked at Eye", "Looked at Eye", 0),
                new SkillOnPlayerMechanic(new long[] {56035, 56381 }, "Quantum Quake", new MechanicPlotlySetting(Symbols.Hourglass,Colors.Brown), "S.Thrower", "Hit by rotating SandThrower", "SandThrower", 0, (de, log) => de.HasKilled),
            });
            Extension = "adina";
            Icon = "https://wiki.guildwars2.com/images/a/a0/Mini_Earth_Djinn.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(56351, 56351, InstantCastFinder.DefaultICD), // Seismic Suffering
            };
        }
        
        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            var attackTargets = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget).ToList();
            long first = 0;
            long final = fightData.FightEnd;
            foreach (CombatItem at in attackTargets)
            {
                AgentItem hand = agentData.GetAgent(at.DstAgent, at.Time);
                AgentItem atAgent = agentData.GetAgent(at.SrcAgent, at.Time);
                var attackables = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.SrcMatchesAgent(atAgent)).ToList();
                var attackOn = attackables.Where(x => x.DstAgent == 1 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var attackOff = attackables.Where(x => x.DstAgent == 0 && x.Time >= first + 2000).Select(x => x.Time).ToList();
                var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(hand) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                CombatItem pos = posFacingHP.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Position);
                int id = (int)ArcDPSEnums.TrashID.HandOfErosion;
                if (pos != null)
                {
                    (float x, float y, _) = AbstractMovementEvent.UnpackMovementData(pos.DstAgent, 0);
                    if ((Math.Abs(x - 15570.5) < 10 && Math.Abs(y + 693.117) < 10) ||
                            (Math.Abs(x - 14277.2) < 10 && Math.Abs(y + 2202.52) < 10))
                    {
                        id = (int)ArcDPSEnums.TrashID.HandOfEruption;
                    }
                }
                for (int i = 0; i < attackOn.Count; i++)
                {
                    long start = attackOn[i];
                    long end = final;
                    if (i <= attackOff.Count - 1)
                    {
                        end = attackOff[i];
                    }
                    AgentItem extra = agentData.AddCustomNPCAgent(start, end, hand.Name, hand.Spec, id, false, hand.Toughness, hand.Healing, hand.Condition, hand.Concentration, hand.HitboxWidth, hand.HitboxHeight);
                    foreach (CombatItem c in combatData)
                    {
                        if (extra.InAwareTimes(c.Time))
                        {
                            if (c.SrcMatchesAgent(hand, extensions))
                            {
                                c.OverrideSrcAgent(extra.Agent);
                            }
                            if (c.DstMatchesAgent(hand, extensions))
                            {
                                c.OverrideDstAgent(extra.Agent);
                            }
                        }
                    }
                    foreach (CombatItem c in posFacingHP)
                    {
                        var cExtra = new CombatItem(c);
                        cExtra.OverrideTime(extra.FirstAware);
                        cExtra.OverrideSrcAgent(extra.Agent);
                        combatData.Add(cExtra);
                    }
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

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractBuffEvent> radiantBlindnesses = GetFilteredList(log.CombatData, RadiantBlindness, p, true, true);
            int radiantBlindnessStart = 0;
            foreach (AbstractBuffEvent c in radiantBlindnesses)
            {
                if (c is BuffApplyEvent)
                {
                    radiantBlindnessStart = (int)c.Time;
                }
                else
                {
                    replay.Decorations.Add(new CircleDecoration(true, 0, 90, (radiantBlindnessStart, (int)c.Time), "rgba(200, 0, 200, 0.3)", new AgentConnector(p)));
                }
            }
        }


        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Adina:
                    var doubleQuantumQuakes = cls.Where(x => x.SkillId == 56035).ToList();
                    foreach (AbstractCastEvent c in doubleQuantumQuakes)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 2990; // casttime 0
                        int duration = c.ActualDuration;
                        int width = 1100; int height = 60;
                        foreach (int angle in new List<int> { 90, 270 })
                        {
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, angle, width / 2, (start, start + preCastTime), "rgba(255, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, angle, width / 2, 360, (start + preCastTime, start + duration), "rgba(255, 50, 0, 0.5)", new AgentConnector(target)));
                        }
                    }
                    //
                    var tripleQuantumQuakes = cls.Where(x => x.SkillId == 56381).ToList();
                    foreach (AbstractCastEvent c in tripleQuantumQuakes)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 2990; // casttime 0
                        int duration = c.ActualDuration;
                        int width = 1100; int height = 60;
                        foreach (int angle in new List<int> { 30, 150, 270})
                        {
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, angle, width / 2, (start, start + preCastTime), "rgba(255, 100, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, angle, width / 2, 360, (start + preCastTime, start + duration), "rgba(255, 50, 0, 0.5)", new AgentConnector(target)));
                        }

                    }
                    //
                    var terraforms = cls.Where(x => x.SkillId == 56049).ToList();
                    foreach (AbstractCastEvent c in terraforms)
                    {
                        int start = (int)c.Time;
                        int delay = 2000; // casttime 0 from skill def
                        int duration = 5000;
                        int radius = 1100;
                        replay.Decorations.Add(new CircleDecoration(false, start + duration, radius, (start + delay, start + duration), "rgba(255, 150, 0, 0.7)", new AgentConnector(target)));
                    }
                    //
                    List<AbstractBuffEvent> diamondPalisades = GetFilteredList(log.CombatData, DiamondPalisade, target, true, true);
                    int diamondPalisadeStart = 0;
                    foreach (AbstractBuffEvent c in diamondPalisades)
                    {
                        if (c is BuffApplyEvent)
                        {
                            diamondPalisadeStart = (int)c.Time;
                        }
                        else
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, 90, (diamondPalisadeStart, (int)c.Time), "rgba(200, 0, 0, 0.3)", new AgentConnector(target)));
                        }
                    }
                    //
                    var boulderBarrages = cls.Where(x => x.SkillId == 56648).ToList();
                    foreach (AbstractCastEvent c in boulderBarrages)
                    {
                        int start = (int)c.Time;
                        int duration = 4600; // cycle 3 from skill def
                        int radius = 1100;
                        replay.Decorations.Add(new CircleDecoration(true, start + duration, radius, (start, start + duration), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    }
                    break;
                default:
                    break;

            }
        }


        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Adina not found");
            }
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
            var pillarApplies = log.CombatData.GetBuffData(PillarPandemonium).OfType<BuffApplyEvent>().Where(x => x.To == mainTarget.AgentItem).ToList();
            Dictionary<long, List<BuffApplyEvent>> pillarAppliesGroupByTime = ParserHelper.GroupByTime(pillarApplies);
            var mainPhaseEnds = new List<long>();
            foreach (KeyValuePair<long, List<BuffApplyEvent>> pair in pillarAppliesGroupByTime)
            {
                if (pair.Value.Count == 6)
                {
                    mainPhaseEnds.Add(pair.Key);
                }
            }
            AbstractCastEvent boulderBarrage = mainTarget.GetCastEvents(log, 0, log.FightData.FightEnd).FirstOrDefault(x => x.SkillId == 56648 && x.Time < 6000);
            start = boulderBarrage == null ? 0 : boulderBarrage.EndTime;
            if (mainPhaseEnds.Any())
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
            else if (start > 0 && !invuls.Any())
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
            try
            {
                var splitPhasesMap = new List<string>()
                {
                        "https://i.imgur.com/gJ55jKy.png",
                        "https://i.imgur.com/c2Oz5bj.png",
                        "https://i.imgur.com/P4SGbrc.png",
                };
                var mainPhasesMap = new List<string>()
                {
                        "https://i.imgur.com/IQn2RJV.png",
                        "https://i.imgur.com/3pO7eCB.png",
                        "https://i.imgur.com/ZFw590w.png",
                        "https://i.imgur.com/2P7UE8q.png"
                };
                var crMaps = new List<string>();
                int mainPhaseIndex = 0;
                int splitPhaseIndex = 0;
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
                GetCombatReplayMap(log).MatchMapsToPhases(crMaps, phases, log.FightData.FightEnd);
            } 
            catch(Exception)
            {
                log.UpdateProgressWithCancellationCheck("Failed to associate Adina Combat Replay maps");
            }
            
            //
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/IQn2RJV.png",
                            (866, 1000),
                            (13840, -2698, 15971, -248)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Adina);
            if (target == null)
            {
                throw new MissingKeyActorsException("Adina not found");
            }
            return (target.GetHealth(combatData) > 23e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
