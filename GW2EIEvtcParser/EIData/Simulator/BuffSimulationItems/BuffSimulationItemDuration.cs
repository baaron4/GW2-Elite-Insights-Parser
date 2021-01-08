using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSimulationItemDuration : BuffSimulationItem
    {
        private readonly AgentItem _src;
        private readonly AgentItem _seedSrc;
        private readonly bool _isExtension;

        public BuffSimulationItemDuration(BuffStackItem other) : base(other.Start, other.Duration)
        {
            _src = other.Src;
            _seedSrc = other.SeedSrc;
            _isExtension = other.IsExtension;
        }

        public override void OverrideEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetStack()
        {
            return 1;
        }

        public override List<AgentItem> GetSources()
        {
            return new List<AgentItem>() { _src };
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
        {
            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            AgentItem agent = _src;
            AgentItem seedAgent = _seedSrc;
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.IncrementValue(cDur);
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }
            if (_isExtension)
            {
                if (distrib.TryGetValue(agent, out toModify))
                {
                    toModify.IncrementExtension(cDur);
                }
                else
                {
                    distrib.Add(agent, new BuffDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
            }
            if (agent != seedAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.IncrementExtended(cDur);
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }
            if (agent == ParserHelper._unknownAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.IncrementUnknownExtension(cDur);
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
