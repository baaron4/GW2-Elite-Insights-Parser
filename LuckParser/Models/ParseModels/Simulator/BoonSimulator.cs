using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

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
                this.boon_duration = other.boon_duration - duration_shift;
                // if duration shift > 0 this means the boon ticked, aka already in simulation, we remove the overstack
                this.overstack = duration_shift > 0 ? 0 : other.overstack;
                this.src = other.src;
            }
        }

        // Fields
        protected readonly List<BoonStackItem> boon_stack;
        protected List<BoonSimulationItem> simulation = new List<BoonSimulationItem>();
        protected int capacity;
        private ParsedLog log;
        private StackingLogic logic;

        // Constructor
        public BoonSimulator(int capacity, ParsedLog log, StackingLogic logic)
        {
            this.capacity   = capacity;
            this.boon_stack = new List<BoonStackItem>(capacity);
            this.log = log;
            this.logic = logic;
        }  

        public List<BoonSimulationItem> GetSimulationResult()
        {
            return new List<BoonSimulationItem>(simulation);
        }

        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fight_duration">Duration of the fight</param>
        public void Trim(long fight_duration)
        {
            for (int i = simulation.Count - 1; i >= 0; i--)
            {
                BoonSimulationItem data = simulation[i];
                if (data.GetEnd() > fight_duration)
                {
                    data.SetEnd(fight_duration);
                }
                else
                {
                    break;
                }
            }
            simulation.RemoveAll(x => x.GetDuration(0) <= 0);
        }

        public void Simulate(List<BoonLog> logs, long fight_duration)
        {
            long t_curr = 0;
            long t_prev = 0;
            foreach (BoonLog log in logs)
            {
                t_curr = log.GetTime();
                Update(t_curr - t_prev);
                Add(log.GetValue(), log.GetSrcInstid(), t_curr, log.GetOverstack());
                t_prev = t_curr;
            }
            Update(fight_duration - t_prev);
            simulation.RemoveAll(x => x.GetDuration(0) <= 0);
            boon_stack.Clear();
        }

        public abstract void Update(long time_passed);
        
        // Public Methods
        public void Add(long boon_duration, ushort srcinstid, long start, long overstack)
        {
            var toAdd = new BoonStackItem(start, boon_duration, srcinstid, overstack);
            // Find empty slot
            if (boon_stack.Count < capacity)
            {
                boon_stack.Add(toAdd);
                logic.Sort(log, boon_stack);
            }
            // Replace lowest value
            else
            {
                bool found = logic.StackEffect(log, toAdd, boon_stack, simulation);
                if (!found)
                {
                    long overstackValue = overstack + boon_duration;
                    ushort srcValue = srcinstid;
                    for (int j = simulation.Count - 1; j >= 0; j--)
                    {
                        if (simulation[j].AddOverstack(srcValue, overstackValue))
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
