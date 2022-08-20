using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class InterpolationConnector : Connector
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

        internal class InterpolationDescriptor
        {
            public int InterpolationMethod { get; set; }
            public IReadOnlyList<float> Positions { get; set; }
            public InterpolationDescriptor(IReadOnlyList<ParametricPoint3D> points, InterpolationMethod interpolationMethod, CombatReplayMap map)
            {
                InterpolationMethod = (int)interpolationMethod;
                var positions = new List<float>();
                foreach (ParametricPoint3D pos in points)
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
            return new InterpolationDescriptor(Positions, _method, map);
        }
    }
}
