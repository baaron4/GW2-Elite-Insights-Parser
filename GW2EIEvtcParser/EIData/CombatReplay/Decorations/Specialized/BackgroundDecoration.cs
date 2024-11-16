
namespace GW2EIEvtcParser.EIData;

internal abstract class BackgroundDecoration : Decoration
{
    internal abstract class BackgroundDecorationMetadata : _DecorationMetadata
    {
    }
    internal abstract class BackgroundDecorationRenderingData : _DecorationRenderingData
    {
        protected BackgroundDecorationRenderingData((long, long) lifespan) : base(lifespan)
        {
        }
    }
    internal BackgroundDecoration(BackgroundDecorationMetadata metadata, BackgroundDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }
}
