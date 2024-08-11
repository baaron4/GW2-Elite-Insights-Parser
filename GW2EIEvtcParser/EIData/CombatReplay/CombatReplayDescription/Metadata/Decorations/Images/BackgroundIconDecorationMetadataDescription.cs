using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class BackgroundIconDecorationMetadataDescription : GenericIconDecorationMetadataDescription
    {

        internal BackgroundIconDecorationMetadataDescription(BackgroundIconDecorationMetadata decoration) : base(decoration)
        {
            Type = "BackgroundIconDecoration";
        }
    }

}
