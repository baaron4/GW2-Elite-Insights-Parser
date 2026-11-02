using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIBuilders.JsonModels.JsonLogBuilder;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier;

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

    //TODO_PERF(Rennorb)
    private static EXTJsonBarrierDist BuildBarrierDist(long id, List<EXTBarrierEvent> list, ParsedEvtcLog log, UsedJsonData usedData)
    {
        var jsonBarrierDist = new EXTJsonBarrierDist
        {
            IndirectBarrier = list.Exists(x => x is EXTNonDirectBarrierEvent)
        };
        if (jsonBarrierDist.IndirectBarrier)
        {
            if (!usedData.buffs.ContainsKey(id))
            {
                if (log.Buffs.BuffsByIDs.TryGetValue(id, out var buff))
                {
                    usedData.buffs[id] = buff;
                }
                else
                {
                    SkillItem skill = list.First().Skill;
                    var auxBuff = new Buff(skill.Name, id, skill.Icon);
                    usedData.buffs[id] = auxBuff;
                }
            }
        }
        else
        {
            usedData.skills.TryAdd(id, list.First().Skill);
        }
        jsonBarrierDist.Id = id;
        jsonBarrierDist.Min = int.MaxValue;
        jsonBarrierDist.Max = int.MinValue;
        foreach (EXTBarrierEvent barrierEvt in list)
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

    internal static IEnumerable<EXTJsonBarrierDist> BuildBarrierDistList(IEnumerable<IGrouping<long, EXTBarrierEvent>> dlsByID, ParsedEvtcLog log, UsedJsonData usedData)
    {
        return dlsByID.Select(group => BuildBarrierDist(group.Key, group.ToList(), log, usedData));
    }
}
