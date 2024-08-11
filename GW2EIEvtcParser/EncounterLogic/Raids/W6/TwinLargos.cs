using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TwinLargos : MythwrightGambit
    {
        public TwinLargos(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstBuffApplyMechanic(Waterlogged, "Waterlogged", new MechanicPlotlySetting(Symbols.HexagonOpen,Colors.LightBlue), "Debuff","Waterlogged (stacking water debuff)", "Waterlogged",0),
            new PlayerDstHitMechanic(VaporRush, "Vapor Rush", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightBlue), "Charge","Vapor Rush (Triple Charge)", "Vapor Rush Charge",0),
            new PlayerDstHitMechanic(TidalPoolSkill, "Tidal Pool", new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Pool","Tidal Pool", "Tidal Pool",0),
            new EnemyCastStartMechanic(AquaticBarrage, "Aquatic Barrage Start", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Breakbar", "Breakbar",0),
            new EnemyCastEndMechanic(AquaticBarrage, "Aquatic Barrage End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Breakbar broken", "CCed",0),
            new PlayerDstHitMechanic(SeaSwell, "Sea Swell", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkPurple), "Wave","Sea Swell (Shockwave)", "Shockwave",0),
            new PlayerDstHitMechanic(Geyser, "Geyser", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Teal), "KB/Launch","Geyser (Launching Aoes)", "Launch Field",0),
            new PlayerDstBuffApplyMechanic(TidalPoolBuff, "Tidal Pool", new MechanicPlotlySetting(Symbols.Diamond,Colors.Teal), "Poison","Expanding Water Field", "Water Poison",0),
            new PlayerDstHitMechanic(AquaticDetainmentHit, "Aquatic Detainment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Float","Aquatic Detainment (Float Bubble)", "Float Bubble",6000),
            new PlayerDstHitMechanic(AquaticVortex, "Aquatic Vortex", new MechanicPlotlySetting(Symbols.StarSquareOpenDot,Colors.LightBlue), "Tornado","Aquatic Vortex (Water Tornados)", "Tornado",0),
            new PlayerDstHitMechanic(VaporJet, "Vapor Jet", new MechanicPlotlySetting(Symbols.Square,Colors.Pink), "Steal","Vapor Jet (Boon Steal)", "Boon Steal",0),
            new EnemyDstBuffApplyMechanic(EnragedTwinLargos, "Enraged", new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Red), "Enrage","Enraged", "Enrage",0),
            new PlayerDstBuffApplyMechanic(AquaticAuraKenut, "Aquatic Aura Kenut", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Teal), "Ken Aura","Increasing Damage Debuff on Kenut's Last Platform", "Kenut Aura Debuff",0),
            new PlayerDstBuffApplyMechanic(AquaticAuraNikare, "Aquatic Aura Nikare", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Teal), "Nik Aura","Increasing Damage Debuff on Nikare's Last Platform", "Nikare Aura Debuff",0),
            new PlayerDstHitMechanic(CycloneBurst, "Cyclone Burst", new MechanicPlotlySetting(Symbols.YUpOpen,Colors.Pink), "Y Field","Cyclone Burst (triangular rotating fields on Kenut)", "Cyclone Burst",0),
            });
            Extension = "twinlargos";
            Icon = EncounterIconTwinLargos;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayTwinLargos,
                            (765, 1000),
                            (10846, -3878, 18086, 5622)/*,
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256)*/);
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Nikare,
                (int)ArcDPSEnums.TargetID.Kenut
            };
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(NikareAquaticAura, NikareAquaticAura),
                new DamageCastFinder(KenutAquaticAura, KenutAquaticAura),
            };
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Nikare,
                (int)ArcDPSEnums.TargetID.Kenut
            };
        }

        internal override List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            NegateDamageAgainstBarrier(combatData, Targets.Select(x => x.AgentItem).ToList());
            return new List<AbstractHealthDamageEvent>();
        }

        private static List<PhaseData> GetTargetPhases(ParsedEvtcLog log, AbstractSingleActor target, string baseName)
        {
            long start = 0;
            long end = 0;
            long fightEnd = log.FightData.FightEnd;
            var targetPhases = new List<PhaseData>();
            var states = new List<AbstractTimeCombatEvent>();
            states.AddRange(log.CombatData.GetEnterCombatEvents(target.AgentItem));
            states.AddRange(GetFilteredList(log.CombatData, Determined762, target, true, true).Where(x => x is BuffApplyEvent));
            states.AddRange(log.CombatData.GetDeadEvents(target.AgentItem));
            states.Sort((x, y) => x.Time.CompareTo(y.Time));
            for (int i = 0; i < states.Count; i++)
            {
                AbstractTimeCombatEvent state = states[i];
                if (state is EnterCombatEvent)
                {
                    start = state.Time;
                    if (i == states.Count - 1)
                    {
                        targetPhases.Add(new PhaseData(start, fightEnd));
                    }
                }
                else
                {
                    end = Math.Min(state.Time, fightEnd);
                    targetPhases.Add(new PhaseData(start, end));
                    if (i == states.Count - 1 && targetPhases.Count < 3)
                    {
                        targetPhases.Add(new PhaseData(end, fightEnd));
                    }
                }
            }
            targetPhases.RemoveAll(x => x.DurationInMS < ParserHelper.PhaseTimeLimit);
            for (int i = 0; i < targetPhases.Count; i++)
            {
                PhaseData phase = targetPhases[i];
                phase.Name = baseName + " P" + (i + 1);
                phase.AddTarget(target);
            }
            return targetPhases;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Kenut,
                (int)ArcDPSEnums.TargetID.Nikare
            };
        }

        private static void FallBackPhases(AbstractSingleActor target, List<PhaseData> phases, ParsedEvtcLog log, bool firstPhaseAt0)
        {
            IReadOnlyCollection<AgentItem> pAgents = log.PlayerAgents;
            // clean Nikare/Kenut missing enter combat events related bugs
            switch (phases.Count)
            {
                case 2:
                    {
                        PhaseData p1 = phases[0];
                        PhaseData p2 = phases[1];
                        // P1 and P2 merged
                        if (p1.Start == p2.Start)
                        {
                            AbstractHealthDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p1.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                            if (hit != null)
                            {
                                p2.OverrideStart(hit.Time);
                                p2.Name += " (Fallback)";
                            }
                            else
                            {
                                p2.OverrideStart(p1.End);
                                p2.Name += " (Bad Fallback)";
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        PhaseData p1 = phases[0];
                        PhaseData p2 = phases[1];
                        PhaseData p3 = phases[2];
                        // P1 and P2 merged
                        if (p1.Start == p2.Start)
                        {
                            AbstractHealthDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p1.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                            if (hit != null)
                            {
                                p2.OverrideStart(hit.Time);
                                p2.Name += " (Fallback)";
                            }
                            else
                            {
                                p2.OverrideStart(p1.End);
                                p2.Name += " (Bad Fallback)";
                            }
                        }
                        // P1/P2 and P3 are merged
                        if (p1.Start == p3.Start || p2.Start == p3.Start)
                        {
                            AbstractHealthDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p2.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                            if (hit != null)
                            {
                                p3.OverrideStart(hit.Time);
                                p3.Name += " (Fallback)";
                            }
                            else
                            {
                                p3.OverrideStart(p2.End);
                                p3.Name += " (Bad Fallback)";
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            if (!firstPhaseAt0 && phases.Count > 0 && phases.First().Start == 0)
            {
                PhaseData p1 = phases[0];
                AbstractHealthDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= 0 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                if (hit != null)
                {
                    p1.OverrideStart(hit.Time);
                    p1.Name += " (Fallback)";
                }
                else
                {
                    p1.Name += " (Bad Fallback)";
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor nikare = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Nikare)) ?? throw new MissingKeyActorsException("Nikare not found");
            phases[0].AddTarget(nikare);
            AbstractSingleActor kenut = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Kenut));
            if (kenut != null)
            {
                phases[0].AddTarget(kenut);
            }
            if (!requirePhases)
            {
                return phases;
            }
            List<PhaseData> nikPhases = GetTargetPhases(log, nikare, "Nikare");
            FallBackPhases(nikare, nikPhases, log, true);
            phases.AddRange(nikPhases);
            if (kenut != null)
            {
                List<PhaseData> kenPhases = GetTargetPhases(log, kenut, "Kenut");
                FallBackPhases(kenut, kenPhases, log, false);
                phases.AddRange(kenPhases);
            }
            return phases;
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            if (TargetHPPercentUnderThreshold(ArcDPSEnums.TargetID.Kenut, fightData.FightStart, combatData, Targets) ||
                TargetHPPercentUnderThreshold(ArcDPSEnums.TargetID.Nikare, fightData.FightStart, combatData, Targets))
            {
                return FightData.EncounterStartStatus.Late;
            }
            return FightData.EncounterStartStatus.Normal;
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            ComputeFightTargets(agentData, combatData, extensions);
            // discard hp update events after determined apply
            AbstractSingleActor nikare = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Nikare)) ?? throw new MissingKeyActorsException("Nikare not found");
            var nikareHPUpdates = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.SrcMatchesAgent(nikare.AgentItem)).ToList();
            if (nikareHPUpdates.Any(x => HealthUpdateEvent.GetHealthPercent(x) != 100 && HealthUpdateEvent.GetHealthPercent(x) != 0))
            {
                CombatItem lastHPUpdate = nikareHPUpdates.Last();
                if (lastHPUpdate.DstAgent == 10000)
                {
                    lastHPUpdate.OverrideSrcAgent(0);
                }
            }
            AbstractSingleActor kenut = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Kenut));
            if (kenut != null)
            {
                var kenutHPUpdates = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.SrcMatchesAgent(kenut.AgentItem)).ToList();
                if (kenutHPUpdates.Any(x => HealthUpdateEvent.GetHealthPercent(x) != 100 && HealthUpdateEvent.GetHealthPercent(x) != 0))
                {
                    CombatItem lastHPUpdate = kenutHPUpdates.Last();
                    if (lastHPUpdate.DstAgent == 10000)
                    {
                        lastHPUpdate.OverrideSrcAgent(0);
                    }
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Nikare:
                    //CC
                    var barrageN = cls.Where(x => x.SkillId == AquaticBarrage).ToList();
                    foreach (AbstractCastEvent c in barrageN)
                    {
                        replay.Decorations.Add(new CircleDecoration(250, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    //Platform wipe (CM only)
                    var aquaticDomainN = cls.Where(x => x.SkillId == AquaticDomain).ToList();
                    foreach (AbstractCastEvent c in aquaticDomainN)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        uint radius = 800;
                        replay.Decorations.Add(new CircleDecoration(radius, (start, end), Colors.Yellow, 0.3, new AgentConnector(target)).UsingGrowingEnd(end));
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.Kenut:
                    //CC
                    var barrageK = cls.Where(x => x.SkillId == AquaticBarrage).ToList();
                    foreach (AbstractCastEvent c in barrageK)
                    {
                        replay.Decorations.Add(new CircleDecoration(250, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    //Platform wipe (CM only)
                    var aquaticDomainK = cls.Where(x => x.SkillId == AquaticDomain).ToList();
                    foreach (AbstractCastEvent c in aquaticDomainK)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        uint radius = 800;
                        replay.Decorations.Add(new CircleDecoration(radius, (start, end), Colors.Yellow, 0.3, new AgentConnector(target)).UsingGrowingEnd(end));
                    }
                    var shockwave = cls.Where(x => x.SkillId == SeaSwell).ToList();
                    foreach (AbstractCastEvent c in shockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 960;
                        int duration = 3000;
                        uint radius = 1200;
                        (long, long) lifespan = (start + delay, start + delay + duration);
                        GeographicalConnector connector = new AgentConnector(target);
                        replay.AddShockwave(connector, lifespan, Colors.SkyBlue, 0.5, radius);
                    }
                    var boonSteal = cls.Where(x => x.SkillId == VaporJet).ToList();
                    foreach (AbstractCastEvent c in boonSteal)
                    {
                        int start = (int)c.Time;
                        int delay = 1000;
                        int duration = 500;
                        uint width = 500;
                        uint height = 250;
                        Point3D facing = target.GetCurrentRotation(log, start);
                        if (facing != null)
                        {
                            var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new Point3D(width / 2, 0), true);
                            var rotationConnextor = new AngleConnector(facing);
                            replay.AddDecorationWithBorder((RectangleDecoration)new RectangleDecoration(width, height, (start + delay, start + delay + duration), Colors.LightOrange, 0.4, positionConnector).UsingRotationConnector(rotationConnextor));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // Water "Poison Bomb"
            var waterToDrop = p.GetBuffStatus(log, TidalPoolBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in waterToDrop)
            {
                int timer = 5000;
                int duration = 83000;
                uint debuffRadius = 100;
                uint radius = 500;
                int toDropStart = (int)seg.Start;
                int toDropEnd = (int)seg.End;
                replay.AddDecorationWithFilledWithGrowing(new CircleDecoration(debuffRadius, seg, Colors.Orange, 0.4, new AgentConnector(p)).UsingFilled(false), true, toDropStart + timer);
                Point3D position = p.GetCurrentInterpolatedPosition(log, toDropEnd);
                if (position != null)
                {
                    replay.AddDecorationWithGrowing(new CircleDecoration(radius, debuffRadius, (toDropEnd, toDropEnd + duration), Colors.DarkWhite, 0.5, new PositionConnector(position)).UsingFilled(false), toDropStart + duration);
                }
                replay.AddOverheadIcon(seg, p, ParserIcons.TidalPoolOverhead);
            }
            // Bubble (Aquatic Detainment)
            var bubble = p.GetBuffStatus(log, AquaticDetainmentBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            uint bubbleRadius = 100;
            foreach (Segment seg in bubble)
            {
                replay.Decorations.Add(new CircleDecoration(bubbleRadius, seg, Colors.LightBlue, 0.3, new AgentConnector(p)));
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Twin Largos";
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor nikare = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Nikare)) ?? throw new MissingKeyActorsException("Nikare not found");
            bool nikareHasCastAquaticDomain = combatData.GetAnimatedCastData(nikare.AgentItem).Any(x => x.SkillId == AquaticDomain);
            if (nikareHasCastAquaticDomain) // aquatic domain only present in CM
            {
                return FightData.EncounterMode.CM;
            }
            FightData.EncounterMode mode = (nikare.GetHealth(combatData) > 18e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal; //Health of Nikare;
            AbstractSingleActor kenut = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Kenut));
            if (kenut != null)
            {
                if (combatData.GetAnimatedCastData(kenut.AgentItem).Any(x => x.SkillId == AquaticDomain)) // aquatic domain only present in CM
                {
                    return FightData.EncounterMode.CM;
                }
                if (mode != FightData.EncounterMode.CM && kenut.GetHealth(combatData) > 16e6) // Health of Kenut
                {
                    mode = FightData.EncounterMode.CM;
                }
                if (mode == FightData.EncounterMode.CM && combatData.GetDamageTakenData(kenut.AgentItem).Any(x => x.CreditedFrom.IsPlayer && x.HealthDamage > 0) && !nikareHasCastAquaticDomain) // Kenut engaged but nikare never cast Aquatic Domain -> normal mode
                {
                    mode = FightData.EncounterMode.Normal;
                }
            }
            if (mode == FightData.EncounterMode.CM && combatData.GetHealthUpdateEvents(nikare.AgentItem).Any(x => x.HealthPercent < 70) && !nikareHasCastAquaticDomain) // Nikare went below 70% but never cast Aquatic Domain
            {
                mode = FightData.EncounterMode.Normal;
            }
            return mode;
        }
    }
}
