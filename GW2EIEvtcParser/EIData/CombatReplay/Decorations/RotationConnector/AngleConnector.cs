using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class AngleConnector : RotationConnector
    {
        protected float StartAngle { get; set; }

        protected float RotationSpeed { get; set; }

        public AngleConnector(float startAngle)
        {
            StartAngle = startAngle;
            RotationSpeed = startAngle;
        }

        public AngleConnector(float startAngle, float rotationSpeed)
        {
            StartAngle = startAngle;
            RotationSpeed = rotationSpeed;
        }

        public class AngleConnectorDescriptor : RotationConnectorDescriptor
        {
            public IReadOnlyList<float> Angles { get; private set; }
            public AngleConnectorDescriptor(AngleConnector connector, CombatReplayMap map) : base(connector, map)
            {
                Angles = new List<float>() { 
                    connector.StartAngle,
                    connector.RotationSpeed,
                };
            }
        }

        public override RotationConnectorDescriptor GetRotationConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new AngleConnectorDescriptor(this, map);
        }
    }
}
