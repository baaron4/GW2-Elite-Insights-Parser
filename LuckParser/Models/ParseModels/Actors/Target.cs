using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Target : AbstractMasterActor
    {
        // Constructors
        public Target(AgentItem agent) : base(agent)
        {
        }

        public int Health { get; set; } = -1;
        public List<Point> HealthOverTime { get; } = new List<Point>();

        /*public void AddCustomCastLog(long time, long skillID, int expDur, ParseEnum.Activation startActivation, int actDur, ParseEnum.Activation endActivation, ParsedLog log)
        {
            if (CastLogs.Count == 0)
            {
                GetCastLogs(log, 0, log.FightData.FightEnd);
            }
            CastLogs.Add(new CastLog(time, skillID, expDur, startActivation, actDur, endActivation, Agent, InstID));
        }*/

        // Private Methods

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            CombatReplay.Icon = GeneralHelper.GetNPCIcon(ID);
            log.FightData.Logic.ComputeAdditionalTargetData(this, log);
            List<Point3D> facings = CombatReplay.Rotations;
            if (facings.Any())
            {
                CombatReplay.Actors.Add(new FacingActor(new Tuple<int, int>((int)CombatReplay.TimeOffsets.Item1, (int)CombatReplay.TimeOffsets.Item2), new AgentConnector(this), facings));
            }
        }

        public List<double[]> Get1SHealthGraph(ParsedLog log, List<PhaseData> phases)
        {
            List<double[]> res = new List<double[]>();
            // fill the graph, full precision
            List<double> listFull = new List<double>();
            for (int i = 0; i <= phases[0].GetDuration(); i++)
            {
                listFull.Add(100.0);
            }
            int totalTime = 0;
            double curHealth = 100.0;
            foreach (Point p in HealthOverTime)
            {
                int time = p.X;
                if (time < 0)
                {
                    continue;
                }
                if (time > phases[0].GetDuration())
                {
                    break;
                }
                for (; totalTime < time; totalTime++)
                {
                    listFull[totalTime] = curHealth;
                }
                curHealth = p.Y / 100.0;
                listFull[time] = curHealth;
            }
            // fill
            for (; totalTime <= phases[0].GetDuration(); totalTime++)
            {
                listFull[totalTime] = curHealth;
            }
            foreach (PhaseData phase in phases)
            {
                int seconds = (int)phase.GetDuration("s");
                bool needsLastPoint = seconds * 1000 != phase.GetDuration();
                double[] hps = new double[seconds + (needsLastPoint ? +2 : 1)];
                int time = (int)phase.Start;
                int i = 0;
                for (i = 0; i <= seconds; i++)
                {
                    hps[i] = listFull[time];
                    time += 1000;
                }
                if (needsLastPoint)
                {
                    hps[i] = listFull[(int)phase.End];
                }
                res.Add(hps);
            }
            return res;
        }
        

        //
        private class Serializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
            public double[] Rotations { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            Serializable aux = new Serializable
            {
                Img = CombatReplay.Icon,
                Type = "Target",
                ID = GetCombatReplayID(),
                Start = CombatReplay.TimeOffsets.Item1,
                End = CombatReplay.TimeOffsets.Item2,
                Positions = new double[2 * CombatReplay.Positions.Count],
                Rotations = new double[2 * CombatReplay.Rotations.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                Tuple<double, double> coord = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = coord.Item1;
                aux.Positions[i++] = coord.Item2;
            }
            i = 0;
            foreach (Point3D facing in CombatReplay.Rotations)
            {
                aux.Rotations[i++] = Point3D.GetRotationFromFacing(facing);
                aux.Rotations[i++] = facing.Time;
            }
            return JsonConvert.SerializeObject(aux);
        }

        public override int GetCombatReplayID()
        {
            return (InstID + "_" + CombatReplay.TimeOffsets.Item1 + "_" + CombatReplay.TimeOffsets.Item2).GetHashCode();
        }

        /*protected override void setHealingLogs(ParsedLog log)
        {
            // nothing to do
        }

        protected override void setHealingReceivedLogs(ParsedLog log)
        {
            // nothing to do
        }*/
    }
}