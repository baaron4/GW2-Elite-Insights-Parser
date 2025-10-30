using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTHealing;

internal static class EXTJsonMinionsHealingStatsBuilder
{

    public static EXTJsonMinionsHealingStats BuildMinionsHealingStats(Minions minions, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
    {
        IReadOnlyList<PhaseData> phases = log.LogData.GetPhases(log);
        var totalHealing = new List<int>(phases.Count);
        var totalAlliedHealing = new List<List<int>>(log.Friendlies.Count);
        var totalIncomingHealing = new List<int>(phases.Count);
        var alliedHealingDist = new List<List<List<EXTJsonHealingDist>>>(log.Friendlies.Count);
        var totalHealingDist = new List<List<EXTJsonHealingDist>>(phases.Count);
        var totalIncomingHealingDist = new List<List<EXTJsonHealingDist>>(phases.Count);
        var res = new EXTJsonMinionsHealingStats()
        {
            TotalHealing = totalHealing,
            TotalAlliedHealing = totalAlliedHealing,
            TotalIncomingHealing = totalIncomingHealing,
            AlliedHealingDist = alliedHealingDist,
            TotalHealingDist = totalHealingDist,
            TotalIncomingHealingDist = totalIncomingHealingDist
        };
        foreach (SingleActor friendly in log.Friendlies)
        {
            var totalAllyHealing = new List<int>(phases.Count);
            totalAlliedHealing.Add(totalAllyHealing);
            //
            var allyHealingDist = new List<List<EXTJsonHealingDist>>(phases.Count);
            alliedHealingDist.Add(allyHealingDist);
            foreach (PhaseData phase in phases)
            {
                var list = minions.EXTHealing.GetOutgoingHealEvents(friendly, log, phase.Start, phase.End);
                totalAllyHealing.Add(list.Sum(x => x.HealingDone));
                allyHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(list.GroupBy(x => x.SkillID), log, skillMap, buffMap));
            }
        }
        foreach (PhaseData phase in phases)
        {
            var list = minions.EXTHealing.GetOutgoingHealEvents(null, log, phase.Start, phase.End);
            totalHealing.Add(list.Sum(x => x.HealingDone));
            totalHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(list.GroupBy(x => x.SkillID), log, skillMap, buffMap));
            var listInc = minions.EXTHealing.GetIncomingHealEvents(null, log, phase.Start, phase.End);
            totalIncomingHealing.Add(listInc.Sum(x => x.HealingDone));
            totalIncomingHealingDist.Add(EXTJsonHealingStatsBuilderCommons.BuildHealingDistList(listInc.GroupBy(x => x.SkillID), log, skillMap, buffMap));
        }
        return res;
    }

}
