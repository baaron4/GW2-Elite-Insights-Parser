using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities;

internal static class JsonActorCombatReplayDataBuilder
{
    public static JsonActorCombatReplayData BuildJsonActorCombatReplayDataBuilder(SingleActor actor, ParsedEvtcLog log, RawFormatSettings settings)
    {
        CombatReplayMap map = log.FightData.Logic.GetCombatReplayMap(log);
        SingleActorCombatReplayDescription description = actor.GetCombatReplayDescription(map, log);
        var actorCombatReplayData = new JsonActorCombatReplayData()
        {
            IconURL = description.Img,
            Start = description.Start,
            End = description.End
        };
        if (settings.RawFormatTimelineArrays)
        {
            //
            var jsonPositions = new List<(float, float)>();
            for (int i = 0; i < description.Positions.Count; i += 2)
            {
                jsonPositions.Add((description.Positions[i], description.Positions[i + 1]));
            }
            actorCombatReplayData.Positions = jsonPositions;
            //
            if (description.Dead != null)
            {
                var jsonDeads = new List<(long, long)>();
                for (int i = 0; i < description.Dead.Count; i += 2)
                {
                    jsonDeads.Add((description.Dead[i], description.Dead[i + 1]));
                }
                actorCombatReplayData.Dead = jsonDeads;
            }
            if (description.Dc != null)
            {
                var jsonDcs = new List<(long, long)>();
                for (int i = 0; i < description.Dc.Count; i += 2)
                {
                    jsonDcs.Add((description.Dc[i], description.Dc[i + 1]));
                }
                actorCombatReplayData.Dc = jsonDcs;
            }
            if (description.Down != null)
            {
                var jsonDowns = new List<(long, long)>();
                for (int i = 0; i < description.Down.Count; i += 2)
                {
                    jsonDowns.Add((description.Down[i], description.Down[i + 1]));
                }
                actorCombatReplayData.Down = jsonDowns;
            }
            //
            actorCombatReplayData.Orientations = description.Angles;
        }

        //
        return actorCombatReplayData;
    }

}
