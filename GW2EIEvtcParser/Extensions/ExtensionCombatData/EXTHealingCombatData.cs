using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTHealingCombatData
    {
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _healData;
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _healReceivedData;
        private readonly Dictionary<long, List<EXTAbstractHealingEvent>> _healDataById;

        internal EXTHealingCombatData(Dictionary<AgentItem, List<EXTAbstractHealingEvent>> healData, Dictionary<AgentItem, List<EXTAbstractHealingEvent>> healReceivedData, Dictionary<long, List<EXTAbstractHealingEvent>> healDataById) 
        {
            _healData = healData;
            _healReceivedData = healReceivedData;
            _healDataById = healDataById;
        }

    }
}
