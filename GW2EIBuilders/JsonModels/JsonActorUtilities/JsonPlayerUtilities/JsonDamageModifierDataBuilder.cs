using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using Newtonsoft.Json;
using static GW2EIJSON.JsonDamageModifierData;

namespace GW2EIBuilders.JsonModels
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
            var jsonDamageModifierData = new JsonDamageModifierData();
            jsonDamageModifierData.Id = ID;
            jsonDamageModifierData.DamageModifiers = data;
            return jsonDamageModifierData;
        }


        public static List<JsonDamageModifierData> GetDamageModifiers(Dictionary<string, List<DamageModifierStat>> damageModDict, ParsedEvtcLog log, Dictionary<string, JsonLog.DamageModDesc> damageModDesc)
        {
            var dict = new Dictionary<int, List<JsonDamageModifierItem>>();
            foreach (string key in damageModDict.Keys)
            {
                DamageModifier dMod = log.DamageModifiers.DamageModifiersByName[key];
                int iKey = dMod.ID;
                string nKey = "d" + iKey;
                if (!damageModDesc.ContainsKey(nKey))
                {
                    damageModDesc[nKey] = JsonLogBuilder.BuildDamageModDesc(dMod);
                }
                dict[iKey] = damageModDict[key].Select(x => BuildJsonDamageModifierItem(x)).ToList();
            }
            var res = new List<JsonDamageModifierData>();
            foreach (KeyValuePair<int, List<JsonDamageModifierItem>> pair in dict)
            {
                res.Add(BuildJsonDamageModifierData(pair.Key, pair.Value));
            }
            return res;
        }

        public static List<JsonDamageModifierData>[] GetDamageModifiersTarget(Player player, ParsedEvtcLog log, Dictionary<string, JsonLog.DamageModDesc> damageModDesc)
        {
            var res = new List<JsonDamageModifierData>[log.FightData.Logic.Targets.Count];
            for (int i = 0; i < log.FightData.Logic.Targets.Count; i++)
            {
                NPC tar = log.FightData.Logic.Targets[i];
                res[i] = GetDamageModifiers(player.GetDamageModifierStats(log, tar), log, damageModDesc);
            }
            return res;
        }
    }
}
