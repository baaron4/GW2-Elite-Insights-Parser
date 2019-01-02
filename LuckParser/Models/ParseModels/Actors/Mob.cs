using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Mob : AbstractMasterActor
    {
        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {        
        }

        //setters
        protected override void SetDamageLogs(ParsedLog log)
        {
        }
        protected override void SetDamageTakenLogs(ParsedLog log)
        {
        }
        protected override void SetCastLogs(ParsedLog log)
        {
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log)
        {
            CombatReplay.Icon = GeneralHelper.GetNPCIcon(ID);
            log.FightData.Logic.ComputeAdditionalThrashMobData(this, log);
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
                Type = "Mob",
                Positions = new double[2 * CombatReplay.Positions.Count],
                Start = CombatReplay.TimeOffsets.Item1,
                End = CombatReplay.TimeOffsets.Item2,
                ID = GetCombatReplayID()
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
    }
}
