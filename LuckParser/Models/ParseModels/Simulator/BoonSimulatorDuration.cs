using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorDuration : BoonSimulator
    {
        // Constructor
        public BoonSimulatorDuration(int capacity) : base(capacity)
        {
            //super(capacity);
        }

        // Public Methods

        public override long getStackValue()
        {
            // return boon_stack.stream().mapToInt(Integer::intValue).sum();
            //check for overflow
            if (boon_stack.Count == 0)
            {
                return 0;
            }
            long total = boon_stack[0].getDuration();
            for (int i = 1; i < boon_stack.Count; i++)
            {
                if (total > 0 && boon_stack[i].getDuration() > long.MaxValue - total)
                {
                    //Overflow
                    return long.MaxValue;
                }
                else
                {
                    //ok
                    total += boon_stack[i].getDuration();
                }
            }
            return total;
        }

        public override void simulate(List<LogBoon> logs)
        {
            long t_curr = 0;
            long t_prev = 0;
            foreach (LogBoon log in logs)
            {
                t_curr = log.getTime();
                update(t_curr - t_prev);
                add(log.getValue(), log.getSrcInstid(), t_curr);
                t_prev = t_curr;
            }
            boon_stack.Clear();
        }

        public override void update(long time_passed)
        {
            if (boon_stack.Count() > 0)
            {
                BoonSimulationItem toAdd = boon_stack[0];
                if (simulation.Count > 0)
                {
                    BoonSimulationItem last = simulation.Last();
                    if (last.getEnd() > toAdd.getStart())
                    {
                        last.setEnd(toAdd.getStart());
                    }
                }
                simulation.Add(boon_stack[0]);
                boon_stack[0] = new BoonSimulationItem(boon_stack[0], time_passed, -time_passed);
                long diff = time_passed - Math.Abs(Math.Min(boon_stack[0].getDuration(), 0));
                for (int i = 1; i < boon_stack.Count(); i++)
                {
                    boon_stack[i] = new BoonSimulationItem(boon_stack[i], diff, 0);
                }
                if (boon_stack[0].getDuration() <= 0)
                {
                    // Spend leftover time
                    long leftover = Math.Abs(boon_stack[0].getDuration());
                    boon_stack.RemoveAt(0);
                    update(leftover);
                }

            }
        }


        public override void addStacksBetween(List<long> boon_stacks, long time_between)
        {
        }

        public override void trim(long fight_duration)
        {
            if (simulation.Count > 0)
            {
                BoonSimulationItem last = simulation.Last();
                if (last.getEnd() > fight_duration)
                {
                    last.setEnd(fight_duration);
                }
            }
        }
    }
}