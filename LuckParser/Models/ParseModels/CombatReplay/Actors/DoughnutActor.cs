using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class DoughnutActor : Actor
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color, position)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        //

        private class DoughnutSerializable<T> : Serializable<T>
        {
            public int InnerRadius { get; set; }
            public int OuterRadius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            if (Type == PositionType.Array)
            {
                DoughnutSerializable<int[]> aux = new DoughnutSerializable<int[]>
                {
                    Type = "Doughnut",
                    OuterRadius = OuterRadius,
                    InnerRadius = InnerRadius,
                    Fill = Filled,
                    Color = Color,
                    Growing = Growing,
                    Start = Lifespan.Item1,
                    End = Lifespan.Item2,
                    Position = new int[2]
                };
                Tuple<int, int> mapPos = map.GetMapCoord(Position.X, Position.Y);
                aux.Position[0] = mapPos.Item1;
                aux.Position[1] = mapPos.Item2;
                return JsonConvert.SerializeObject(aux);
            }
            else
            {

                DoughnutSerializable<int> aux = new DoughnutSerializable<int>()
                {
                    Type = "Doughnut",
                    OuterRadius = OuterRadius,
                    InnerRadius = InnerRadius,
                    Fill = Filled,
                    Color = Color,
                    Growing = Growing,
                    Start = Lifespan.Item1,
                    End = Lifespan.Item2,
                    Position = master.GetCombatReplayID()
                };
                return JsonConvert.SerializeObject(aux);
            }
        }

    }
}
