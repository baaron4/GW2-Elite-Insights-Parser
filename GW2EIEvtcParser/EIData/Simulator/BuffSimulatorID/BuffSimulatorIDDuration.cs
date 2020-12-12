using System;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulatorIDDuration : BuffSimulatorID
    {
        private BuffStackItem _activeStack;

        // Constructor
        public BuffSimulatorIDDuration(ParsedEvtcLog log) : base(log)
        {
        }

        public override void Activate(uint stackID)
        {
            BuffStackItem active = BuffStack.Find(x => x.StackID == stackID);
            _activeStack = active ?? throw new InvalidOperationException("Activate has failed");
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItem(start, duration, src, stackID);
            BuffStack.Add(toAdd);
            //AddedSimulationResult.Add(new BuffCreationItem(src, duration, start, toAdd.ID));
            if (overstackDuration > 0)
            {
                OverrideCandidates.Add((overstackDuration, src));
            }
            if (addedActive)
            {
                _activeStack = toAdd;
            }
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0 && _activeStack != null)
            {
                var toAdd = new BuffSimulationItemDuration(_activeStack);
                GenerationSimulation.Add(toAdd);
                long timeDiff = _activeStack.Duration - timePassed;
                long diff;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = _activeStack.Duration;
                    leftOver = timePassed - diff;
                }
                else
                {
                    diff = timePassed;
                }
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                BuffStackItem oldActive = _activeStack;
                _activeStack.Shift(diff, diff);
                for (int i = 0; i < BuffStack.Count; i++)
                {
                    if (BuffStack[i] != oldActive)
                    {
                        BuffStack[i].Shift(diff, 0);
                    }
                }
                // that means the stack was not an extension, extend duration to match time passed
                if (_activeStack.Duration == 0)
                {
                    _activeStack.Shift(0, -leftOver);
                }
                Update(leftOver);
            }
        }
    }
}

