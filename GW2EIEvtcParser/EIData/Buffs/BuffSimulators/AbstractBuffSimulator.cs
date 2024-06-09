using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class AbstractBuffSimulator
    {
        // Fields
        public List<BuffSimulationItem> GenerationSimulation { get; } = new List<BuffSimulationItem>();
        public List<BuffSimulationItemOverstack> OverstackSimulationResult { get; } = new List<BuffSimulationItemOverstack>();
        public List<BuffSimulationItemWasted> WasteSimulationResult { get; } = new List<BuffSimulationItemWasted>();

        public Buff Buff { get; }

        protected ParsedEvtcLog Log { get; }

        // Constructor
        protected AbstractBuffSimulator(ParsedEvtcLog log, Buff buff)
        {
            Buff = buff;
            Log = log;
        }


        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fightDuration">Duration of the fight</param>
        private void Trim(long fightDuration)
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

        protected abstract void UpdateSimulator(AbstractBuffEvent buffEvent);

        public void Simulate(List<AbstractBuffEvent> buffEvents, long fightStart, long fightEnd)
        {
            if (GenerationSimulation.Count != 0)
            {
                return;
            }
            long firstTimeValue = buffEvents.Count > 0 ? Math.Min(buffEvents.First().Time, fightStart) : fightStart;
            long timeCur = firstTimeValue;
            long timePrev = firstTimeValue;
            foreach (AbstractBuffEvent buffEvent in buffEvents)
            {
                timeCur = buffEvent.Time;
                if (timeCur - timePrev < 0)
                {
                    throw new InvalidOperationException("Negative passed time in boon simulation");
                }
                Update(timeCur - timePrev);
                UpdateSimulator(buffEvent);
                timePrev = timeCur;
            }
            Update(fightEnd - timePrev);
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
            Clear();
            Trim(fightEnd);
        }

        protected abstract void Clear();

        protected abstract void Update(long timePassed);

        public abstract void Add(long duration, AgentItem src, long time, uint stackID, bool addedActive, long overridenDuration, uint overridenStackID);

        public abstract void Remove(AgentItem by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID);

        public abstract void Extend(long extension, long oldValue, AgentItem src, long time, uint stackID);

        public abstract void Activate(uint stackID);
        public abstract void Reset(uint stackID, long toDuration);
    }
}

