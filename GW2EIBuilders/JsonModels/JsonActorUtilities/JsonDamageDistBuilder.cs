using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    internal static class JsonDamageDistBuilder
    {
        private static JsonDamageDist BuildJsonDamageDist(long id, List<AbstractHealthDamageEvent> dmList, List<AbstractBreakbarDamageEvent> brList, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
        {
            var jsonDamageDist = new JsonDamageDist();
            jsonDamageDist.IndirectDamage = dmList.Exists(x => x is NonDirectHealthDamageEvent) || brList.Exists(x => x is NonDirectBreakbarDamageEvent);
            if (jsonDamageDist.IndirectDamage)
            {
                if (!buffMap.ContainsKey(id))
                {
                    if (log.Buffs.BuffsByIds.TryGetValue(id, out Buff buff))
                    {
                        buffMap[id] = buff;
                    }
                    else
                    {
                        SkillItem skill = log.SkillData.Get(id);
                        var auxBuff = new Buff(skill.Name, id, skill.Icon);
                        buffMap[id] = auxBuff;
                    }
                }
            }
            else
            {
                if (!skillMap.ContainsKey(id))
                {
                    SkillItem skill = log.SkillData.Get(id);
                    skillMap[id] = skill;
                }
            }
            jsonDamageDist.Id = id;
            jsonDamageDist.Min = int.MaxValue;
            jsonDamageDist.Max = int.MinValue;
            foreach (AbstractHealthDamageEvent dmgEvt in dmList)
            {
                jsonDamageDist.Hits += dmgEvt.DoubleProcHit ? 0 : 1;
                jsonDamageDist.TotalDamage += dmgEvt.HealthDamage;
                if (dmgEvt.HasHit)
                {
                    jsonDamageDist.Min = Math.Min(jsonDamageDist.Min, dmgEvt.HealthDamage);
                    jsonDamageDist.Max = Math.Max(jsonDamageDist.Max, dmgEvt.HealthDamage);
                    jsonDamageDist.AgainstMoving += dmgEvt.AgainstMoving ? 1 : 0;
                }
                if (!jsonDamageDist.IndirectDamage)
                {
                    if (dmgEvt.HasHit)
                    {
                        jsonDamageDist.Flank += dmgEvt.IsFlanking ? 1 : 0;
                        jsonDamageDist.Glance += dmgEvt.HasGlanced ? 1 : 0;
                        jsonDamageDist.Crit += dmgEvt.HasCrit ? 1 : 0;
                        jsonDamageDist.CritDamage += dmgEvt.HasCrit ? dmgEvt.HealthDamage : 0;
                    }
                    jsonDamageDist.Missed += dmgEvt.IsBlind ? 1 : 0;
                    jsonDamageDist.Evaded += dmgEvt.IsEvaded ? 1 : 0;
                    jsonDamageDist.Blocked += dmgEvt.IsBlocked ? 1 : 0;
                    jsonDamageDist.Interrupted += dmgEvt.HasInterrupted ? 1 : 0;
                }
                jsonDamageDist.ConnectedHits += dmgEvt.HasHit ? 1 : 0;
                jsonDamageDist.Invulned += dmgEvt.IsAbsorbed ? 1 : 0;
                jsonDamageDist.ShieldDamage += dmgEvt.ShieldDamage;
            }
            jsonDamageDist.Min = jsonDamageDist.Min == int.MaxValue ? 0 : jsonDamageDist.Min;
            jsonDamageDist.Max = jsonDamageDist.Max == int.MinValue ? 0 : jsonDamageDist.Max;
            jsonDamageDist.TotalBreakbarDamage = Math.Round(brList.Sum(x => x.BreakbarDamage), 1);
            return jsonDamageDist;
        }

        internal static List<JsonDamageDist> BuildJsonDamageDistList(Dictionary<long, List<AbstractHealthDamageEvent>> dlsByID, Dictionary<long, List<AbstractBreakbarDamageEvent>> brlsByID, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
        {
            var res = new List<JsonDamageDist>();
            foreach (KeyValuePair<long, List<AbstractHealthDamageEvent>> pair in dlsByID)
            {
                if (!brlsByID.TryGetValue(pair.Key, out List<AbstractBreakbarDamageEvent> brls))
                {
                    brls = new List<AbstractBreakbarDamageEvent>();
                }
                res.Add(BuildJsonDamageDist(pair.Key, pair.Value, brls, log, skillMap, buffMap));
            }
            foreach (KeyValuePair<long, List<AbstractBreakbarDamageEvent>> pair in brlsByID)
            {
                if (dlsByID.ContainsKey(pair.Key))
                {
                    continue;
                }
                res.Add(BuildJsonDamageDist(pair.Key, new List<AbstractHealthDamageEvent>(), pair.Value, log, skillMap, buffMap));
            }
            return res;
        }

    }
}
