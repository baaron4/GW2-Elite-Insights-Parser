using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class FinalGameplayStatsAll : FinalGameplayStats
    {
        // Rates
        public int Wasted { get; set; }
        public double TimeWasted { get; set; }
        public int Saved { get; set; }
        public double TimeSaved { get; set; }
        public double StackDist { get; set; }

        // boons
        public double AvgBoons { get; set; }
        public double AvgActiveBoons { get; set; }
        public double AvgConditions { get; set; }
        public double AvgActiveConditions { get; set; }

        // Counts
        public int SwapCount { get; set; }

        public FinalGameplayStatsAll(ParsedLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor, null)
        {
            // If fake actor, stop
            if (actor.IsFakeActor)
            {
                return;
            }
            int phaseIndex = log.FightData.GetPhases(log).IndexOf(phase);
            foreach (AbstractCastEvent cl in actor.GetCastLogs(log, phase.Start, phase.End))
            {
                switch (cl.Status)
                {
                    case AbstractCastEvent.AnimationStatus.Iterrupted:
                        Wasted++;
                        TimeWasted += cl.SavedDuration;
                        break;
                    case AbstractCastEvent.AnimationStatus.Reduced:
                        Saved++;
                        TimeSaved += cl.SavedDuration;
                        break;
                }
                if (cl.SkillId == SkillItem.WeaponSwapId)
                {
                    SwapCount++;
                }
            }
            TimeSaved = Math.Round(TimeSaved / 1000.0, GeneralHelper.TimeDigit);
            TimeWasted = -Math.Round(TimeWasted / 1000.0, GeneralHelper.TimeDigit);

            double avgBoons = 0;
            foreach (long duration in actor.GetBuffPresence(log, phaseIndex).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == BuffNature.Boon).Select(x => x.Value))
            {
                avgBoons += duration;
            }
            AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BuffDigit);
            long activeDuration = phase.GetActorActiveDuration(actor, log);
            AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, GeneralHelper.BuffDigit) : 0.0;

            double avgCondis = 0;
            foreach (long duration in actor.GetBuffPresence(log, phaseIndex).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == BuffNature.Condition).Select(x => x.Value))
            {
                avgCondis += duration;
            }
            AvgConditions = Math.Round(avgCondis / phase.DurationInMS, GeneralHelper.BuffDigit);
            AvgActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, GeneralHelper.BuffDigit) : 0.0;

            if (log.CombatData.HasMovementData && actor is Player)
            {
                var positions = actor.GetCombatReplayPolledPositions(log).Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
                List<Point3D> stackCenterPositions = log.Statistics.GetStackCenterPositions(log);
                int offset = actor.GetCombatReplayPolledPositions(log).Count(x => x.Time < phase.Start);
                if (positions.Count > 1)
                {
                    var distances = new List<float>();
                    for (int time = 0; time < positions.Count; time++)
                    {

                        float deltaX = positions[time].X - stackCenterPositions[time + offset].X;
                        float deltaY = positions[time].Y - stackCenterPositions[time + offset].Y;
                        //float deltaZ = positions[time].Z - StackCenterPositions[time].Z;


                        distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                    }
                    StackDist = distances.Sum() / distances.Count;
                }
                else
                {
                    StackDist = -1;
                }
            }
        }
    }
}
