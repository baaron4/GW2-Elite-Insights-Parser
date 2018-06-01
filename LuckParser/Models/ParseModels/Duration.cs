using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Duration : AbstractBoon
    {
        // Constructor
        public Duration(int capacity) : base(capacity)
        {
            //super(capacity);
        }

        // Public Methods

        public override long getStackValue(ushort src = 0)
        {
            // return boon_stack.stream().mapToInt(Integer::intValue).sum();
            //check for overflow
            List<SrcDuration> toCheck = boon_stack;
            if (src > 0)
            {
                toCheck = toCheck.Where(x => x.src == src).ToList();
            }
            if (toCheck.Count == 0)
            {
                return 0;
            }
            long total = toCheck[0].duration;
            for (int i = 1; i < toCheck.Count; i++)
            {
                if (total > 0 && toCheck[i].duration > long.MaxValue - total)
                {
                    //Overflow
                    return long.MaxValue;
                }
                else
                {
                    //ok
                    total += toCheck[i].duration;
                }
            }
            return total;
        }

        public override void update(long time_passed)
        {

            if (boon_stack.Count() > 0)
            {
                // Clear stack
                if (time_passed >= getStackValue())
                {
                    boon_stack.Clear();
                    return;
                }
                // Remove from the longest duration
                else
                {
                    boon_stack[0] = new SrcDuration(boon_stack[0].duration - time_passed, boon_stack[0].src);
                    if (boon_stack[0].duration <= 0)
                    {
                        // Spend leftover time
                        time_passed = Math.Abs(boon_stack[0].duration);
                        boon_stack.RemoveAt(0);
                        update(time_passed);
                    }
                }
            }
        }


        public override void addStacksBetween(List<long> boon_stacks, long time_between, ushort src = 0)
        {
        }
    }
}