
namespace GW2EIEvtcParser.EIData
{
    internal abstract class BackgroundDecoration : GenericDecoration
    {
        internal abstract class BackgroundDecorationMetadata : GenericDecorationMetadata
        {
        }
        internal abstract class BackgroundDecorationRenderingData : GenericDecorationRenderingData
        {
            protected BackgroundDecorationRenderingData((long, long) lifespan) : base(lifespan)
            {
            }
        }
        internal BackgroundDecoration(BackgroundDecorationMetadata metadata, BackgroundDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }
    }
}
