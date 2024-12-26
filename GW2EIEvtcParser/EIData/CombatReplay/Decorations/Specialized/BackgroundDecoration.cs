
namespace GW2EIEvtcParser.EIData;

internal abstract class BackgroundDecoration : Decoration
{
    public abstract class BackgroundDecorationMetadata : _DecorationMetadata
    {
    }
    public abstract class BackgroundDecorationRenderingData : _DecorationRenderingData
    {
        protected BackgroundDecorationRenderingData((long, long) lifespan) : base(lifespan)
        {
        }
    }
    protected BackgroundDecoration(BackgroundDecorationMetadata metadata, BackgroundDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }
}
