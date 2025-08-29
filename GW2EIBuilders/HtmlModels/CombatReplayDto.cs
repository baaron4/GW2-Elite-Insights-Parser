using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels;

internal class CombatReplayDto
{
    public List<CombatReplayRenderingDescription> DecorationRenderings { get; set; }
    public List<CombatReplayMetadataDescription> DecorationMetadata { get; set; }
    public List<SingleActorCombatReplayDescription> Actors { get; set; }
    public int[] Sizes { get; set; }
    public float InchToPixel { get; set; }
    public int PollingRate { get; set; }

    public CombatReplayDto(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        CombatReplayMap map = log.LogData.Logic.GetCombatReplayMap(log);
        (Actors, DecorationRenderings, DecorationMetadata) = log.GetCombatReplayDescriptions(usedSkills, usedBuffs);
        (int width, int height) = map.GetPixelMapSize();
        Sizes = [width, height];
        InchToPixel = map.GetInchToPixel();
        PollingRate = ParserHelper.CombatReplayPollingRate;
    }


}
