using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    internal static class JsonDamageDistBuilder
    {
        private static JsonDamageDist BuildJsonDamageDist(long id, List<AbstractHealthDamageEvent> list, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonDamageDist = new JsonDamageDist();
            jsonDamageDist.IndirectDamage = list.Exists(x => x is NonDirectHealthDamageEvent);
            if (jsonDamageDist.IndirectDamage)
            {
                if (!buffDesc.ContainsKey("b" + id))
                {
                    if (log.Buffs.BuffsByIds.TryGetValue(id, out Buff buff))
                    {
                        buffDesc["b" + id] = JsonLogBuilder.BuildBuffDesc(buff, log);
                    }
                    else
                    {
                        SkillItem skill = list.First().Skill;
                        var auxBoon = new Buff(skill.Name, id, skill.Icon);
                        buffDesc["b" + id] = JsonLogBuilder.BuildBuffDesc(auxBoon, log);
                    }
                }
            }
            else
            {
                if (!skillDesc.ContainsKey("s" + id))
                {
                    SkillItem skill = list.First().Skill;
                    skillDesc["s" + id] = JsonLogBuilder.BuildSkillDesc(skill, log.LogData.GW2Build, log.SkillData);
                }
            }
            jsonDamageDist.Id = id;
            jsonDamageDist.Min = int.MaxValue;
            jsonDamageDist.Max = int.MinValue;
            foreach (AbstractHealthDamageEvent dmgEvt in list)
            {
                jsonDamageDist.Hits += dmgEvt.DoubleProcHit ? 0 : 1;
                jsonDamageDist.TotalDamage += dmgEvt.HealthDamage;
                if (dmgEvt.HasHit)
                {
                    jsonDamageDist.Min = Math.Min(jsonDamageDist.Min, dmgEvt.HealthDamage);
                    jsonDamageDist.Max = Math.Max(jsonDamageDist.Max, dmgEvt.HealthDamage);
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
            return jsonDamageDist;
        }

        internal static List<JsonDamageDist> BuildJsonDamageDistList(Dictionary<long, List<AbstractHealthDamageEvent>> dlsByID, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<JsonDamageDist>();
            foreach (KeyValuePair<long, List<AbstractHealthDamageEvent>> pair in dlsByID)
            {
                res.Add(BuildJsonDamageDist(pair.Key, pair.Value, log, skillDesc, buffDesc));
            }
            return res;
        }

    }
}
