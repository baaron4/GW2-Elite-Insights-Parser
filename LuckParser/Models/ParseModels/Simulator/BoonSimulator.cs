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
            public long overstack;

            public BoonStackItem(long start, long boon_duration, ushort srcinstid, long overstack)
            {
                this.start = start;
                this.boon_duration = boon_duration;
                this.src = srcinstid;
                this.overstack = overstack;
            }

            public BoonStackItem(BoonStackItem other, long start_shift, long duration_shift)
            {
                this.start = Math.Max(other.start + start_shift, 0);
                this.boon_duration = other.boon_duration + duration_shift;
                // if duration shift > 0 this means the boon ticked, aka already in simulation, we remove the overstack
                this.overstack = duration_shift > 0 ? 0 : other.overstack;
                this.src = other.src;
            }
        }

        // Fields
        protected List<BoonStackItem> boon_stack = new List<BoonStackItem>();
        protected List<BoonSimulationItem> simulation = new List<BoonSimulationItem>();
        protected int capacity;

        // Constructor
        public BoonSimulator(int capacity)
        {
            this.capacity = capacity;
        }
        public List<BoonSimulationItem> getSimulationResult()
        {
            return new List<BoonSimulationItem>(simulation);
        }
        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fight_duration">Duration of the fight</param>
        public void trim(long fight_duration)
        {
            for (int i = simulation.Count - 1; i >= 0; i--)
            {
                BoonSimulationItem data = simulation[i];
                if (data.getEnd() > fight_duration)
                {
                    data.setEnd(fight_duration);
                }
                else
                {
                    break;
                }
            }
        }
        public void simulate(List<LogBoon> logs, long fight_duration)
        {
            long t_curr = 0;
            long t_prev = 0;
            foreach (LogBoon log in logs)
            {
                t_curr = log.getTime();
                update(t_curr - t_prev);
                add(log.getValue(), log.getSrcInstid(), t_curr, log.getOverstack());
                t_prev = t_curr;
            }
            update(fight_duration - t_prev);
            simulation = simulation.Where(x => x.getDuration(0) > 0).ToList();
            boon_stack.Clear();
        }
        public abstract void update(long time_passed);
        
        // Public Methods
        public void add(long boon_duration, ushort srcinstid, long start, long overstack)
        {
            BoonStackItem toAdd = new BoonStackItem(start, boon_duration, srcinstid, overstack);
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