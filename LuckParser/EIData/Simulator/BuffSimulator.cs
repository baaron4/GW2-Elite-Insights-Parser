using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{
    public abstract class BuffSimulator
    {

        public class BuffStackItem
        {
            public long Start { get; private set; }
            public long Duration { get; private set; }
            public AgentItem Src { get; private set; }
            public AgentItem SeedSrc { get; }
            public bool IsExtension { get; private set; }

            public List<(AgentItem src, long value)> Extensions { get; } = new List<(AgentItem src, long value)>();

            public BuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, bool isExtension)
            {
                Start = start;
                SeedSrc = seedSrc;
                Duration = boonDuration;
                Src = src;
                IsExtension = isExtension;
            }

            public BuffStackItem(long start, long boonDuration, AgentItem src)
            {
                Start = start;
                SeedSrc = src;
                Duration = boonDuration;
                Src = src;
                IsExtension = false;
            }

            public void Shift(long startShift, long durationShift)
            {
                Start += startShift;
                Duration -= durationShift;
                if (Duration == 0 && Extensions.Count > 0)
                {
                    (AgentItem src, long value) = Extensions.First();
                    Extensions.RemoveAt(0);
                    Src = src;
                    Duration = value;
                    IsExtension = true;
                }
            }

            public long TotalBoonDuration()
            {
                long res = Duration;
                foreach ((AgentItem src, long value) in Extensions)
                {
                    res += value;
                }
                return res;
            }

            public void Extend(long value, AgentItem src)
            {
                Extensions.Add((src, value));
            }
        }

        // Fields
        protected List<BuffStackItem> BuffStack { get; }
        public List<BuffSimulationItem> GenerationSimulation { get; } = new List<BuffSimulationItem>();
        public List<BuffSimulationItemOverstack> OverstackSimulationResult { get; } = new List<BuffSimulationItemOverstack>();
        public List<BuffSimulationItemWasted> WasteSimulationResult { get; } = new List<BuffSimulationItemWasted>();
        protected int Capacity { get; }
        private readonly ParsedLog _log;
        private readonly StackingLogic _logic;

        // Constructor
        protected BuffSimulator(int capacity, ParsedLog log, StackingLogic logic)
        {
            Capacity = capacity;
            BuffStack = new List<BuffStackItem>(capacity);
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
                BuffSimulationItem data = GenerationSimulation[i];
                if (data.End > fightDuration)
                {
                    data.OverrideEnd(fightDuration);
                }
                else
                {
                    break;
                }
            }
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
        }

        public void Simulate(List<AbstractBuffEvent> logs, long fightDuration)
        {
            long firstTimeValue = logs.Count > 0 ? Math.Min(logs.First().Time, 0) : 0;
            long timeCur = firstTimeValue;
            long timePrev = firstTimeValue;
            foreach (AbstractBuffEvent log in logs)
            {
                timeCur = log.Time;
                if (timeCur - timePrev < 0)
                {
                    throw new InvalidOperationException("Negative passed time in boon simulation");
                }
                Update(timeCur - timePrev);
                log.UpdateSimulator(this);
                timePrev = timeCur;
            }
            Update(fightDuration - timePrev);
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
            BuffStack.Clear();
        }

        protected abstract void Update(long timePassed);

        public void Add(long duration, AgentItem src, long start)
        {
            var toAdd = new BuffStackItem(start, duration, src);
            // Find empty slot
            if (BuffStack.Count < Capacity)
            {
                BuffStack.Add(toAdd);
                _logic.Sort(_log, BuffStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BuffStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, start));
                }
            }
        }

        protected void Add(long duration, AgentItem src, AgentItem seedSrc, long time, bool atFirst, bool isExtension)
        {
            var toAdd = new BuffStackItem(time, duration, src, seedSrc, isExtension);
            // Find empty slot
            if (BuffStack.Count < Capacity)
            {
                if (atFirst)
                {
                    BuffStack.Insert(0, toAdd);
                }
                else
                {

                    BuffStack.Add(toAdd);
                }
                _logic.Sort(_log, BuffStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BuffStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, time));
                }
            }
        }

        public void Remove(long duration, long time, ParseEnum.BuffRemove removeType)
        {
            switch (removeType)
            {
                case ParseEnum.BuffRemove.All:
                    foreach (BuffStackItem stackItem in BuffStack)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                        if (stackItem.Extensions.Count > 0)
                        {
                            foreach ((AgentItem src, long value) in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                            }
                        }
                    }
                    BuffStack.Clear();
                    break;
                case ParseEnum.BuffRemove.Single:
                    for (int i = 0; i < BuffStack.Count; i++)
                    {
                        BuffStackItem stackItem = BuffStack[i];
                        if (Math.Abs(duration - stackItem.TotalBoonDuration()) < 10)
                        {
                            WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                            if (stackItem.Extensions.Count > 0)
                            {
                                foreach ((AgentItem src, long value) in stackItem.Extensions)
                                {
                                    WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                                }
                            }
                            BuffStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            _logic.Sort(_log, BuffStack);
        }

        public abstract void Extend(long extension, long oldValue, AgentItem src, long time);
    }
}
