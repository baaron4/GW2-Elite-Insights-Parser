using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels
{
    internal class MechanicChartDataDto
    {
        public string Symbol { get; internal set; }
        public int Size { get; internal set; }
        public string Color { get; internal set; }
        public List<List<List<object>>> Points { get; internal set; }
        public bool Visible { get; internal set; }

        private static List<List<object>> GetMechanicChartPoints(List<MechanicEvent> mechanicLogs, PhaseData phase, ParsedEvtcLog log, bool enemyMechanic)
        {
            var res = new List<List<object>>();
            if (!enemyMechanic)
            {
                var playerIndex = new Dictionary<AbstractActor, int>();
                for (int p = 0; p < log.PlayerList.Count; p++)
                {
                    playerIndex.Add(log.PlayerList[p], p);
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
                var targetIndex = new Dictionary<AbstractActor, int>();
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

        private static List<List<List<object>>> BuildMechanicGraphPointData(ParsedEvtcLog log, List<MechanicEvent> mechanicLogs, bool enemyMechanic)
        {
            var list = new List<List<List<object>>>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                list.Add(GetMechanicChartPoints(mechanicLogs, phase, log, enemyMechanic));
            }
            return list;
        }

        internal static List<MechanicChartDataDto> BuildMechanicsChartData(ParsedEvtcLog log)
        {
            var mechanicsChart = new List<MechanicChartDataDto>();
            foreach (Mechanic mech in log.MechanicData.GetPresentMechanics(log, 0))
            {
                List<MechanicEvent> mechanicLogs = log.MechanicData.GetMechanicLogs(log, mech);
                var dto = new MechanicChartDataDto
                {
                    Color = mech.PlotlySetting.Color,
                    Symbol = mech.PlotlySetting.Symbol,
                    Size = mech.PlotlySetting.Size,
                    Visible = (mech.SkillId == SkillItem.DeathId || mech.SkillId == SkillItem.DownId),
                    Points = BuildMechanicGraphPointData(log, mechanicLogs, mech.IsEnemyMechanic)
                };
                mechanicsChart.Add(dto);
            }
            return mechanicsChart;
        }
    }
}
