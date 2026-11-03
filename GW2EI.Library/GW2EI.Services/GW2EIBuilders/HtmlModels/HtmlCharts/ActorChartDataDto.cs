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

        for (int minionGroupIndex = 0; minionGroupIndex < minionGroups.Count; minionGroupIndex++)
        {
            Minions minionGroup = minionGroups[minionGroupIndex];
            if (!minionGroup.IsUniquePerTimeFrame)
            {
                continue;
            }

            foreach (NPC minion in minionGroup.MinionList)
            {
                MinionCharts.Add(minionGroupIndex, new MinionChartDto(log, phase, minion));
            }
        }
    }
}
