using static GW2EIEvtcParser.EIData.AttachedDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class AttachedDecorationMetadataDescription : DecorationMetadataDescription
{

    internal AttachedDecorationMetadataDescription(AttachedDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
    }
}
