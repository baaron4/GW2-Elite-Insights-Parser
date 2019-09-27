using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;

namespace LuckParser.EIData
{
    public class BuffSimulatorIntensity : BuffSimulator
    {
        private readonly List<(AgentItem agent, bool extension)> _lastSrcRemoves = new List<(AgentItem agent, bool extension)>();
        // Constructor
        public BuffSimulatorIntensity(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long time)
        {
            if ((BuffStack.Count > 0 && oldValue > 0) || BuffStack.Count == Capacity)
            {
                BuffStackItem minItem = BuffStack.MinBy(x => Math.Abs(x.TotalBoonDuration() - oldValue));
                if (minItem != null)
                {
                    minItem.Extend(extension, src);
                }
            }
            else
            {
                if (_lastSrcRemoves.Count > 0)
                {
                    Add(oldValue + extension, src, _lastSrcRemoves.First().agent, time, false, _lastSrcRemoves.First().extension);
                    _lastSrcRemoves.RemoveAt(0);
                }
                else
                {
                    Add(oldValue + extension, src, time);
                }
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0)
            {
                if (timePassed > 0)
                {
                    _lastSrcRemoves.Clear();
                }
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
                if (leftOver > 0)
                {
                    Update(leftOver);
                }
            }
        }
    }
}
