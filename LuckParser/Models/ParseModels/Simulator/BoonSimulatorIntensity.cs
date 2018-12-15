using LuckParser.Models.DataModels;
using System;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorIntensity : BoonSimulator
    {

        // Constructor
        public BoonSimulatorIntensity(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, ushort src)
        {
            if (BoonStack.Count > 0)
            {
                foreach (BoonStackItem bsi in BoonStack)
                {
                    if (Math.Abs(bsi.BoonDuration - oldValue) < 4)
                    {
                        bsi.Extend(extension, src);
                        if (src == 0)
                        {
                            UnknownExtensionSimulationResult.Add(new BoonSimulationItemExtension(extension, bsi.Start, bsi.OriginalSrc));
                        }
                        return;
                    }
                }
            }
#if DEBUG
            throw new InvalidOperationException("No buff to extend");
#endif
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0 && timePassed > 0)
            {
                var toAdd = new BoonSimulationItemIntensity(BoonStack);
                if (GenerationSimulation.Count > 0)
                {
                    BoonSimulationItem last = GenerationSimulation.Last();
                    if (last.End > toAdd.Start)
                    {
                        last.SetEnd(toAdd.Start);
                    }
                }
                GenerationSimulation.Add(toAdd);
                // Subtract from each
                for (int i = BoonStack.Count - 1; i >= 0; i--)
                {
                    var item = new BoonStackItem(BoonStack[i], timePassed, timePassed);
                    BoonStack[i] = item;
                }
                BoonStack.RemoveAll(x => x.BoonDuration < 1);
            }
        }
    }
}
