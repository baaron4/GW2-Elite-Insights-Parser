using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericIconDecoration : GenericAttachedDecoration
    {
        public string Image { get; }
        public int PixelSize { get; }
        public int WorldSize { get; }

        public GenericIconDecoration(string icon, int pixelSize, (long start, long end) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
            Image = icon;
            PixelSize = pixelSize;
        }

        public GenericIconDecoration(string icon, int pixelSize, int worldSize, (long start, long end) lifespan, GeographicalConnector connector) : this(icon, pixelSize, lifespan, connector)
        {
            WorldSize = worldSize;
        }

        public GenericIconDecoration(string icon, int pixelSize, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, (lifespan.Start, lifespan.End), connector)
        {
        }

        public GenericIconDecoration(string icon, int pixelSize, int worldSize, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, (lifespan.Start, lifespan.End), connector)
        {
        }
    }
}
