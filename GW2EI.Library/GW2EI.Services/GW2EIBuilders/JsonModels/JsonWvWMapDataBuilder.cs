using GW2EIEvtcParser;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;

namespace GW2EIBuilders.JsonModels;

internal static class JsonWvWMapDataBuilder
{
    public static JsonWvWMapData BuildJsonWvWMapData(ParsedEvtcLog log, WvWTeamsEvent wvwTeamsEvent, UsedJsonData usedData)
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

        usedData.teams.UnionWith([wvwTeamsEvent.BlueTeamID, wvwTeamsEvent.GreenTeamID, wvwTeamsEvent.RedTeamID]);

        var wvwObjectiveStatusEvents = log.CombatData.GetWvWObjectStatusEvents();
        var objectiveData = new List<JsonWvWMapData.JsonWvWObjectiveData>(wvwObjectiveStatusEvents.Count);
        foreach (var objectiveStatusEvent in wvwObjectiveStatusEvents)
        {
            objectiveData.Add(new JsonWvWMapData.JsonWvWObjectiveData()
            {
                MapID = objectiveStatusEvent.MapID,
                ObjectiveID = objectiveStatusEvent.ObjectiveID,
                ObjectiveType = objectiveStatusEvent.ObjectiveType.GetObjectiveTypeName(),
                Owners = objectiveStatusEvent.Owners.Select(x => new long[2] { x.TeamID, x.Time }).ToList()
            }
            );
        }
        jsonWvWMapData.ObjectiveData = objectiveData;

        return jsonWvWMapData;
    }

}
