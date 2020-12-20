using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulatorIDIntensity : BuffSimulatorID
    {
        // Constructor
        public BuffSimulatorIDIntensity(ParsedEvtcLog log, Buff buff) : base(log, buff)
        {
        }

        public override void Activate(uint stackID)
        {
            BuffStackItemID active = BuffStack.FirstOrDefault(x => x.StackID == stackID);
            if (active == null)
            {
                throw new InvalidOperationException("Activate has failed");
            }
            active.Activate();
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
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Any() && timePassed > 0)
            {
                List<BuffStackItemID> BuffStackToUse = BuffStack;
                long diff = Math.Min(BuffStackToUse.Min(x => x.Duration), timePassed);
                if (diff == 0)
                {
                    BuffStackToUse = BuffStack.Where(x => x.Duration > 0).ToList();
                    if (!BuffStackToUse.Any())
                    {
                        return;
                    }
                    diff = Math.Min(BuffStackToUse.Min(x => x.Duration), timePassed);
                }
                var toAdd = new BuffSimulationItemIntensity(BuffStackToUse);
                GenerationSimulation.Add(toAdd);
                long leftOver = timePassed - diff;
                if (toAdd.End > toAdd.Start + diff)
                {
                    toAdd.OverrideEnd(toAdd.Start + diff);
                }
                // Subtract from each
                foreach (BuffStackItemID buffStackItem in BuffStackToUse)
                {
                    buffStackItem.Shift(diff, diff);
                }
                Update(leftOver);
            }
        }
    }
}

