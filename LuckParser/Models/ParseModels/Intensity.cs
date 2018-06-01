using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Intensity : AbstractBoon
    {
        // Constructor
        public Intensity(int capacity) : base(capacity)
        {
            //super(capacity);
        }

        // Public Methods

        public override long getStackValue(ushort src = 0)
        {
            if (src != 0)
            {
                return boon_stack.Where(x => x.src == src ).Count();
            }
            return boon_stack.Count();
        }
        

        public override void update(long time_passed)
        {

            // Subtract from each
            for (int i = 0; i < boon_stack.Count(); i++)
            {
                boon_stack[i] = new SrcDuration(boon_stack[i].duration - time_passed, boon_stack[i].src);
            }
            // Remove negatives
            int indexcount = 0;
            foreach (long iter in boon_stack.Select(x => x.duration).ToList())
            {

                if (iter <= 0)
                {
                    boon_stack.RemoveAt(indexcount);
                    indexcount--;
                }
                indexcount++;
            }
            //for (iterator<int> iter = boon_stack.listIterator(); iter.hasNext();)
            //{
            //    int stack = iter.next();
            //    if (stack <= 0)
            //    {
            //        iter.remove();
            //    }
            //}
        }


        public override void addStacksBetween(List<long> boon_stacks, long time_between, ushort src = 0)
        {

            // Create copy of the boon
            Intensity boon_copy = new Intensity(capacity);
            boon_copy.boon_stack = new List<SrcDuration>(boon_stack);
            List<SrcDuration> stacks = boon_copy.boon_stack;

            // Simulate the boon stack decreasing
            if (stacks.Count() > 0)
            {

                long time_passed = 0;
                long min_duration = stacks.Select(x => x.duration).Min();

                // Remove minimum duration from stack
                for (long i = 1; i < time_between; i++)
                {
                    if ((i - time_passed) >= min_duration)
                    {
                        boon_copy.update(i - time_passed);
                        if (stacks.Count() > 0)
                        {
                            min_duration = stacks.Select(x => x.duration).Min();
                        }
                        time_passed = i;
                    }
                    boon_stacks.Add(boon_copy.getStackValue(src));
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
    }
}