using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Builders.HtmlModels
{
    public class CombatReplayDto
    {
        public List<object> Actors { get; set; }
        public int[] Sizes { get; set; }
        public int MaxTime { get; set; }
        public float Inch { get; set; }
        public int PollingRate { get; set; }
        public List<CombatReplayMap.MapItem> Maps { get; set; }

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
