using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;

namespace GW2EIEvtcParser.EIData
{
    public class BackgroundIconDecorationMetadataDescription : GenericIconDecorationMetadataDescription
    {

        internal BackgroundIconDecorationMetadataDescription(BackgroundIconDecorationMetadata decoration) : base(decoration)
        {
            Type = "BackgroundIconDecoration";
        }
    }

}
