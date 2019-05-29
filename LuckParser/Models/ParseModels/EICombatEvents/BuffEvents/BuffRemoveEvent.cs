using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffRemoveEvent : AbstractBuffEvent
    {
        private readonly ParseEnum.BuffRemove _removeType;
        private readonly int _removedStacks;
        private readonly long _lastRemovedDuration;
        public BuffRemoveEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            _removeType = evtcItem.IsBuffRemove;
            Src = agentData.GetAgentByInstID(evtcItem.DstMasterInstid > 0 ? evtcItem.DstMasterInstid : evtcItem.DstInstid, evtcItem.Time);
            Dst = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.Time);
            _lastRemovedDuration = evtcItem.BuffDmg;
            _removedStacks = evtcItem.Result;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(Src, Value, Time, _removeType);
        }
    }
}
