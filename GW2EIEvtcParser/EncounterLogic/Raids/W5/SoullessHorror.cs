﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class SoullessHorror : RaidLogic
    {
        public SoullessHorror(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new HitOnPlayerMechanic(47327, "Vortex Slash", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Donut In","Vortex Slash (Inner Donut hit)", "Inner Donut",0),
            new HitOnPlayerMechanic(48432, "Vortex Slash", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Donut Out","Vortex Slash (Outer Donut hit)", "Outer Donut", 0),
            new HitOnPlayerMechanic(47430, "Soul Rift", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Golem","Soul Rift (stood in Golem Aoe)", "Golem Aoe",0),
            new HitOnPlayerMechanic(48363, "Quad Slash", new MechanicPlotlySetting("star-diamond-open","rgb(255,140,0)"), "Slice1","Quad Slash (4 Slices, First hit)", "4 Slices 1",0),
            new HitOnPlayerMechanic(47915, "Quad Slash", new MechanicPlotlySetting("star-square-open","rgb(255,140,0)"), "Slice2","Quad Slash (4 Slices, Second hit)", "4 Slices 2",0),
            new HitOnPlayerMechanic(47363, "Spinning Slash", new MechanicPlotlySetting("star-triangle-up-open","rgb(128,0,0)"), "Scythe","Spinning Slash (hit by Scythe)", "Scythe",0),
            new HitOnPlayerMechanic(48500, "Death Bloom", new MechanicPlotlySetting("octagon","rgb(255,140,0)"), "8Slice","Death Bloom (8 Slices)", "8 Slices",0),
            new PlayerBuffApplyMechanic(47434, "Fixated", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixate","Fixated (Special Action Key)", "Fixated",0),
            new PlayerBuffApplyMechanic(47414, "Necrosis", new MechanicPlotlySetting("star-open","rgb(255,0,255)"), "Necrosis","Necrosis (Tanking Debuff)", "Necrosis Debuff",50),
            new HitOnPlayerMechanic(48327, "Corrupt the Living", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Spin","Corrupt the Living (Torment+Poisen Spin)", "Torment+Poisen Spin",0),
            new HitOnPlayerMechanic(47756, "Wurm Spit", new MechanicPlotlySetting("diamond-open","rgb(0,128,128)"), "Spit","Wurm Spit", "Wurm Spit",0),
            new EnemyCastStartMechanic(48662, "Howling Death", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Howling Death (Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(48662, "Howling Death", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Howling Death (Breakbar) broken", "CCed",0, (ce, log) => ce.ActualDuration <= 6800),
            new EnemyCastEndMechanic(48662, "Howling Death", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Howling Death (Breakbar failed) ", "CC Fail",0, (ce,log) => ce.ActualDuration > 6800),

            });
            Extension = "sh";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://wiki.guildwars2.com/images/d/d4/Mini_Desmina.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/A45pVJy.png",
                            (3657, 3657),
                            (-12223, -771, -8932, 2420),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Scythe,
                ArcDPSEnums.TrashID.TormentedDead,
                ArcDPSEnums.TrashID.SurgingSoul,
                ArcDPSEnums.TrashID.FleshWurm
            };
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.SoullessHorror);
                if (mainTarget == null)
                {
                    throw new InvalidOperationException("Soulless Horror not found");
                }
                AbstractBuffEvent buffOnDeath = combatData.GetBuffData(895).Where(x => x.To == mainTarget.AgentItem && x is BuffApplyEvent).LastOrDefault();
                if (buffOnDeath != null)
                {
                    fightData.SetSuccess(true, buffOnDeath.Time);
                }
            }
        }

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            // Necrosis adjust
            AdjustTimeRefreshBuff(buffsByDst, buffsById, 47414);
            return res;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.SoullessHorror);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Soulless Horror not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var howling = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == 48662).ToList();
            long start = 0;
            int i = 1;
            foreach (AbstractCastEvent c in howling)
            {
                var phase = new PhaseData(start, Math.Min(c.Time, fightDuration), "Pre-Breakbar " + i++);
                phase.Targets.Add(mainTarget);
                start = c.EndTime;
                phases.Add(phase);
            }
            if (fightDuration - start > 3000)
            {
                var lastPhase = new PhaseData(start, fightDuration, "Final");
                lastPhase.Targets.Add(mainTarget);
                phases.Add(lastPhase);
            }
            return phases;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.SoullessHorror:
                    var howling = cls.Where(x => x.SkillId == 48662).ToList();
                    foreach (AbstractCastEvent c in howling)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(true, start + c.ExpectedDuration, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    var vortex = cls.Where(x => x.SkillId == 47327).ToList();
                    foreach (AbstractCastEvent c in vortex)
                    {
                        start = (int)c.Time;
                        end = start + 4000;
                        Point3D next = replay.PolledPositions.FirstOrDefault(x => x.Time >= start);
                        Point3D prev = replay.PolledPositions.LastOrDefault(x => x.Time <= start);
                        if (next != null || prev != null)
                        {
                            replay.Decorations.Add(new CircleDecoration(false, 0, 380, (start, end), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, start)));
                            replay.Decorations.Add(new CircleDecoration(true, end, 380, (start, end), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, start)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 380, 760, (end, end + 1000), "rgba(255, 150, 0, 0.5)", new InterpolatedPositionConnector(prev, next, start)));
                        }
                    }
                    var deathBloom = cls.Where(x => x.SkillId == 48500).ToList();
                    foreach (AbstractCastEvent c in deathBloom)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 3500, Point3D.GetRotationFromFacing(facing) + (i * 360 / 8), 360 / 12, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }

                    }
                    var quad1 = cls.Where(x => x.SkillId == 48363).ToList();
                    var quad2 = cls.Where(x => x.SkillId == 47915).ToList();
                    foreach (AbstractCastEvent c in quad1)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 3500, Point3D.GetRotationFromFacing(facing) + (i * 360 / 4), 360 / 12, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }

                    }
                    foreach (AbstractCastEvent c in quad2)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            replay.Decorations.Add(new PieDecoration(true, 0, 3500, Point3D.GetRotationFromFacing(facing) + 45 + (i * 360 / 4), 360 / 12, (start, end), "rgba(255,200,0,0.5)", new AgentConnector(target)));
                        }

                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Scythe:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 80, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.TormentedDead:
                    if (replay.Positions.Count == 0)
                    {
                        break;
                    }
                    replay.Decorations.Add(new CircleDecoration(true, 0, 400, (end, end + 60000), "rgba(255, 0, 0, 0.5)", new PositionConnector(replay.Positions.Last())));
                    break;
                case (int)ArcDPSEnums.TrashID.SurgingSoul:
                    List<Point3D> positions = replay.Positions;
                    if (positions.Count < 2)
                    {
                        break;
                    }
                    if (positions[0].X < -12000 || positions[0].X > -9250)
                    {
                        replay.Decorations.Add(new RectangleDecoration(true, 0, 240, 660, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(target)));
                        break;
                    }
                    else if (positions[0].Y < -525 || positions[0].Y > 2275)
                    {
                        replay.Decorations.Add(new RectangleDecoration(true, 0, 645, 238, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(target)));
                        break;
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.FleshWurm:
                    break;
                default:
                    break;
            }

        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            var necrosis = combatData.GetBuffData(47414).Where(x => x is BuffApplyEvent).ToList();
            if (necrosis.Count == 0)
            {
                return 0;
            }
            // split necrosis
            var splitNecrosis = new Dictionary<AgentItem, List<AbstractBuffEvent>>();
            foreach (AbstractBuffEvent c in necrosis)
            {
                AgentItem tank = c.To;
                if (!splitNecrosis.ContainsKey(tank))
                {
                    splitNecrosis.Add(tank, new List<AbstractBuffEvent>());
                }
                splitNecrosis[tank].Add(c);
            }
            List<AbstractBuffEvent> longestNecrosis = splitNecrosis.Values.First(l => l.Count == splitNecrosis.Values.Max(x => x.Count));
            long minDiff = long.MaxValue;
            for (int i = 0; i < longestNecrosis.Count - 1; i++)
            {
                AbstractBuffEvent cur = longestNecrosis[i];
                AbstractBuffEvent next = longestNecrosis[i + 1];
                long timeDiff = next.Time - cur.Time;
                if (timeDiff > 1000 && minDiff > timeDiff)
                {
                    minDiff = timeDiff;
                }
            }
            return (minDiff < 11000) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
