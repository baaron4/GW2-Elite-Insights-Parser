using LuckParser.Models.DataModels;
using System;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorDuration : BoonSimulator
    {
        private ushort _lastSrcRemove = 0;
        // Constructor
        public BoonSimulatorDuration(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, ushort src, long start)
        {
            if (BoonStack.Count > 0 && oldValue > 0)
            {
                BoonStack[0].Extend(extension, src);
                if (src == 0)
                {
                    UnknownExtensionSimulationResult.Add(new BoonSimulationItemExtension(extension, BoonStack[0].Start, BoonStack[0].OriginalSrc));
                }
                return;
            }
            else
            {
                Add(oldValue + extension, src > 0 ? src : _lastSrcRemove, start, true);
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0)
            {
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
                BoonStack[0] = new BoonStackItem(BoonStack[0], timePassed, timePassed);
                long diff = timePassed - Math.Abs(Math.Min(BoonStack[0].BoonDuration, 0));
                for (int i = 1; i < BoonStack.Count; i++)
                {
                    BoonStack[i] = new BoonStackItem(BoonStack[i], diff, 0);
                }
                if (BoonStack[0].BoonDuration <= 0)
                {
                    _lastSrcRemove = BoonStack[0].Src;
                    // Spend leftover time
                    long leftover = Math.Abs(BoonStack[0].BoonDuration);
                    BoonStack.RemoveAt(0);
                    Update(leftover);
                }
            }
        }
    }
}
