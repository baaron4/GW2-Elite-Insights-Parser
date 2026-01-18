using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels;

internal static class JsonCombatReplayMetaDataBuilder
{
    public static JsonCombatReplayMetaData BuildJsonCombatReplayMetaData(ParsedEvtcLog log, RawFormatSettings settings)
    {
        CombatReplayMap map = log.LogData.Logic.GetCombatReplayMap(log);
        var (actors, decorationRendering, decorationMetadata) = log.GetCombatReplayDescriptions([], []);
        var mapDecorations = decorationRendering.OfType<ArenaDecorationRenderingDescription>().ToList();
        var maps = new List<JsonCombatReplayMetaData.CombatReplayMap>(mapDecorations.Count);
        var jsonCR = new JsonCombatReplayMetaData()
        {
            InchToPixel = map.GetInchToPixel(),
            Sizes = [map.GetPixelMapSize().width, map.GetPixelMapSize().height],
            PollingRate = ParserHelper.CombatReplayPollingRate,
            Maps = maps
        };
        //
        var mapMetaDatas = decorationMetadata.OfType<ArenaDecorationMetadataDescription>().GroupBy(x => x.Signature).ToDictionary(x => x.Key, x => x.First());
        foreach (var mapItem in mapDecorations)
        {
            if (mapItem.ConnectedTo is PositionConnectorDescription posConnector &&  mapMetaDatas.TryGetValue(mapItem.MetadataSignature, out var metadata))
            {
                maps.Add(new JsonCombatReplayMetaData.CombatReplayMap()
                {
                    Url = metadata.Image,
                    Interval = [mapItem.Start, mapItem.End],
                    Position = posConnector.Position
                });
            }
        }
        //
        return jsonCR;
    }

}
