using System;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData
{
    public class EffectGUIDEvent : IDToGUIDEvent
    {
        internal EffectGUIDEvent(CombatItem evtcItem) : base(evtcItem)
        {
        }

    }
}
