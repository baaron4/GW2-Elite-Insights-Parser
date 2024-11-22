using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal class HealingLogic : QueueLogic
{
    private bool _noSort = false;
    private struct CompareHealing
    {

        private static uint GetHealing(BuffStackItem stack)
        {
            return stack.SeedSrc.Healing;
        }

        public static int Compare(BuffStackItem x, BuffStackItem y)
        {
            return -GetHealing(x).CompareTo(GetHealing(y));
        }
    }

    protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
    {
        if (_noSort)
        {
            return;
        }
        stacks.Sort(CompareHealing.Compare);
    }

    public override bool FindLowestValue(ParsedEvtcLog log, BuffStackItemPool pool, BuffStackItem toAdd, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes, long overridenDuration, uint overridenStackID)
    {
        if (stacks.Count <= 1)
        {
            throw new InvalidDataException("Queue logic based must have a > 1 capacity");
        }

        BuffStackItem? toRemove = null;
        if (overridenStackID > 0)
        {
            toRemove = stacks.FirstOrDefault(x => x.StackID == overridenStackID);
        }
        if (toRemove == null)
        {
            toRemove = stacks.MinBy(x => Math.Abs(x.TotalDuration - overridenDuration));
        }
        wastes.Add(new BuffSimulationItemWasted(toRemove.Src, toRemove.Duration, toRemove.Start));
        if (toRemove.Extensions.Count != 0)
        {
            foreach ((AgentItem src, long value) in toRemove.Extensions)
            {
                wastes.Add(new BuffSimulationItemWasted(src, value, toRemove.Start));
            }
        }
        pool.ReleaseBuffStackItem(toRemove);
        stacks[stacks.IndexOf(toRemove)] = toAdd;
        Sort(log, stacks);
        return true;
    }

    public override void Activate(List<BuffStackItem> stacks, uint stackID)
    {
        int toActivateIdx = stacks.FindIndex(x => x.StackID == stackID);
        if (toActivateIdx != -1)
        {
            _noSort = true;
            var toActivate = stacks[toActivateIdx];
            stacks.RemoveAt(toActivateIdx);
            if (stacks.Count > 0 && stacks[0].TotalDuration < 50)
            {
                stacks[0] = toActivate;
            }
            else
            {
                stacks.Insert(0, toActivate);
            }
        }
    }
}
