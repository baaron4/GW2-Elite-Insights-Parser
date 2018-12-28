using System;
using System.Collections.Generic;
using LuckParser.Models.DataModels;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class HealingLogic : QueueLogic
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
    }
}
