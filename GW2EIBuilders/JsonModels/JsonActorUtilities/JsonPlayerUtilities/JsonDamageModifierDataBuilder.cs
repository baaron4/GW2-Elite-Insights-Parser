using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using static GW2EIJSON.JsonDamageModifierData;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities
{
    /// <summary>
    /// Class representing damage modifier data
    /// </summary>
    internal static class JsonDamageModifierDataBuilder
    {

        private static JsonDamageModifierItem BuildJsonDamageModifierItem(DamageModifierStat extraData)
        {
            var jsonDamageModifierItem = new JsonDamageModifierItem();
            jsonDamageModifierItem.HitCount = extraData.HitCount;
            jsonDamageModifierItem.TotalHitCount = extraData.TotalHitCount;
            jsonDamageModifierItem.DamageGain = extraData.DamageGain;
            jsonDamageModifierItem.TotalDamage = extraData.TotalDamage;
            return jsonDamageModifierItem;
        }

        private static JsonDamageModifierData BuildJsonDamageModifierData(int ID, List<JsonDamageModifierItem> data)
        {
            var jsonDamageModifierData = new JsonDamageModifierData
            {
                Id = ID,
                DamageModifiers = data
            };
            return jsonDamageModifierData;
        }


        public static List<JsonDamageModifierData> GetOutgoingDamageModifiers(AbstractSingleActor player, List<IReadOnlyDictionary<string, DamageModifierStat>> damageModDicts, ParsedEvtcLog log, Dictionary<long, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods)
        {
            var dict = new Dictionary<int, List<JsonDamageModifierItem>>();
            var profEnums = new HashSet<ParserHelper.Source>(ParserHelper.SpecToSources(player.Spec));
            foreach (IReadOnlyDictionary<string, DamageModifierStat> damageModDict in damageModDicts)
            {
                foreach (string key in damageModDict.Keys)
                {
                    DamageModifier dMod = log.DamageModifiers.OutgoingDamageModifiersByName[key];
                    int iKey = dMod.ID;
                    if (!damageModMap.ContainsKey(iKey))
                    {
                        damageModMap[iKey] = dMod;
                    }
                    if (profEnums.Contains(dMod.Src))
                    {
                        if (personalDamageMods.TryGetValue(player.Spec.ToString(), out HashSet<long> hashSet))
                        {
                            hashSet.Add(iKey);
                        }
                        else
                        {
                            personalDamageMods[player.Spec.ToString()] = new HashSet<long>()
                                {
                                    iKey
                                };
                        }
                    }
                    if (dict.TryGetValue(iKey, out List<JsonDamageModifierItem> list))
                    {
                        list.Add(BuildJsonDamageModifierItem(damageModDict[key]));
                    }
                    else
                    {
                        dict[iKey] = new List<JsonDamageModifierItem>
                        {
                            BuildJsonDamageModifierItem(damageModDict[key])
                        };
                    }
                }
            }

            var res = new List<JsonDamageModifierData>();
            foreach (KeyValuePair<int, List<JsonDamageModifierItem>> pair in dict)
            {
                res.Add(BuildJsonDamageModifierData(pair.Key, pair.Value));
            }
            return res;
        }

        public static List<JsonDamageModifierData> GetIncomingDamageModifiers(AbstractSingleActor player, List<IReadOnlyDictionary<string, DamageModifierStat>> damageModDicts, ParsedEvtcLog log, Dictionary<long, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods)
        {
            var dict = new Dictionary<int, List<JsonDamageModifierItem>>();
            var profEnums = new HashSet<ParserHelper.Source>(ParserHelper.SpecToSources(player.Spec));
            foreach (IReadOnlyDictionary<string, DamageModifierStat> damageModDict in damageModDicts)
            {
                foreach (string key in damageModDict.Keys)
                {
                    DamageModifier dMod = log.DamageModifiers.IncomingDamageModifiersByName[key];
                    int iKey = dMod.ID;
                    if (!damageModMap.ContainsKey(iKey))
                    {
                        damageModMap[iKey] = dMod;
                    }
                    if (profEnums.Contains(dMod.Src))
                    {
                        if (personalDamageMods.TryGetValue(player.Spec.ToString(), out HashSet<long> hashSet))
                        {
                            hashSet.Add(iKey);
                        }
                        else
                        {
                            personalDamageMods[player.Spec.ToString()] = new HashSet<long>()
                                {
                                    iKey
                                };
                        }
                    }
                    if (dict.TryGetValue(iKey, out List<JsonDamageModifierItem> list))
                    {
                        list.Add(BuildJsonDamageModifierItem(damageModDict[key]));
                    }
                    else
                    {
                        dict[iKey] = new List<JsonDamageModifierItem>
                        {
                            BuildJsonDamageModifierItem(damageModDict[key])
                        };
                    }
                }
            }

            var res = new List<JsonDamageModifierData>();
            foreach (KeyValuePair<int, List<JsonDamageModifierItem>> pair in dict)
            {
                res.Add(BuildJsonDamageModifierData(pair.Key, pair.Value));
            }
            return res;
        }

        public static List<JsonDamageModifierData>[] GetOutgoingDamageModifiersTarget(AbstractSingleActor player, ParsedEvtcLog log, Dictionary<long, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods, IReadOnlyList<PhaseData> phases)
        {
            var res = new List<JsonDamageModifierData>[log.FightData.Logic.Targets.Count];
            for (int i = 0; i < log.FightData.Logic.Targets.Count; i++)
            {
                AbstractSingleActor tar = log.FightData.Logic.Targets[i];
                res[i] = GetOutgoingDamageModifiers(player, phases.Select(x => player.GetOutgoingDamageModifierStats(tar, log, x.Start, x.End)).ToList(), log, damageModMap, personalDamageMods);
            }
            return res;
        }

        public static List<JsonDamageModifierData>[] GetIncomingDamageModifiersTarget(AbstractSingleActor player, ParsedEvtcLog log, Dictionary<long, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods, IReadOnlyList<PhaseData> phases)
        {
            var res = new List<JsonDamageModifierData>[log.FightData.Logic.Targets.Count];
            for (int i = 0; i < log.FightData.Logic.Targets.Count; i++)
            {
                AbstractSingleActor tar = log.FightData.Logic.Targets[i];
                res[i] = GetIncomingDamageModifiers(player, phases.Select(x => player.GetIncomingDamageModifierStats(tar, log, x.Start, x.End)).ToList(), log, damageModMap, personalDamageMods);
            }
            return res;
        }
    }
}
