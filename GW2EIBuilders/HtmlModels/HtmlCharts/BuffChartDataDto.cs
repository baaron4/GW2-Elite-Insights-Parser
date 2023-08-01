using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts
{
    internal class BuffChartDataDto
    {
        public long Id { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }
        public List<object[]> States { get; set; }

        private static string GetBuffColor(string name)
        {
            switch (name)
            {

                case "Aegis": return "rgb(102,255,255)";
                case "Fury": return "rgb(255,153,0)";
                case "Might": return "rgb(153,0,0)";
                case "Protection": return "rgb(102,255,255)";
                case "Quickness": return "rgb(255,0,255)";
                case "Regeneration": return "rgb(0,204,0)";
                case "Resistance": return "rgb(255, 153, 102)";
                case "Retaliation": return "rgb(255, 51, 0)";
                case "Resolution": return "rgb(255, 51, 0)";
                case "Stability": return "rgb(153, 102, 0)";
                case "Swiftness": return "rgb(255,255,0)";
                case "Vigor": return "rgb(102, 153, 0)";
                case "Alacrity": return "rgb(0,102,255)";

                case "Glyph of Empowerment": return "rgb(204, 153, 0)";
                case "Sun Spirit": return "rgb(255, 102, 0)";
                case "Banner of Strength": return "rgb(153, 0, 0)";
                case "Banner of Discipline": return "rgb(0, 51, 0)";
                case "Spotter": return "rgb(0,255,0)";
                case "Stone Spirit": return "rgb(204, 102, 0)";
                case "Storm Spirit": return "rgb(102, 0, 102)";
                case "Empower Allies": return "rgb(255, 153, 0)";
                default:
                    return "";
            }

        }

        private BuffChartDataDto(BuffsGraphModel bgm, List<Segment> bChart, PhaseData phase)
        {
            Id = bgm.Buff.ID;
            Visible = (bgm.Buff.Name == "Might" || bgm.Buff.Name == "Quickness" || bgm.Buff.Name == "Vulnerability");
            Color = GetBuffColor(bgm.Buff.Name);
            States = Segment.ToObjectList(bChart, phase.Start, phase.End);
        }

        private static BuffChartDataDto BuildBuffGraph(BuffsGraphModel bgm, PhaseData phase, Dictionary<long, Buff> usedBuffs)
        {
            var bChart = bgm.BuffChart.Where(x => x.End >= phase.Start && x.Start <= phase.End
            ).ToList();
            if (bChart.Count == 0 || (bChart.Count == 1 && bChart.First().Value == 0))
            {
                return null;
            }
            usedBuffs[bgm.Buff.ID] = bgm.Buff;
            return new BuffChartDataDto(bgm, bChart, phase);
        }

        private static void BuildBoonGraphData(List<BuffChartDataDto> list, IReadOnlyList<Buff> listToUse, Dictionary<long, BuffsGraphModel> boonGraphData, PhaseData phase, Dictionary<long, Buff> usedBuffs)
        {
            foreach (Buff buff in listToUse)
            {
                if (boonGraphData.TryGetValue(buff.ID, out BuffsGraphModel bgm))
                {
                    BuffChartDataDto graph = BuildBuffGraph(bgm, phase, usedBuffs);
                    if (graph != null)
                    {
                        list.Add(graph);
                    }
                }
                boonGraphData.Remove(buff.ID);
            }
        }

        private static List<BuffChartDataDto> BuildBuffGraphData(ParsedEvtcLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, BuffsGraphModel> buffGraphData, Dictionary<long, Buff> usedBuffs)
        {
            var list = new List<BuffChartDataDto>();
            BuildBoonGraphData(list, log.StatisticsHelper.PresentBoons, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.StatisticsHelper.PresentConditions, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.StatisticsHelper.PresentOffbuffs, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.StatisticsHelper.PresentSupbuffs, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.StatisticsHelper.PresentDefbuffs, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.StatisticsHelper.PresentDebuffs, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.StatisticsHelper.PresentGearbuffs, buffGraphData, phase, usedBuffs);
            var footList = new List<BuffChartDataDto>();
            BuildBoonGraphData(footList, log.StatisticsHelper.PresentNourishements, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(footList, log.StatisticsHelper.PresentEnhancements, buffGraphData, phase, usedBuffs);
            BuildBoonGraphData(footList, log.StatisticsHelper.PresentOtherConsumables, buffGraphData, phase, usedBuffs);
            foreach (BuffsGraphModel bgm in buffGraphData.Values)
            {
                if (bgm.Buff.Classification == Buff.BuffClassification.Hidden)
                {
                    continue;
                }
                BuffChartDataDto graph = BuildBuffGraph(bgm, phase, usedBuffs);
                if (graph != null)
                {
                    list.Add(graph);
                }
            }
            list.AddRange(footList);
            list.Reverse();
            return list;
        }

        public static List<BuffChartDataDto> BuildBuffGraphData(ParsedEvtcLog log, AbstractSingleActor p, PhaseData phase, Dictionary<long, Buff> usedBuffs)
        {
            return BuildBuffGraphData(log, p, phase, p.GetBuffGraphs(log).ToDictionary(x => x.Key, x => x.Value), usedBuffs);
        }

        public static List<BuffChartDataDto> BuildBuffGraphData(ParsedEvtcLog log, AbstractSingleActor p, AbstractSingleActor by, PhaseData phase, Dictionary<long, Buff> usedBuffs)
        {
            return BuildBuffGraphData(log, p, phase, p.GetBuffGraphs(log, by).ToDictionary(x => x.Key, x => x.Value), usedBuffs);
        }
    }
}
