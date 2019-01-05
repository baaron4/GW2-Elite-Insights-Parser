using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class DoughnutActor : FormActor
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutActor(bool fill, int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        //

        private class DoughnutSerializable : FormSerializable
        {
            public int InnerRadius { get; set; }
            public int OuterRadius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
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
                ConnectedTo = ConnectedTo.GetConnectedTo(map)
            };
            return JsonConvert.SerializeObject(aux);
        }

    }
}
