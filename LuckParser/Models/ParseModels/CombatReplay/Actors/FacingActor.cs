using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class FacingActor : GenericActor
    {
        private List<int> _data = new List<int>();

        public FacingActor((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach(Point3D facing in facings)
            {
                _data.Add(-Point3D.GetRotationFromFacing(facing));
            }
        }

        //
        protected class FacingSerializable : GenericActorSerializable
        {
            public object[] FacingData;
        }

        public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map)
        {
            FacingSerializable aux = new FacingSerializable
            {
                Type = "Facing",
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map),
                FacingData = new object[_data.Count]
            };
            int i = 0;
            foreach(int angle in _data)
            {
                aux.FacingData[i++] = angle;
            }
            return aux;
        }
    }
}
