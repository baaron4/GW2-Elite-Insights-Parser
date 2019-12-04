using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FacingDecoration : GenericAttachedDecoration
    {
        public List<int> Angles { get; } = new List<int>();

        public FacingDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach (Point3D facing in facings)
            {
                Angles.Add(-Point3D.GetRotationFromFacing(facing));
            }
        }

        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new FacingDecorationSerializable(log, this, map);
        }
    }
}
