using LuckParser.Models.DataModels;
using System;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorDuration : BoonSimulator
    {
        
        // Constructor
        public BoonSimulatorDuration(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        // Public Methods
    
        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0)
            {
                var toAdd = new BoonSimulationItemDuration(BoonStack[0]);
                if (Simulation.Count > 0)
                {
                    var last = Simulation.Last();
                    if (last.End > toAdd.Start)
                    {
                        last.SetEnd(toAdd.Start);
                    }
                }
                Simulation.Add(toAdd);
                BoonStack[0] = new BoonStackItem(BoonStack[0], timePassed, timePassed);
                long diff = timePassed - Math.Abs(Math.Min(BoonStack[0].BoonDuration, 0));
                for (int i = 1; i < BoonStack.Count; i++)
                {
                    BoonStack[i] = new BoonStackItem(BoonStack[i], diff, 0);
                }
                if (BoonStack[0].BoonDuration <= 0)
                {
                    // Spend leftover time
                    long leftover = Math.Abs(BoonStack[0].BoonDuration);
                    BoonStack.RemoveAt(0);
                    Update(leftover);
                }
            }
        }      
    }
}
