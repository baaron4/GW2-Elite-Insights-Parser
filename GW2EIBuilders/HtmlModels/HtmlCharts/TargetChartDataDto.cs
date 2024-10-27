using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class TargetChartDataDto : ActorChartDataDto
{
    public readonly IReadOnlyList<int> Total;
    public readonly IReadOnlyList<int> TotalPower;
    public readonly IReadOnlyList<int> TotalCondition;
    public readonly List<object[]>? BreakbarPercentStates;

    public TargetChartDataDto(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target) : base(log, phase, target, false)
    {
        Total = target.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.All);
        TotalPower = target.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power);
        TotalCondition = target.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition);
        BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, phase);
    }
}
