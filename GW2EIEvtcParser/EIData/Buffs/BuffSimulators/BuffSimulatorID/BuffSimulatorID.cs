using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class BuffSimulatorID(ParsedEvtcLog log, Buff buff) : AbstractBuffSimulator(log, buff)
{
    protected class BuffStackItemID : BuffStackItem
    {

        public bool Active { get; protected set; } = false;

        public BuffStackItemID(long start, long boonDuration, AgentItem src, bool active, long stackID) : base(start, boonDuration, src, stackID)
        {
            Active = active;
        }

        public void Activate()
        {
            Active = true;
        }

        public void Disable()
        {
            Active = false;
        }

        public override void Shift(long startShift, long durationShift)
        {
            if (!Active)
            {
                base.Shift(startShift, 0);
                return;
            }
            base.Shift(startShift, durationShift);
        }
    }

    protected readonly List<BuffStackItemID> BuffStack = [];

    protected override void AfterSimulate()
    {
        BuffStack.Clear();
    }

    protected override void UpdateSimulator(AbstractBuffEvent buffEvent)
    {
        buffEvent.UpdateSimulator(this, false);
    }

    public override void Extend(long extension, long oldValue, AgentItem src, long time, uint stackID)
    {
        BuffStackItem toExtend = BuffStack.FirstOrDefault(x => x.StackID == stackID) ?? throw new EIBuffSimulatorIDException($"Extend has failed: Could not find stack id {stackID}.");
        toExtend.Extend(extension, src);
    }

    public override void Remove(AgentItem by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID)
    {
        int toRemoveIdx = -1;
        switch (removeType)
        {
            case ArcDPSEnums.BuffRemove.All:
                // remove all due to despawn event
                if (removedStacks == BuffRemoveAllEvent.FullRemoval)
                {
                    BuffStack.Clear();
                    return;
                }
                if (BuffStack.Count != 1)
                {
                    if (BuffStack.Count < removedStacks)
                    {
                        throw new EIBuffSimulatorIDException("Remove all failed");
                    }
                    // buff cleanse all
                    for (int i = 0; i < BuffStack.Count; i++)
                    {
                        BuffStackItem stackItem = BuffStack[i];
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                        if (stackItem.Extensions.Count != 0)
                        {
                            foreach ((AgentItem src, long value) in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                            }
                        }
                    }
                    BuffStack.Clear();
                    return;
                }
                toRemoveIdx = 0;
                break;
            case ArcDPSEnums.BuffRemove.Single:
                toRemoveIdx = BuffStack.FindIndex(x => x.StackID == stackID);
                break;
            default:
                throw new InvalidDataException("Unknown remove type");
        }
        if (toRemoveIdx == -1)
        {
            throw new EIBuffSimulatorIDException("Remove has failed");
        }
        var toRemove = BuffStack[toRemoveIdx];
        BuffStack.RemoveAt(toRemoveIdx);

        if (removedDuration > ParserHelper.BuffSimulatorDelayConstant)
        {
            // safe checking, this can happen when an inactive stack is being removed but it was actually active
            if (Math.Abs(removedDuration - toRemove.TotalDuration) > ParserHelper.BuffSimulatorDelayConstant && !toRemove.Active)
            {
                toRemove.Activate();
                toRemove.Shift(0, Math.Abs(removedDuration - toRemove.TotalDuration));
            }
            // Removed due to override
            if (by == ParserHelper._unknownAgent)
            {
                WasteSimulationResult.Add(new BuffSimulationItemWasted(toRemove.Src, toRemove.Duration, time));
                if (toRemove.Extensions.Count != 0)
                {
                    foreach ((AgentItem src, long value) in toRemove.Extensions)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                    }
                }
            }
            // Removed due to a cleanse
            else
            {
                WasteSimulationResult.Add(new BuffSimulationItemWasted(toRemove.Src, toRemove.Duration, time));
                if (toRemove.Extensions.Count != 0)
                {
                    foreach ((AgentItem src, long value) in toRemove.Extensions)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                    }
                }
            }
        }
    }

    public override void Reset(uint stackID, long toDuration)
    {
        BuffStackItemID toDisable = BuffStack.FirstOrDefault(x => x.StackID == stackID) ?? throw new EIBuffSimulatorIDException("Reset has failed");
        toDisable.Disable();
    }

}

