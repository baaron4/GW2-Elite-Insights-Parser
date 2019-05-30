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
        private readonly ParseEnum.IFF _iff;
        private readonly long _lastRemovedDuration;
        public BuffRemoveEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, offset)
        {
            _removeType = evtcItem.IsBuffRemove;
            Src = agentData.GetAgentByInstID(evtcItem.DstMasterInstid > 0 ? evtcItem.DstMasterInstid : evtcItem.DstInstid, evtcItem.Time);
            Dst = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.Time);
            _lastRemovedDuration = evtcItem.BuffDmg;
            _removedStacks = evtcItem.Result;
            _iff = evtcItem.IFF;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override bool IsBoonSimulatorCompliant()
        {
            return BuffID != BoonHelper.NoBuff &&
                _removeType != ParseEnum.BuffRemove.Manual && // don't check manual
                !(_removeType == ParseEnum.BuffRemove.Single && _iff == ParseEnum.IFF.Unknown && Dst == GeneralHelper.UnknownAgent) && // weird single stack remove
                !(_removeType == ParseEnum.BuffRemove.Single && Value <= 50) && // low value single stack remove that can mess up with the simulator if server delay
                !(_removeType == ParseEnum.BuffRemove.All && Value <= 50 && _lastRemovedDuration <= 50); // low value all stack remove that can mess up with the simulator if server delay
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Remove(Src, Value, Time, _removeType);
        }
    }
}
