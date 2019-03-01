using LuckParser.Parser;
using System;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorDuration : BoonSimulator
    {
        private AgentItem _lastSrcRemove = GeneralHelper.UnknownAgent;
        // Constructor
        public BoonSimulatorDuration(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start)
        {
            if ((BoonStack.Count > 0 && oldValue > 0) || BoonStack.Count == Capacity)
            {
                BoonStack[0].Extend(extension, src, start);
            }
            else
            {
                Add(oldValue + extension, src, _lastSrcRemove, start, true);
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemove = GeneralHelper.UnknownAgent;
                   var toAdd = new BoonSimulationItemDuration(BoonStack[0]);
                if (GenerationSimulation.Count > 0)
                {
                    var last = GenerationSimulation.Last();
                    if (last.End > toAdd.Start)
                    {
                        last.SetEnd(toAdd.Start);
                    }
                }
                GenerationSimulation.Add(toAdd);
                long timeDiff = BoonStack[0].BoonDuration - timePassed;
                long diff = 0;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = BoonStack[0].BoonDuration;
                    leftOver = timePassed - diff;
                }
                else
                {
                    diff = timePassed;
                }
                BoonStack[0] = new BoonStackItem(BoonStack[0], diff, diff);
                for (int i = 1; i < BoonStack.Count; i++)
                {
                    BoonStack[i] = new BoonStackItem(BoonStack[i], diff, 0);
                }
                if (BoonStack[0].BoonDuration == 0)
                {
                    _lastSrcRemove = BoonStack[0].SeedSrc;
                    BoonStack.RemoveAt(0);
                }
                Update(leftOver);
            }
        }
    }
}
