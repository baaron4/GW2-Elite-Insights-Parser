namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecoration : FormDecoration
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecoration(bool fill, int growing, int width, int height, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Height = height;
            Width = width;
        }
        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new RectangleDecorationSerializable(log, this, map);
        }
    }
}
