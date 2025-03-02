using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

using BuffVolumeDataItem = List<double>;

internal class BuffVolumeData
{
    public List<BuffVolumeDataItem> Data { get; set; }

    private BuffVolumeData(IReadOnlyDictionary<long, BuffVolumeStatistics> buffVolumes, IReadOnlyList<Buff> listToUse)
    {
        Data = new List<BuffVolumeDataItem>(listToUse.Count);
        foreach (Buff buff in listToUse)
        {
            var buffVals = new BuffVolumeDataItem();
            Data.Add(buffVals);

            if (buffVolumes.TryGetValue(buff.ID, out var volume))
            {
                buffVals.Add(volume.Incoming);
                buffVals.Add(volume.IncomingByExtension);
                buffVals.Add(volume.IncomingByUnknownExtension);
            }
        }
    }

    private BuffVolumeData(IReadOnlyDictionary<long, BuffVolumeByActorStatistics> buffVolumes, IReadOnlyList<Buff> listToUse, SingleActor actor)
    {
        Data = new List<BuffVolumeDataItem>(listToUse.Count);
        foreach (Buff buff in listToUse)
        {
            if (buffVolumes.TryGetValue(buff.ID, out var toUse) && toUse.IncomingBy.ContainsKey(actor))
            {
                Data.Add(
                    [
                        toUse.IncomingBy[actor],
                        toUse.IncomingByExtensionBy[actor],
                    ]);
            }
            else
            {
                Data.Add(
                    [
                        0,
                        0,
                    ]);
            }
        }
    }

    private BuffVolumeData(IReadOnlyList<Buff> listToUse, IReadOnlyDictionary<long, BuffVolumeStatistics> volumes)
    {
        Data = new List<BuffVolumeDataItem>(listToUse.Count);
        foreach (Buff buff in listToUse)
        {
            if (volumes.TryGetValue(buff.ID, out var volume))
            {
                Data.Add(
                    [
                        volume.Outgoing,
                        volume.OutgoingByExtension,
                    ]);
            }
            else
            {
                Data.Add(
                    [
                        0,
                        0,
                    ]);
            }
        }
    }

    private BuffVolumeData(Spec spec, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, IReadOnlyDictionary<long, BuffVolumeStatistics> volumes)
    {
        Data = new List<BuffVolumeDataItem>(buffsBySpec[spec].Count);
        foreach (Buff buff in buffsBySpec[spec])
        {
            var boonVals = new BuffVolumeDataItem();
            Data.Add(boonVals);
            if (volumes.TryGetValue(buff.ID, out var volume))
            {
                boonVals.Add(volume.Incoming);
                boonVals.Add(volume.IncomingByExtension);
                boonVals.Add(volume.IncomingByUnknownExtension);
            }
            else
            {
                boonVals.Add(0);
            }
        }
    }

    //////
    public static List<BuffVolumeData> BuildBuffIncomingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<BuffVolumeData>(log.Friendlies.Count);
        bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
        bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(actor.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End), listToUse));
        }
        return list;
    }

    public static List<BuffVolumeData> BuildActiveBuffIncomingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<BuffVolumeData>(log.Friendlies.Count);
        bool boonTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Boon);
        bool conditionTable = listToUse.Any(x => x.Classification == Buff.BuffClassification.Condition);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(actor.GetActiveBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End), listToUse));
        }
        return list;
    }

    //////
    public static List<BuffVolumeData> BuildPersonalBuffIncomingVolueData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
    {
        var list = new List<BuffVolumeData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(actor.Spec, buffsBySpec, actor.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)));
        }
        return list;
    }

    public static List<BuffVolumeData> BuildActivePersonalBuffIncomingVolumeData(ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> buffsBySpec, PhaseData phase)
    {
        var list = new List<BuffVolumeData>(log.Friendlies.Count);
        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(actor.Spec, buffsBySpec, actor.GetActiveBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End)));
        }
        return list;
    }


    //////
    public static List<BuffVolumeData> BuildBuffOutgoingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
    {
        var list = new List<BuffVolumeData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(listToUse, actor.GetBuffVolumes(type, log, phase.Start, phase.End)));
        }
        return list;
    }

    public static List<BuffVolumeData> BuildActiveBuffOutgoingVolumeData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, BuffEnum type)
    {
        var list = new List<BuffVolumeData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(listToUse, actor.GetActiveBuffVolumes(type, log, phase.Start, phase.End)));
        }
        return list;
    }
    // 
    private static List<BuffVolumeData> BuildBuffVolumeDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, SingleActor player)
    {
        IReadOnlyDictionary<long, BuffVolumeByActorStatistics> buffs = player.GetBuffVolumesDictionary(log, phase.Start, phase.End);
        var list = new List<BuffVolumeData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(buffs, listToUse, actor));
        }
        return list;
    }
    public static List<List<BuffVolumeData>> BuildBuffVolumeDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<List<BuffVolumeData>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(BuildBuffVolumeDictionaryData(log, listToUse, phase, actor));
        }
        return list;
    }

    private static List<BuffVolumeData> BuildActiveBuffVolumeDictionaryData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase, SingleActor player)
    {
        IReadOnlyDictionary<long, BuffVolumeByActorStatistics> buffs = player.GetActiveBuffVolumesDictionary(log, phase.Start, phase.End);
        var list = new List<BuffVolumeData>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new BuffVolumeData(buffs, listToUse, actor));
        }
        return list;
    }

    public static List<List<BuffVolumeData>> BuildActiveBuffVolumeDictionariesData(ParsedEvtcLog log, IReadOnlyList<Buff> listToUse, PhaseData phase)
    {
        var list = new List<List<BuffVolumeData>>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(BuildActiveBuffVolumeDictionaryData(log, listToUse, phase, actor));
        }
        return list;
    }

    /////
    public static List<BuffVolumeData> BuildTargetConditionVolumeData(ParsedEvtcLog log, PhaseData phase, SingleActor actor)
    {
        return BuildBuffVolumeDictionaryData(log, log.StatisticsHelper.PresentConditions, phase, actor);
    }

    public static BuffVolumeData BuildTargetConditionIncomingVolumeData(ParsedEvtcLog log, PhaseData phase, SingleActor target)
    {
        IReadOnlyDictionary<long, BuffVolumeStatistics> buffs = target.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End);
        return new BuffVolumeData(buffs, log.StatisticsHelper.PresentConditions);
    }

    public static BuffVolumeData BuildTargetBoonIncomingVolumeData(ParsedEvtcLog log, PhaseData phase, SingleActor target)
    {
        IReadOnlyDictionary<long, BuffVolumeStatistics> buffs = target.GetBuffVolumes(BuffEnum.Self, log, phase.Start, phase.End);
        return new BuffVolumeData(buffs, log.StatisticsHelper.PresentBoons);
    }
}
