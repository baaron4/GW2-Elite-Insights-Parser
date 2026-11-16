using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;

namespace GW2EIBuilders.JsonModels.JsonActors;

/// <summary>
/// Class corresponding to a specific instance of <see cref="JsonMinionsBuilder"/>
/// </summary>
internal static class JsonMinionInstanceBuilder
{

    public static JsonMinionInstance BuildJsonMinionInstance(SingleActor minion, ParsedEvtcLog log, RawFormatSettings settings, UsedJsonData usedData, bool friendly)
    {
        var jsonMinionInstance = new JsonMinionInstance
        {
            FirstAware = (int)minion.FirstAware,
            LastAware = (int)minion.LastAware,
            InstanceID = minion.InstID,
            UniqueID = minion.UniqueID,
        };
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);

        jsonMinionInstance.Buffs = JsonNPCBuilder.GetNPCJsonBuffsUptime(minion, log, settings, usedData, false);
        jsonMinionInstance.BuffVolumes = JsonNPCBuilder.GetNPCJsonBuffVolumes(minion, log, usedData, false);
        if (friendly)
        {
            jsonMinionInstance.BuffsActive = JsonNPCBuilder.GetNPCJsonBuffsUptime(minion, log, settings, usedData, true);
            jsonMinionInstance.BuffVolumesActive = JsonNPCBuilder.GetNPCJsonBuffVolumes(minion, log, usedData, true);
            jsonMinionInstance.ActiveTimes = phases.Select(x => minion.GetActiveDuration(log, x.Start, x.End)).ToList();
        }

        var minionCastEvents = minion.GetIntersectingCastEvents(log);
        if (minionCastEvents.Count > 0)
        {
            jsonMinionInstance.Rotation = JsonRotationBuilder.BuildJsonRotationList(log, minionCastEvents.GroupBy(x => x.SkillID), usedData).ToList();
        }

        if (settings.RawFormatTimelineArrays)
        {
            IReadOnlyDictionary<long, BuffGraph> buffGraphs = minion.GetBuffGraphs(log);
            jsonMinionInstance.BoonsStates = JsonBuffsUptimeBuilder.GetBuffStates(buffGraphs[SkillIDs.NumberOfBoons]);
            jsonMinionInstance.ConditionsStates = JsonBuffsUptimeBuilder.GetBuffStates(buffGraphs[SkillIDs.NumberOfConditions]);
            // Health
            jsonMinionInstance.HealthPercents = minion.GetHealthUpdates(log).Select(x => new List<double>() { x.Start, x.Value }).ToList();
            jsonMinionInstance.BarrierPercents = minion.GetBarrierUpdates(log).Select(x => new List<double>() { x.Start, x.Value }).ToList();
        }

        return jsonMinionInstance;
    }

}
