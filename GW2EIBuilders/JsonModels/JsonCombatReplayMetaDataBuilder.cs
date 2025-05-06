using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels;

internal static class JsonCombatReplayMetaDataBuilder
{
    public static JsonCombatReplayMetaData BuildJsonCombatReplayMetaData(ParsedEvtcLog log, RawFormatSettings settings)
    {
        CombatReplayMap map = log.FightData.Logic.GetCombatReplayMap(log);
        var maps = new List<JsonCombatReplayMetaData.CombatReplayMap>(map.Maps.Count);
        var jsonCR = new JsonCombatReplayMetaData()
        {
            InchToPixel = map.GetInchToPixel(),
            Sizes = [map.GetPixelMapSize().width, map.GetPixelMapSize().height],
            PollingRate = ParserHelper.CombatReplayPollingRate,
            Maps = maps
        };
        //
        foreach (CombatReplayMap.MapItem mapItem in map.Maps)
        {
            maps.Add(new JsonCombatReplayMetaData.CombatReplayMap()
            {
                Url = mapItem.Link,
                Interval = [mapItem.Start, mapItem.End]
            });
        }
        //
        return jsonCR;
    }

}
