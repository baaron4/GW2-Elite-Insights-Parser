using static GW2EIEvtcParser.EIData.TextDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class TextOverheadDecorationMetadataDescription : TextDecorationMetadataDescription
{

    internal TextOverheadDecorationMetadataDescription(TextDecorationMetadata decoration, ColorTokenHandler colorTokenHandler) : base(decoration, colorTokenHandler)
    {
        Type = Types.TextOverhead;
    }
}
