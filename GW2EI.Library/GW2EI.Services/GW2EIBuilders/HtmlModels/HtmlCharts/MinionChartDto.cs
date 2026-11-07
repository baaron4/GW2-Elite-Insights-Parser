using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class MinionChartDto : ActorChartDataDto
{

    public MinionChartDto(ParsedEvtcLog log, PhaseData phase, NPC minion) : base(log, phase, minion, true)
    {

    }
}
