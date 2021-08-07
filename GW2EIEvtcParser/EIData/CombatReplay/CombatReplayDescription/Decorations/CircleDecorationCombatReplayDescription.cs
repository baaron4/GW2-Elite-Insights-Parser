namespace GW2EIEvtcParser.EIData
{
    public class CircleDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public int Radius { get; }
        public int MinRadius { get; }

        internal CircleDecorationCombatReplayDescription(ParsedEvtcLog log, CircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }

}
