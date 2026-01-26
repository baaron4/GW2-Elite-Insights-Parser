using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using static GW2EIJSON.JsonDamageModifierData;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities;

/// <summary>
/// Class representing damage modifier data
/// </summary>
internal static class JsonDamageModifierDataBuilder
{

    private static JsonDamageModifierItem BuildJsonDamageModifierItem(DamageModifierStat extraData)
    {
        var jsonDamageModifierItem = new JsonDamageModifierItem
        {
            HitCount = extraData.HitCount,
            TotalHitCount = extraData.TotalHitCount,
            DamageGain = extraData.DamageGain,
            TotalDamage = extraData.TotalDamage
        };
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


    public static List<JsonDamageModifierData> GetOutgoingDamageModifiers(SingleActor player, List<IReadOnlyDictionary<int, DamageModifierStat>> damageModDicts, ParsedEvtcLog log, Dictionary<int, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods)
    {
        var dict = new Dictionary<int, List<JsonDamageModifierItem>>(50);
        var profEnums = new HashSet<ParserHelper.Source>(ParserHelper.SpecToSources(player.Spec));
        foreach (IReadOnlyDictionary<int, DamageModifierStat> damageModDict in damageModDicts)
        {
            foreach (int key in damageModDict.Keys)
            {
                DamageModifier dMod = log.DamageModifiers.OutgoingDamageModifiersByID[key];
                int iKey = dMod.ID;
                if (!damageModMap.ContainsKey(iKey))
                {
                    damageModMap[iKey] = dMod;
                }
                if (profEnums.Intersect(dMod.Srcs).Any())
                {
                    if (personalDamageMods.TryGetValue(player.Spec.ToString(), out var personalDamageModSet))
                    {
                        personalDamageModSet.Add(iKey);
                    }
                    else
                    {
                        personalDamageMods[player.Spec.ToString()] =
                            [
                                iKey
                            ];
                    }
                }
                if (dict.TryGetValue(iKey, out var jsonDamageModList))
                {
                    jsonDamageModList.Add(BuildJsonDamageModifierItem(damageModDict[key]));
                }
                else
                {
                    dict[iKey] =
                    [
                        BuildJsonDamageModifierItem(damageModDict[key])
                    ];
                }
            }
        }

        var res = new List<JsonDamageModifierData>(dict.Count);
        foreach (KeyValuePair<int, List<JsonDamageModifierItem>> pair in dict)
        {
            res.Add(BuildJsonDamageModifierData(pair.Key, pair.Value));
        }
        return res;
    }

    public static List<JsonDamageModifierData> GetIncomingDamageModifiers(SingleActor player, List<IReadOnlyDictionary<int, DamageModifierStat>> damageModDicts, ParsedEvtcLog log, Dictionary<int, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods)
    {
        var dict = new Dictionary<int, List<JsonDamageModifierItem>>(50);
        var profEnums = new HashSet<ParserHelper.Source>(ParserHelper.SpecToSources(player.Spec));
        foreach (IReadOnlyDictionary<int, DamageModifierStat> damageModDict in damageModDicts)
        {
            foreach (int key in damageModDict.Keys)
            {
                DamageModifier dMod = log.DamageModifiers.IncomingDamageModifiersByID[key];
                int iKey = dMod.ID;
                if (!damageModMap.ContainsKey(iKey))
                {
                    damageModMap[iKey] = dMod;
                }
                if (profEnums.Intersect(dMod.Srcs).Any())
                {
                    if (personalDamageMods.TryGetValue(player.Spec.ToString(), out var personalDamageModSet))
                    {
                        personalDamageModSet.Add(iKey);
                    }
                    else
                    {
                        personalDamageMods[player.Spec.ToString()] =
                            [
                                iKey
                            ];
                    }
                }
                if (dict.TryGetValue(iKey, out var jsonDamageModList))
                {
                    jsonDamageModList.Add(BuildJsonDamageModifierItem(damageModDict[key]));
                }
                else
                {
                    dict[iKey] =
                    [
                        BuildJsonDamageModifierItem(damageModDict[key])
                    ];
                }
            }
        }

        var res = new List<JsonDamageModifierData>(dict.Count);
        foreach (KeyValuePair<int, List<JsonDamageModifierItem>> pair in dict)
        {
            res.Add(BuildJsonDamageModifierData(pair.Key, pair.Value));
        }
        return res;
    }

    public static List<JsonDamageModifierData>[] GetOutgoingDamageModifiersTarget(SingleActor player, ParsedEvtcLog log, Dictionary<int, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods, IReadOnlyList<PhaseData> phases)
    {
        var res = new List<JsonDamageModifierData>[log.LogData.Logic.Targets.Count];
        for (int i = 0; i < log.LogData.Logic.Targets.Count; i++)
        {
            SingleActor tar = log.LogData.Logic.Targets[i];
            res[i] = GetOutgoingDamageModifiers(player, phases.Select(x => player.GetOutgoingDamageModifierStats(tar, log, x.Start, x.End)).ToList(), log, damageModMap, personalDamageMods);
        }
        return res;
    }

    public static List<JsonDamageModifierData>[] GetIncomingDamageModifiersTarget(SingleActor player, ParsedEvtcLog log, Dictionary<int, DamageModifier> damageModMap, Dictionary<string, HashSet<long>> personalDamageMods, IReadOnlyList<PhaseData> phases)
    {
        var res = new List<JsonDamageModifierData>[log.LogData.Logic.Targets.Count];
        for (int i = 0; i < log.LogData.Logic.Targets.Count; i++)
        {
            SingleActor tar = log.LogData.Logic.Targets[i];
            res[i] = GetIncomingDamageModifiers(player, phases.Select(x => player.GetIncomingDamageModifierStats(tar, log, x.Start, x.End)).ToList(), log, damageModMap, personalDamageMods);
        }
        return res;
    }
}
