using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconDecoration : GenericIconDecoration
    {
        public float Opacity { get; }

        public IconDecoration(string icon, int pixelSize, float opacity, (long start, long end) lifespan, GeographicalConnector connector) : base(icon, pixelSize, lifespan, connector)
        {
            Opacity = opacity;
        }

        public IconDecoration(string icon, int pixelSize, int worldSize, float opacity, (long start, long end) lifespan, GeographicalConnector connector) : base(icon, pixelSize, worldSize, lifespan, connector)
        {
            Opacity = opacity;
        }

        public IconDecoration(string icon, int pixelSize, float opacity, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public IconDecoration(string icon, int pixelSize, int worldSize, float opacity, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new IconDecorationCombatReplayDescription(log, this, map);
        }
    }
}
