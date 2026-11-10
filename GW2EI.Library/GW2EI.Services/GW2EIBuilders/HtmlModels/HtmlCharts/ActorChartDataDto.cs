using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal abstract class ActorChartDataDto
{
    public readonly List<double[]>? HealthStates;
    public readonly List<double[]>? BarrierStates;
    public readonly List<List<MinionChartDto>> MinionCharts;

    public ActorChartDataDto(ParsedEvtcLog log, PhaseData phase, SingleActor actor, bool nullableHPStates)
    {
        HealthStates = ChartDataDto.BuildHealthStates(log, actor, phase, nullableHPStates);
        BarrierStates = ChartDataDto.BuildBarrierStates(log, actor, phase);

        var minionGroups = actor.GetMinions(log);
        MinionCharts = new();

        for (int minionGroupIndex = 0; minionGroupIndex < minionGroups.Count; minionGroupIndex++)
        {
            Minions minionGroup = minionGroups[minionGroupIndex];
            List<MinionChartDto> minionCharts;
            if (!minionGroup.IsWhitelisted(log))
            {
                minionCharts = [];
            }
            else
            {
                minionCharts = new List<MinionChartDto>(minionGroup.MinionList.Count);
                foreach (NPC minion in minionGroup.MinionList)
                {
                    minionCharts.Add(new MinionChartDto(log, phase, minion));
                }
            }
            MinionCharts.Add(minionCharts);
        }
    }
}
