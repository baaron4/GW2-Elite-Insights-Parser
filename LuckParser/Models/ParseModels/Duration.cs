using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Duration:AbstractBoon
    {
        // Constructor
        public  Duration(int capacity):base(capacity)
        {
            //super(capacity);
        }

        // Public Methods
        
    public override int getStackValue()
        {
            // return boon_stack.stream().mapToInt(Integer::intValue).sum();
            //check for overflow
            int total = boon_stack[0];
            for (int i =0;i<boon_stack.Count-1;i++) {
                if (total > 0 && boon_stack[i + 1] > uint.MaxValue - total)
                {
                    //Overflow
                    return int.MaxValue;
                }
                else {
                    //ok
                    total += boon_stack[i + 1];
                }
            }
            return boon_stack.Sum();
        }

        
    public override void update(long time_passed)
        {

            if (boon_stack.Count() > 0)
            {
                // Clear stack
                if (time_passed >= (long)getStackValue())
                {
                    boon_stack.Clear();
                    return;
                }
                // Remove from the longest duration
                else
                {
                    boon_stack[0] = (int)((long)boon_stack[0] - time_passed);
                    if (boon_stack[0] <= 0)
                    {
                        // Spend leftover time
                        time_passed = (long)Math.Abs(boon_stack[0]);
                        boon_stack.RemoveAt(0);
                        update(time_passed);
                    }
                }
            }
        }

        
    public override void addStacksBetween(List<int> boon_stacks, long time_between)
        {
        }
    }
}