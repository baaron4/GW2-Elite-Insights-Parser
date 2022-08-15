namespace GW2EIEvtcParser.EIData
{
    public class RotatedRectangleDecorationDescription : RectangleDecorationDescription
    {
        public float Rotation { get; }
        public int RadialTranslation { get; }
        public int SpinAngle { get; }

        internal RotatedRectangleDecorationDescription(ParsedEvtcLog log, RotatedRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "RotatedRectangle";
            Rotation = decoration.Rotation;
            RadialTranslation = decoration.RadialTranslation;
            SpinAngle = decoration.SpinAngle;
        }

    }
}
