using System;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulatorIDDuration : BuffSimulatorID
    {
        private BuffStackItemID _activeStack;

        // Constructor
        public BuffSimulatorIDDuration(ParsedEvtcLog log, Buff buff) : base(log, buff)
        {
        }

        public override void Activate(uint stackID)
        {
            _activeStack = BuffStack.FirstOrDefault(x => x.StackID == stackID);
            if (_activeStack == null)
            {
                throw new EIBuffSimulatorIDException("Activate has failed");
            }
            _activeStack.Activate();
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItemID(start, duration, src, addedActive, stackID);
            BuffStack.Add(toAdd);
            //AddedSimulationResult.Add(new BuffCreationItem(src, duration, start, toAdd.ID));
            /*if (overstackDuration > 0)
            {
                OverrideCandidates.Add((overstackDuration, src));
            }*/
            if (addedActive)
            {
                _activeStack = toAdd;
            }
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Any() && timePassed > 0)
            {
                long diff = timePassed;
                long leftOver = 0;
                if (_activeStack != null && _activeStack.Duration > 0)
                {
                    var toAdd = new BuffSimulationItemDuration(_activeStack);
                    GenerationSimulation.Add(toAdd);
                    long timeDiff = _activeStack.Duration - timePassed;
                    if (timeDiff < 0)
                    {
                        diff = _activeStack.Duration;
                        leftOver = timePassed - diff;
                    }
                    if (toAdd.End > toAdd.Start + diff)
                    {
                        toAdd.OverrideEnd(toAdd.Start + diff);
                    }
                    _activeStack.Shift(0, diff);
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

