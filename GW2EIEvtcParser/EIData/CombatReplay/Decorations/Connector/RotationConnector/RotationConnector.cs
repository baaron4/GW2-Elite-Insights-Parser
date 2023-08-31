using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class RotationConnector : Connector
    {
        public abstract class RotationConnectorDescriptor
        {

            public RotationConnectorDescriptor(RotationConnector connector, CombatReplayMap map)
            {
            }
        }
    }
}
