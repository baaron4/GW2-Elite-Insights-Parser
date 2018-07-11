using System;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorDuration : BoonSimulator
    {
        
        // Constructor
        public BoonSimulatorDuration(int capacity) : base(capacity)
        {
        }

        // Public Methods
    
        public override void update(long time_passed)
        {
            if (boon_stack.Count > 0)
            {
                var toAdd = new BoonSimulationItemDuration(boon_stack[0]);
                if (simulation.Count > 0)
                {
                    var last = simulation.Last();
                    if (last.getEnd() > toAdd.getStart())
                    {
                        last.setEnd(toAdd.getStart());
                    }
                }
                simulation.Add(toAdd);
                boon_stack[0] = new BoonStackItem(boon_stack[0], time_passed, -time_passed);
                long diff = time_passed - Math.Abs(Math.Min(boon_stack[0].boon_duration, 0));
                for (int i = 1; i < boon_stack.Count; i++)
                {
                    boon_stack[i] = new BoonStackItem(boon_stack[i], diff, 0);
                }
                if (boon_stack[0].boon_duration <= 0)
                {
                    // Spend leftover time
                    long leftover = Math.Abs(boon_stack[0].boon_duration);
                    boon_stack.RemoveAt(0);
                    update(leftover);
                }
            }
        }

        protected override void sort()
        {
            boon_stack.Sort((a, b) => b.boon_duration.CompareTo(a.boon_duration));
        }
    }
}
