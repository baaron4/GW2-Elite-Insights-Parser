using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIJSON.JsonStatistics;

namespace GW2EIBuilders.JsonModels.JsonActors;

/// <summary>
/// Base class for Players and NPCs
/// </summary>
/// <seealso cref="JsonPlayerBuilder"/> 
/// <seealso cref="JsonNPCBuilder"/>
internal static class JsonActorBuilder
{

    public static void FillJsonActor(JsonActor jsonActor, SingleActor actor, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        //
        jsonActor.FirstAware = (int)actor.FirstAware;
        jsonActor.LastAware = (int)actor.LastAware;
        jsonActor.Name = actor.Character;
        jsonActor.TotalHealth = actor.GetHealth(log.CombatData);
        jsonActor.Toughness = actor.Toughness;
        jsonActor.Healing = actor.Healing;
        jsonActor.Concentration = actor.Concentration;
        jsonActor.Condition = actor.Condition;
        jsonActor.HitboxHeight = actor.HitboxHeight;
        jsonActor.HitboxWidth = actor.HitboxWidth;
        jsonActor.InstanceID = actor.AgentItem.ParentAgentItem?.Merged.InstID ?? actor.AgentItem.InstID;
        jsonActor.IsFake = actor.IsFakeActor;
        TeamChangeEvent? teamChange = log.CombatData.GetTeamChangeEvents(actor.AgentItem).LastOrDefault();
        if (teamChange != null)
        {
            if (teamChange.TeamIDInto > 0)
            {
                jsonActor.TeamID = teamChange.TeamIDInto;
            }
            else if (teamChange.TeamIDComingFrom > 0)
            {
                jsonActor.TeamID = teamChange.TeamIDComingFrom;
            }
        }
        //
        jsonActor.DpsAll = phases.Select(phase => JsonStatisticsBuilder.BuildJsonDPS(actor.GetDamageStats(log, phase.Start, phase.End))).ToArray();
        jsonActor.StatsAll = phases.Select(phase => JsonStatisticsBuilder.BuildJsonGameplayStatsAll(actor.GetGameplayStats(log, phase.Start, phase.End), actor.GetOffensiveStats(null, log, phase.Start, phase.End))).ToArray();
        jsonActor.Defenses = phases.Select(phase => JsonStatisticsBuilder.BuildJsonDefensesAll(actor.GetDefenseStats(log, phase.Start, phase.End))).ToArray();
        //
        IReadOnlyDictionary<long, Minions> minionsList = actor.GetMinions(log);
        if (minionsList.Values.Any())
        {
            jsonActor.Minions = minionsList.Values.Select(x => JsonMinionsBuilder.BuildJsonMinions(x, log, settings, skillMap, buffMap)).ToList();
        }
        //
        var casts = actor.GetIntersectingCastEvents(log);
        if (casts.Any())
        {
            jsonActor.Rotation = JsonRotationBuilder.BuildJsonRotationList(log, casts.GroupBy(x => x.SkillID), skillMap).ToList();
        }
        //
        if (settings.RawFormatTimelineArrays)
        {
            var damage1S = new IReadOnlyList<int>[phases.Count];
            var powerDamage1S = new IReadOnlyList<int>[phases.Count];
            var conditionDamage1S = new IReadOnlyList<int>[phases.Count];
            var breakbarDamage1S = new IReadOnlyList<double>?[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                damage1S[i] = actor.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.All).Values;
                powerDamage1S[i] = actor.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power).Values;
                conditionDamage1S[i] = actor.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition).Values;
                breakbarDamage1S[i] = actor.GetBreakbarDamageGraph(log, phase.Start, phase.End, null)?.Values;
            }
            jsonActor.Damage1S = damage1S;
            jsonActor.PowerDamage1S = powerDamage1S;
            jsonActor.ConditionDamage1S = conditionDamage1S;
            jsonActor.BreakbarDamage1S = breakbarDamage1S;
        }
        if (!log.CombatData.HasBreakbarDamageData)
        {
            jsonActor.BreakbarDamage1S = null;
        }
        //
        jsonActor.TotalDamageDist = BuildDamageDistData(actor, phases, log, skillMap, buffMap);
        jsonActor.TotalDamageTaken = BuildDamageTakenDistData(actor, phases, log, skillMap, buffMap);
        //
        if (settings.RawFormatTimelineArrays)
        {
            IReadOnlyDictionary<long, BuffGraph> buffGraphs = actor.GetBuffGraphs(log);
            jsonActor.BoonsStates = JsonBuffsUptimeBuilder.GetBuffStates(buffGraphs[SkillIDs.NumberOfBoons])?.ToList();
            jsonActor.ConditionsStates = JsonBuffsUptimeBuilder.GetBuffStates(buffGraphs[SkillIDs.NumberOfConditions])?.ToList();
            if (buffGraphs.TryGetValue(SkillIDs.NumberOfActiveCombatMinions, out var states))
            {
                jsonActor.ActiveCombatMinions = JsonBuffsUptimeBuilder.GetBuffStates(states).ToList();
            }
            // Health
            jsonActor.HealthPercents = actor.GetHealthUpdates(log).Select(x => new List<double>() { x.Start, x.Value }).ToList();
            jsonActor.BarrierPercents = actor.GetBarrierUpdates(log).Select(x => new List<double>() { x.Start, x.Value }).ToList();
        }
        if (log.CanCombatReplay)
        {
            jsonActor.CombatReplayData = JsonActorCombatReplayDataBuilder.BuildJsonActorCombatReplayDataBuilder(actor, log, settings);
        }
    }

    private static List<JsonDamageDist>[] BuildDamageDistData(SingleActor actor, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
    {
        var res = new List<JsonDamageDist>[phases.Count];
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            res[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(
                actor.GetJustActorDamageEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                actor.GetJustActorBreakbarDamageEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                log,
                skillMap,
                buffMap
            );
        }
        return res;
    }

    private static List<JsonDamageDist>[] BuildDamageTakenDistData(SingleActor actor, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
    {
        var res = new List<JsonDamageDist>[phases.Count];
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            res[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(
                actor.GetDamageTakenEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                actor.GetBreakbarDamageTakenEvents(null, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                log,
                skillMap,
                buffMap
           );
        }
        return res;
    }
}
