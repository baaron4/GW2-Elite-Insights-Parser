using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class FacingActor : GenericActor
    {
        private List<(double angle, long time)> _data = new List<(double angle, long time)>();

        public FacingActor((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach(Point3D facing in facings)
            {
                _data.Add((Point3D.GetRotationFromFacing(facing), facing.Time));
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
            foreach((double angle, long time) in _data)
            {
                aux.FacingData[i++] = new object[2]
                {
                    angle,
                    time
                };
            }
            return aux;
        }
    }
}
