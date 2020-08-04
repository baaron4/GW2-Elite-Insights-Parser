namespace GW2EIEvtcParser.EIData
{
    public class CircleDecorationSerializable : FormDecorationSerializable
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleDecorationSerializable(ParsedEvtcLog log, CircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }

}
