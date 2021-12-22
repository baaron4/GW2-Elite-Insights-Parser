using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier
{
    internal static class EXTJsonBarrierStatsBuilderCommons
    {
        internal static EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics BuildOutgoingBarrierStatistics(EXTFinalOutgoingBarrierStat stats)
        {
            return new EXTJsonBarrierStatistics.EXTJsonOutgoingBarrierStatistics()
            {
                Barrier = stats.Barrier,
                Bps = stats.Bps,

                ActorBarrier = stats.ActorBarrier,
                ActorBps = stats.ActorBps
            };
        }

        internal static EXTJsonBarrierStatistics.EXTJsonIncomingBarrierStatistics BuildIncomingBarrierStatistics(EXTFinalIncomingBarrierStat stats)
        {
            return new EXTJsonBarrierStatistics.EXTJsonIncomingBarrierStatistics()
            {
                Barrier = stats.BarrierReceived
            };
        }

        private static EXTJsonBarrierDist BuildBarrierDist(long id, List<EXTAbstractBarrierEvent> list, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var jsonBarrierDist = new EXTJsonBarrierDist();
            jsonBarrierDist.IndirectBarrier = list.Exists(x => x is EXTNonDirectBarrierEvent);
            if (jsonBarrierDist.IndirectBarrier)
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
                    skillDesc["s" + id] = JsonLogBuilder.BuildSkillDesc(skill, log);
                }
            }
            jsonBarrierDist.Id = id;
            jsonBarrierDist.Min = int.MaxValue;
            jsonBarrierDist.Max = int.MinValue;
            foreach (EXTAbstractBarrierEvent barrierEvt in list)
            {
                jsonBarrierDist.Hits++; ;
                jsonBarrierDist.TotalBarrier += barrierEvt.BarrierGiven;
                jsonBarrierDist.Min = Math.Min(jsonBarrierDist.Min, barrierEvt.BarrierGiven);
                jsonBarrierDist.Max = Math.Max(jsonBarrierDist.Max, barrierEvt.BarrierGiven);
            }
            jsonBarrierDist.Min = jsonBarrierDist.Min == int.MaxValue ? 0 : jsonBarrierDist.Min;
            jsonBarrierDist.Max = jsonBarrierDist.Max == int.MinValue ? 0 : jsonBarrierDist.Max;
            return jsonBarrierDist;
        }

        internal static List<EXTJsonBarrierDist> BuildBarrierDistList(Dictionary<long, List<EXTAbstractBarrierEvent>> dlsByID, ParsedEvtcLog log, Dictionary<string, JsonLog.SkillDesc> skillDesc, Dictionary<string, JsonLog.BuffDesc> buffDesc)
        {
            var res = new List<EXTJsonBarrierDist>();
            foreach (KeyValuePair<long, List<EXTAbstractBarrierEvent>> pair in dlsByID)
            {
                res.Add(BuildBarrierDist(pair.Key, pair.Value, log, skillDesc, buffDesc));
            }
            return res;
        }
    }
}
