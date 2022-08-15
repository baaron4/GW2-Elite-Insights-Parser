namespace GW2EIEvtcParser.EIData
{
    public class PieDecorationDescription : CircleDecorationDescription
    {
        public float Direction { get; set; }
        public float OpeningAngle { get; set; }

        internal PieDecorationDescription(ParsedEvtcLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
