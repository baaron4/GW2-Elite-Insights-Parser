using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class RotationConnector
    {
        public abstract class RotationConnectorDescriptor
        {

            public RotationConnectorDescriptor(RotationConnector connector, CombatReplayMap map)
            {
            }
        }
        public abstract RotationConnectorDescriptor GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
    }
}
