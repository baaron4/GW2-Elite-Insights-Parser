using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulatorIntensity : BuffSimulator
    {
        private readonly List<(AgentItem agent, bool extension)> _lastSrcRemoves = new List<(AgentItem agent, bool extension)>();
        // Constructor
        public BuffSimulatorIntensity(int capacity, ParsedEvtcLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start, uint stackID)
        {
            if ((BuffStack.Count > 0 && oldValue > 0) || BuffStack.Count == Capacity)
            {
                BuffStackItem minItem = BuffStack.MinBy(x => Math.Abs(x.TotalDuration - oldValue));
                if (minItem != null)
                {
                    minItem.Extend(extension, src);
                }
            }
            else
            {
                if (_lastSrcRemoves.Count > 0)
                {
                    Add(oldValue + extension, src, _lastSrcRemoves.First().agent, start, false, _lastSrcRemoves.First().extension);
                    _lastSrcRemoves.RemoveAt(0);
                }
                else
                {
                    Add(oldValue + extension, src, start, 0, true, 0);
                }
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemoves.Clear();
                var toAdd = new BuffSimulationItemIntensity(BuffStack);
                GenerationSimulation.Add(toAdd);
                long diff = Math.Min(BuffStack.Min(x => x.Duration), timePassed);
                long leftOver = timePassed - diff;
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                // Subtract from each
                for (int i = BuffStack.Count - 1; i >= 0; i--)
                {
                    BuffStack[i].Shift(diff, diff);
                    if (BuffStack[i].Duration == 0)
                    {
                        _lastSrcRemoves.Add((BuffStack[i].SeedSrc, BuffStack[i].IsExtension));
                    }
                }
                BuffStack.RemoveAll(x => x.Duration == 0);
                Update(leftOver);
            }
        }
    }
}
