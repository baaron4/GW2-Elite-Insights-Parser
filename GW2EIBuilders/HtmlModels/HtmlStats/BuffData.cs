using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

using BuffDataItem = List<double>;

internal class BuffData
{
    public double Avg { get; set; }
    public List<BuffDataItem> Data { get; set; }

    private BuffData(IReadOnlyDictionary<long, BuffStatistics> buffs, IReadOnlyList<Buff> listToUse, double avg)
    {
        Avg = avg;
        Data = new List<BuffDataItem>(listToUse.Count);
        foreach (Buff buff in listToUse)
        {
            var buffVals = new BuffDataItem();
            Data.Add(buffVals);

            if (buffs.TryGetValue(buff.ID, out var uptime))
            {
                buffVals.Add(uptime.Uptime);
                if (uptime.Presence > 0)
                {
                    buffVals.Add(uptime.Presence);
                }
            }
        }
    }

    private BuffData(IReadOnlyDictionary<long, BuffByActorStatistics> buffs, IReadOnlyList<Buff> listToUse, SingleActor actor)
    {
        Data = new List<BuffDataItem>(listToUse.Count);
        foreach (Buff buff in listToUse)
        {
            if (buffs.TryGetValue(buff.ID, out var toUse) && toUse.GeneratedBy.ContainsKey(actor))
            {
                Data.Add(
                    [
                        toUse.GeneratedBy[actor],
                        toUse.OverstackedBy[actor],
                        toUse.WastedFrom[actor],
                        toUse.UnknownExtensionFrom[actor],
                        toUse.ExtensionBy[actor],
                        toUse.ExtendedFrom[actor],
                        toUse.GeneratedPresenceBy[actor]
                    ]);
            }
            else
            {
                Data.Add(
                    [
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0
                    ]);
            }
        }
    }

    private BuffData(IReadOnlyList<Buff> listToUse, IReadOnlyDictionary<long, BuffStatistics> uptimes)
    {
        Data = new List<BuffDataItem>(listToUse.Count);
        foreach (Buff buff in listToUse)
        {
            if (uptimes.TryGetValue(buff.ID, out var uptime))
            {
                Data.Add(
                    [
                        uptime.Generation,
                        uptime.Overstack,
                        uptime.Wasted,
                        uptime.UnknownExtended,
                        uptime.ByExtension,
                        uptime.Extended,
                        uptime.GenerationPresence
                    ]);
            }
            else
            {
                Data.Add(
                    [
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0
                    ]);
            }
        }
    }

    private BuffData(Spec spec, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, IReadOnlyDictionary<long, BuffStatistics> uptimes)
    {
        Data = new List<BuffDataItem>(buffsBySpec[spec].Count);
        foreach (Buff buff in buffsBySpec[spec])
        {
            var boonVals = new BuffDataItem();
            Data.Add(boonVals);
            if (uptimes.TryGetValue(buff.ID, out var uptime))
            {
                boonVals.Add(uptime.Uptime);
                if (uptime.Presence > 0)
                {
                    boonVals.Add(uptime.Presence);
                }
            }
            else
            {
                boonVals.Add(0);
            }
        }
    }

    //////
    public static List<BuffData> BuildBuffUptimeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<BuffData>(log.Friendlies.Count);
        bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
        bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

        foreach (SingleActor actor in log.Friendlies)
        {
            double avg = 0.0;
            if (boonTable)
            {
                avg = actor.GetGameplayStats(log, phase.Start, phase.End).AverageBoons;
            }
            else if (conditionTable)
            {
                avg = actor.GetGameplayStats(log, phase.Start, phase.End).AverageConditions;
            }
            list.Add(new BuffData(actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End), listToUse, avg));
        }
        return list;
    }

    public static List<BuffData> BuildActiveBuffUptimeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<BuffData>(log.Friendlies.Count);
        bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
        bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

        foreach (SingleActor actor in log.Friendlies)
        {
            double avg = 0.0;
            if (boonTable)
            {
                avg = actor.GetGameplayStats(log, phase.Start, phase.End).AverageActiveBoons;
            }
            else if (conditionTable)
            {
                avg = actor.GetGameplayStats(log, phase.Start, phase.End).AverageActiveConditions;
            }
            list.Add(new BuffData(actor.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End), listToUse, avg));
        }
        return list;
    }

    //////
    public static List<BuffData> BuildPersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
    {
        var list = new List<BuffData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffData(actor.Spec, buffsBySpec, actor.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End)));
        }
        return list;
    }

    public static List<BuffData> BuildActivePersonalBuffUptimeData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
    {
        var list = new List<BuffData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffData(actor.Spec, buffsBySpec, actor.GetActiveBuffs(BuffEnum.Self, log, phase.Start, phase.End)));
        }
        return list;
    }


    //////
    public static List<BuffData> BuildBuffGenerationData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
    {
        var list = new List<BuffData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffData(listToUse, actor.GetBuffs(type, log, phase.Start, phase.End)));
        }
        return list;
    }

    public static List<BuffData> BuildActiveBuffGenerationData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
    {
        var list = new List<BuffData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffData(listToUse, actor.GetActiveBuffs(type, log, phase.Start, phase.End)));
        }
        return list;
    }
    // 
    private static List<BuffData> BuildBuffDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, SingleActor player)
    {
        IReadOnlyDictionary<long, BuffByActorStatistics> buffs = player.GetBuffsDictionary(log, phase.Start, phase.End);
        var list = new List<BuffData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffData(buffs, listToUse, actor));
        }
        return list;
    }
    public static List<List<BuffData>> BuildBuffDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<List<BuffData>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(BuildBuffDictionaryData(log, listToUse, phase, actor));
        }
        return list;
    }

    private static List<BuffData> BuildActiveBuffDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, SingleActor player)
    {
        IReadOnlyDictionary<long, BuffByActorStatistics> buffs = player.GetActiveBuffsDictionary(log, phase.Start, phase.End);
        var list = new List<BuffData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffData(buffs, listToUse, actor));
        }
        return list;
    }

    public static List<List<BuffData>> BuildActiveBuffDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<List<BuffData>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(BuildActiveBuffDictionaryData(log, listToUse, phase, actor));
        }
        return list;
    }

    /////
    public static List<BuffData> BuildTargetConditionData(ParsedEvtcLog log, PhaseData phase, SingleActor actor)
    {
        return BuildBuffDictionaryData(log, log.StatisticsHelper.PresentConditions, phase, actor);
    }

    public static BuffData BuildTargetConditionUptimeData(ParsedEvtcLog log, PhaseData phase, SingleActor target)
    {
        IReadOnlyDictionary<long, BuffStatistics> buffs = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
        return new BuffData(buffs, log.StatisticsHelper.PresentConditions, target.GetGameplayStats(log, phase.Start, phase.End).AverageConditions);
    }

    public static BuffData BuildTargetBoonUptimeData(ParsedEvtcLog log, PhaseData phase, SingleActor target)
    {
        IReadOnlyDictionary<long, BuffStatistics> buffs = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
        return new BuffData(buffs, log.StatisticsHelper.PresentBoons, target.GetGameplayStats(log, phase.Start, phase.End).AverageBoons);
    }
}
