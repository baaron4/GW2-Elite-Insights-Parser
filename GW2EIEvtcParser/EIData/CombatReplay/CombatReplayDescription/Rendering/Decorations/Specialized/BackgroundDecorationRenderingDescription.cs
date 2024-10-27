using static GW2EIEvtcParser.EIData.BackgroundDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class BackgroundDecorationRenderingDescription : GenericDecorationRenderingDescription
{
    internal BackgroundDecorationRenderingDescription(BackgroundDecorationRenderingData decoration, string metadataSignature) : base(decoration, metadataSignature)
    {

    }

}
