using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconOverheadDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class IconOverheadDecorationMetadataDescription : IconDecorationMetadataDescription
    {

        internal IconOverheadDecorationMetadataDescription(IconOverheadDecorationMetadata decoration) : base(decoration)
        {
            Type = "IconOverheadDecoration";
        }
    }

}
