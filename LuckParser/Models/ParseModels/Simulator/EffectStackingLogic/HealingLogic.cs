using System;
using System.Collections.Generic;
using LuckParser.Models.DataModels;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class HealingLogic : QueueLogic
    {

        private struct CompareHealing
        {
            private readonly ParsedLog _log;

            public CompareHealing(ParsedLog log)
            {
                _log = log;
            }

            private uint GetHealing(BoonStackItem stack)
            {
                AgentItem agent = _log.AgentData.GetAgentByInstID(stack.SeedSrc, _log.FightData.ToLogSpace(stack.SeedTime));
                return agent.Healing;
            }

            public int Compare(BoonStackItem x, BoonStackItem y)
            {             
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        public override void Sort(ParsedLog log, List<BoonStackItem> stacks)
        {
            CompareHealing comparator = new CompareHealing(log);
            stacks.Sort(comparator.Compare);        
        }
    }
}
