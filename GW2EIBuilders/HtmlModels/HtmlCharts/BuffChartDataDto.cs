using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class BuffChartDataDto
{
    public long Id { get; set; }
    public string Color { get; set; }
    public bool Visible { get; set; }
    public List<double[]> States { get; set; }

    private static string GetBuffColor(string name)
    {
        return name switch
        {
            "Aegis" => "rgb(102,255,255)",
            "Fury" => "rgb(255,153,0)",
            "Might" => "rgb(153,0,0)",
            "Protection" => "rgb(102,255,255)",
            "Quickness" => "rgb(255,0,255)",
            "Regeneration" => "rgb(0,204,0)",
            "Resistance" => "rgb(255, 153, 102)",
            "Retaliation" => "rgb(255, 51, 0)",
            "Resolution" => "rgb(255, 51, 0)",
            "Stability" => "rgb(153, 102, 0)",
            "Swiftness" => "rgb(255,255,0)",
            "Vigor" => "rgb(102, 153, 0)",
            "Alacrity" => "rgb(0,102,255)",
            "Glyph of Empowerment" => "rgb(204, 153, 0)",
            "Sun Spirit" => "rgb(255, 102, 0)",
            "Banner of Strength" => "rgb(153, 0, 0)",
            "Banner of Discipline" => "rgb(0, 51, 0)",
            "Spotter" => "rgb(0,255,0)",
            "Stone Spirit" => "rgb(204, 102, 0)",
            "Storm Spirit" => "rgb(102, 0, 102)",
            "Empower Allies" => "rgb(255, 153, 0)",
            _ => "",
        };
    }

    private BuffChartDataDto(BuffGraph bgm, IReadOnlyList<GenericSegment<double>> bChart, PhaseData phase)
    {
        Id = bgm.Buff.ID;
        Visible = (bgm.Buff.Name == "Might" || bgm.Buff.Name == "Quickness" || bgm.Buff.Name == "Vulnerability");
        Color = GetBuffColor(bgm.Buff.Name);
        States = bChart.ToObjectList(phase.Start, phase.End);
    }

    private static BuffChartDataDto? BuildBuffGraph(BuffGraph bgm, PhaseData phase, Dictionary<long, Buff> usedBuffs)
    {
        var bChart = bgm.Values.Where(x => x.End >= phase.Start && x.Start <= phase.End).ToList();
        if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
        {
            return null;
        }
        usedBuffs[bgm.Buff.ID] = bgm.Buff;
        return new BuffChartDataDto(bgm, bChart, phase);
    }

    private static void BuildBoonGraphData(List<BuffChartDataDto> list, IReadOnlyList<Buff> listToUse, Dictionary<long, BuffGraph> boonGraphData, PhaseData phase, Dictionary<long, Buff> usedBuffs)
    {
        foreach (Buff buff in listToUse)
        {
            if(boonGraphData.Remove(buff.ID, out var bgm))
            {
                BuffChartDataDto? graph = BuildBuffGraph(bgm, phase, usedBuffs);
                if (graph != null)
                {
                    list.Add(graph);
                }
            }
        }
    }

    private static List<BuffChartDataDto> BuildBuffGraphData(ParsedEvtcLog log, PhaseData phase, Dictionary<long, BuffGraph> buffGraphData, Dictionary<long, Buff> usedBuffs)
    {
        var list = new List<BuffChartDataDto>(
            log.StatisticsHelper.PresentBoons.Count +
            log.StatisticsHelper.PresentConditions.Count +
            log.StatisticsHelper.PresentOffbuffs.Count +
            log.StatisticsHelper.PresentSupbuffs.Count +
            log.StatisticsHelper.PresentDefbuffs.Count +
            log.StatisticsHelper.PresentDebuffs.Count +
            log.StatisticsHelper.PresentGearbuffs.Count
        );
        BuildBoonGraphData(list, log.StatisticsHelper.PresentBoons, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(list, log.StatisticsHelper.PresentConditions, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(list, log.StatisticsHelper.PresentOffbuffs, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(list, log.StatisticsHelper.PresentSupbuffs, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(list, log.StatisticsHelper.PresentDefbuffs, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(list, log.StatisticsHelper.PresentDebuffs, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(list, log.StatisticsHelper.PresentGearbuffs, buffGraphData, phase, usedBuffs);
        var footList = new List<BuffChartDataDto>(
            log.StatisticsHelper.PresentNourishements.Count +
            log.StatisticsHelper.PresentEnhancements.Count +
            log.StatisticsHelper.PresentOtherConsumables.Count
        );
        BuildBoonGraphData(footList, log.StatisticsHelper.PresentNourishements, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(footList, log.StatisticsHelper.PresentEnhancements, buffGraphData, phase, usedBuffs);
        BuildBoonGraphData(footList, log.StatisticsHelper.PresentOtherConsumables, buffGraphData, phase, usedBuffs);
        foreach (BuffGraph bgm in buffGraphData.Values)
        {
            if (bgm.Buff.Classification == Buff.BuffClassification.Hidden)
            {
                continue;
            }
            BuffChartDataDto? graph = BuildBuffGraph(bgm, phase, usedBuffs);
            if (graph != null)
            {
                list.Add(graph);
            }
        }
        list.AddRange(footList);
        list.Reverse();
        return list;
    }

    public static List<BuffChartDataDto> BuildBuffGraphData(ParsedEvtcLog log, SingleActor p, PhaseData phase, Dictionary<long, Buff> usedBuffs)
    {
        return BuildBuffGraphData(log, phase, p.GetBuffGraphs(log).ToDictionary(x => x.Key, x => x.Value), usedBuffs);
    }

    public static List<BuffChartDataDto> BuildBuffGraphData(ParsedEvtcLog log, SingleActor p, SingleActor by, PhaseData phase, Dictionary<long, Buff> usedBuffs)
    {
        return BuildBuffGraphData(log, phase, p.GetBuffGraphs(log, by).ToDictionary(x => x.Key, x => x.Value), usedBuffs);
    }
}
