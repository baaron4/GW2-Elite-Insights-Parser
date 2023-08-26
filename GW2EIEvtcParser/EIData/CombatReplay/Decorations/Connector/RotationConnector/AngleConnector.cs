using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class AngleConnector : RotationConnector
    {
        protected float StartAngle { get; set; }

        protected float SpinAngle { get; set; }

        public AngleConnector(float startAngle)
        {
            StartAngle = startAngle;
            SpinAngle = 0;
        }

        public AngleConnector(Point3D rotationVector)
        {
            StartAngle = Point3D.GetRotationFromFacing(rotationVector);
            SpinAngle = 0;
        }

        public AngleConnector(float startAngle, float spinAngle) : this(startAngle)
        {
            SpinAngle = spinAngle;
        }

        public AngleConnector(Point3D rotationVector, float spinAngle) : this(rotationVector)
        {
            SpinAngle = spinAngle;
        }

        public class AngleConnectorDescriptor : RotationConnectorDescriptor
        {
            public IReadOnlyList<float> Angles { get; private set; }
            public AngleConnectorDescriptor(AngleConnector connector, CombatReplayMap map) : base(connector, map)
            {
                Angles = new List<float>() { 
                    -connector.StartAngle,
                    -connector.SpinAngle,
                };
            }
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new AngleConnectorDescriptor(this, map);
        }
    }
}
