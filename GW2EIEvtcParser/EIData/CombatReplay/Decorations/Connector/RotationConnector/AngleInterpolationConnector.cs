using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class AngleInterpolationConnector : RotationConnector
    {
        /// <summary>
        /// Points for angles, in degrees, around Z axis
        /// </summary>
        protected IReadOnlyList<ParametricPoint1D> Angles { get; set; }

        private readonly InterpolationMethod _method;

        public AngleInterpolationConnector(IReadOnlyList<ParametricPoint1D> angles, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
        {
            Angles = angles;
            _method = interpolationMethod;
        }

        /// <summary>
        /// Will create an AngleInterpolationConnector using the angle around Z axis for each (destination - origin) vectors
        /// </summary>
        /// <param name="originPoints">Origin points</param>
        /// <param name="destinationPoints">Destination</param>
        /// <param name="interpolationMethod"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public AngleInterpolationConnector(IReadOnlyList<ParametricPoint3D> originPoints, IReadOnlyList<ParametricPoint3D> destinationPoints, InterpolationMethod interpolationMethod = InterpolationMethod.Linear)
        {
            if (!originPoints.Any() || !destinationPoints.Any())
            {
                throw new InvalidOperationException("Must at least have one point");
            }
            var angles = new List<ParametricPoint1D>();
            for (int i = 0; i < Math.Min(originPoints.Count, destinationPoints.Count); i++)
            {
                long time = originPoints[i].Time;
                if (time != destinationPoints[i].Time)
                {
                    throw new InvalidOperationException("Origin and Destination points must have the same timestamp");
                }
                Point3D vector = destinationPoints[i] - originPoints[i];
                float angle = Point3D.GetZRotationFromFacing(vector);
                angles.Add(new ParametricPoint1D(angle, time));
            }
            Angles = angles;
            _method = interpolationMethod;
        }

        protected class InterpolationConnectorDescriptor : RotationConnectorDescriptor
        {
            public int InterpolationMethod { get; private set; }
            public IReadOnlyList<float> Angles { get; private set; }
            public InterpolationConnectorDescriptor(AngleInterpolationConnector connector, CombatReplayMap map) : base(connector, map)
            {
                InterpolationMethod = (int)connector._method;
                var angles = new List<float>();
                foreach (ParametricPoint1D angle in connector.Angles)
                {
                    angles.Add(-angle.X);
                    angles.Add(angle.Time);
                }
                Angles = angles;
            }
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new InterpolationConnectorDescriptor(this, map);
        }
    }
}
