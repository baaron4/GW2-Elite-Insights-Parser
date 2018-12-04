using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Target : AbstractMasterPlayer
    {
        // Constructors
        public Target(AgentItem agent) : base(agent)
        {
        }

        public int Health { get; set; } = -1;
        public List<Point> HealthOverTime { get; } = new List<Point>();

        public void AddCustomCastLog(CastLog cl, ParsedLog log)
        {
            if (CastLogs.Count == 0)
            {
                GetCastLogs(log, 0, log.FightData.FightEnd);
            }
            CastLogs.Add(cl);
        }

        // Private Methods

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            CombatReplay.Icon = GeneralHelper.GetNPCIcon(ID);
            log.FightData.Logic.ComputeAdditionalTargetData(this, log);
        }
        

        //
        private class Serializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
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
                Positions = new double[2 * CombatReplay.Positions.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                Tuple<double, double> coord = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = coord.Item1;
                aux.Positions[i++] = coord.Item2;
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