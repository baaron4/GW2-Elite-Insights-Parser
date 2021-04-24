using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using Newtonsoft.Json;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIJSON.JsonBuffsUptime;
using static GW2EIJSON.JsonPlayerBuffsGeneration;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class representing a player
    /// </summary>
    internal static class JsonPlayerBuilder
    {
        
        public static JsonPlayer BuildJsonPlayer(Player player, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc, Dictionary<string, JsonLog.DamageModDesc> damageModDesc, Dictionary<string, HashSet<long>> personalBuffs)
        {
            var jsonPlayer = new JsonPlayer();
            JsonActorBuilder.FillJsonActor(jsonPlayer, player, log, settings, skillDesc, buffDesc);
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            //
            jsonPlayer.Account = player.Account;
            jsonPlayer.Weapons = player.GetWeaponsArray(log).Select(w => w ?? "Unknown").ToArray();
            jsonPlayer.Group = player.Group;
            jsonPlayer.Profession = player.Prof;
            jsonPlayer.FriendlyNPC = player.IsCustomActor;
            jsonPlayer.ActiveTimes = phases.Select(x => player.GetActiveDuration(log, x.Start, x.End)).ToList();
            jsonPlayer.HasCommanderTag = player.HasCommanderTag;
            //
            jsonPlayer.Support = phases.Select(phase => JsonStatisticsBuilder.BuildJsonPlayerSupport(player.GetPlayerSupportStats(log, phase.Start, phase.End))).ToArray();           
            var targetDamage1S = new IReadOnlyList<int>[log.FightData.Logic.Targets.Count][];
            var targetPowerDamage1S = new IReadOnlyList<int>[log.FightData.Logic.Targets.Count][];
            var targetConditionDamage1S = new IReadOnlyList<int>[log.FightData.Logic.Targets.Count][];
            var targetBreakbarDamage1S = new IReadOnlyList<double>[log.FightData.Logic.Targets.Count][];
            var dpsTargets = new JsonStatistics.JsonDPS[log.FightData.Logic.Targets.Count][];
            var statsTargets = new JsonStatistics.JsonGameplayStats[log.FightData.Logic.Targets.Count][];
            var targetDamageDist = new IReadOnlyList<JsonDamageDist>[log.FightData.Logic.Targets.Count][];
            for (int j = 0; j < log.FightData.Logic.Targets.Count; j++)
            {
                NPC target = log.FightData.Logic.Targets[j];
                var graph1SDamageList = new IReadOnlyList<int>[phases.Count];
                var graph1SPowerDamageList = new IReadOnlyList<int>[phases.Count];
                var graph1SConditionDamageList = new IReadOnlyList<int>[phases.Count];
                var graph1SBreakbarDamageList = new IReadOnlyList<double>[phases.Count];
                var targetDamageDistList = new IReadOnlyList<JsonDamageDist>[phases.Count];
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    if (settings.RawFormatTimelineArrays)
                    {
                        graph1SDamageList[i] = player.Get1SDamageList(log, phase.Start, phase.End, target, DamageType.All);
                        graph1SPowerDamageList[i] = player.Get1SDamageList(log, phase.Start, phase.End, target, DamageType.Power);
                        graph1SConditionDamageList[i] = player.Get1SDamageList(log, phase.Start, phase.End, target, DamageType.Condition);
                        graph1SBreakbarDamageList[i] = player.Get1SBreakbarDamageList(log, phase.Start, phase.End, target);
                    }
                    targetDamageDistList[i] = JsonDamageDistBuilder.BuildJsonDamageDistList(player.GetDamageEvents(target, log, phase.Start, phase.End).GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList()), log, skillDesc, buffDesc);
                }
                if (settings.RawFormatTimelineArrays)
                {
                    targetDamage1S[j] = graph1SDamageList;
                    targetPowerDamage1S[j] = graph1SPowerDamageList;
                    targetConditionDamage1S[j] = graph1SConditionDamageList;
                    targetBreakbarDamage1S[j] = graph1SBreakbarDamageList;
                }
                targetDamageDist[j] = targetDamageDistList;
                dpsTargets[j] = phases.Select(phase => JsonStatisticsBuilder.BuildJsonDPS(player.GetDPSStats(target, log, phase.Start, phase.End))).ToArray();
                statsTargets[j] = phases.Select(phase => JsonStatisticsBuilder.BuildJsonGameplayStats(player.GetGameplayStats(target, log, phase.Start, phase.End))).ToArray();
            }
            if (settings.RawFormatTimelineArrays)
            {
                jsonPlayer.TargetDamage1S = targetDamage1S;
                jsonPlayer.TargetPowerDamage1S = targetPowerDamage1S;
                jsonPlayer.TargetConditionDamage1S = targetConditionDamage1S;
                jsonPlayer.TargetBreakbarDamage1S = targetBreakbarDamage1S;
            }
            jsonPlayer.TargetDamageDist = targetDamageDist;
            jsonPlayer.DpsTargets = dpsTargets;
            jsonPlayer.StatsTargets = statsTargets;
            if (!log.CombatData.HasBreakbarDamageData)
            {
                jsonPlayer.TargetBreakbarDamage1S = null;
            }
            //
            jsonPlayer.BuffUptimes = GetPlayerJsonBuffsUptime(player, phases.Select(phase => player.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList(), log, settings, buffDesc, personalBuffs);
            jsonPlayer.SelfBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            jsonPlayer.GroupBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.Group, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            jsonPlayer.OffGroupBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.OffGroup, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            jsonPlayer.SquadBuffs = GetPlayerBuffGenerations(phases.Select(phase => player.GetBuffs(BuffEnum.Squad, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            //
            jsonPlayer.BuffUptimesActive = GetPlayerJsonBuffsUptime(player, phases.Select(phase => player.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList(), log, settings, buffDesc, personalBuffs);
            jsonPlayer.SelfBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            jsonPlayer.GroupBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.Group, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            jsonPlayer.OffGroupBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.OffGroup, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            jsonPlayer.SquadBuffsActive = GetPlayerBuffGenerations(phases.Select(phase => player.GetActiveBuffs(BuffEnum.Squad, log, phase.Start, phase.End)).ToList(), log, buffDesc);
            //
            IReadOnlyList<Consumable> consumables = player.GetConsumablesList(log, 0, log.FightData.FightEnd);
            if (consumables.Any())
            {
                var consumablesJSON = new List<JsonConsumable>();
                foreach (Consumable food in consumables)
                {
                    if (!buffDesc.ContainsKey("b" + food.Buff.ID))
                    {
                        buffDesc["b" + food.Buff.ID] = JsonLogBuilder.BuildBuffDesc(food.Buff, log);
                    }
                    consumablesJSON.Add(JsonConsumableBuilder.BuildJsonConsumable(food));
                }
                jsonPlayer.Consumables = consumablesJSON;
            }
            //
            IReadOnlyList<DeathRecap> deathRecaps = player.GetDeathRecaps(log);
            if (deathRecaps.Any())
            {
                jsonPlayer.DeathRecap = deathRecaps.Select(x => JsonDeathRecapBuilder.BuildJsonDeathRecap(x)).ToList();
            }
            // 
            jsonPlayer.DamageModifiers = JsonDamageModifierDataBuilder.GetDamageModifiers(player.GetDamageModifierStats(log, null), log, damageModDesc);
            jsonPlayer.DamageModifiersTarget = JsonDamageModifierDataBuilder.GetDamageModifiersTarget(player, log, damageModDesc);
            return jsonPlayer;
        }

        private static List<JsonPlayerBuffsGeneration> GetPlayerBuffGenerations(List<IReadOnlyDictionary<long, FinalPlayerBuffs>> buffs, ParsedEvtcLog log, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            var uptimes = new List<JsonPlayerBuffsGeneration>();
            foreach (KeyValuePair<long, FinalPlayerBuffs> pair in buffs[0])
            {
                Buff buff = log.Buffs.BuffsByIds[pair.Key];
                if (!buffDesc.ContainsKey("b" + pair.Key))
                {
                    buffDesc["b" + pair.Key] = JsonLogBuilder.BuildBuffDesc(buff, log);
                }
                var data = new List<JsonBuffsGenerationData>();
                for (int i = 0; i < phases.Count; i++)
                {
                    if (buffs[i].TryGetValue(pair.Key, out FinalPlayerBuffs val))
                    {
                        JsonBuffsGenerationData value = JsonPlayerBuffsGenerationBuilder.BuildJsonBuffsGenerationData(val);
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

            if (!uptimes.Any())
            {
                return null;
            }

            return uptimes;
        }

        private static List<JsonBuffsUptime> GetPlayerJsonBuffsUptime(Player player, List<IReadOnlyDictionary<long, FinalPlayerBuffs>> buffs, ParsedEvtcLog log, RawFormatSettings settings, Dictionary<string, JsonLog.BuffDesc> buffDesc, Dictionary<string, HashSet<long>> personalBuffs)
        {
            var res = new List<JsonBuffsUptime>();
            var profEnums = new HashSet<ParserHelper.Source>(ProfToEnum(player.Prof));
            IReadOnlyList<PhaseData> phases = log.FightData.GetNonDummyPhases(log);
            foreach (KeyValuePair<long, FinalPlayerBuffs> pair in buffs[0])
            {
                Buff buff = log.Buffs.BuffsByIds[pair.Key];
                var data = new List<JsonBuffsUptimeData>();
                for (int i = 0; i < phases.Count; i++)
                {
                    PhaseData phase = phases[i];
                    Dictionary<long, FinalBuffsDictionary> buffsDictionary = player.GetBuffsDictionary(log, phase.Start, phase.End);
                    if (buffs[i].TryGetValue(pair.Key, out FinalPlayerBuffs val))
                    {
                        JsonBuffsUptimeData value = JsonBuffsUptimeBuilder.BuildJsonBuffsUptimeData(val, buffsDictionary[pair.Key]);
                        data.Add(value);
                    }
                    else
                    {
                        var value = new JsonBuffsUptimeData();
                        data.Add(value);
                    }
                }
                if (buff.Nature == Buff.BuffNature.GraphOnlyBuff && profEnums.Contains(buff.Source))
                {
                    if (player.GetBuffDistribution(log, phases[0].Start, phases[0].End).GetUptime(pair.Key) > 0)
                    {
                        if (personalBuffs.TryGetValue(player.Prof, out HashSet<long> list) && !list.Contains(pair.Key))
                        {
                            list.Add(pair.Key);
                        }
                        else
                        {
                            personalBuffs[player.Prof] = new HashSet<long>()
                                {
                                    pair.Key
                                };
                        }
                    }
                }
                res.Add(JsonBuffsUptimeBuilder.BuildJsonBuffsUptime(player, pair.Key, log, settings, data, buffDesc));
            }
            return res;
        }

    }
}
