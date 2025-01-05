using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;

namespace GW2EIEvtcParser.EIData;

public class BackgroundIconDecorationMetadataDescription : ImageDecorationMetadataDescription
{

    internal BackgroundIconDecorationMetadataDescription(BackgroundIconDecorationMetadata decoration) : base(decoration)
    {
        Type = "BackgroundIcon";
    }
}
