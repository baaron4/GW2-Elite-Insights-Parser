using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    internal class CombatReplayDto
    {
        public List<object> Actors { get; set; }
        public int[] Sizes { get; set; }
        public long MaxTime { get; set; }
        public float Inch { get; set; }
        public int PollingRate { get; set; }
        public IReadOnlyList<CombatReplayMap.MapItem> Maps { get; set; }

        public CombatReplayDto(ParsedEvtcLog log)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatMap(log);
            Actors = GetCombatReplayActors(log, map);
            Maps = map.Maps;
            (int width, int height) = map.GetPixelMapSize();
            Sizes = new int[2] { width, height };
            Inch = map.GetInch();
            MaxTime = log.PlayerList.First().GetCombatReplayPolledPositions(log).Last().Time;
        }


        private static List<object> GetCombatReplayActors(ParsedEvtcLog log, CombatReplayMap map)
        {
            var actors = new List<object>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                if (actor.IsFakeActor)
                {
                    continue;
                }
                if (actor.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(actor.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in actor.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (AbstractSingleActor actor in log.FightData.Logic.TrashMobs)
            {
                if (actor.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(actor.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in actor.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (AbstractSingleActor actor in log.FightData.Logic.Targets)
            {
                if (actor.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                actors.Add(actor.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in actor.GetCombatReplayActors(log))
                {
                    actors.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            return actors;
        }
    }
}
