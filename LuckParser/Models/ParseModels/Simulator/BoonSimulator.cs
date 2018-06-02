using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {
 
        public struct BoonStackItem
        {
            public long start;
            public long boon_duration;
            public ushort src;

            public BoonStackItem(long start, long boon_duration, ushort srcinstid)
            {
                this.start = start;
                this.boon_duration = boon_duration;
                this.src = srcinstid;
            }

            public BoonStackItem(BoonStackItem other, long start_shift, long duration_shift)
            {
                this.start = Math.Max(other.start + start_shift, 0);
                this.boon_duration = other.boon_duration + duration_shift;
                this.src = other.src;
            }
        }

        // Fields
        protected List<BoonStackItem> boon_stack = new List<BoonStackItem>();
        protected int capacity;

        // Constructor
        public BoonSimulator(int capacity)
        {
            this.capacity = capacity;
        }
        public abstract List<BoonSimulationItem> getSimulationResult();
        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fight_duration">Duration of the fight</param>
        public abstract void trim(long fight_duration);
        public abstract void simulate(List<LogBoon> logs);
        public abstract void update(long time_passed);
        
        // Public Methods
        public void add(long boon_duration, ushort srcinstid, long start = 0)
        {
            BoonStackItem toAdd = new BoonStackItem(start, boon_duration, srcinstid);
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
                if (boon_stack[index].boon_duration < boon_duration)
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
            boon_stack = boon_stack.OrderByDescending(x=>x.boon_duration).ToList();
        }
    }
}