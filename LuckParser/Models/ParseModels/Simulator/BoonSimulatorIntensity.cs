using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorIntensity : BoonSimulator
    {
        // Constructor
        public BoonSimulatorIntensity(int capacity) : base(capacity)
        {
            //super(capacity);
        }

        // Public Methods

        public long getStackValue()
        {
            return boon_stack.Count();
        }
        

        public override void update(long time_passed)
        {

            // Subtract from each
            /*for (int i = 0; i < boon_stack.Count(); i++)
            {
                boon_stack[i] = new BoonSimulationItem(boon_stack[i], time_passed, -time_passed);
            }
            // Remove negatives
            int indexcount = 0;
            foreach (long iter in boon_stack.Select(x => x.getDuration()).ToList())
            {

                if (iter <= 0)
                {
                    boon_stack.RemoveAt(indexcount);
                    indexcount--;
                }
                indexcount++;
            }*/
            //for (iterator<int> iter = boon_stack.listIterator(); iter.hasNext();)
            //{
            //    int stack = iter.next();
            //    if (stack <= 0)
            //    {
            //        iter.remove();
            //    }
            //}
        }


        public void addStacksBetween(List<long> boon_stacks, long time_between)
        {
            // Create copy of the boon
            BoonSimulatorIntensity boon_copy = new BoonSimulatorIntensity(capacity);
            boon_copy.boon_stack = new List<BoonStackItem>(boon_stack);
            List<BoonStackItem> stacks = boon_copy.boon_stack;
            // Simulate the boon stack decreasing
            if (stacks.Count() > 0)
            {
                long time_passed = 0;
                long min_duration = stacks.Select(x => x.boon_duration).Min();

                // Remove minimum duration from stack
                for (long i = 1; i < time_between; i++)
                {
                    if ((i - time_passed) >= min_duration)
                    {
                        boon_copy.update(i - time_passed);
                        if (stacks.Count() > 0)
                        {
                            min_duration = stacks.Select(x => x.boon_duration).Min();
                        }
                        time_passed = i;
                    }
                    boon_stacks.Add(boon_copy.getStackValue());
                }
            }
            // Fill in remaining time with 0 values
            else
            {
                for (long i = 1; i < time_between; i++)
                {
                    boon_stacks.Add(0);
                }
            }
        }

        public override void simulate(List<LogBoon> logs)
        {
            return;
        }

        public override void trim(long fight_duration)
        {
            return;
        }

        public override List<BoonSimulationItem> getSimulationResult()
        {
            return new List<BoonSimulationItem>();
        }
    }
}