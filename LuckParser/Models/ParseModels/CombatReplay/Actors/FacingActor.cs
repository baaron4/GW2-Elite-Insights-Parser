using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class FacingActor : GenericActor
    {
        private List<Tuple<double, long>> _data = new List<Tuple<double, long>>();

        public FacingActor(Tuple<int, int> lifespan, Connector connector, List<Point3D> facings) : base(lifespan, connector)
        {
            foreach(Point3D facing in facings)
            {
                _data.Add(new Tuple<double, long>(Point3D.GetRotationFromFacing(facing), facing.Time));
            }
        }

        //
        protected class FacingSerializable : GenericSerializable
        {
            public object[] FacingData;
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            FacingSerializable aux = new FacingSerializable
            {
                Type = "Facing",
                Start = Lifespan.Item1,
                End = Lifespan.Item2,
                ConnectedTo = ConnectedTo.GetConnectedTo(map),
                FacingData = new object[_data.Count]
            };
            int i = 0;
            foreach(var item in _data)
            {
                aux.FacingData[i++] = new object[2]
                {
                    item.Item1,
                    item.Item2
                };
            }
            return JsonConvert.SerializeObject(aux);
        }
    }
}
