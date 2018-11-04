using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public string Name { get; set; }
        public bool DrawStart { get; set; }
        public bool DrawLabel { get; set; }
        public bool DrawEnd { get; set; }
        public bool DrawArea { get; set; }
        public List<Boss> Targets { get; } = new List<Boss>();

        public PhaseData(long start, long end)
        {
            Start = start;
            End = end;
            DrawLabel = true;
        }
        
        public long GetDuration(string format = "ms")
        {
            switch (format)
            {
                case "m":
                    return (End - Start) / 60000;
                case "s":
                    return (End - Start) / 1000;
                default:
                    return (End - Start);
            }

        }

        public bool InInterval(long time, long offset = 0)
        {
            return Start <= time - offset && time - offset <= End;
        }

        public void OverrideStart(long start)
        {
            Start = start;
        }

        public void OverrideEnd(long end)
        {
            End = end;
        }

        public void OverrideTimes(long offset, CombatData combatData)
        {
            if (Targets.Count > 0)
            {
                List<CombatItem> deathEvents = combatData.GetStatesData(DataModels.ParseEnum.StateChange.ChangeDead);
                Start = Math.Max(Start, Targets.Min(x => x.FirstAware)- offset);
                long end = long.MinValue;
                foreach (Boss target in Targets)
                {
                    long dead = target.LastAware;
                    CombatItem died = deathEvents.FirstOrDefault(x => x.SrcInstid == target.InstID && x.Time >= target.FirstAware && x.Time <= target.LastAware);
                    if (died != null)
                    {
                        dead = died.Time;
                    }
                    end = Math.Max(end, dead);
                }
                End = Math.Min(End, end - offset);
            }
        }
    }
}
