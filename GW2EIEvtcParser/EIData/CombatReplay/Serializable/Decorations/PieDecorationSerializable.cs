namespace GW2EIEvtcParser.EIData
{
    public class PieDecorationSerializable : CircleDecorationSerializable
    {
        public int Direction { get; set; }
        public int OpeningAngle { get; set; }

        internal PieDecorationSerializable(ParsedEvtcLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
