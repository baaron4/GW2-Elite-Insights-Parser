using GW2EIBuilders.JsonModels.JsonActorUtilities;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing;
using GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIJSON.JsonBuffsUptime;
using static GW2EIJSON.JsonBuffVolumes;
using static GW2EIJSON.JsonPlayerBuffOutgoingVolumes;
using static GW2EIJSON.JsonPlayerBuffsGeneration;

namespace GW2EIBuilders.JsonModels.JsonActors;

/// <summary>
/// Class representing a player
/// </summary>
internal static class JsonPlayerBuilder
{

    public static JsonPlayer BuildJsonPlayer(SingleActor player, ParsedEvtcLog log, RawFormatSettings settings, UsedJsonData usedData)
    {
        var jsonPlayer = new JsonPlayer();
        JsonActorBuilder.FillJsonActor(jsonPlayer, player, log, settings, usedData);
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        //
        jsonPlayer.Account = player.Account;
        var weaponSets = player.GetWeaponSets(log);
        jsonPlayer.Weapons = weaponSets[^1].ToArray().Weapons;
        jsonPlayer.WeaponSets = weaponSets.Select(x =>
        {
            var (Weapons, Timeframe) = x.ToArray();
            return new JsonWeaponSet()
            {
                Weapons = Weapons,
                Timeframe = Timeframe,
            };
        }).ToList();
        jsonPlayer.Group = player.Group;
        jsonPlayer.Profession = player.Spec.ToString();
        jsonPlayer.FriendlyNPC = player is NPC;
        jsonPlayer.NotInSquad = player is PlayerNonSquad;
        jsonPlayer.IsEnglobed = player.AgentItem.IsEnglobedAgent;
        GuildEvent? guildEvent = log.CombatData.GetGuildEvents(player.AgentItem).FirstOrDefault();
        if (guildEvent != null)
        {
            jsonPlayer.GuildID = guildEvent.APIString;
        }
        jsonPlayer.ActiveTimes = phases.Select(x => player.GetActiveDuration(log, x.Start, x.End)).ToList();
        if (player is Player p)
        {
            var isCommander = p.IsCommander(log);
            jsonPlayer.HasCommanderTag = isCommander;
            jsonPlayer.CommanderTagStates = isCommander ? p.GetCommanderStatesNoTagValues(log).Select(x => new List<long>() { x.Start, x.End }).ToList() : null;
        }
        //
        jsonPlayer.Support = phases.Select(phase => JsonStatisticsBuilder.BuildJsonAllySupport(player.GetToAllySupportStats(log, phase.Start, phase.End))).ToArray();
        var targetDamage1S = new IReadOnlyList<int>[log.LogData.Logic.Targets.Count][];
        var targetPowerDamage1S = new IReadOnlyList<int>[log.LogData.Logic.Targets.Count][];
        var targetConditionDamage1S = new IReadOnlyList<int>[log.LogData.Logic.Targets.Count][];
        var targetBreakbarDamage1S = new IReadOnlyList<double>?[log.LogData.Logic.Targets.Count][];
        var dpsTargets = new JsonStatistics.JsonDPS[log.LogData.Logic.Targets.Count][];
        var statsTargets = new JsonStatistics.JsonGameplayStats[log.LogData.Logic.Targets.Count][];
        var targetDamageDist = new IReadOnlyList<JsonDamageDist>[log.LogData.Logic.Targets.Count][];

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
        for (int j = 0; j < log.LogData.Logic.Targets.Count; j++)
        {
            SingleActor target = log.LogData.Logic.Targets[j];
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
                    player.GetOffensiveStats(target, log, phase.Start, phase.End).DownContributionPerSkillID,
                    log, usedData
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
                jsonPlayer.ActiveClones = JsonBuffsUptimeBuilder.GetBuffStates(states);
            }
            if (buffGraphs.TryGetValue(SkillIDs.NumberOfRangerPets, out states))
            {
                jsonPlayer.ActiveRangerPets = JsonBuffsUptimeBuilder.GetBuffStates(states);
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
        jsonPlayer.BuffUptimes = GetPlayerJsonBuffsUptime(player, log, settings, usedData, false);
        jsonPlayer.SelfBuffs = GetPlayerBuffGenerations(player, BuffEnum.Self, log, usedData, false);
        jsonPlayer.GroupBuffs = GetPlayerBuffGenerations(player, BuffEnum.Group, log, usedData, false);
        jsonPlayer.OffGroupBuffs = GetPlayerBuffGenerations(player, BuffEnum.OffGroup, log, usedData, false);
        jsonPlayer.SquadBuffs = GetPlayerBuffGenerations(player, BuffEnum.Squad, log, usedData, false);
        //
        jsonPlayer.BuffUptimesActive = GetPlayerJsonBuffsUptime(player, log, settings, usedData, true);
        jsonPlayer.SelfBuffsActive = GetPlayerBuffGenerations(player, BuffEnum.Self, log, usedData, true);
        jsonPlayer.GroupBuffsActive = GetPlayerBuffGenerations(player, BuffEnum.Group, log, usedData, true);
        jsonPlayer.OffGroupBuffsActive = GetPlayerBuffGenerations(player, BuffEnum.OffGroup, log, usedData, true);
        jsonPlayer.SquadBuffsActive = GetPlayerBuffGenerations(player, BuffEnum.Squad, log, usedData, true);
        //
        jsonPlayer.BuffVolumes = GetPlayerJsonBuffVolumes(player, log, settings, usedData, false);
        jsonPlayer.SelfBuffVolumes = GetPlayerBuffOutgoingVolumes(player, BuffEnum.Self, log, usedData, false);
        jsonPlayer.GroupBuffVolumes = GetPlayerBuffOutgoingVolumes(player, BuffEnum.Group, log, usedData, false);
        jsonPlayer.OffGroupBuffVolumes = GetPlayerBuffOutgoingVolumes(player, BuffEnum.OffGroup, log, usedData, false);
        jsonPlayer.SquadBuffVolumes = GetPlayerBuffOutgoingVolumes(player, BuffEnum.Squad, log, usedData, false);
        //
        jsonPlayer.BuffVolumesActive = GetPlayerJsonBuffVolumes(player, log, settings, usedData, true);
        jsonPlayer.SelfBuffVolumesActive = GetPlayerBuffOutgoingVolumes(player, BuffEnum.Self, log, usedData, true);
        jsonPlayer.GroupBuffVolumesActive = GetPlayerBuffOutgoingVolumes(player, BuffEnum.Group, log, usedData, true);
        jsonPlayer.OffGroupBuffVolumesActive = GetPlayerBuffOutgoingVolumes(player, BuffEnum.OffGroup, log, usedData, true);
        jsonPlayer.SquadBuffVolumesActive = GetPlayerBuffOutgoingVolumes(player, BuffEnum.Squad, log, usedData, true);
        //
        IReadOnlyList<Consumable> consumables = player.GetConsumablesList(log);
        if (consumables.Count > 0)
        {
            var consumablesJSON = new List<JsonConsumable>(consumables.Count);
            foreach (Consumable food in consumables)
            {
                usedData.buffs.TryAdd(food.Buff.ID, food.Buff);
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
        jsonPlayer.DamageModifiers = JsonDamageModifierDataBuilder.GetOutgoingDamageModifiers(player, phases.Select(x => player.GetOutgoingDamageModifierStats(null, log, x.Start, x.End)).ToList(), log, usedData);
        jsonPlayer.DamageModifiersTarget = JsonDamageModifierDataBuilder.GetOutgoingDamageModifiersTarget(player, log, usedData, phases);
        jsonPlayer.IncomingDamageModifiers = JsonDamageModifierDataBuilder.GetIncomingDamageModifiers(player, phases.Select(x => player.GetIncomingDamageModifierStats(null, log, x.Start, x.End)).ToList(), log, usedData);
        jsonPlayer.IncomingDamageModifiersTarget = JsonDamageModifierDataBuilder.GetIncomingDamageModifiersTarget(player, log, usedData, phases);
        if (log.CombatData.HasEXTHealing)
        {
            jsonPlayer.EXTHealingStats = EXTJsonPlayerHealingStatsBuilder.BuildPlayerHealingStats(player, log, settings, usedData);
        }
        if (log.CombatData.HasEXTBarrier)
        {
            jsonPlayer.EXTBarrierStats = EXTJsonPlayerBarrierStatsBuilder.BuildPlayerBarrierStats(player, log, settings, usedData);
        }
        return jsonPlayer;
    }

    //TODO_PERF(Rennorb)
    private static List<JsonPlayerBuffsGeneration>? GetPlayerBuffGenerations(SingleActor player, BuffEnum type, ParsedEvtcLog log, UsedJsonData usedData, bool useActiveTime)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var buffs = phases.Select(phase => useActiveTime ? player.GetActiveBuffs(type, log, phase.Start, phase.End) : player.GetBuffs(type, log, phase.Start, phase.End)).ToList();
        var uptimes = new List<JsonPlayerBuffsGeneration>(buffs[0].Count);
        foreach (KeyValuePair<long, BuffStatistics> pair in buffs[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            usedData.buffs.TryAdd(pair.Key, buff);
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

    private static List<JsonBuffsUptime> GetPlayerJsonBuffsUptime(SingleActor player, ParsedEvtcLog log, RawFormatSettings settings, UsedJsonData usedData, bool useActiveTime)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var buffs = phases.Select(phase => useActiveTime ? player.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End) : player.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList();
        var buffDictionaries = phases.Select(phase => useActiveTime ? player.GetActiveBuffsDictionary(log, phase.Start, phase.End) : player.GetBuffsDictionary(log, phase.Start, phase.End)).ToList();
        var res = new List<JsonBuffsUptime>(buffs[0].Count);
        var profEnums = new HashSet<Source>(SpecToSources(player.Spec));
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
            if (buff.Classification == Buff.BuffClassification.Other && profEnums.Intersect(buff.Sources).Any())
            {
                if (player.GetBuffDistribution(log, phases[0].Start, phases[0].End).GetUptime(pair.Key) > 0)
                {
                    if (usedData.personalBuffs.TryGetValue(player.Spec.ToString(), out var personalBuffSet))
                    {
                        personalBuffSet.Add(pair.Key);
                    }
                    else
                    {
                        usedData.personalBuffs[player.Spec.ToString()] =
                            [
                                pair.Key
                            ];
                    }
                }
            }
            res.Add(JsonBuffsUptimeBuilder.BuildJsonBuffsUptime(player, pair.Key, log, settings, data, usedData));
        }
        return res;
    }

    //TODO_PERF(Rennorb)
    private static List<JsonPlayerBuffOutgoingVolumes>? GetPlayerBuffOutgoingVolumes(SingleActor player, BuffEnum type, ParsedEvtcLog log, UsedJsonData usedData, bool useActiveTime)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var buffVolumes = phases.Select(phase => useActiveTime ? player.GetActiveBuffVolumes(type, log, phase.Start, phase.End) : player.GetBuffVolumes(type, log, phase.Start, phase.End)).ToList();
        var uptimes = new List<JsonPlayerBuffOutgoingVolumes>(buffVolumes[0].Count);
        foreach (KeyValuePair<long, BuffVolumeStatistics> pair in buffVolumes[0])
        {
            Buff buff = log.Buffs.BuffsByIDs[pair.Key];
            if (buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            usedData.buffs.TryAdd(pair.Key, buff);
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

    private static List<JsonBuffVolumes> GetPlayerJsonBuffVolumes(SingleActor player, ParsedEvtcLog log, RawFormatSettings settings, UsedJsonData usedData, bool useActiveTime)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var buffVolumes = phases.Select(phase => useActiveTime ? player.GetActiveBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End) : player.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)).ToList();
        var buffVolumeDictionaries = phases.Select(phase => useActiveTime ? player.GetActiveBuffVolumesDictionary(log, phase.Start, phase.End) : player.GetBuffVolumesDictionary(log, phase.Start, phase.End)).ToList();
        var res = new List<JsonBuffVolumes>(buffVolumes[0].Count);
        var profEnums = new HashSet<Source>(SpecToSources(player.Spec));
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
            if (buff.Classification == Buff.BuffClassification.Other && profEnums.Intersect(buff.Sources).Any())
            {
                if (player.GetBuffDistribution(log, phases[0].Start, phases[0].End).GetUptime(pair.Key) > 0)
                {
                    if (usedData.personalBuffs.TryGetValue(player.Spec.ToString(), out var hashSet))
                    {
                        hashSet.Add(pair.Key);
                    }
                    else
                    {
                        usedData.personalBuffs[player.Spec.ToString()] =
                            [
                                pair.Key
                            ];
                    }
                }
            }
            res.Add(JsonBuffVolumesBuilder.BuildJsonBuffVolumes(pair.Key, log, data, usedData));
        }
        return res;
    }

}
