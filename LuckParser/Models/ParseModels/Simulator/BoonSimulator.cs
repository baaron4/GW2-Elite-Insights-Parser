using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulator
    {

        public class BoonStackItem
        {
            public long Start { get; private set; }
            public long SeedTime { get; private set; }
            public long ApplicationTime { get; private set; }
            public long BoonDuration { get; private set; }
            public ushort Src { get; private set; }
            public ushort SeedSrc { get; }

            public List<Tuple<ushort, long, long>> Extensions { get; } = new List<Tuple<ushort, long, long>>();

            public BoonStackItem(long start, long boonDuration, ushort srcinstid, ushort originalSrc)
            {
                Start = start;
                SeedTime = start;
                ApplicationTime = start;
                SeedSrc = originalSrc;
                BoonDuration = boonDuration;
                Src = srcinstid;
            }

            public BoonStackItem(BoonStackItem other, long startShift, long durationShift)
            {
                Start = other.Start + startShift;
                SeedTime = other.SeedTime;
                ApplicationTime = other.ApplicationTime;
                BoonDuration = other.BoonDuration - durationShift;
                Src = other.Src;
                SeedSrc = other.SeedSrc;
                Extensions = other.Extensions;
                if (BoonDuration == 0 && Extensions.Count > 0)
                {
                    Tuple<ushort, long, long> ext = Extensions.First();
                    Extensions.RemoveAt(0);
                    ApplicationTime = ext.Item3;
                    Src = ext.Item1;
                    BoonDuration = ext.Item2;
                }
            }

            public void Extend(long value, ushort src, long time)
            {
                Extensions.Add(new Tuple<ushort, long, long>(src, value, time));
            }
        }

        // Fields
        protected readonly List<BoonStackItem> BoonStack;
        protected readonly List<BoonSimulationItem> GenerationSimulation = new List<BoonSimulationItem>();
        public GenerationSimulationResult GenerationSimulationResult => new GenerationSimulationResult(GenerationSimulation);
        public readonly List<BoonSimulationItemOverstack> OverstackSimulationResult = new List<BoonSimulationItemOverstack>();
        public readonly List<BoonSimulationItemWasted> WasteSimulationResult = new List<BoonSimulationItemWasted>();
        public readonly List<BoonSimulationItemCleanse> CleanseSimulationResult = new List<BoonSimulationItemCleanse>();
        protected readonly int Capacity;
        private readonly ParsedLog _log;
        private readonly StackingLogic _logic;

        // Constructor
        protected BoonSimulator(int capacity, ParsedLog log, StackingLogic logic)
        {
            Capacity = capacity;
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
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
        }

        public void Simulate(List<BoonLog> logs, long fightDuration)
        {
            long firstTimeValue = logs.Count > 0 ? Math.Min(logs.First().Time, 0) : 0;
            long timeCur = firstTimeValue;
            long timePrev = firstTimeValue;
            foreach (BoonLog log in logs)
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
            BoonStack.Clear();
        }

        protected abstract void Update(long timePassed);

        public void Add(long boonDuration, ushort srcinstid, ushort originalSrc, long start, bool atFirst = false)
        {
            var toAdd = new BoonStackItem(start, boonDuration, srcinstid, originalSrc);
            // Find empty slot
            if (BoonStack.Count < Capacity)
            {
                if (atFirst)
                {
                    BoonStack.Insert(0, toAdd);
                }
                else
                {

                    BoonStack.Add(toAdd);
                }
                _logic.Sort(_log, BoonStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BoonStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BoonSimulationItemOverstack(srcinstid, boonDuration, start));
                }
            }
        }

        public void Remove(ushort provokedBy, long boonDuration, long start, ParseEnum.BuffRemove removeType)
        {
            if (GenerationSimulation.Count > 0)
            {
                BoonSimulationItem last = GenerationSimulation.Last();
                if (last.End > start)
                {
                    last.SetEnd(start);
                }
            }
            switch (removeType)
            {
                case ParseEnum.BuffRemove.All:
                    foreach (BoonStackItem stackItem in BoonStack)
                    {
                        WasteSimulationResult.Add(new BoonSimulationItemWasted(stackItem.Src, stackItem.BoonDuration, start, stackItem.ApplicationTime));
                        CleanseSimulationResult.Add(new BoonSimulationItemCleanse(provokedBy, stackItem.BoonDuration, start));
                        if (stackItem.Extensions.Count > 0)
                        {
                            foreach (var item in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BoonSimulationItemWasted(item.Item1, item.Item2, start, item.Item3));
                                CleanseSimulationResult.Add(new BoonSimulationItemCleanse(provokedBy, item.Item2, start));
                            }
                        }
                    }
                    BoonStack.Clear();
                    break;
                case ParseEnum.BuffRemove.Single:
                case ParseEnum.BuffRemove.Manual:
                    for (int i = 0; i < BoonStack.Count; i++)
                    {
                        BoonStackItem stackItem = BoonStack[i];
                        if (Math.Abs(boonDuration - stackItem.BoonDuration) < 10)
                        {
                            WasteSimulationResult.Add(new BoonSimulationItemWasted(stackItem.Src, stackItem.BoonDuration, start, stackItem.ApplicationTime));
                            CleanseSimulationResult.Add(new BoonSimulationItemCleanse(provokedBy, stackItem.BoonDuration, start));
                            if (stackItem.Extensions.Count > 0)
                            {
                                foreach (var item in stackItem.Extensions)
                                {
                                    WasteSimulationResult.Add(new BoonSimulationItemWasted(item.Item1, item.Item2, start, item.Item3));
                                    CleanseSimulationResult.Add(new BoonSimulationItemCleanse(provokedBy, item.Item2, start));
                                }
                            }
                            BoonStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            _logic.Sort(_log, BoonStack);
        }

        public abstract void Extend(long extension, long oldValue, ushort src, long start);
    }
}
