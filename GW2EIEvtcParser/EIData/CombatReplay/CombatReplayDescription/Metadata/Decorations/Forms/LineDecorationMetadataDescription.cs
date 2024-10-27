using static GW2EIEvtcParser.EIData.LineDecoration;

namespace GW2EIEvtcParser.EIData;

public class LineDecorationMetadataDescription : FormDecorationMetadataDescription
{

    internal LineDecorationMetadataDescription(LineDecorationMetadata decoration) : base(decoration)
    {
        Type = "Line";
    }
}
