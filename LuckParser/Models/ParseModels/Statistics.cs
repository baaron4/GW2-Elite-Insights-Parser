using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Statistics
    {

        public static double[] getBoonUptime(AbstractBoon boon, List<BoonLog> boon_logs, BossData b_data,int players) {
            double fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            double boonDur = 0.00;
            double os = 0.00;
            if (players <= 0) {
                players = 1;
            }
            foreach (BoonLog bl in boon_logs) {
                boonDur = boonDur + bl.getValue();
                os = os + bl.getOverstack();
            }
            double[] doubles = {(boonDur -os)/(fight_duration*players), boonDur/(fight_duration*players) };
            return  doubles ;
        }
        public static List<Point> getBoonIntervalsList(AbstractBoon boon, List<BoonLog> boon_logs,BossData b_data)
        {
            // Initialise variables
            int t_prev = 0;
            int t_curr = 0;
            List<Point> boon_intervals = new List<Point>();

            // Loop: update then add durations
            foreach (BoonLog log in boon_logs)
            {
                t_curr = log.getTime();
                boon.update(t_curr - t_prev);
                boon.add(log.getValue());
                boon_intervals.Add(new Point(t_curr, t_curr + boon.getStackValue()));
                t_prev = t_curr;
            }

            // Merge intervals
            boon_intervals = Utility.mergeIntervals(boon_intervals);

            // Trim duration overflow
            int fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            int last = boon_intervals.Count() - 1;
            if (boon_intervals[last].Y > fight_duration)
            {
                Point mod = boon_intervals[last];
                mod.Y = fight_duration;
                boon_intervals[last] = mod;
            }

            return boon_intervals;
        }

        public static String getBoonDuration(List<Point> boon_intervals,BossData b_data)
        {
            // Calculate average duration
            double average_duration = 0.0;
            foreach (Point p in boon_intervals)
            {
                average_duration = average_duration + (p.Y - p.X);
            }
            return String.Format("{0:0}%", 100*(average_duration / (b_data.getLastAware() - b_data.getFirstAware())));
        }

        public static String[] getBoonDuration(List<Point> boon_intervals, List<Point> fight_intervals)
        {
            // Phase durations
            String[] phase_durations = new String[fight_intervals.Count()];

            // Loop: add intervals in between, merge, calculate duration
            for (int i = 0; i < fight_intervals.Count(); i++)
            {
                Point p = fight_intervals[i];
                List<Point> boons_intervals_during_phase = new List<Point>();
                foreach (Point b in boon_intervals)
                {
                    if (b.X < p.Y && p.X < b.Y)
                    {
                        if (p.X <= b.X && b.Y <= p.Y)
                        {
                            boons_intervals_during_phase.Add(b);
                        }
                        else if (b.X < p.X && p.Y < b.Y)
                        {
                            boons_intervals_during_phase.Add(p);
                        }
                        else if (b.X < p.X && b.Y <= p.Y)
                        {
                            boons_intervals_during_phase.Add(new Point(p.X, b.Y));
                        }
                        else if (p.X <= b.X && p.Y < b.Y)
                        {
                            boons_intervals_during_phase.Add(new Point(b.X, p.Y));
                        }
                    }
                }
                double duration = 0.0;
                foreach (Point b in boons_intervals_during_phase)
                {
                    duration = duration + (b.Y - b.X);
                }
                phase_durations[i] = String.Format("{0:0.0}", (duration / (p.Y - p.X))*100);
            }
            return phase_durations;
        }

        public static List<int> getBoonStacksList(AbstractBoon boon, List<BoonLog> boon_logs,BossData b_data)
        {
            // Initialise variables
            int t_prev = 0;
            int t_curr = 0;
            List<int> boon_stacks = new List<int>();
            boon_stacks.Add(0);

            // Loop: fill, update, and add to stacks
            foreach (BoonLog log in boon_logs)
            {
                t_curr = log.getTime();
                boon.addStacksBetween(boon_stacks, t_curr - t_prev);
                boon.update(t_curr - t_prev);
                boon.add(log.getValue());
                if (t_curr != t_prev)
                {
                    boon_stacks.Add(boon.getStackValue());
                }
                else
                {
                    boon_stacks[boon_stacks.Count() - 1] = boon.getStackValue();
                }
                t_prev = t_curr;
            }

            // Fill in remaining stacks
            boon.addStacksBetween(boon_stacks, b_data.getLastAware() - b_data.getFirstAware() - t_prev);
            boon.update(1);
            boon_stacks.Add(boon.getStackValue());
            return boon_stacks;
        }

        public static String getAverageStacks(List<int> boon_stacks)
        {
            // Calculate average stacks
            double average_stacks = boon_stacks.Sum();
            double average = average_stacks / boon_stacks.Count();
            if (average > 10.0)
            {
                return String.Format("{0:0.00}", average);
            }
            else
            {
                return String.Format("{0:0.00}", average);
            }
        }

        public static String[] getAverageStacks(List<int> boon_stacks, List<Point> fight_intervals)
        {
            // Phase stacks
            String[] phase_stacks = new String[fight_intervals.Count()];

            // Loop: get sublist and calculate average stacks
            for (int i = 0; i < fight_intervals.Count(); i++)
            {
                Point p = fight_intervals[i];
                List<int> phase_boon_stacks = new List<int>(boon_stacks.GetRange(p.X, p.Y-p.X));
                double average_stacks = phase_boon_stacks.Sum();
                double average = average_stacks / phase_boon_stacks.Count();
                if (average > 10.0)
                {
                    phase_stacks[i] = String.Format("{0:0.00}", average);
                }
                else
                {
                    phase_stacks[i] = String.Format("{0:0.00}", average);
                }
            }
            return phase_stacks;
        }
    }
}