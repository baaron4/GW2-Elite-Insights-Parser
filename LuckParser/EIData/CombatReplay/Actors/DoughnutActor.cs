using LuckParser.Parser;

namespace LuckParser.EIData
{
    public class DoughnutActor : FormActor
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutActor(bool fill, int growing, int innerRadius, int outerRadius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
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

        public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            DoughnutSerializable aux = new DoughnutSerializable
            {
                Type = "Doughnut",
                OuterRadius = OuterRadius,
                InnerRadius = InnerRadius,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log)
            };
            return aux;
        }

    }
}
