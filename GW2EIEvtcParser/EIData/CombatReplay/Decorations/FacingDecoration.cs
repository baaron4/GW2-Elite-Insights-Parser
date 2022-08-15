using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class FacingDecoration : GenericAttachedDecoration
    {
        public List<float> Angles { get; } = new List<float>();

        public FacingDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach (Point3D facing in facings)
            {
                if(facing.Time >= lifespan.start && facing.Time <= lifespan.end)
                {
                    Angles.Add(-Point3D.GetRotationFromFacing(facing));
                }
            }
        }

        //

        public override GenericDecorationDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new FacingDecorationDescription(log, this, map);
        }
    }
}
