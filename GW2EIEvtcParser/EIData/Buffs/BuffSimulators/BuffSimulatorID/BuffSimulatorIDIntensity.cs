using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulatorIDIntensity : BuffSimulatorID
    {
        private readonly int _capacity;
        // Constructor
        public BuffSimulatorIDIntensity(ParsedEvtcLog log, Buff buff, int capacity) : base(log, buff)
        {
            _capacity = capacity;
        }

        public override void Activate(uint stackID)
        {
            BuffStackItemID active = BuffStack.FirstOrDefault(x => x.StackID == stackID) ?? throw new EIBuffSimulatorIDException("Activate has failed");
            active.Activate();
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, long overridenDuration, uint overridenStackID)
        {
            BuffStack.Add(new BuffStackItemID(start, duration, src, addedActive, stackID));
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count != 0 && timePassed > 0)
            {
                long diff = timePassed;
                long leftOver = 0;
                var activeStacks = BuffStack.Where(x => x.Active && x.Duration > 0).ToList();
                if (activeStacks.Count > _capacity)
                {
                    activeStacks = activeStacks.Take(_capacity).ToList();
                }
                if (activeStacks.Count != 0)
                {
                    var toAdd = new BuffSimulationItemIntensity(activeStacks);
                    GenerationSimulation.Add(toAdd);
                    long currentDur = activeStacks.Min(x => x.Duration);
                    long timeDiff = currentDur - timePassed;
                    if (timeDiff < 0)
                    {
                        diff = currentDur;
                        leftOver = timePassed - diff;
                    }
                    if (toAdd.End > toAdd.Start + diff)
                    {
                        toAdd.OverrideEnd(toAdd.Start + diff);
                    }
                    foreach (BuffStackItemID buffStackItem in activeStacks)
                    {
                        buffStackItem.Shift(0, diff);
                    }
                }
                foreach (BuffStackItemID buffStackItem in BuffStack)
                {
                    buffStackItem.Shift(diff, 0);
                }
                Update(leftOver);
            }
        }
    }
}

