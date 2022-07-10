using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class MarkerGUIDEvent : IDToGUIDEvent
    {
        internal MarkerGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
        }

    }
}
