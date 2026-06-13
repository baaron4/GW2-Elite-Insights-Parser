using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels;

internal static class JsonWvWMapDataBuilder
{
    public static JsonWvWMapData BuildJsonWvWMapData(ParsedEvtcLog log, WvWTeamsEvent wvwTeamsEvent,  HashSet<ulong> teampMap)
    {
        var jsonWvWMapData = new JsonWvWMapData
        {
            BlueShardID = wvwTeamsEvent.BlueShardID,
            RedShardID = wvwTeamsEvent.RedShardID,
            GreenShardID = wvwTeamsEvent.GreenShardID,

            BlueTeamID = wvwTeamsEvent.BlueTeamID,
            GreenTeamID = wvwTeamsEvent.GreenTeamID,
            RedTeamID = wvwTeamsEvent.RedTeamID,
        };

        teampMap.UnionWith([wvwTeamsEvent.BlueTeamID, wvwTeamsEvent.GreenTeamID, wvwTeamsEvent.RedTeamID]);

        var wvwObjectiveStatusEvents = log.CombatData.GetWvWObjectStatusEvents();
        foreach (var objectiveStatusEvent in wvwObjectiveStatusEvents)
        {
            jsonWvWMapData.ObjectiveData.Add(new JsonWvWMapData.JsonWvWObjectiveData()
                {
                    MapID = objectiveStatusEvent.MapID,
                    ObjectiveID = objectiveStatusEvent.ObjectiveID,
                    ObjectiveType = objectiveStatusEvent.ObjectiveType.GetObjectiveTypeName(),
                    Owners = objectiveStatusEvent.Owners.Select(x => new long[2] {x.TeamID, x.Time}).ToList()
                }
            );
        }

        return jsonWvWMapData;
    }

}
