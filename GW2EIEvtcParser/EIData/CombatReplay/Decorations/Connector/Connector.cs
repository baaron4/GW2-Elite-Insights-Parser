using System.Collections.Generic;
using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
    }
}
