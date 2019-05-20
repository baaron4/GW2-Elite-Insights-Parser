using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class CombatReplayDto
    {
        public List<object> Actors;
        public int[] Sizes;
        public int MaxTime;
        public float Inch;
        public int PollingRate;
        public string MapLink;

        public CombatReplayDto(ParsedLog log)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatMap();
            Actors = GetCombatReplayActors(log, map);
            MapLink = map.Link;
            (int width, int height) = map.GetPixelMapSize();
            Sizes = new int[2] { width, height };
            Inch = map.GetInch();
            MaxTime = log.PlayerList.First().GetCombatReplayTimes(log).Last();
        }


        private List<object> GetCombatReplayActors(ParsedLog log, CombatReplayMap map)
        {
            List<object> actors = new List<object>();
            foreach (Player p in log.PlayerList)
            {
                if (p.IsFakeActor)
                {
                    continue;
                }
                if (p.GetCombatReplayPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(p.GetCombatReplayJSON(map, log));
                foreach (GenericActor a in p.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (Mob m in log.FightData.Logic.TrashMobs)
            {
                if (m.GetCombatReplayPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(m.GetCombatReplayJSON(map, log));
                foreach (GenericActor a in m.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
                if (target.GetCombatReplayPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(target.GetCombatReplayJSON(map, log));
                foreach (GenericActor a in target.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            return actors;
        }
    }
}
