﻿using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels;

internal class CombatReplayDto
{
    public List<CombatReplayRenderingDescription> DecorationRenderings { get; set; }
    public List<CombatReplayDecorationMetadataDescription> DecorationMetadata { get; set; }
    public List<SingleActorCombatReplayDescription> Actors { get; set; }
    public int[] Sizes { get; set; }
    public long MaxTime { get; set; }
    public float InchToPixel { get; set; }
    public int PollingRate { get; set; }
    public IReadOnlyList<CombatReplayMap.MapItem> Maps { get; set; }

    public CombatReplayDto(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        CombatReplayMap map = log.FightData.Logic.GetCombatReplayMap(log);
        (Actors, DecorationRenderings, DecorationMetadata) = log.GetCombatReplayDescriptions(usedSkills, usedBuffs);
        Maps = map.Maps;
        (int width, int height) = map.GetPixelMapSize();
        Sizes = [width, height];
        InchToPixel = map.GetInchToPixel();
        MaxTime = log.PlayerList.First().GetCombatReplayPolledPositions(log).Last().Time;
        PollingRate = ParserHelper.CombatReplayPollingRate;
    }


}
