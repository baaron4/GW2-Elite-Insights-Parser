using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulatorDuration : BuffSimulator
    {
        private (AgentItem agent, bool extension) _lastSrcRemove = (ParserHelper._unknownAgent, false);
        // Constructor
        public BuffSimulatorDuration(ParsedEvtcLog log, StackingLogic logic, Buff buff) : base(log, logic, buff)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start, uint stackID)
        {
            if ((BuffStack.Any() && oldValue > 0) || Logic.IsFull(BuffStack))
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
            if (BuffStack.Any() && timePassed > 0)
            {
                BuffStackItem activeStack = BuffStack[0];
                _lastSrcRemove = (ParserHelper._unknownAgent, false);
                var toAdd = new BuffSimulationItemDuration(activeStack);
                GenerationSimulation.Add(toAdd);
                long timeDiff = activeStack.Duration - timePassed;
                long diff = timePassed;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = activeStack.Duration;
                    leftOver = timePassed - diff;
                }
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                activeStack.Shift(0, diff);
                foreach (BuffStackItem buffStackItem in BuffStack)
                {
                    buffStackItem.Shift(diff, 0);
                }
                if (activeStack.Duration == 0)
                {
                    _lastSrcRemove = (activeStack.SeedSrc, activeStack.IsExtension);
                    BuffStack.RemoveAt(0);
                }
                Update(leftOver);
            }
        }
    }
}
