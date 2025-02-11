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
            Total = p.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.All),
            Taken = p.Get1SDamageTakenList(log, phase.Start, phase.End, null, ParserHelper.DamageType.All),
            Targets = new(phase.Targets.Count)
        };
        PowerDamage = new PlayerDamageChartDto<int>()
        {
            Total = p.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power),
            Taken = p.Get1SDamageTakenList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Power),
            Targets = new(phase.Targets.Count)
        };
        ConditionDamage = new PlayerDamageChartDto<int>()
        {
            Total = p.Get1SDamageList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition),
            Taken = p.Get1SDamageTakenList(log, phase.Start, phase.End, null, ParserHelper.DamageType.Condition),
            Targets = new(phase.Targets.Count)
        };
        BreakbarDamage = new PlayerDamageChartDto<double>()
        {
            Total = p.Get1SBreakbarDamageList(log, phase.Start, phase.End, null),
            Taken = p.Get1SBreakbarDamageTakenList(log, phase.Start, phase.End, null),
            Targets = new (phase.Targets.Count)
        };
        foreach (SingleActor target in phase.Targets.Keys)
        {
            Damage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target, ParserHelper.DamageType.All));
            PowerDamage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target, ParserHelper.DamageType.Power));
            ConditionDamage.Targets.Add(p.Get1SDamageList(log, phase.Start, phase.End, target, ParserHelper.DamageType.Condition));
            BreakbarDamage.Targets.Add(p.Get1SBreakbarDamageList(log, phase.Start, phase.End, target));
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
