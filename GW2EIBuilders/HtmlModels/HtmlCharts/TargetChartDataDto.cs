using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class TargetChartDataDto : ActorChartDataDto
{
    public readonly IReadOnlyList<int> Total;
    public readonly IReadOnlyList<int> TotalPower;
    public readonly IReadOnlyList<int> TotalCondition;
    public readonly List<double[]>? BreakbarPercentStates;

    public TargetChartDataDto(ParsedEvtcLog log, PhaseData phase, SingleActor target) : base(log, phase, target, false)
    {
        Total = target.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.All).Values;
        TotalPower = target.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power).Values;
        TotalCondition = target.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition).Values;
        BreakbarPercentStates = ChartDataDto.BuildBreakbarPercentStates(log, target, phase);
    }
}
