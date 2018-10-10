using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class DoughnutActor : Actor
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutActor(bool fill, int growing,  int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color) : base(fill, growing, lifespan, color)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }

        public DoughnutActor(bool fill, int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, lifespan, color, position)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }

        public DoughnutActor(bool fill, int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(fill, growing, lifespan, color, prev, next, time)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        //

        private class DoughnutSerializable : Serializable
        {
            public int InnerRadius { get; set; }
            public int OuterRadius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            if (ConnectedTo != null)
            {
                Tuple<int, int> mapPos = map.GetMapCoord(ConnectedTo.X, ConnectedTo.Y);
                DoughnutSerializable aux = new DoughnutSerializable
                {
                    Type = "Doughnut",
                    OuterRadius = OuterRadius,
                    InnerRadius = InnerRadius,
                    Fill = Filled,
                    Color = Color,
                    Growing = Growing,
                    Start = Lifespan.Item1,
                    End = Lifespan.Item2,
                    ConnectedTo = new int[2]
                    {
                        mapPos.Item1,
                        mapPos.Item2
                    }
                };
                return JsonConvert.SerializeObject(aux);
            }
            else
            {

                DoughnutSerializable aux = new DoughnutSerializable()
                {
                    Type = "Doughnut",
                    OuterRadius = OuterRadius,
                    InnerRadius = InnerRadius,
                    Fill = Filled,
                    Color = Color,
                    Growing = Growing,
                    Start = Lifespan.Item1,
                    End = Lifespan.Item2,
                    ConnectedTo = master.GetCombatReplayID()
                };
                return JsonConvert.SerializeObject(aux);
            }
        }

    }
}
