using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Logic
{
    public class TwinLargos : RaidLogic
    {
        public TwinLargos(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBuffApplyMechanic(51935, "Waterlogged", new MechanicPlotlySetting("hexagon-open","rgb(0,140,255)"), "Debuff","Waterlogged (stacking water debuff)", "Waterlogged",0),
            new HitOnPlayerMechanic(52876, "Vapor Rush", new MechanicPlotlySetting("triangle-left-open","rgb(0,140,255)"), "Charge","Vapor Rush (Triple Charge)", "Vapor Rush Charge",0),
            new HitOnPlayerMechanic(52812, "Tidal Pool", new MechanicPlotlySetting("circle","rgb(0,140,255)"), "Pool","Tidal Pool", "Tidal Pool",0),
            new EnemyCastStartMechanic(51977, "Aquatic Barrage Start", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Breakbar", "Breakbar",0),
            new EnemyCastEndMechanic(51977, "Aquatic Barrage End", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Breakbar broken", "CCed",0),
            new HitOnPlayerMechanic(53018, "Sea Swell", new MechanicPlotlySetting("circle-open","rgb(30,30,80)"), "Wave","Sea Swell (Shockwave)", "Shockwave",0),
            new HitOnPlayerMechanic(53130, "Geyser", new MechanicPlotlySetting("hexagon","rgb(0,255,255)"), "KB/Launch","Geyser (Launching Aoes)", "Launch Field",0),
            new PlayerBuffApplyMechanic(53097, "Water Bomb Debuff", new MechanicPlotlySetting("diamond","rgb(0,255,255)"), "Poison","Expanding Water Field", "Water Poison",0),
            new HitOnPlayerMechanic(52931, "Aquatic Detainment", new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Float","Aquatic Detainment (Float Bubble)", "Float Bubble",6000),
            new HitOnPlayerMechanic(52130, "Aquatic Vortex", new MechanicPlotlySetting("star-square-open-dot","rgb(0,200,255)"), "Tornado","Aquatic Vortex (Water Tornados)", "Tornado",0),
            new HitOnPlayerMechanic(51965, "Vapor Jet", new MechanicPlotlySetting("square","rgb(255,150,0)"), "Steal","Vapor Jet (Boon Steal)", "Boon Steal",0),
            new EnemyBuffApplyMechanic(52626, "Enraged", new MechanicPlotlySetting("star-diamond","rgb(255,0,0)"), "Enrage","Enraged", "Enrage",0),
            new PlayerBuffApplyMechanic(52211, "Aquatic Aura Kenut", new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "Ken Aura","Increasing Damage Debuff on Kenut's Last Platform", "Kenut Aura Debuff",0),
            new PlayerBuffApplyMechanic(52929, "Aquatic Aura Nikare", new MechanicPlotlySetting("diamond-open","rgb(0,255,255)"), "Nik Aura","Increasing Damage Debuff on Nikare's Last Platform", "Nikare Aura Debuff",0),
            new HitOnPlayerMechanic(51999, "Cyclone Burst", new MechanicPlotlySetting("y-up-open","rgb(255,150,0)"), "Y Field","Cyclone Burst (triangular rotating fields on Kenut)", "Cyclone Burst",0),
            });
            Extension = "twinlargos";
            Icon = "https://i.imgur.com/6O5MT7v.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/JOoJRXM.png",
                            (3205, 4191),
                            (10846, -3878, 18086, 5622),
                            (-21504, -21504, 24576, 24576),
                            (13440, 14336, 15360, 16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Kenut,
                (ushort)ParseEnum.TargetIDS.Nikare
            };
        }

        protected override List<ushort> GetSuccessCheckIds()
        {
            return GetFightTargetsIDs();
        }

        public override List<AbstractDamageEvent> SpecialDamageEventProcess(Dictionary<AgentItem, List<AbstractDamageEvent>> damageBySrc, Dictionary<AgentItem, List<AbstractDamageEvent>> damageByDst, Dictionary<long, List<AbstractDamageEvent>> damageById, long offset, SkillData skillData)
        {
            NegateDamageAgainstBarrier(Targets.Select(x => x.AgentItem).ToList(), damageByDst);
            return new List<AbstractDamageEvent>();
        }

        private List<PhaseData> GetTargetPhases(ParsedLog log, Target target, string[] names)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
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
                phase.Name = names[i];
                phase.Targets.Add(target);
            }
            return targetPhases;
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Kenut,
                (ushort)ParseEnum.TargetIDS.Nikare
            };
        }

        private void FallBackPhases(Target target, List<PhaseData> phases, ParsedLog log, bool firstPhaseAt0)
        {
            HashSet<AgentItem> pAgents = log.PlayerAgents;
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
                            AbstractDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p1.End + 5000 && (pAgents.Contains(x.From) || pAgents.Contains(x.From.Master)) && x.Damage > 0 && x is DirectDamageEvent);
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
                            AbstractDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p1.End + 5000 && (pAgents.Contains(x.From) || pAgents.Contains(x.From.Master)) && x.Damage > 0 && x is DirectDamageEvent);
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
                            AbstractDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= p2.End + 5000 && (pAgents.Contains(x.From) || pAgents.Contains(x.From.Master)) && x.Damage > 0 && x is DirectDamageEvent);
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
                AbstractDamageEvent hit = log.CombatData.GetDamageTakenData(target.AgentItem).FirstOrDefault(x => x.Time >= 0 && (pAgents.Contains(x.From) || pAgents.Contains(x.From.Master)) && x.Damage > 0 && x is DirectDamageEvent);
                if (hit != null)
                {
                    p1.OverrideStart(hit.Time);
                }
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target nikare = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Nikare);
            if (nikare == null)
            {
                throw new InvalidOperationException("Nikare not found");
            }
            phases[0].Targets.Add(nikare);
            Target kenut = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Kenut);
            if (kenut != null)
            {
                phases[0].Targets.Add(kenut);
            }
            if (!requirePhases)
            {
                return phases;
            }
            List<PhaseData> nikPhases = GetTargetPhases(log, nikare, new string[] { "Nikare P1", "Nikare P2", "Nikare P3" });
            FallBackPhases(nikare, nikPhases, log, true);
            phases.AddRange(nikPhases);
            if (kenut != null)
            {
                List<PhaseData> kenPhases = GetTargetPhases(log, kenut, new string[] { "Kenut P1", "Kenut P2", "Kenut P3" });
                FallBackPhases(kenut, kenPhases, log, false);
                phases.AddRange(kenPhases);
            }
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            return phases;
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Nikare:
                    //CC
                    var barrageN = cls.Where(x => x.SkillId == 51977).ToList();
                    foreach (AbstractCastEvent c in barrageN)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Platform wipe (CM only)
                    var aquaticDomainN = cls.Where(x => x.SkillId == 52374).ToList();
                    foreach (AbstractCastEvent c in aquaticDomainN)
                    {
                        int start = (int)c.Time;
                        int duration = c.ActualDuration;
                        int end = start + duration;
                        int radius = 800;
                        replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(255, 255, 0, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (ushort)ParseEnum.TargetIDS.Kenut:
                    //CC
                    var barrageK = cls.Where(x => x.SkillId == 51977).ToList();
                    foreach (AbstractCastEvent c in barrageK)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //Platform wipe (CM only)
                    var aquaticDomainK = cls.Where(x => x.SkillId == 52374).ToList();
                    foreach (AbstractCastEvent c in aquaticDomainK)
                    {
                        int start = (int)c.Time;
                        int duration = c.ActualDuration;
                        int end = start + duration;
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
                            int rotation = Point3D.GetRotationFromFacing(facing);
                            replay.Decorations.Add(new RotatedRectangleDecoration(false, 0, width, height, rotation, width / 2, (start + delay, start + delay + duration), "rgba(255, 175, 0, 0.8)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, rotation, width / 2, (start + delay, start + delay + duration), "rgba(255, 175, 0, 0.2)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
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

        public override string GetFightName()
        {
            return "Twin Largos";
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Nikare);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 18e6) ? 1 : 0; //Health of Nikare
        }
    }
}
