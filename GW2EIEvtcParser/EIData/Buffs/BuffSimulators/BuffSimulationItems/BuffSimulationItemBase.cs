using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulationItemBase : BuffSimulationItem
    {
        private readonly AgentItem _src;
        private readonly AgentItem _seedSrc;
        private readonly bool _isExtension;
        private long _originalDuration { get; }

        protected internal BuffSimulationItemBase(BuffStackItem buffStackItem) : base(buffStackItem.Start, buffStackItem.Duration)
        {
            _src = buffStackItem.Src;
            _seedSrc = buffStackItem.SeedSrc;
            _isExtension = buffStackItem.IsExtension;
            _originalDuration = buffStackItem.Duration;
        }

        public override void OverrideEnd(long end)
        {
            Duration = Math.Min(Math.Max(end - Start, 0), Duration);
        }

        public override int GetActiveStacks()
        {
            return 1;
        }

        public override int GetStacks()
        {
            return 1;
        }

        public override IReadOnlyList<long> GetActualDurationPerStack()
        {
            return new List<long>() { _originalDuration };
        }

        public override List<AgentItem> GetSources()
        {
            return new List<AgentItem>() { _src };
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
        {
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
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
