using System;
using LuckParser.Models.DataModels;
using System.Collections.Generic;
using static LuckParser.Models.ParseModels.BoonSimulator;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulationItemDuration : BoonSimulationItem
    {

        private readonly long _applicationTime;
        private readonly long _seedTime;
        private readonly ushort _src;
        private readonly ushort _seedSrc;

        public BoonSimulationItemDuration(BoonStackItem other) : base(other.Start, other.BoonDuration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _applicationTime = other.ApplicationTime;
            _seedTime = other.SeedTime;
        }

        public override void SetEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetStack()
        {
            return 1;
        }

        public override void SetBoonDistributionItem(BoonDistribution distribs, long start, long end, long boonid, ParsedLog log)
        {
            Dictionary<AgentItem, BoonDistributionItem> distrib = GetDistrib(distribs, boonid);
            long cDur = GetClampedDuration(start, end);
            AgentItem agent = log.AgentData.GetAgentByInstID(_src, log.FightData.ToLogSpace(_applicationTime));
            AgentItem seedAgent = log.AgentData.GetAgentByInstID(_seedSrc, log.FightData.ToLogSpace(_seedTime));
            if (distrib.TryGetValue(agent, out BoonDistributionItem toModify))
            {
                toModify.Value += cDur;
                distrib[agent] = toModify;
            }
            else
            {
                distrib.Add(agent, new BoonDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }
            if (agent != seedAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.Extension += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BoonDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
                if (distrib.TryGetValue(agent, out toModify))
                {
                    toModify.Extended += cDur;
                    distrib[agent] = toModify;
                }
                else
                {
                    distrib.Add(agent, new BoonDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (_src == 0)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.UnknownExtension += cDur;
                    distrib[seedAgent] = toModify;
                }
                else
                {
                    distrib.Add(seedAgent, new BoonDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
