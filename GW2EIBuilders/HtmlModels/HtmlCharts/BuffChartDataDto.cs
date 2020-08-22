using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{
    public class BuffChartDataDto
    {
        public long Id { get; internal set; }
        public string Color { get; internal set; }
        public bool Visible { get; internal set; }
        public List<object[]> States { get; internal set; }    

        private BuffChartDataDto(BuffsGraphModel bgm, List<Segment> bChart, PhaseData phase)
        {
            Id = bgm.Buff.ID;
            Visible = (bgm.Buff.Name == "Might" || bgm.Buff.Name == "Quickness" || bgm.Buff.Name == "Vulnerability");
            Color = HTMLBuilder.GetLink("Color-" + bgm.Buff.Name);
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

        private static void BuildBoonGraphData(List<BuffChartDataDto> list, List<Buff> listToUse, Dictionary<long, BuffsGraphModel> boonGraphData, PhaseData phase, Dictionary<long, Buff> usedBuffs)
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

        internal static List<BuffChartDataDto> BuildBoonGraphData(ParsedEvtcLog log, AbstractSingleActor p, int phaseIndex, Dictionary<long, Buff> usedBuffs)
        {
            var list = new List<BuffChartDataDto>();
            PhaseData phase = log.FightData.GetPhases(log)[phaseIndex];
            var boonGraphData = p.GetBuffGraphs(log).ToDictionary(x => x.Key, x => x.Value);
            BuildBoonGraphData(list, log.Statistics.PresentBoons, boonGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.Statistics.PresentConditions, boonGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.Statistics.PresentOffbuffs, boonGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.Statistics.PresentSupbuffs, boonGraphData, phase, usedBuffs);
            BuildBoonGraphData(list, log.Statistics.PresentDefbuffs, boonGraphData, phase, usedBuffs);
            foreach (BuffsGraphModel bgm in boonGraphData.Values)
            {
                BuffChartDataDto graph = BuildBuffGraph(bgm, phase, usedBuffs);
                if (graph != null)
                {
                    list.Add(graph);
                }
            }
            if (p.GetType() == typeof(Player))
            {
                foreach (NPC mainTarget in log.FightData.GetMainTargets(log))
                {
                    boonGraphData = mainTarget.GetBuffGraphs(log);
                    foreach (BuffsGraphModel bgm in boonGraphData.Values.Reverse().Where(x => x.Buff.Name == "Compromised" || x.Buff.Name == "Unnatural Signet" || x.Buff.Name == "Fractured - Enemy" || x.Buff.Name == "Erratic Energy"))
                    {
                        BuffChartDataDto graph = BuildBuffGraph(bgm, phase, usedBuffs);
                        if (graph != null)
                        {
                            list.Add(graph);
                        }
                    }
                }
            }
            list.Reverse();
            return list;
        }
    }
}
