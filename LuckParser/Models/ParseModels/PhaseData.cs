using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        public long Start { get; private set; }
        public long End { get; private set; }
        public long DurationInS { get; private set; }
        public long DurationInMS { get; private set; }
        public long DurationInM { get; private set; }
        public string Name { get; set; }
        public bool DrawStart { get; set; } = true;
        public bool DrawEnd { get; set; } = true;
        public bool DrawArea { get; set; } = true;
        public List<Target> Targets { get; } = new List<Target>();

        public PhaseData(long start, long end)
        {
            Start = start;
            End = end;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        public bool InInterval(long time)
        {
            return Start <= time && time <= End;
        }

        public void OverrideStart(long start)
        {
            Start = start;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        public void OverrideEnd(long end)
        {
            End = end;
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }

        /// <summary>
        /// Override times in a manner that the phase englobes the targets present in the phase (if possible)
        /// </summary>
        /// <param name="log"></param>
        public void OverrideTimes(ParsedLog log)
        {
            if (Targets.Count > 0)
            {
                List<CombatItem> deathEvents = log.CombatData.GetStates(ParseEnum.StateChange.ChangeDead);
                Start = Math.Max(Start, log.FightData.ToFightSpace(Targets.Min(x => x.FirstAware)));
                long end = long.MinValue;
                foreach (Target target in Targets)
                {
                    long dead = target.LastAware;
                    CombatItem died = deathEvents.FirstOrDefault(x => x.SrcInstid == target.InstID && x.Time >= target.FirstAware && x.Time <= target.LastAware);
                    if (died != null)
                    {
                        dead = died.Time;
                    }
                    end = Math.Max(end, log.FightData.ToFightSpace(dead));
                }
                End = Math.Min(Math.Min(End, end), log.FightData.FightDuration);
            }
            DurationInM = (End - Start) / 60000;
            DurationInMS = (End - Start);
            DurationInS = (End - Start) / 1000;
        }
    }
}
