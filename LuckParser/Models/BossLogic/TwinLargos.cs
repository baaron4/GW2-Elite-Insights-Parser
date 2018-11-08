using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class TwinLargos : RaidLogic
    {
        public TwinLargos(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(51935, "Waterlogged", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'hexagon-open',color:'rgb(0,140,255)'", "Wtlg","Waterlogged (stacking water debuff)", "Waterlogged",0),
            new Mechanic(52876, "Vapor Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'triangle-left-open',color:'rgb(0,140,255)'", "Chrg","Vapor Rush (Triple Charge)", "Vapor Rush Charge",0),
            new Mechanic(52812, "Tidal Pool", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'circle',color:'rgb(0,140,255)'", "Pool","Tidal Pool", "Tidal Pool",0),
            new Mechanic(51977, "Aquatic Barrage Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Nikare, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "CC","Breakbar", "Breakbar",0),
            new Mechanic(51977, "Aquatic Barrage End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Nikare, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Breakbar broken", "CCed",0),
            new Mechanic(53018, "Sea Swell", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'circle-open',color:'rgb(30,30,80)'", "Shkwv","Sea Swell (Shockwave)", "Shockwave",0),
            new Mechanic(53130, "Geyser", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'hexagon',color:'rgb(0,255,255)'", "Gysr","Geyser (Launching Aoes)", "Launch Field",0),
            new Mechanic(53097, "Water Bomb Debuff", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'diamond',color:'rgb(0,255,255)'", "Psn","Expanding Water Field", "Water Poison",0),
            new Mechanic(52931, "Aquatic Detainment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'circle-open',color:'rgb(0,0,255)'", "Float","Aquatic Detainment (Float Bubble)", "Float Bubble",6000),
            new Mechanic(52130, "Aquatic Vortex", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'star-square-open-dot',color:'rgb(0,200,255)'", "Tndo","Aquatic Vortex (Water Tornados)", "Tornado",0),
            new Mechanic(51965, "Vapor Jet", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'square',color:'rgb(255,150,0)'", "Steal","Vapor Jet (Boon Steal)", "Boon Steal",0),
            new Mechanic(52626, "Enraged (Nikare)", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Nikare, "symbol:'star-diamond',color:'rgb(255,0,0)'", "N.Enrg","Enraged (Kenut Dead)", "Nikare Enrage",0,(condition => condition.CombatItem.SrcInstid == Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Nikare).InstID)),
            new Mechanic(52626, "Enraged (Kenut)", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Nikare, "symbol:'star-diamond',color:'rgb(255,0,0)'", "K.Enrg","Enraged (Nikare Dead)", "Kenut Enrage",0,(condition => condition.CombatItem.SrcInstid == Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Kenut).InstID)),
            new Mechanic(52211, "Aquatic Aura Kenut", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'square-open',color:'rgb(0,255,255)'", "K.Aura","Increasing Damage Debuff on Kenut's Last Platform", "Kenut Aura Debuff",0),
            new Mechanic(52929, "Aquatic Aura Nikare", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Nikare, "symbol:'diamond-open',color:'rgb(0,255,255)'", "N.Aura","Increasing Damage Debuff on Nikare's Last Platform", "Nikare Aura Debuff",0),
            new Mechanic(51999, "Cyclone Burst", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Nikare, "symbol:'y-up-open',color:'rgb(255,150,0)'", "Y-Fld","Cyclone Burst (triangular rotating fields)", "Cyclone Burst",0),
            }); 
            Extension = "twinlargos";
            IconUrl = "https://i.imgur.com/6O5MT7v.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/JOoJRXM.png",
                            Tuple.Create(3205, 4191),
                            Tuple.Create(10846, -3878, 18086, 5622),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.Kenut,
                (ushort)ParseEnum.BossIDS.Nikare
            };
        }

        private List<PhaseData> GetTargetPhases(ParsedLog log, Boss target, string[] names)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> targetPhases = new List<PhaseData>();
            List<CombatItem> states = log.CombatData.GetStatesData(ParseEnum.StateChange.EnterCombat).Where(x => x.SrcInstid == target.InstID).ToList();
            states.AddRange(GetFilteredList(log, 762, target).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.None));
            states.AddRange(log.CombatData.GetStatesData(ParseEnum.StateChange.ChangeDead).Where(x => x.SrcAgent == target.Agent));
            states.Sort((x, y) => x.Time < y.Time ? -1 : 1);
            for (int i = 0; i < states.Count; i++)
            {
                CombatItem state = states[i];
                if (state.IsStateChange == ParseEnum.StateChange.EnterCombat)
                {
                    start = state.Time - log.FightData.FightStart;
                    if (i == states.Count - 1)
                    {
                        targetPhases.Add(new PhaseData(start, fightDuration));
                    }
                }
                else
                {
                    end = Math.Min(state.Time - log.FightData.FightStart, fightDuration);
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
                phase.DrawEnd = true;
                phase.DrawStart = true;
                phase.DrawArea = true;             
                phase.Targets.Add(target);
            }
            return targetPhases;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Boss nikare = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Nikare);
            if (nikare == null)
            {
                throw new InvalidOperationException("Nikare not found");
            }
            phases[0].Targets.Add(nikare);
            Boss kenut = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Kenut);
            if (kenut != null)
            {
                phases[0].Targets.Add(kenut);
            }
            if (!requirePhases)
            {
                return phases;
            }
            List<PhaseData> nikPhases = GetTargetPhases(log, nikare, new string[]{ "Nikare P1", "Nikare P2", "Nikare P3" } );
            if (kenut != null)
            {
                phases.AddRange(GetTargetPhases(log, kenut, new string[] { "Kenut P1", "Kenut P2", "Kenut P3" }));
                // clean Nikare related bugs
                switch (nikPhases.Count)
                {
                    case 2:
                        {
                            PhaseData p1 = nikPhases[0];
                            PhaseData p2 = nikPhases[1];
                            // P1 and P2 merged
                            if (p1.Start == p2.Start)
                            {
                                CombatItem combatItem = log.CombatData.GetStatesData(ParseEnum.StateChange.ExitCombat).Where(x => x.SrcInstid == kenut.InstID).FirstOrDefault();
                                if (combatItem != null)
                                {
                                    p2.OverrideStart(combatItem.Time - log.FightData.FightStart);
                                }
                                else
                                {
                                    nikPhases.Remove(p2);
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            PhaseData p1 = nikPhases[0];
                            PhaseData p2 = nikPhases[1];
                            PhaseData p3 = nikPhases[2];
                            // P1 and P2 merged
                            if (p1.Start == p2.Start)
                            {
                                CombatItem combatItem = log.CombatData.GetStatesData(ParseEnum.StateChange.ExitCombat).Where(x => x.SrcInstid == kenut.InstID).FirstOrDefault();
                                if (combatItem != null)
                                {
                                    p2.OverrideStart(combatItem.Time - log.FightData.FightStart);
                                }
                                // P1 and P3 are merged
                                if (p1.Start == p3.Start)
                                {
                                    p3.OverrideStart(p2.End);
                                }
                            }
                            // p2 and p3 are merged
                            else if (p2.Start == p3.Start)
                            {
                                p3.OverrideStart(p2.End);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            phases.AddRange(nikPhases);
            phases.Sort((x, y) => x.Start < y.Start ? -1 : 1);          
            return phases;
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Nikare:
                    //CC
                    List<CastLog> barrageN = cls.Where(x => x.SkillId == 51977).ToList();
                    foreach (CastLog c in barrageN)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 250, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
                    }
                    //Platform wipe (CM only)
                    List<CastLog> aquaticDomainN = cls.Where(x => x.SkillId == 52374).ToList();
                    foreach (CastLog c in aquaticDomainN)
                    {
                        int start = (int)c.Time;
                        int duration = c.ActualDuration;
                        int end = start + duration;
                        int radius = 800;
                        replay.Actors.Add(new CircleActor(true, end, radius, new Tuple<int, int>(start,end), "rgba(255, 255, 0, 0.3)", new AgentConnector(boss)));
                    }
                    break;
                case (ushort)ParseEnum.BossIDS.Kenut:
                    //CC
                    List<CastLog> barrageK = cls.Where(x => x.SkillId == 51977).ToList();
                    foreach (CastLog c in barrageK)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 250, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
                    }
                    //Platform wipe (CM only)
                    List<CastLog> aquaticDomainK = cls.Where(x => x.SkillId == 52374).ToList();
                    foreach (CastLog c in aquaticDomainK)
                    {
                        int start = (int)c.Time;
                        int duration = c.ActualDuration;
                        int end = start + duration;
                        int radius = 800;
                        replay.Actors.Add(new CircleActor(true, end, radius, new Tuple<int, int>(start, end), "rgba(255, 255, 0, 0.3)", new AgentConnector(boss)));
                    }
                    List<CastLog> shockwave = cls.Where(x => x.SkillId == 53018).ToList();
                    foreach (CastLog c in shockwave)
                    {
                        int start = (int)c.Time;
                        int delay = 960;
                        int duration = 3000;
                        int radius = 1200;
                        replay.Actors.Add(new CircleActor(false, start + delay + duration, radius, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(100, 200, 255, 0.5)", new AgentConnector(boss)));
                    }
                    List<CastLog> boonSteal = cls.Where(x => x.SkillId == 51965).ToList();
                    foreach (CastLog c in boonSteal)
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
                            replay.Actors.Add(new RotatedRectangleActor(false, 0, width, height, rotation, width / 2, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 175, 0, 0.8)", new AgentConnector(boss)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, rotation, width / 2, new Tuple<int, int>(start + delay, start + delay + duration), "rgba(255, 175, 0, 0.2)", new AgentConnector(boss)));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // Water "Poison Bomb"
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> waterToDrop = GetFilteredList(log, 53097, p);
            int toDropStart = 0;
            foreach (CombatItem c in waterToDrop)
            {
                int timer = 5000;
                int duration = 83000;
                int debuffRadius = 100;
                int radius = 500;
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    toDropStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int toDropEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(false, 0, debuffRadius, new Tuple<int, int>(toDropStart, toDropEnd), "rgba(255, 100, 0, 0.4)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, toDropStart + timer, debuffRadius, new Tuple<int, int>(toDropStart, toDropEnd), "rgba(255, 100, 0, 0.4)", new AgentConnector(p)));
                    Point3D poisonNextPos = replay.Positions.FirstOrDefault(x => x.Time >= toDropEnd);
                    Point3D poisonPrevPos = replay.Positions.LastOrDefault(x => x.Time <= toDropEnd);
                    if (poisonNextPos != null || poisonPrevPos != null)
                    {
                        replay.Actors.Add(new CircleActor(true, toDropStart + duration, radius, new Tuple<int, int>(toDropEnd, toDropEnd + duration), "rgba(100, 100, 100, 0.3)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), debuffRadius));
                        replay.Actors.Add(new CircleActor(false, toDropStart + duration, radius, new Tuple<int, int>(toDropEnd, toDropEnd + duration), "rgba(230, 230, 230, 0.4)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), debuffRadius));
                    }
                }
            }
            // Bubble (Aquatic Detainment)
            List<CombatItem> bubble = GetFilteredList(log, 51755, p);
            int bubbleStart = 0;
            foreach (CombatItem c in bubble)
            {
                int radius = 100;
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    bubbleStart = Math.Max((int)(c.Time - log.FightData.FightStart), 0);
                }
                else
                {
                    int bubbleEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(true, 0, radius, new Tuple<int, int>(bubbleStart, bubbleEnd), "rgba(0, 200, 255, 0.3)", new AgentConnector(p)));
                }
            }
        }

        public override string GetFightName()
        {
            return "Twin Largos";
        }

        public override int IsCM(ParsedLog log)
        {
            Boss target = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Nikare);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health > 18e6) ? 1 : 0; //Health of Nikare
        }
    }
}
