using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels;

internal class CombatReplayDto
{
    public List<CombatReplayRenderingDescription> DecorationRenderings { get; set; }
    public List<CombatReplayMetadataDescription> DecorationMetadata { get; set; }
    public List<SingleActorCombatReplayDescription> Actors { get; set; }
    public List<List<double>>? DefaultViewpoints { get; set; }
    public int[] Sizes { get; set; }
    public float[] MapBounds { get; set; }
    public float InchToPixel { get; set; }
    public int PollingRate { get; set; }

    public CombatReplayDto(ParsedEvtcLog log, UsedData used)
    {
        CombatReplayMap map = log.LogData.Logic.GetCombatReplayMap(log);
        (Actors, DecorationRenderings, DecorationMetadata) = log.GetCombatReplayDescriptions(used.skills, used.buffs);
        (int width, int height) = map.GetPixelMapSize();
        Sizes = [width, height];
        MapBounds = [map.TopX, map.TopY, map.BottomX, map.BottomY];
        InchToPixel = map.GetInchToPixel();
        PollingRate = ParserHelper.CombatReplayPollingRate;
        if (map.DefaultViewpoints != null)
        {
            DefaultViewpoints = new(map.DefaultViewpoints.Count);
            foreach (var defaultViewpoint in map.DefaultViewpoints)
            {
                DefaultViewpoints.Add([defaultViewpoint.XTranslatePercent, defaultViewpoint.YTranslatePercent, defaultViewpoint.Scale, defaultViewpoint.EncounterID]);
            }
        }
    }


}
