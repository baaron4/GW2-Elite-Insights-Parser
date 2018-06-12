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
            if (boon_stack.Count() > 0)
            {

                BoonSimulationItemIntensity toAdd = new BoonSimulationItemIntensity(boon_stack);
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
                for (int i = 0; i < boon_stack.Count(); i++)
                {
                    boon_stack[i] = new BoonStackItem(boon_stack[i], time_passed, -time_passed);
                }
                // Remove negatives
                boon_stack = boon_stack.Where(x => x.boon_duration > 0).ToList();

            }
        }
    }
}