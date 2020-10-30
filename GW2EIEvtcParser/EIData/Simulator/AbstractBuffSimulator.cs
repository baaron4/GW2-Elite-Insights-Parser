using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class AbstractBuffSimulator
    {
        internal class BuffStackItem
        {
            public long Start { get; private set; }
            public long Duration { get; private set; }
            public AgentItem Src { get; private set; }
            public AgentItem SeedSrc { get; }
            public bool IsExtension { get; private set; }

            public long TotalDuration {
                get
                {
                    long res = Duration;
                    foreach ((AgentItem src, long value) in Extensions)
                    {
                        res += value;
                    }
                    return res;
                }
            }

            public long StackID { get; protected set; } = 0;

            public List<(AgentItem src, long value)> Extensions { get; } = new List<(AgentItem src, long value)>();

            public BuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, long stackID, bool isExtension)
            {
                Start = start;
                SeedSrc = seedSrc;
                Duration = boonDuration;
                Src = src;
                IsExtension = isExtension;
                StackID = stackID;
            }

            public BuffStackItem(long start, long boonDuration, AgentItem src, long stackID)
            {
                StackID = stackID;
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

            public void Extend(long value, AgentItem src)
            {
                Extensions.Add((src, value));
            }
        }
        // Fields
        protected List<BuffStackItem> BuffStack { get; set; }
        public List<BuffSimulationItem> GenerationSimulation { get; } = new List<BuffSimulationItem>();
        public List<BuffSimulationItemOverstack> OverstackSimulationResult { get; } = new List<BuffSimulationItemOverstack>();
        public List<BuffSimulationItemWasted> WasteSimulationResult { get; } = new List<BuffSimulationItemWasted>();

        protected ParsedEvtcLog Log { get; }

        // Constructor
        protected AbstractBuffSimulator(ParsedEvtcLog log)
        {
            BuffStack = new List<BuffStackItem>();
            Log = log;
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

        public abstract void Add(long duration, AgentItem src, long time, uint stackID, bool addedActive, uint overstackDuration);

        public abstract void Remove(AgentItem by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID);

        public abstract void Extend(long extension, long oldValue, AgentItem src, long time, uint stackID);

        public abstract void Activate(uint stackID);
        public abstract void Reset(uint stackID, long toDuration);
    }
}

