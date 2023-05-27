using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class FacingDecoration : GenericAttachedDecoration
    {
        public List<float> Angles { get; } = new List<float>();

        protected FacingDecoration((int start, int end) lifespan, AgentConnector connector, IReadOnlyList<ParametricPoint3D> facings) : base(lifespan, connector)
        {
            foreach (ParametricPoint3D facing in facings)
            {
                if(facing.Time >= lifespan.start && facing.Time <= lifespan.end)
                {
                    Angles.Add(-Point3D.GetRotationFromFacing(facing));
                }
            }
        }
    }
}
