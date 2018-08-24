using LuckParser.Models.DataModels;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorIntensity : BoonSimulator
    {
        
        // Constructor
        public BoonSimulatorIntensity(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        // Public Methods
             
        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0)
            {
                var toAdd = new BoonSimulationItemIntensity(BoonStack);
                if (Simulation.Count > 0)
                {
                    BoonSimulationItem last = Simulation.Last();
                    if (last.GetEnd() > toAdd.GetStart())
                    {
                        last.SetEnd(toAdd.GetStart());
                    }
                }
                Simulation.Add(toAdd);
                // Subtract from each
                for(int i = BoonStack.Count - 1; i >= 0; i--)
                {
                    var item = new BoonStackItem(BoonStack[i], timePassed, timePassed);
                    BoonStack[i] = item;
                }
                BoonStack.RemoveAll(x => x.BoonDuration < 1);
            }
        }
    }
}
