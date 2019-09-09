using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Builders.HtmlModels
{
    public class MechanicChartDataDto
    {
        public string Symbol { get; set; }
        public int Size { get; set; }
        public string Color { get; set; }
        public List<List<List<object>>> Points { get; set; }
        public bool Visible { get; set; }

        public static List<List<object>> GetMechanicChartPoints(List<MechanicEvent> mechanicLogs, PhaseData phase, ParsedLog log, bool enemyMechanic)
        {
            var res = new List<List<object>>();
            if (!enemyMechanic)
            {
                var playerIndex = new Dictionary<DummyActor, int>();
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
                var targetIndex = new Dictionary<DummyActor, int>();
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
    }
}
