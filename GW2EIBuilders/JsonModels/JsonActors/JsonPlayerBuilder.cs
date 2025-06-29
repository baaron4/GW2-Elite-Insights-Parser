using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIJSON.JsonBuffsUptime;
using static GW2EIJSON.JsonBuffVolumes;
using static GW2EIJSON.JsonPlayerBuffsGeneration;
using static GW2EIJSON.JsonPlayerBuffOutgoingVolumes;

namespace GW2EIBuilders.JsonModels.JsonActors;

/// <summary>
/// Class representing a player
/// </summary>
internal static class JsonPlayerBuilder
{

    public static JsonPlayer BuildJsonPlayer(SingleActor player, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap, Dictionary<int, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalBuffs, Dictionary<string, HashSet<long>> personalDamageMods)
    {
        var jsonPlayer = new JsonPlayer();
        JsonActorBuilder.FillJsonActor(jsonPlayer, player, log, settings, skillMap, buffMap);
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        //
        jsonPlayer.Account = player.Account;
        jsonPlayer.Weapons = player.GetWeaponSets(log).ToArray();
        jsonPlayer.Group = player.Group;
        jsonPlayer.Profession = player.Spec.ToString();
        jsonPlayer.FriendlyNPC = player is NPC;
        jsonPlayer.NotInSquad = player is PlayerNonSquad;
        GuildEvent? guildEvent = log.CombatData.GetGuildEvents(player.AgentItem).FirstOrDefault();
        if (guildEvent != null)
        {
            jsonPlayer.GuildID = guildEvent.APIString;
        }
        jsonPlayer.ActiveTimes = phases.Select(x => player.GetActiveDuration(log, x.Start, x.End)).ToList();
        if (player is Player p)
        {
            jsonPlayer.HasCommanderTag = p.IsCommander(log);
            jsonPlayer.CommanderTagStates = jsonPlayer.HasCommanderTag ? p.GetCommanderStatesNoTagValues(log).Select(x => new List<long>() { x.Start, x.End }).ToList() : null;
        }
        //
        jsonPlayer.Support = phases.Select(phase => JsonStatisticsBuilder.BuildJsonAllySupport(player.GetToAllySupportStats(log, phase.Start, phase.End))).ToArray();
        var targetDamage1S = new IReadOnlyList<int>[log.FightData.Logic.Targets.Count][];
        var targetPowerDamage1S = new IReadOnlyList<int>[log.FightData.Logic.Targets.Count][];
        var targetConditionDamage1S = new IReadOnlyList<int>[log.FightData.Logic.Targets.Count][];
        var targetBreakbarDamage1S = new IReadOnlyList<double>?[log.FightData.Logic.Targets.Count][];
        var dpsTargets = new JsonStatistics.JsonDPS[log.FightData.Logic.Targets.Count][];
        var statsTargets = new JsonStatistics.JsonGameplayStats[log.FightData.Logic.Targets.Count][];
        var targetDamageDist = new IReadOnlyList<JsonDamageDist>[log.FightData.Logic.Targets.Count][];

        if (settings.RawFormatTimelineArrays)
        {
            var damageTaken1S = new IReadOnlyList<int>[phases.Count];
            var powerDamageTaken1S = new IReadOnlyList<int>[phases.Count];
            var conditionDamageTaken1S = new IReadOnlyList<int>[phases.Count];
            var breakbarDamageTaken1S = new IReadOnlyList<double>?[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                damageTaken1S[i] = player.GetDamageTakenGraph(log, phase.Start, phase.End, null, DamageType.All).Values;
                powerDamageTaken1S[i] = player.GetDamageTakenGraph(log, phase.Start, phase.End, null, DamageType.Power).Values;
                conditionDamageTaken1S[i] = player.GetDamageTakenGraph(log, phase.Start, phase.End, null, DamageType.Condition).Values;
                breakbarDamageTaken1S[i] = player.GetBreakbarDamageTakenGraph(log, phase.Start, phase.End, null)?.Values;
            }
            jsonPlayer.DamageTaken1S = damageTaken1S;
            jsonPlayer.PowerDamageTaken1S = powerDamageTaken1S;
            jsonPlayer.ConditionDamageTaken1S = conditionDamageTaken1S;
            jsonPlayer.BreakbarDamageTaken1S = breakbarDamageTaken1S;
        }
        for (int j = 0; j < log.FightData.Logic.Targets.Count; j++)
        {
            SingleActor target = log.FightData.Logic.Targets[j];
            var graph1SDamageList = new IReadOnlyList<int>[phases.Count];
            var graph1SPowerDamageList = new IReadOnlyList<int>[phases.Count];
            var graph1SConditionDamageList = new IReadOnlyList<int>[phases.Count];
            var graph1SBreakbarDamageList = new IReadOnlyList<double>?[phases.Count];
            var targetDamageDistList = new IReadOnlyList<JsonDamageDist>[phases.Count];
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (settings.RawFormatTimelineArrays)
                {
                    graph1SDamageList[i] = player.GetDamageGraph(log, phase.Start, phase.End, target, DamageType.All).Values;
                    graph1SPowerDamageList[i] = player.GetDamageGraph(log, phase.Start, phase.End, target, DamageType.Power).Values;
                    graph1SConditionDamageList[i] = player.GetDamageGraph(log, phase.Start, phase.End, target, DamageType.Condition).Values;
                    graph1SBreakbarDamageList[i] = player.GetBreakbarDamageGraph(log, phase.Start, phase.End, target)?.Values;
                }
                targetDamageDistList[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(
                    player.GetJustActorDamageEvents(target, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                    player.GetJustActorBreakbarDamageEvents(target, log, phase.Start, phase.End).GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList()),
                    log,
                    skillMap,
                    buffMap
                );
            }
            if (settings.RawFormatTimelineArrays)
            {
                targetDamage1S[j] = graph1SDamageList;
                targetPowerDamage1S[j] = graph1SPowerDamageList;
                targetConditionDamage1S[j] = graph1SConditionDamageList;
                targetBreakbarDamage1S[j] = graph1SBreakbarDamageList;
            }
            targetDamageDist[j] = targetDamageDistList;
            dpsTargets[j] = phases.Select(phase => JsonStatisticsBuilder.BuildJsonDPS(player.GetDamageStats(target, log, phase.Start, phase.End))).ToArray();
            statsTargets[j] = phases.Select(phase => JsonStatisticsBuilder.BuildJsonGameplayStats(player.GetOffensiveStats(target, log, phase.Start, phase.End))).ToArray();
        }
        if (settings.RawFormatTimelineArrays)
        {
            jsonPlayer.TargetDamage1S = targetDamage1S;
            jsonPlayer.TargetPowerDamage1S = targetPowerDamage1S;
            jsonPlayer.TargetConditionDamage1S = targetConditionDamage1S;
            jsonPlayer.TargetBreakbarDamage1S = targetBreakbarDamage1S;
            IReadOnlyDictionary<long, BuffGraph> buffGraphs = player.GetBuffGraphs(log);
            if (buffGraphs.TryGetValue(SkillIDs.NumberOfClones, out var states))
            {
                jsonPlayer.ActiveClones = JsonBuffsUptimeBuilder.GetBuffStates(states).ToList();
            }
            if (buffGraphs.TryGetValue(SkillIDs.NumberOfRangerPets, out states))
            {
                jsonPlayer.ActiveRangerPets = JsonBuffsUptimeBuilder.GetBuffStates(states).ToList();
            }
        }
        jsonPlayer.TargetDamageDist = targetDamageDist;
        jsonPlayer.DpsTargets = dpsTargets;
        jsonPlayer.StatsTargets = statsTargets;
        if (!log.CombatData.HasBreakbarDamageData)
        {
            jsonPlayer.TargetBreakbarDamage1S = null;
        }
        //
        var selfBuffs = phases.Select(phase => player.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList();
        jsonPlayer.BuffUptimes = GetPlayerJsonBuffsUptime(player, selfBuffs, phases.Select(phase => player.GetBuffsDictionary(log, phase.Start, phase.End)).ToList(), log, settings, buffMap, personalBuffs);
        jsonPlayer.SelfBuffs = GetPlayerBuffGenerations(selfBuffs, log, buffMap);
        jsonPlayer.GroupBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.Group, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.OffGroupBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.OffGroup, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.SquadBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.Squad, log, phase.Start, phase.End)).ToList(), log, buffMap);
        //
        var selfActiveBuffs = phases.Select(phase => player.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList();
        jsonPlayer.BuffUptimesActive = GetPlayerJsonBuffsUptime(player, selfActiveBuffs, phases.Select(phase => player.GetActiveBuffsDictionary(log, phase.Start, phase.End)).ToList(), log, settings, buffMap, personalBuffs);
        jsonPlayer.SelfBuffsActive = GetPlayerBuffGenerations(selfActiveBuffs, log, buffMap);
        jsonPlayer.GroupBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.Group, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.OffGroupBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.OffGroup, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.SquadBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.Squad, log, phase.Start, phase.End)).ToList(), log, buffMap);
        //
        var selfBuffVolumes = phases.Select(phase => player.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)).ToList();
        jsonPlayer.BuffVolumes = GetPlayerJsonBuffVolumes(player, selfBuffVolumes, phases.Select(phase => player.GetBuffVolumesDictionary(log, phase.Start, phase.End)).ToList(), log, settings, buffMap, personalBuffs);
        jsonPlayer.SelfBuffVolumes = GetPlayerBuffOutgoingVolumes(selfBuffVolumes, log, buffMap);
        jsonPlayer.GroupBuffVolumes = GetPlayerBuffOutgoingVolumes(phases.Select(phase => player.GetBuffVolumes(BuffEnum.Group, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.OffGroupBuffVolumes = GetPlayerBuffOutgoingVolumes(phases.Select(phase => player.GetBuffVolumes(BuffEnum.OffGroup, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.SquadBuffVolumes = GetPlayerBuffOutgoingVolumes(phases.Select(phase => player.GetBuffVolumes(BuffEnum.Squad, log, phase.Start, phase.End)).ToList(), log, buffMap);
        //
        var selfBuffActiveVolumes = phases.Select(phase => player.GetActiveBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)).ToList();
        jsonPlayer.BuffVolumesActive = GetPlayerJsonBuffVolumes(player, selfBuffActiveVolumes, phases.Select(phase => player.GetActiveBuffVolumesDictionary(log, phase.Start, phase.End)).ToList(), log, settings, buffMap, personalBuffs);
        jsonPlayer.SelfBuffVolumesActive = GetPlayerBuffOutgoingVolumes(selfBuffActiveVolumes, log, buffMap);
        jsonPlayer.GroupBuffVolumesActive = GetPlayerBuffOutgoingVolumes(phases.Select(phase => player.GetActiveBuffVolumes(BuffEnum.Group, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.OffGroupBuffVolumesActive = GetPlayerBuffOutgoingVolumes(phases.Select(phase => player.GetActiveBuffVolumes(BuffEnum.OffGroup, log, phase.Start, phase.End)).ToList(), log, buffMap);
        jsonPlayer.SquadBuffVolumesActive = GetPlayerBuffOutgoingVolumes(phases.Select(phase => player.GetActiveBuffVolumes(BuffEnum.Squad, log, phase.Start, phase.End)).ToList(), log, buffMap);
        //
        IReadOnlyList<Consumable> consumables = player.GetConsumablesList(log, log.FightData.FightStart, log.FightData.FightEnd);
        if (consumables.Count > 0)
        {
            var consumablesJSON = new List<JsonConsumable>(consumables.Count);
            foreach (Consumable food in consumables)
            {
                if (!buffMap.ContainsKey(food.Buff.ID))
                {
                    buffMap[food.Buff.ID] = food.Buff;
                }
                consumablesJSON.Add(JsonConsumableBuilder.BuildJsonConsumable(food));
            }
            jsonPlayer.Consumables = consumablesJSON;
        }
        //
        IReadOnlyList<DeathRecap> deathRecaps = player.GetDeathRecaps(log);
        if (deathRecaps.Count > 0)
        {
            jsonPlayer.DeathRecap = deathRecaps.Select(x => JsonDeathRecapBuilder.BuildJsonDeathRecap(x)).ToList();
        }
        // 
        jsonPlayer.DamageModifiers = JsonDamageModifierDataBuilder.GetOutgoingDamageModifiers(player, phases.Select(x => player.GetOutgoingDamageModifierStats(null, log, x.Start, x.End)).ToList(), log, damageModMap, personalDamageMods);
        jsonPlayer.DamageModifiersTarget = JsonDamageModifierDataBuilder.GetOutgoingDamageModifiersTarget(player, log, damageModMap, personalDamageMods, phases);
        jsonPlayer.IncomingDamageModifiers = JsonDamageModifierDataBuilder.GetIncomingDamageModifiers(player, phases.Select(x => player.GetIncomingDamageModifierStats(null, log, x.Start, x.End)).ToList(), log, damageModMap, personalDamageMods);
        jsonPlayer.IncomingDamageModifiersTarget = JsonDamageModifierDataBuilder.GetIncomingDamageModifiersTarget(player, log, damageModMap, personalDamageMods, phases);
        if (log.CombatData.HasEXTHealing)
        {
            jsonPlayer.EXTHealingStats = EXTJsonPlayerHealingStatsBuilder.BuildPlayerHealingStats(player, log, settings, skillMap, buffMap);
        }
        if (log.CombatData.HasEXTBarrier)
        {
            jsonPlayer.EXTBarrierStats = EXTJsonPlayerBarrierStatsBuilder.BuildPlayerBarrierStats(player, log, settings, skillMap, buffMap);
        }
        return jsonPlayer;
    }

    //TODO(Rennorb) @perf
    private static List<JsonPlayerBuffsGeneration>? GetPlayerBuffGenerations(List<IReadOnlyDictionary<long, BuffStatistics>> buffs, ParsedEvtcLog log, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        var uptimes = new List<JsonPlayerBuffsGeneration>(buffs[0].Count);
        foreach (KeyValuePair<long, BuffStatistics> pair in buffs[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            if (!buffMap.ContainsKey(pair.Key))
            {
                buffMap[pair.Key] = buff;
            }
            var data = new List<JsonBuffsGenerationData>(phases.Count);
            for (int i = 0; i < phases.Count; i++)
            {
                if (buffs[i].TryGetValue(pair.Key, out var buffStats))
                {
                    JsonBuffsGenerationData value = JsonPlayerBuffsGenerationBuilder.BuildJsonBuffsGenerationData(buffStats);
                    data.Add(value);
                }
                else
                {
                    var value = new JsonBuffsGenerationData();
                    data.Add(value);
                }
            }
            var jsonBuffs = new JsonPlayerBuffsGeneration()
            {
                BuffData = data,
                Id = pair.Key
            };
            uptimes.Add(jsonBuffs);
        }

        if (uptimes.Count == 0)
        {
            return null;
        }

        return uptimes;
    }

    private static List<JsonBuffsUptime> GetPlayerJsonBuffsUptime(SingleActor player, List<IReadOnlyDictionary<long, BuffStatistics>> buffs, List<IReadOnlyDictionary<long, BuffByActorStatistics>> buffDictionaries, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, Buff> buffMap, Dictionary<string, HashSet<long>> personalBuffs)
    {
        var res = new List<JsonBuffsUptime>(buffs[0].Count);
        var profEnums = new HashSet<Source>(SpecToSources(player.Spec));
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        foreach (KeyValuePair<long, BuffStatistics> pair in buffs[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            var data = new List<JsonBuffsUptimeData>(phases.Count);
            for (int i = 0; i < phases.Count; i++)
            {
                if (buffs[i].TryGetValue(pair.Key, out var buffStats) && buffDictionaries[i].TryGetValue(pair.Key, out var buffDicts))
                {
                    JsonBuffsUptimeData value = JsonBuffsUptimeBuilder.BuildJsonBuffsUptimeData(buffStats, buffDicts);
                    data.Add(value);
                }
                else
                {
                    var value = new JsonBuffsUptimeData();
                    data.Add(value);
                }
            }
            if (buff.Classification == Buff.BuffClassification.Other && profEnums.Contains(buff.Source))
            {
                if (player.GetBuffDistribution(log, phases[0].Start, phases[0].End).GetUptime(pair.Key) > 0)
                {
                    if (personalBuffs.TryGetValue(player.Spec.ToString(), out var personalBuffSet))
                    {
                        personalBuffSet.Add(pair.Key);
                    }
                    else
                    {
                        personalBuffs[player.Spec.ToString()] =
                            [
                                pair.Key
                            ];
                    }
                }
            }
            res.Add(JsonBuffsUptimeBuilder.BuildJsonBuffsUptime(player, pair.Key, log, settings, data, buffMap));
        }
        return res;
    }

    //TODO(Rennorb) @perf
    private static List<JsonPlayerBuffOutgoingVolumes>? GetPlayerBuffOutgoingVolumes(List<IReadOnlyDictionary<long, BuffVolumeStatistics>> buffVolumes, ParsedEvtcLog log, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        var uptimes = new List<JsonPlayerBuffOutgoingVolumes>(buffVolumes[0].Count);
        foreach (KeyValuePair<long, BuffVolumeStatistics> pair in buffVolumes[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            if (!buffMap.ContainsKey(pair.Key))
            {
                buffMap[pair.Key] = buff;
            }
            var data = new List<JsonBuffOutgoingVolumesData>(phases.Count);
            for (int i = 0; i < phases.Count; i++)
            {
                if (buffVolumes[i].TryGetValue(pair.Key, out var buffVolumeStats))
                {
                    JsonBuffOutgoingVolumesData value = JsonPlayerBuffOutgoingVolumesBuilder.BuildJsonBuffsOutgoingVolumesData(buffVolumeStats);
                    data.Add(value);
                }
                else
                {
                    var value = new JsonBuffOutgoingVolumesData();
                    data.Add(value);
                }
            }
            var jsonBuffs = new JsonPlayerBuffOutgoingVolumes()
            {
                BuffVolumeData = data,
                Id = pair.Key
            };
            uptimes.Add(jsonBuffs);
        }

        if (uptimes.Count == 0)
        {
            return null;
        }

        return uptimes;
    }

    private static List<JsonBuffVolumes> GetPlayerJsonBuffVolumes(SingleActor player, List<IReadOnlyDictionary<long, BuffVolumeStatistics>> buffVolumes, List<IReadOnlyDictionary<long, BuffVolumeByActorStatistics>> buffVolumeDictionaries, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<long, Buff> buffMap, Dictionary<string, HashSet<long>> personalBuffs)
    {
        var res = new List<JsonBuffVolumes>(buffVolumes[0].Count);
        var profEnums = new HashSet<Source>(SpecToSources(player.Spec));
        IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
        foreach (KeyValuePair<long, BuffVolumeStatistics> pair in buffVolumes[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            var data = new List<JsonBuffVolumesData>(phases.Count);
            for (int i = 0; i < phases.Count; i++)
            {
                if (buffVolumes[i].TryGetValue(pair.Key, out var buffVol) && buffVolumeDictionaries[i].TryGetValue(pair.Key, out var buffVolDict))
                {
                    JsonBuffVolumesData value = JsonBuffVolumesBuilder.BuildJsonBuffVolumesData(buffVol, buffVolDict);
                    data.Add(value);
                }
                else
                {
                    var value = new JsonBuffVolumesData();
                    data.Add(value);
                }
            }
            if (buff.Classification == Buff.BuffClassification.Other && profEnums.Contains(buff.Source))
            {
                if (player.GetBuffDistribution(log, phases[0].Start, phases[0].End).GetUptime(pair.Key) > 0)
                {
                    if (personalBuffs.TryGetValue(player.Spec.ToString(), out var hashSet))
                    {
                        hashSet.Add(pair.Key);
                    }
                    else
                    {
                        personalBuffs[player.Spec.ToString()] =
                            [
                                pair.Key
                            ];
                    }
                }
            }
            res.Add(JsonBuffVolumesBuilder.BuildJsonBuffVolumes(pair.Key, log, data, buffMap));
        }
        return res;
    }

}
