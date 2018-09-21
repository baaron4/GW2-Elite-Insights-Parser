using LuckParser.Models.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class Boss : AbstractMasterPlayer
    {
        // Constructors
        public Boss(AgentItem agent) : base(agent)
        {
        }

        public int Health { get; set; } = -1;
        public List<Point> HealthOverTime { get; set; } = new List<Point>();

        public void AddCustomCastLog(CastLog cl, ParsedLog log)
        {
            if (CastLogs.Count == 0)
            {
                GetCastLogs(log, 0, log.FightData.FightEnd);
            }
            CastLogs.Add(cl);
        }

        // Private Methods

        protected override void SetDamageTakenLogs(ParsedLog log)
        {
            // nothing to do
            /*long time_start = log.getBossData().getFirstAware();
            foreach (CombatItem c in log.getDamageTakenData())
            {
                if (agent.getInstid() == c.getDstInstid() && c.getTime() > log.getBossData().getFirstAware() && c.getTime() < log.getBossData().getLastAware())
                {//selecting player as target
                    long time = c.getTime() - time_start;
                    foreach (AgentItem item in log.AgentData.getAllAgentsList())
                    {//selecting all
                        addDamageTakenLog(time, item.getInstid(), c);
                    }
                }
            }*/
        }

        protected override void SetAdditionalCombatReplayData(ParsedLog log, int pollingRate)
        {
            log.FightData.Logic.ComputeAdditionalBossData(CombatReplay, GetCastLogs(log, 0, log.FightData.FightDuration), log);
        }

        protected override void SetCombatReplayIcon(ParsedLog log)
        {
            CombatReplay.Icon = log.FightData.Logic.GetReplayIcon();
        }

        //
        private class Serializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int[] Positions { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            Serializable aux = new Serializable
            {
                Img = CombatReplay.Icon,
                Type = "Boss",
                Positions = new int[2 * CombatReplay.Positions.Count]
            };
            int i = 0;
            foreach (Point3D pos in CombatReplay.Positions)
            {
                Tuple<int, int> coord = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions[i++] = coord.Item1;
                aux.Positions[i++] = coord.Item2;
            }

            return JsonConvert.SerializeObject(aux);
        }

        public override int GetCombatReplayID()
        {
            return 0;
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