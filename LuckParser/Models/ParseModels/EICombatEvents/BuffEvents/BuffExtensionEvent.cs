using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class BuffExtensionEvent : BuffApplyEvent
    {
        private readonly long _oldValue;
        private readonly long _extension;

        public BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            Src = null;
            _extension = evtcItem.Value;
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
        }

        public override void TryFindSrc(ParsedLog log)
        {
            if (Src == null)
            {
                Src = log.Boons.TryFindSrc(Dst, Time, _extension, log, BuffID);
            }
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Extend(Value, _oldValue, Src, Time);
        }
    }
}
