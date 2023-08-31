using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class InterpolationConnector : GeographicalConnector
    {
        protected IReadOnlyList<ParametricPoint3D> Positions { get; set; }

        internal enum InterpolationMethod
        {
            Linear = 0
        }

        private readonly InterpolationMethod _method;

        public InterpolationConnector(IReadOnlyList<ParametricPoint3D> positions, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
        {
            Positions = positions;
            _method = interpolationMethod;
        }

        protected class InterpolationConnectorDescriptor : GeographicalConnectorDescriptor
        {
            public int InterpolationMethod { get; private set; }
            public IReadOnlyList<float> Positions { get; private set; }
            public InterpolationConnectorDescriptor(InterpolationConnector connector, CombatReplayMap map) : base(connector, map)
            {
                InterpolationMethod = (int)connector._method;
                var positions = new List<float>();
                foreach (ParametricPoint3D pos in connector.Positions)
                {
                    (float x, float y) = map.GetMapCoord(pos.X, pos.Y);
                    positions.Add(x);
                    positions.Add(y);
                    positions.Add(pos.Time);
                }
                Positions = positions;
            }
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new InterpolationConnectorDescriptor(this, map);
        }
    }
}
