using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal abstract class ActorChartDataDto
{
    public readonly List<double[]>? HealthStates;
    public readonly List<double[]>? BarrierStates;
    public readonly Dictionary<int, MinionChartDto> MinionCharts;

    public ActorChartDataDto(ParsedEvtcLog log, PhaseData phase, SingleActor actor, bool nullableHPStates)
    {
        HealthStates = ChartDataDto.BuildHealthStates(log, actor, phase, nullableHPStates);
        BarrierStates = ChartDataDto.BuildBarrierStates(log, actor, phase);

        var minionGroups = actor.GetMinions(log);
        MinionCharts = new();

        foreach (Minions minionGroup in minionGroups)
        {
            if (!minionGroup.IsUniquePerTimeFrame)
            {
                continue;
            }

            foreach (NPC minion in minionGroup.MinionList)
            {
                MinionCharts.Add(minion.UniqueID, new MinionChartDto(log, phase, minion));
            }
        }
    }
}
