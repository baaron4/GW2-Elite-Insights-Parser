using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorIntensity : BoonSimulator
    {
        
        // Constructor
        public BoonSimulatorIntensity(int capacity) : base(capacity)
        {
        }

        // Public Methods
             
        public override void update(long time_passed)
        {
            if (boon_stack.Count > 0)
            {
                var toAdd = new BoonSimulationItemIntensity(boon_stack);
                if (simulation.Count > 0)
                {
                    BoonSimulationItem last = simulation.Last();
                    if (last.getEnd() > toAdd.getStart())
                    {
                        last.setEnd(toAdd.getStart());
                    }
                }
                simulation.Add(toAdd);
                // Subtract from each
                for(int i = boon_stack.Count - 1; i >= 0; i--)
                {
                    var item = new BoonStackItem(boon_stack[i], time_passed, -time_passed);
                    if(item.boon_duration <= 0)
                    {
                        boon_stack.RemoveAt(i);
                    }
                    else
                    {
                        boon_stack[i] = item;
                    }
                }
            }
        }

        protected override void sort()
        {
            // no need to sort intensity based items
        }
    }
}
