using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class PlayerChartDataDto : ActorChartDataDto
{
    public readonly PlayerDamageChartDto<int> Damage;
    public readonly PlayerDamageChartDto<int> PowerDamage;
    public readonly PlayerDamageChartDto<int> ConditionDamage;
    public readonly PlayerDamageChartDto<double> BreakbarDamage;

    private PlayerChartDataDto(ParsedEvtcLog log, PhaseData phase, SingleActor p) : base(log, phase, p, true)
    {
        Damage = new PlayerDamageChartDto<int>()
        {
            Total = p.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.All).Values,
            Taken = p.GetDamageTakenGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.All).Values,
            Targets = new(phase.Targets.Count)
        };
        PowerDamage = new PlayerDamageChartDto<int>()
        {
            Total = p.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power).Values,
            Taken = p.GetDamageTakenGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power).Values,
            Targets = new(phase.Targets.Count)
        };
        ConditionDamage = new PlayerDamageChartDto<int>()
        {
            Total = p.GetDamageGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition).Values,
            Taken = p.GetDamageTakenGraph(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition).Values,
            Targets = new(phase.Targets.Count)
        };
        BreakbarDamage = new PlayerDamageChartDto<double>()
        {
            Total = p.GetBreakbarDamageGraph(log, phase.Start, phase.End, null)?.Values,
            Taken = p.GetBreakbarDamageTakenGraph(log, phase.Start, phase.End, null)?.Values,
            Targets = new (phase.Targets.Count)
        };
        foreach (SingleActor target in phase.Targets.Keys)
        {
            Damage.Targets.Add(p.GetDamageGraph(log, phase.Start, phase.End, target, ParserHelper.DamageType.All).Values);
            PowerDamage.Targets.Add(p.GetDamageGraph(log, phase.Start, phase.End, target, ParserHelper.DamageType.Power).Values);
            ConditionDamage.Targets.Add(p.GetDamageGraph(log, phase.Start, phase.End, target, ParserHelper.DamageType.Condition).Values);
            BreakbarDamage.Targets.Add(p.GetBreakbarDamageGraph(log, phase.Start, phase.End, target)?.Values);
        }
    }

    public static List<PlayerChartDataDto> BuildPlayersGraphData(ParsedEvtcLog log, PhaseData phase)
    {
        var list = new List<PlayerChartDataDto>(log.Friendlies.Count);

        foreach (SingleActor actor in log.Friendlies)
        {
            list.Add(new PlayerChartDataDto(log, phase, actor));
        }
        return list;
    }
}
