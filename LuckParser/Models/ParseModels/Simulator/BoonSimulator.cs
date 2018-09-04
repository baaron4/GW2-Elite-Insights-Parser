using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {
 
        public struct BoonStackItem
        {
            public readonly long Start;
            public readonly long BoonDuration;
            public readonly ushort Src;
            public readonly long Overstack;

            public BoonStackItem(long start, long boonDuration, ushort srcinstid, long overstack)
            {
                Start = start;
                BoonDuration = boonDuration;
                Src = srcinstid;
                Overstack = overstack;
            }

            public BoonStackItem(BoonStackItem other, long startShift, long durationShift)
            {
                Start = Math.Max(other.Start + startShift, 0);
                BoonDuration = other.BoonDuration - durationShift;
                // if duration shift > 0 this means the boon ticked, aka already in simulation, we remove the overstack
                Overstack = durationShift > 0 ? 0 : other.Overstack;
                Src = other.Src;
            }
        }

        // Fields
        protected readonly List<BoonStackItem> BoonStack;
        protected readonly List<BoonSimulationItem> Simulation = new List<BoonSimulationItem>();
        private readonly int _capacity;
        private readonly ParsedLog _log;
        private readonly StackingLogic _logic;

        // Constructor
        protected BoonSimulator(int capacity, ParsedLog log, StackingLogic logic)
        {
            _capacity = capacity;
            BoonStack = new List<BoonStackItem>(capacity);
            _log = log;
            _logic = logic;
        }  

        public BoonSimulationResult GetSimulationResult()
        {
            return new BoonSimulationResult(Simulation);
        }

        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fightDuration">Duration of the fight</param>
        public void Trim(long fightDuration)
        {
            for (int i = Simulation.Count - 1; i >= 0; i--)
            {
                BoonSimulationItem data = Simulation[i];
                if (data.End > fightDuration)
                {
                    data.SetEnd(fightDuration);
                }
                else
                {
                    break;
                }
            }
            Simulation.RemoveAll(x => x.GetItemDuration() <= 0);
        }

        public void Simulate(List<BoonLog> logs, long fightDuration)
        {
            long timeCur = 0;
            long timePrev = 0;
            foreach (BoonLog log in logs)
            {
                timeCur = log.GetTime();
                Update(timeCur - timePrev);
                Add(log.GetValue(), log.GetSrcInstid(), timeCur, log.GetOverstack());
                timePrev = timeCur;
            }
            Update(fightDuration - timePrev);
            Simulation.RemoveAll(x => x.GetItemDuration() <= 0);
            BoonStack.Clear();
        }

        protected abstract void Update(long timePassed);
        
        private void Add(long boonDuration, ushort srcinstid, long start, long overstack)
        {
            var toAdd = new BoonStackItem(start, boonDuration, srcinstid, overstack);
            // Find empty slot
            if (BoonStack.Count < _capacity)
            {
                BoonStack.Add(toAdd);
                _logic.Sort(_log, BoonStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BoonStack, Simulation);
                if (!found)
                {
                    long overstackValue = overstack + boonDuration;
                    ushort srcValue = srcinstid;
                    if (Simulation.Count == 0)
                    {
                        Simulation.Add(new BoonSimulationOverstackItem(new BoonStackItem(start, 1, srcValue, overstackValue)));
                    }
                    else
                    {
                        Simulation.Insert(Simulation.Count - 1, new BoonSimulationOverstackItem(new BoonStackItem(start, 1, srcValue, overstackValue)));
                    }
                }
            }
        }
    }
}
