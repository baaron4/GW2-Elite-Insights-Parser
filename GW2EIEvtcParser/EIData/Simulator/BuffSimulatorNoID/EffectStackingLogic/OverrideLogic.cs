using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    public class OverrideLogic : StackingLogic
    {
        public override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            stacks.Sort((x, y) => x.TotalBoonDuration().CompareTo(y.TotalBoonDuration()));
        }

        public override bool StackEffect(ParsedEvtcLog log, BuffStackItem stackItem, List<BuffStackItem> stacks, List<BuffSimulationItemWasted> wastes)
        {
            if (stacks.Count == 0)
            {
                return false;
            }
            BuffStackItem stack = stacks[0];
            if (stack.TotalBoonDuration() <= stackItem.TotalBoonDuration() + GeneralHelper.ServerDelayConstant)
            {
                wastes.Add(new BuffSimulationItemWasted(stack.Src, stack.Duration, stack.Start));
                if (stack.Extensions.Count > 0)
                {
                    foreach ((AgentItem src, long value) in stack.Extensions)
                    {
                        wastes.Add(new BuffSimulationItemWasted(src, value, stack.Start));
                    }
                }
                stacks[0] = stackItem;
                Sort(log, stacks);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
