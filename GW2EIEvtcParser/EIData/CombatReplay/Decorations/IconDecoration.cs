using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconDecoration : GenericAttachedDecoration
    {
        public string Image { get; }
        public int PixelSize { get; }
        public int WorldSize { get; }
        public float Opacity { get; }

        public IconDecoration(string icon, int pixelSize, float opacity, (long start, long end) lifespan, Connector connector) : base(lifespan, connector)
        {
            Image = icon;
            PixelSize = pixelSize;
            Opacity = opacity;
        }

        public IconDecoration(string icon, int pixelSize, int worldSize, float opacity, (long start, long end) lifespan, Connector connector) : this(icon, pixelSize, opacity, lifespan, connector)
        {
            WorldSize = worldSize;
        }

        public IconDecoration(string icon, int pixelSize, float opacity, Segment lifespan, Connector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public IconDecoration(string icon, int pixelSize, int worldSize, float opacity, Segment lifespan, Connector connector) : this(icon, pixelSize, worldSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new IconDecorationCombatReplayDescription(log, this, map);
        }
    }
}
