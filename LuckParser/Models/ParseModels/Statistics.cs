using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Statistics
    {
        /*public static double[] getBoonUptime(AbstractBoon boon, List<BoonLog> boon_logs, BossData b_data, int players)
        {
            double fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            double boonDur = 0.00;
            double os = 0.00;

            if (players <= 0)
            {
                players = 1;
            }
            foreach (BoonLog bl in boon_logs)
            {
                if (bl.getValue() + bl.getTime() > fight_duration)
                {
                    os = os + (bl.getValue() - (fight_duration - bl.getTime())) + bl.getOverstack();
                }
                else {
                    os = os + bl.getOverstack();
                }
                boonDur = boonDur + bl.getValue();
                
            }
            double[] doubles = { (boonDur-os ) / (fight_duration * players), boonDur / (fight_duration * players) };

            return doubles;
        }*/
        // DURATION
        public static List<Point> getBoonIntervalsList(BoonSimulator boon, List<LogBoon> boon_logs,BossData b_data)
        { 
            // Initialise variables
            long t_prev = 0;
            long t_curr = 0;
            List<Point> boon_intervals = new List<Point>();

            // Loop: update then add durations
            foreach (LogBoon log in boon_logs)
            {
                t_curr = log.getTime();
                boon.update(t_curr - t_prev);
                boon.add(log.getValue(), log.getSrcInstid());
                long duration = t_curr + boon.getStackValue();
                if (duration < 0) {
                    duration = long.MaxValue;
                }
                t_prev = t_curr;
                boon_intervals.Add(new Point((int)t_curr, (int)duration));
            }

            // Merge intervals
            boon_intervals = Utility.mergeIntervals(boon_intervals);

            // Trim duration overflow
            long fight_duration = b_data.getLastAware() - b_data.getFirstAware();
            int last = boon_intervals.Count() - 1;
            if (last > -1 && boon_intervals[last].Y > fight_duration)
            {
                Point mod = boon_intervals[last];
                mod.Y = (int)fight_duration;
                boon_intervals[last] = mod;
            }
            
            return boon_intervals;
        }

        public static double getBoonDuration(List<Point> boon_intervals,BossData b_data)
        {
            // Calculate average duration
            double average_duration = 0.0;
            foreach (Point p in boon_intervals)
            {
                average_duration = average_duration + (p.Y - p.X);
            }
            double number= 100 * (average_duration / (b_data.getLastAware() - b_data.getFirstAware()));
            /*if (number > 100) {
                int stop = 0;
            }*/
            return number;
        }
    
        // INTENSITY
        public static List<long> getBoonStacksList(BoonSimulator boon, List<LogBoon> boon_logs,BossData b_data)
        {
            // Initialise variables
            long t_prev = 0;
            long t_curr = 0;
            List<long> boon_stacks = new List<long>();
            boon_stacks.Add(0);

            // Loop: fill, update, and add to stacks
            foreach (LogBoon log in boon_logs)
            {
                t_curr = log.getTime();
                boon.addStacksBetween(boon_stacks, t_curr - t_prev);
                boon.update(t_curr - t_prev);
                boon.add(log.getValue(), log.getSrcInstid());
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
            return boon_stacks;
        }

        public static double getAverageStacks(List<long> boon_stacks)
        {
            // Calculate average stacks
            double average_stacks = boon_stacks.Sum();
            double average = average_stacks / boon_stacks.Count();
            
            return average;         
        }

       
    }
}