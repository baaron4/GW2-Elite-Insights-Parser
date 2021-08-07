namespace GW2EIEvtcParser.EIData
{
    public class PieDecorationCombatReplayDescription : CircleDecorationCombatReplayDescription
    {
        public int Direction { get; set; }
        public int OpeningAngle { get; set; }

        internal PieDecorationCombatReplayDescription(ParsedEvtcLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
