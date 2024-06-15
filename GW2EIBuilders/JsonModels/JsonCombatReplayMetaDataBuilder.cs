using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// The root of the JSON
    /// </summary>
    internal static class JsonCombatReplayMetaDataBuilder
    {
        public static JsonCombatReplayMetaData BuildJsonCombatReplayMetaData(ParsedEvtcLog log, RawFormatSettings settings)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatReplayMap(log);
            (int width, int height) = map.GetPixelMapSize();
            var maps = new List<JsonCombatReplayMetaData.CombatReplayMap>();
            var jsonCR = new JsonCombatReplayMetaData()
            {
                InchToPixel = map.GetInchToPixel(),
                Sizes = new int[2] { width, height },
                PollingRate = ParserHelper.CombatReplayPollingRate,
                Maps = maps
            };
            //
            foreach (CombatReplayMap.MapItem mapItem in map.Maps)
            {
                maps.Add(new JsonCombatReplayMetaData.CombatReplayMap()
                {
                    Url = mapItem.Link,
                    Interval = new long[2] { mapItem.Start, mapItem.End }
                });
            }
            //
            return jsonCR;
        }

    }
}
