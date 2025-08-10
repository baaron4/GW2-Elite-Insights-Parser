using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts;

internal class MechanicChartDataDto
{
    public readonly string Symbol;
    public readonly int Size;
    public readonly string Color;
    public readonly List<List<List<object?[]>>> Points;
    public readonly bool Visible;

    private MechanicChartDataDto(ParsedEvtcLog log, Mechanic mech)
    {
        Color = mech.PlotlySetting.Color;
        Symbol = mech.PlotlySetting.Symbol;
        Size = mech.PlotlySetting.Size;
        Visible = !mech.ShowOnTable;
        Points = BuildMechanicGraphPointData(log, log.MechanicData.GetMechanicLogs(log, mech, log.LogData.LogStart, log.LogData.LogEnd), mech.IsEnemyMechanic);
    }

    private static List<List<object?[]>> GetMechanicChartPoints(IReadOnlyList<MechanicEvent> mechanicLogs, PhaseData phase, ParsedEvtcLog log, bool enemyMechanic)
    {
        var res = new List<List<object?[]>>();
        if (!enemyMechanic)
        {
            var playerIndex = new Dictionary<SingleActor, int>(log.Friendlies.Count);
            for (int p = 0; p < log.Friendlies.Count; p++)
            {
                playerIndex.Add(log.Friendlies[p], p);
                res.Add([]);
            }
            foreach (MechanicEvent ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
            {
                double time = (ml.Time - phase.Start) / 1000.0;
                if (playerIndex.TryGetValue(ml.Actor, out int p))
                {
                    res[p].Add([time, null]);
                }
            }
        }
        else
        {
            var targetIndex = new Dictionary<SingleActor, int>(phase.Targets.Count);
            int p = 0;
            foreach (var pair in phase.Targets) {
                targetIndex.Add(pair.Key, p++);
                res.Add([]);
            }
            res.Add([]);
            foreach (MechanicEvent ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
            {
                double time = (ml.Time - phase.Start) / 1000.0;
                if (targetIndex.TryGetValue(ml.Actor, out p))
                {
                    res[p].Add([time, null]);
                }
                else
                {
                    res[^1].Add([time, ml.Actor.Character ]);
                }
            }
        }
        return res;
    }

    private static List<List<List<object?[]>>> BuildMechanicGraphPointData(ParsedEvtcLog log, IReadOnlyList<MechanicEvent> mechanicLogs, bool enemyMechanic)
    {
        var phases = log.LogData.GetPhases(log);
        var list = new List<List<List<object?[]>>>(phases.Count);
        foreach (PhaseData phase in phases)
        {
            list.Add(GetMechanicChartPoints(mechanicLogs, phase, log, enemyMechanic));
        }
        return list;
    }

    public static List<MechanicChartDataDto> BuildMechanicsChartData(ParsedEvtcLog log)
    {
        var mechs = log.MechanicData.GetPresentMechanics(log, log.LogData.LogStart, log.LogData.LogEnd);
        var mechanicsChart = new List<MechanicChartDataDto>(mechs.Count);
        foreach (Mechanic mech in mechs)
        {
            mechanicsChart.Add(new MechanicChartDataDto(log, mech));
        }
        return mechanicsChart;
    }
}
