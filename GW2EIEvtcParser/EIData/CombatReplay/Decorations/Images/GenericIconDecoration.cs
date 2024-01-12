using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericIconDecoration : GenericAttachedDecoration
    {
        public string Image { get; }
        public uint PixelSize { get; }
        public uint WorldSize { get; }

        public GenericIconDecoration(string icon, uint pixelSize, (long start, long end) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
            Image = icon;
            PixelSize = pixelSize;
        }

        public GenericIconDecoration(string icon, uint pixelSize, uint worldSize, (long start, long end) lifespan, GeographicalConnector connector) : this(icon, pixelSize, lifespan, connector)
        {
            WorldSize = worldSize;
        }

        public GenericIconDecoration(string icon, uint pixelSize, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, (lifespan.Start, lifespan.End), connector)
        {
        }

        public GenericIconDecoration(string icon, uint pixelSize, uint worldSize, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, (lifespan.Start, lifespan.End), connector)
        {
        }
    }
}
