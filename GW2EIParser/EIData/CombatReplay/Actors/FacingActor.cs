using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FacingActor : GenericActor
    {
        protected List<int> Data { get; set; } = new List<int>();

        public FacingActor((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach (Point3D facing in facings)
            {
                Data.Add(-Point3D.GetRotationFromFacing(facing));
            }
        }

        //
        protected class FacingSerializable : GenericActorSerializable
        {
            public int[] FacingData { get; set; }
        }

        public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            var aux = new FacingSerializable
            {
                Type = "Facing",
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log),
                FacingData = new int[Data.Count]
            };
            int i = 0;
            foreach (int angle in Data)
            {
                aux.FacingData[i++] = angle;
            }
            return aux;
        }
    }
}
