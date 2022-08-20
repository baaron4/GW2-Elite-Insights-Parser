using System;

namespace GW2EIEvtcParser.EIData
{
    public class ParametricPoint3D : Point3D
    {
        public long Time { get; }


        public ParametricPoint3D(float x, float y, float z, long time) : base(x,y,z)
        {
            Time = time;
        }

        public ParametricPoint3D(ParametricPoint3D a) : this(a.X, a.Y, a.Z, a.Time)
        {
        }

        public ParametricPoint3D(Point3D a, long time) : base(a)
        {
            Time = time;
        }

        public ParametricPoint3D(Point3D a, Point3D b, float ratio, long time) : base(a,b,ratio)
        {
            Time = time;
        }
    }
}
