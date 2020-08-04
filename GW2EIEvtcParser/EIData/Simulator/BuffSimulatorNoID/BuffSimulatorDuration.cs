using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class BuffSimulatorDuration : BuffSimulator
    {
        private (AgentItem agent, bool extension) _lastSrcRemove = (ParseHelper._unknownAgent, false);
        // Constructor
        public BuffSimulatorDuration(int capacity, ParsedEvtcLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start, uint stackID)
        {
            if ((BuffStack.Count > 0 && oldValue > 0) || BuffStack.Count == Capacity)
            {
                BuffStack[0].Extend(extension, src);
            }
            else
            {
                Add(oldValue + extension, src, _lastSrcRemove.agent, start, true, _lastSrcRemove.extension);
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemove = (ParseHelper._unknownAgent, false);
                var toAdd = new BuffSimulationItemDuration(BuffStack[0]);
                GenerationSimulation.Add(toAdd);
                long timeDiff = BuffStack[0].Duration - timePassed;
                long diff;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = BuffStack[0].Duration;
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
                BuffStack[0].Shift(diff, diff);
                for (int i = 1; i < BuffStack.Count; i++)
                {
                    BuffStack[i].Shift(diff, 0);
                }
                if (BuffStack[0].Duration == 0)
                {
                    _lastSrcRemove = (BuffStack[0].SeedSrc, BuffStack[0].IsExtension);
                    BuffStack.RemoveAt(0);
                }
                Update(leftOver);
            }
        }
    }
}
