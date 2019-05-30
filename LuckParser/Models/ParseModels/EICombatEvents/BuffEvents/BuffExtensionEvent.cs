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

        public BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem, agentData, offset)
        {
            By = null;
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
        }

        public override void TryFindSrc(ParsedLog log)
        {
            if (By == null)
            {
                By = log.Boons.TryFindSrc(To, Time, Value, log, BuffID);
            }
        }

        public override void UpdateSimulator(BoonSimulator simulator)
        {
            simulator.Extend(Value, _oldValue, By, Time);
        }
    }
}
