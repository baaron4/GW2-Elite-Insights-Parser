using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;

namespace LuckParser.Builders.HtmlModels
{
    public class CombatReplayDto
    {
        public List<object> Actors;
        public int[] Sizes;
        public int MaxTime;
        public float Inch;
        public int PollingRate;
        public List<CombatReplayMap.MapItem> Maps;

        public CombatReplayDto(ParsedLog log)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatMap(log);
            Actors = GetCombatReplayActors(log, map);
            Maps = map.Maps;
            (int width, int height) = map.GetPixelMapSize();
            Sizes = new int[2] { width, height };
            Inch = map.GetInch();
            MaxTime = log.PlayerList.First().GetCombatReplayTimes(log).Last();
        }


        private List<object> GetCombatReplayActors(ParsedLog log, CombatReplayMap map)
        {
            var actors = new List<object>();
            foreach (Player p in log.PlayerList)
            {
                if (p.IsFakeActor)
                {
                    continue;
                }
                if (p.GetCombatReplayPolledPositions(log).Count == 0)
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
                if (m.GetCombatReplayPolledPositions(log).Count == 0)
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
                if (target.GetCombatReplayPolledPositions(log).Count == 0)
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
