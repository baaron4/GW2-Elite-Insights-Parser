using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.HtmlModels
{  
    public class MechanicChartDataDto
    {       
        public string Symbol;     
        public int Size;
        public string Color;       
        public List<List<List<object>>> Points;      
        public bool Visible;

        public static List<List<object>> GetMechanicChartPoints(List<MechanicLog> mechanicLogs,PhaseData phase, ParsedLog log, bool enemyMechanic)
        {
            List<List<object>> res = new List<List<object>>();
            if (!enemyMechanic)
            {
                Dictionary<DummyActor, int> playerIndex = new Dictionary<DummyActor, int>();
                for (var p = 0; p < log.PlayerList.Count; p++)
                {
                    playerIndex.Add(log.PlayerList[p], p);
                    res.Add(new List<object>());
                }
                foreach (MechanicLog ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
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
                Dictionary<DummyActor, int> targetIndex = new Dictionary<DummyActor, int>();
                for (var p = 0; p < phase.Targets.Count; p++)
                {
                    targetIndex.Add(phase.Targets[p], p);
                    res.Add(new List<object>());
                }
                res.Add(new List<object>());
                foreach (MechanicLog ml in mechanicLogs.Where(x => phase.InInterval(x.Time)))
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
