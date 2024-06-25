using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericIconDecoration : GenericAttachedDecoration
    {
        internal abstract class GenericIconDecorationMetadata : GenericAttachedDecorationMetadata
        {
            public string Image { get; }
            public uint PixelSize { get; }
            public uint WorldSize { get; }
            protected GenericIconDecorationMetadata(string icon, uint pixelSize, uint worldSize) : base()
            {
                Image = icon;
                PixelSize = Math.Max(pixelSize, 1);
                WorldSize = Math.Max(worldSize, 1);
            }
        }
        internal abstract class GenericIconDecorationRenderingData : GenericAttachedDecorationRenderingData
        {
            protected GenericIconDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new GenericIconDecorationMetadata DecorationMetadata => (GenericIconDecorationMetadata)base.DecorationMetadata;
        public string Image => DecorationMetadata.Image;
        public uint PixelSize => DecorationMetadata.PixelSize;
        public uint WorldSize => DecorationMetadata.WorldSize;

        internal GenericIconDecoration(GenericIconDecorationMetadata metadata, GenericIconDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }
    }
}
