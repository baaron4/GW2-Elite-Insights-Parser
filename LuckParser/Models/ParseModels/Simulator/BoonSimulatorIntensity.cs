using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorIntensity : BoonSimulator
    {
        private List<ushort> _lastSrcRemoves = new List<ushort>();
        // Constructor
        public BoonSimulatorIntensity(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, ushort src, long start)
        {
            if ((BoonStack.Count > 0 && oldValue > 0) || BoonStack.Count == Capacity)
            {
                BoonStackItem minItem = BoonStack.MinBy(x => Math.Abs(x.BoonDuration - oldValue));
                if (minItem != null)
                {
                    minItem.Extend(extension, src);
                    if (src == 0)
                    {
                        UnknownExtensionSimulationResult.Add(new BoonSimulationItemExtension(extension, minItem.Start, minItem.OriginalSrc));
                    }
                }
            }
            else
            {
                ushort srcToUse = 0;
                if (_lastSrcRemoves.Count > 0 && src == 0)
                {
                    srcToUse = _lastSrcRemoves.First();
                    Add(oldValue + extension, srcToUse, start);
                    _lastSrcRemoves.RemoveAt(0);
                }
                else
                {
                    srcToUse = src;
                    Add(oldValue + extension, srcToUse, start);
                }
                if (src == 0)
                {
                    UnknownExtensionSimulationResult.Add(new BoonSimulationItemExtension(extension, start, srcToUse));
                }
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemoves.Clear();
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
                    if (item.BoonDuration <= 0)
                    {
                        _lastSrcRemoves.Add(item.OriginalSrc);
                    }
                }
                BoonStack.RemoveAll(x => x.BoonDuration <= 0);
            }
        }
    }
}
