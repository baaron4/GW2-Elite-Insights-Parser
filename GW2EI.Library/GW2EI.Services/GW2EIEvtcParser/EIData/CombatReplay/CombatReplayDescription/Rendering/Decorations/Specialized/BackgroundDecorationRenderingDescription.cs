using static GW2EIEvtcParser.EIData.BackgroundDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class BackgroundDecorationRenderingDescription : DecorationRenderingDescription
{
    internal BackgroundDecorationRenderingDescription(BackgroundDecorationRenderingData decoration, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(decoration, metadataSignature, stringToUlongHandler)
    {

    }

}
