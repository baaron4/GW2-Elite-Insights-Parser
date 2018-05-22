using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Intensity:AbstractBoon
    {
        // Constructor
        public Intensity(int capacity):base(capacity)
        {
            //super(capacity);
        }

        // Public Methods
        
    public override long getStackValue()
        {
            return boon_stack.Count();
        }

        
    public override void update(long time_passed)
        {

            // Subtract from each
            for (int i = 0; i < boon_stack.Count(); i++)
            {
                boon_stack[i] = boon_stack[i] - time_passed;
            }
            // Remove negatives
            int indexcount = 0;
            foreach (int iter in boon_stack.ToList()) {
                
                if (iter <= 0) {
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

       
    public override void addStacksBetween(List<long> boon_stacks, long time_between)
        {

            // Create copy of the boon
            Intensity boon_copy = new Intensity(this.capacity);
            boon_copy.boon_stack = new List<long>(this.boon_stack);
            List<long> stacks = boon_copy.boon_stack;

            // Simulate the boon stack decreasing
            if (stacks.Count() > 0)
            {

                long time_passed = 0;
                long min_duration = stacks.Min();

                // Remove minimum duration from stack
                for (long i = 1; i < time_between; i++)
                {
                    if ((i - time_passed) >= (long)min_duration)
                    {
                        boon_copy.update(i - time_passed);
                        if (stacks.Count() > 0)
                        {
                            min_duration = stacks.Min();
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
    }
}