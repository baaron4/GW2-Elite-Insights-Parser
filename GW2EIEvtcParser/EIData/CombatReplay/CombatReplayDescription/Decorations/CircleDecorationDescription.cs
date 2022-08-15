namespace GW2EIEvtcParser.EIData
{
    public class CircleDecorationDescription : FormDecorationDescription
    {
        public int Radius { get; }
        public int MinRadius { get; }

        internal CircleDecorationDescription(ParsedEvtcLog log, CircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }

}
