using static GW2EIEvtcParser.EIData.TextDecoration;

namespace GW2EIEvtcParser.EIData;

public class TextOverheadDecorationMetadataDescription : TextDecorationMetadataDescription
{

    internal TextOverheadDecorationMetadataDescription(TextDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.TextOverhead;
    }
}
