using System;
using System.Collections.Generic;
using LuckParser.Models.DataModels;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class HealingLogic : StackingLogic
    {

        private static uint GetHealing(BoonStackItem stack, ParsedLog log)
        {
            AgentItem agent = log.AgentData.GetAgentByInstID(stack.SeedSrc, log.FightData.ToLogSpace(stack.SeedTime));
            return agent.Healing;
        }

        private struct CompareHealing
        {
            private readonly ParsedLog _log;

            public CompareHealing(ParsedLog log)
            {
                _log = log;
            }

            public int Compare(BoonStackItem x, BoonStackItem y)
            {             
                return -GetHealing(x, _log).CompareTo(GetHealing(y, _log));
            }
        }

        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            CompareHealing comparator = new CompareHealing(log);
            stacks.Sort(comparator.Compare);        
        }

        public override bool StackEffect(ParsedLog log, BoonStackItem stackItem, List<BoonStackItem> stacks, List<BoonSimulationItemWasted> wastes)
        {
            if (stacks.Count <= 1)
            {
                throw new InvalidOperationException("Queue logic based must have a >1 capacity");
            }
            BoonStackItem minItem = stacks.MinBy(x => GetHealing(x, log));
            if (GetHealing(minItem, log) >= GetHealing(stackItem, log))
            {
                return false;
            }
            wastes.Add(new BoonSimulationItemWasted(minItem.Src, minItem.BoonDuration, minItem.Start, minItem.ApplicationTime));
            if (minItem.Extensions.Count > 0)
            {
                foreach (var item in minItem.Extensions)
                {
                    wastes.Add(new BoonSimulationItemWasted(item.Item1, item.Item2, minItem.Start, item.Item3));
                }
            }
            stacks[stacks.IndexOf(minItem)] = stackItem;
            return true;
        }
    }
}
