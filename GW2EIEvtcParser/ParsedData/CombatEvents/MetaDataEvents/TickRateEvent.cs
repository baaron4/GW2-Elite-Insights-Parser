using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class TickRateEvent : AbstractMetaDataEvent
    {
        public long Time { get; }
        public ulong TickRate { get; }
        internal TickRateEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Time = evtcItem.Time;
            TickRate = evtcItem.SrcAgent;
        }

    }
}
