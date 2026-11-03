using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionInstanceDetailsDto
{
    public int UniqueID { get; set; }
    public List<List<BuffChartDataDto>> BoonGraph { get; set; }

    public MinionInstanceDetailsDto(ParsedEvtcLog log, NPC minion, UsedHtmlData used)
    {
        var phases = log.LogData.GetPhases(log);

        UniqueID = minion.UniqueID;
        BoonGraph = new(phases.Count);

        foreach (PhaseData phase in phases)
        {
            BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, minion, phase, used));
        }
    }
}
