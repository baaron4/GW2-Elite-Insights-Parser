using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLCharts
{
    internal class MechanicChartDataDto
    {
        public string Symbol { get; }
        public int Size { get; }
        public string Color { get; }
        public List<List<List<object>>> Points { get; }
        public bool Visible { get; }

        private MechanicChartDataDto(ParsedEvtcLog log, Mechanic mech)
        {
            Color = mech.PlotlySetting.Color;
            Symbol = mech.PlotlySetting.Symbol;
            Size = mech.PlotlySetting.Size;
            Visible = !mech.ShowOnTable;
            Points = BuildMechanicGraphPointData(log, log.MechanicData.GetMechanicLogs(log, mech, log.FightData.FightStart, log.FightData.FightEnd), mech.IsEnemyMechanic);
        }

        private static List<List<object>> GetMechanicChartPoints(IReadOnlyList<MechanicEvent> mechanicLogs, PhaseData phase, ParsedEvtcLog log, bool enemyMechanic)
        {
            var res = new List<List<object>>();
            if (!enemyMechanic)
            {
                var playerIndex = new Dictionary<AbstractSingleActor, int>();
                for (int p = 0; p < log.Friendlies.Count; p++)
                {
                    playerIndex.Add(log.Friendlies[p], p);
                    res.Add(new List<object>());
                }
                foreach (MechanicEvent ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
                {
                    double time = (ml.Time - phase.Start) / 1000.0;
                    if (playerIndex.TryGetValue(ml.Actor, out int p))
                    {
                        res[p].Add(time);
                    }
                }
            }
            else
            {
                var targetIndex = new Dictionary<AbstractSingleActor, int>();
                for (int p = 0; p < phase.Targets.Count; p++)
                {
                    targetIndex.Add(phase.Targets[p], p);
                    res.Add(new List<object>());
                }
                res.Add(new List<object>());
                foreach (MechanicEvent ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
                {
                    double time = (ml.Time - phase.Start) / 1000.0;
                    if (targetIndex.TryGetValue(ml.Actor, out int p))
                    {
                        res[p].Add(time);
                    }
                    else
                    {
                        res[res.Count - 1].Add(new object[] { time, ml.Actor.Character });
                    }
                }
            }
            return res;
        }

        private static List<List<List<object>>> BuildMechanicGraphPointData(ParsedEvtcLog log, IReadOnlyList<MechanicEvent> mechanicLogs, bool enemyMechanic)
        {
            var list = new List<List<List<object>>>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                list.Add(GetMechanicChartPoints(mechanicLogs, phase, log, enemyMechanic));
            }
            return list;
        }

        public static List<MechanicChartDataDto> BuildMechanicsChartData(ParsedEvtcLog log)
        {
            var mechanicsChart = new List<MechanicChartDataDto>();
            foreach (Mechanic mech in log.MechanicData.GetPresentMechanics(log, log.FightData.FightStart, log.FightData.FightEnd))
            {
                mechanicsChart.Add(new MechanicChartDataDto(log, mech));
            }
            return mechanicsChart;
        }
    }
}
