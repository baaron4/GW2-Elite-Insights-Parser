using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TwinLargos : MythwrightGambit
    {
        public TwinLargos(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(51935, "Waterlogged", new MechanicPlotlySetting("hexagon-open",Colors.LightBlue), "Debuff","Waterlogged (stacking water debuff)", "Waterlogged",0),
            new HitOnPlayerMechanic(52876, "Vapor Rush", new MechanicPlotlySetting("triangle-left-open",Colors.LightBlue), "Charge","Vapor Rush (Triple Charge)", "Vapor Rush Charge",0),
            new HitOnPlayerMechanic(52812, "Tidal Pool", new MechanicPlotlySetting("circle",Colors.LightBlue), "Pool","Tidal Pool", "Tidal Pool",0),
            new EnemyCastStartMechanic(51977, "Aquatic Barrage Start", new MechanicPlotlySetting("diamond-tall",Colors.DarkTeal), "CC","Breakbar", "Breakbar",0),
            new EnemyCastEndMechanic(51977, "Aquatic Barrage End", new MechanicPlotlySetting("diamond-tall",Colors.DarkGreen), "CCed","Breakbar broken", "CCed",0),
            new HitOnPlayerMechanic(53018, "Sea Swell", new MechanicPlotlySetting("circle-open",Colors.DarkPurple), "Wave","Sea Swell (Shockwave)", "Shockwave",0),
            new HitOnPlayerMechanic(53130, "Geyser", new MechanicPlotlySetting("hexagon",Colors.Teal), "KB/Launch","Geyser (Launching Aoes)", "Launch Field",0),
            new PlayerBuffApplyMechanic(53097, "Water Bomb Debuff", new MechanicPlotlySetting("diamond",Colors.Teal), "Poison","Expanding Water Field", "Water Poison",0),
            new HitOnPlayerMechanic(52931, "Aquatic Detainment", new MechanicPlotlySetting("circle-open",Colors.Blue), "Float","Aquatic Detainment (Float Bubble)", "Float Bubble",6000),
            new HitOnPlayerMechanic(52130, "Aquatic Vortex", new MechanicPlotlySetting("star-square-open-dot",Colors.LightBlue), "Tornado","Aquatic Vortex (Water Tornados)", "Tornado",0),
            new HitOnPlayerMechanic(51965, "Vapor Jet", new MechanicPlotlySetting("square",Colors.Pink), "Steal","Vapor Jet (Boon Steal)", "Boon Steal",0),
            new EnemyBuffApplyMechanic(52626, "Enraged", new MechanicPlotlySetting("star-diamond",Colors.Red), "Enrage","Enraged", "Enrage",0),
            new PlayerBuffApplyMechanic(52211, "Aquatic Aura Kenut", new MechanicPlotlySetting("square-open",Colors.Teal), "Ken Aura","Increasing Damage Debuff on Kenut's Last Platform", "Kenut Aura Debuff",0),
            new PlayerBuffApplyMechanic(52929, "Aquatic Aura Nikare", new MechanicPlotlySetting("diamond-open",Colors.Teal), "Nik Aura","Increasing Damage Debuff on Nikare's Last Platform", "Nikare Aura Debuff",0),
            new HitOnPlayerMechanic(51999, "Cyclone Burst", new MechanicPlotlySetting("y-up-open",Colors.Pink), "Y Field","Cyclone Burst (triangular rotating fields on Kenut)", "Cyclone Burst",0),
            });
            Extension = "twinlargos";
            Icon = "https://i.imgur.com/6O5MT7v.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/O8wIWds.png",
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
                new DamageCastFinder(52779, 52779, InstantCastFinder.DefaultICD), // Nikare Aquatic Aura
                new DamageCastFinder(52005, 52005, InstantCastFinder.DefaultICD), // Kenut Aquatic Aura
            };
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return GetTargetsIDs();
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
            long fightDuration = log.FightData.FightEnd;
            var targetPhases = new List<PhaseData>();
            var states = new List<AbstractTimeCombatEvent>();
            states.AddRange(log.CombatData.GetEnterCombatEvents(target.AgentItem));
            states.AddRange(GetFilteredList(log.CombatData, 762, target, true).Where(x => x is BuffApplyEvent));
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
                        targetPhases.Add(new PhaseData(start, fightDuration));
                    }
                }
                else
                {
                    end = Math.Min(state.Time, fightDuration);
                    targetPhases.Add(new PhaseData(start, end));
                    if (i == states.Count - 1 && targetPhases.Count < 3)
                    {
                        targetPhases.Add(new PhaseData(end, fightDuration));
                    }
                }
            }
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
            // clean Nikare related bugs
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
                            }
                            else
                            {
                                p2.OverrideStart(p1.End);
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
                            }
                            else
                            {
                                p2.OverrideStart(p1.End);
                            }
                        }
                        // P1/P2 and P3 are merged
                        if (p1.Start == p3.Start || p2.Start == p3.Start)
                        {
                            AbstractHealthDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p2.End + 5000 && pAgents.Contains(x.From.GetFinalMaster()) && x.HealthDamage > 0 && x is DirectHealthDamageEvent);
                            if (hit != null)
                            {
                                p3.OverrideStart(hit.Time);
                            }
                            else
                            {
                                p3.OverrideStart(p2.End);
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
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor nikare = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Nikare);
            if (nikare == null)
            {
                throw new MissingKeyActorsException("Nikare not found");
            }
            phases[0].AddTarget(nikare);
            AbstractSingleActor kenut = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Kenut);
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

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Nikare:
                    //CC
                    var barrageN = cls.Where(x => x.SkillId == 51977).ToList();
                    foreach (AbstractCastEvent c in barrageN)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Platform wipe (CM only)
                    var aquaticDomainN = cls.Where(x => x.SkillId == 52374).ToList();
                    foreach (AbstractCastEvent c in aquaticDomainN)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        int radius = 800;
                        replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(255, 255, 0, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TargetID.Kenut:
                    //CC
                    var barrageK = cls.Where(x => x.SkillId == 51977).ToList();
                    foreach (AbstractCastEvent c in barrageK)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Platform wipe (CM only)
                    var aquaticDomainK = cls.Where(x => x.SkillId == 52374).ToList();
                    foreach (AbstractCastEvent c in aquaticDomainK)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        int radius = 800;
                        replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(255, 255, 0, 0.3)", new AgentConnector(target)));
                    }
                    var shockwave = cls.Where(x => x.SkillId == 53018).ToList();
                    foreach (AbstractCastEvent c in shockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 960;
                        int duration = 3000;
                        int radius = 1200;
                        replay.Decorations.Add(new CircleDecoration(false, start + delay + duration, radius, (start + delay, start + delay + duration), "rgba(100, 200, 255, 0.5)", new AgentConnector(target)));
                    }
                    var boonSteal = cls.Where(x => x.SkillId == 51965).ToList();
                    foreach (AbstractCastEvent c in boonSteal)
                    {
                        int start = (int)c.Time;
                        int delay = 1000;
                        int duration = 500;
                        int width = 500;
                        int height = 250;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing != null)
                        {
                            float rotation = Point3D.GetRotationFromFacing(facing);
                            replay.Decorations.Add(new RotatedRectangleDecoration(false, 0, width, height, rotation, width / 2, (start + delay, start + delay + duration), "rgba(255, 175, 0, 0.8)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, rotation, width / 2, (start + delay, start + delay + duration), "rgba(255, 175, 0, 0.2)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Water "Poison Bomb"
            List<AbstractBuffEvent> waterToDrop = GetFilteredList(log.CombatData, 53097, p, true);
            int toDropStart = 0;
            foreach (AbstractBuffEvent c in waterToDrop)
            {
                int timer = 5000;
                int duration = 83000;
                int debuffRadius = 100;
                int radius = 500;
                if (c is BuffApplyEvent)
                {
                    toDropStart = (int)c.Time;
                }
                else
                {
                    int toDropEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(false, 0, debuffRadius, (toDropStart, toDropEnd), "rgba(255, 100, 0, 0.4)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, toDropStart + timer, debuffRadius, (toDropStart, toDropEnd), "rgba(255, 100, 0, 0.4)", new AgentConnector(p)));
                    Point3D poisonNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= toDropEnd);
                    Point3D poisonPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= toDropEnd);
                    if (poisonNextPos != null || poisonPrevPos != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, toDropStart + duration, radius, (toDropEnd, toDropEnd + duration), "rgba(100, 100, 100, 0.3)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), debuffRadius));
                        replay.Decorations.Add(new CircleDecoration(false, toDropStart + duration, radius, (toDropEnd, toDropEnd + duration), "rgba(230, 230, 230, 0.4)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), debuffRadius));
                    }
                }
            }
            // Bubble (Aquatic Detainment)
            List<AbstractBuffEvent> bubble = GetFilteredList(log.CombatData, 51755, p, true);
            int bubbleStart = 0;
            foreach (AbstractBuffEvent c in bubble)
            {
                int radius = 100;
                if (c is BuffApplyEvent)
                {
                    bubbleStart = Math.Max((int)c.Time, 0);
                }
                else
                {
                    int bubbleEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, radius, (bubbleStart, bubbleEnd), "rgba(0, 200, 255, 0.3)", new AgentConnector(p)));
                }
            }
        }

        internal override string GetLogicName(ParsedEvtcLog log)
        {
            return "Twin Largos";
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Nikare);
            if (target == null)
            {
                throw new MissingKeyActorsException("Nikare not found");
            }
            return (target.GetHealth(combatData) > 18e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM; //Health of Nikare
        }
    }
}
