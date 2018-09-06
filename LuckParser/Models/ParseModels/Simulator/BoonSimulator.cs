using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {
 
        public class BoonStackItem
        {
            public readonly long Start;
            public readonly long BoonDuration;
            public long InitialBoonDuration
            {
                get
                {
                    return BoonDuration + _overstack;
                }
            }
            public readonly ushort Src;
            private readonly long _overstack;

            public BoonStackItem(long start, long boonDuration, ushort srcinstid, long overstack)
            {
                Start = start;
                BoonDuration = boonDuration;
                Src = srcinstid;
                _overstack = overstack;
            }

            public BoonStackItem(BoonStackItem other, long startShift, long durationShift)
            {
                Start = Math.Max(other.Start + startShift, 0);
                BoonDuration = other.BoonDuration - durationShift;
                _overstack = other._overstack;
                Src = other.Src;
            }
        }

        // Fields
        protected readonly List<BoonStackItem> BoonStack;
        protected readonly List<BoonSimulationItem> GenerationSimulation = new List<BoonSimulationItem>();
        public GenerationSimulationResult GenerationSimulationResult
        {
            get
            {
                return new GenerationSimulationResult(GenerationSimulation);
            }
        }
        public readonly List<BoonSimulationOverstackItem> OverstackSimulation = new List<BoonSimulationOverstackItem>();
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
        

        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fightDuration">Duration of the fight</param>
        public void Trim(long fightDuration)
        {
            for (int i = GenerationSimulation.Count - 1; i >= 0; i--)
            {
                BoonSimulationItem data = GenerationSimulation[i];
                if (data.End > fightDuration)
                {
                    data.SetEnd(fightDuration);
                }
                else
                {
                    break;
                }
            }
            GenerationSimulation.RemoveAll(x => x.GetTotalDuration() <= 0);
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
            GenerationSimulation.RemoveAll(x => x.GetTotalDuration() <= 0);
            BoonStack.Clear();
        }

        protected abstract void Update(long timePassed);
        
        private void Add(long boonDuration, ushort srcinstid, long start, long overstack)
        {
            var toAdd = new BoonStackItem(start, boonDuration, srcinstid, overstack);
            if (overstack > 0)
            {
                OverstackSimulation.Add(new BoonSimulationOverstackItem(srcinstid,overstack,start + boonDuration));
            }
            // Find empty slot
            if (BoonStack.Count < _capacity)
            {
                BoonStack.Add(toAdd);
                _logic.Sort(_log, BoonStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BoonStack, OverstackSimulation);
                if (!found)
                {
                    long overstackValue = boonDuration;
                    ushort srcValue = srcinstid;
                    OverstackSimulation.Add(new BoonSimulationOverstackItem(srcinstid, boonDuration,start));                 
                }
            }
        }
    }
}
