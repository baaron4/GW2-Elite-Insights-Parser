using static GW2EIEvtcParser.EIData.IconOverheadDecoration;

namespace GW2EIEvtcParser.EIData;

public class IconOverheadDecorationMetadataDescription : IconDecorationMetadataDescription
{

    internal IconOverheadDecorationMetadataDescription(IconOverheadDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.IconOverhead;
    }
}
