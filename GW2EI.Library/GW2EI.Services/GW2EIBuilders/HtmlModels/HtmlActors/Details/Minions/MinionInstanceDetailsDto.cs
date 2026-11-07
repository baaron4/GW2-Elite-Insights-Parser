using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionInstanceDetailsDto
{
    public List<List<BuffChartDataDto>> BoonGraph { get; set; }
    public List<List<SkillCastDto>>? Rotation;

    public MinionInstanceDetailsDto(ParsedEvtcLog log, NPC minion, UsedHtmlData used)
    {
        var phases = log.LogData.GetPhases(log);
        BoonGraph = new(phases.Count);
        Rotation = new(phases.Count);

        foreach (PhaseData phase in phases)
        {
            BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, minion, phase, used));
            Rotation.Add(SkillDto.BuildRotationData(log, minion, phase, used));
        }
    }
}
