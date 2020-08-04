namespace GW2EIEvtcParser.EIData
{
    public class RotatedRectangleDecorationSerializable : RectangleDecorationSerializable
    {
        public int Rotation { get; }
        public int RadialTranslation { get; }
        public int SpinAngle { get; }

        internal RotatedRectangleDecorationSerializable(ParsedEvtcLog log, RotatedRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "RotatedRectangle";
            Rotation = decoration.Rotation;
            RadialTranslation = decoration.RadialTranslation;
            SpinAngle = decoration.SpinAngle;
        }

    }
}
