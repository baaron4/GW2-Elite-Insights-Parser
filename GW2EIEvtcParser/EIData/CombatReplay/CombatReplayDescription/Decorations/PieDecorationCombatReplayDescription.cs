namespace GW2EIEvtcParser.EIData
{
    public class PieDecorationCombatReplayDescription : CircleDecorationCombatReplayDescription
    {
        public float Direction { get; set; }
        public float OpeningAngle { get; set; }

        internal PieDecorationCombatReplayDescription(ParsedEvtcLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
