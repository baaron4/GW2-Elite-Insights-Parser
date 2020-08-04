using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

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

        public CombatReplayDto(ParsedEvtcLog log)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatMap(log);
            Actors = GetCombatReplayActors(log, map);
            Maps = map.Maps;
            (int width, int height) = map.GetPixelMapSize();
            Sizes = new int[2] { width, height };
            Inch = map.GetInch();
            MaxTime = log.PlayerList.First().GetCombatReplayTimes(log).Last();
        }


        private static List<object> GetCombatReplayActors(ParsedEvtcLog log, CombatReplayMap map)
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
                foreach (GenericDecoration a in p.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (NPC m in log.FightData.Logic.TrashMobs)
            {
                if (m.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(m.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in m.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (NPC target in log.FightData.Logic.Targets)
            {
                if (target.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(target.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in target.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            return actors;
        }
    }
}
