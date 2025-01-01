namespace GW2EIEvtcParser.EIData;

internal abstract class ImageDecoration : AttachedDecoration
{
    public abstract class ImageDecorationMetadata : AttachedDecorationMetadata
    {
        public readonly string Image;
        public readonly uint PixelSize;
        public readonly uint WorldSize;
        protected ImageDecorationMetadata(string icon, uint pixelSize, uint worldSize) : base()
        {
            Image = icon;
            PixelSize = pixelSize;
            WorldSize = worldSize;
            if (PixelSize == WorldSize && PixelSize == 0)
            {
                throw new InvalidOperationException("Icons must have at least one non zero size");
            }
        }
    }
    public abstract class ImageDecorationRenderingData : AttachedDecorationRenderingData
    {
        protected ImageDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
        }
    }
    private new ImageDecorationMetadata DecorationMetadata => (ImageDecorationMetadata)base.DecorationMetadata;
    public string Image => DecorationMetadata.Image;
    public uint PixelSize => DecorationMetadata.PixelSize;
    public uint WorldSize => DecorationMetadata.WorldSize;

    protected ImageDecoration(ImageDecorationMetadata metadata, ImageDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }
}
