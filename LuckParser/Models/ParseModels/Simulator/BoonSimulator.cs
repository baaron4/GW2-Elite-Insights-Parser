using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {
 
        // Fields
        protected List<BoonSimulationItem> boon_stack = new List<BoonSimulationItem>();
        protected List<BoonSimulationItem> simulation = new List<BoonSimulationItem>();
        protected int capacity;

        // Constructor
        public BoonSimulator(int capacity)
        {
            this.capacity = capacity;
        }

        public abstract void simulate(List<LogBoon> logs);

        public List<BoonSimulationItem> getSimulationResult()
        {
            return new List<BoonSimulationItem>(simulation);
        }

        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fight_duration">Duration of the fight</param>
        public abstract void trim(long fight_duration);

        // Abstract Methods
        public abstract long getStackValue();

        public abstract void update(long time_passed);

        public abstract void addStacksBetween(List<long> boon_stacks, long time_between);

        // Public Methods
        public void add(long boon_duration, ushort srcinstid, long start = 0)
        {
            BoonSimulationItem toAdd = new BoonSimulationItem(start, boon_duration, srcinstid);
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
                if (boon_stack[index].getDuration() < boon_duration)
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
            boon_stack = boon_stack.OrderByDescending(x=>x.getDuration()).ToList();
        }
    }
}