using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {
        public struct SrcDuration
        {
            public long duration;
            public ushort src;
            public SrcDuration(long duration, ushort src)
            {
                this.duration = duration;
                this.src = src;
            }
        }

        // Fields
        protected List<SrcDuration> boon_stack = new List<SrcDuration>();
        protected int capacity;

        // Constructor
        public BoonSimulator(int capacity)
        {
            this.capacity = capacity;
        }

        // Abstract Methods
        public abstract long getStackValue();

        public abstract void update(long time_passed);

        public abstract void addStacksBetween(List<long> boon_stacks, long time_between);

        // Public Methods
        public void add(long boon_duration, ushort srcinstid)
        {
            SrcDuration toAdd = new SrcDuration(boon_duration, srcinstid);
            // Find empty slot
            if (!isFull())
            {
                boon_stack.Add(toAdd);
                sort();
            }
            // Replace lowest value
            else
            {
                int index = boon_stack.Count() - 1;
                if (boon_stack[index].duration < boon_duration)
                {
                    boon_stack[index] = toAdd;
                    sort();
                }
            }
        }
         
        // Protected Methods
        protected bool isFull()
        {
            return boon_stack.Count() >= capacity;
        }

        protected void sort()
        {
            // Collections.sort(boon_stack, Collections.reverseOrder());
            boon_stack = boon_stack.OrderByDescending(x=>x.duration).ToList();
        }
    }
}