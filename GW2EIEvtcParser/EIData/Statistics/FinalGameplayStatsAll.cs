using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalGameplayStatsAll : FinalGameplayStats
    {
        // Rates
        public int Wasted { get; set; }
        public double TimeWasted { get; set; }
        public int Saved { get; set; }
        public double TimeSaved { get; set; }
        public double StackDist { get; set; }
        public double DistToCom { get; set; }

        // boons
        public double AvgBoons { get; set; }
        public double AvgActiveBoons { get; set; }
        public double AvgConditions { get; set; }
        public double AvgActiveConditions { get; set; }

        // Counts
        public int SwapCount { get; set; }

        private static double GetDistanceToTarget(Player player, ParsedEvtcLog log, PhaseData phase, List<Point3D> reference)
        {
            var positions = player.GetCombatReplayPolledPositions(log).Where(x => x.Time >= phase.Start && x.Time <= phase.End).ToList();
            int offset = player.GetCombatReplayPolledPositions(log).Count(x => x.Time < phase.Start);
            if (positions.Count > 1 && reference.Count > 0)
            {
                var distances = new List<float>();
                for (int time = 0; time < positions.Count; time++)
                {

                    float deltaX = positions[time].X - reference[time + offset].X;
                    float deltaY = positions[time].Y - reference[time + offset].Y;
                    //float deltaZ = positions[time].Z - StackCenterPositions[time].Z;


                    distances.Add((float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                }
                return distances.Sum() / distances.Count;
            }
            else
            {
                return -1;
            }
        }

        public FinalGameplayStatsAll(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor actor) : base(log, phase, actor, null)
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

            if (log.CombatData.HasMovementData && actor is Player player)
            {
                StackDist = GetDistanceToTarget(player, log, phase, log.Statistics.GetStackCenterPositions(log));
                DistToCom = GetDistanceToTarget(player, log, phase, log.Statistics.GetStackCommanderPositions(log));
            }
        }
    }
}
