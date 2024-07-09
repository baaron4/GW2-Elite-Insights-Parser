using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.LineDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class LineDecorationMetadataDescription : FormDecorationMetadataDescription
    {

        internal LineDecorationMetadataDescription(LineDecorationMetadata decoration) : base(decoration)
        {
            Type = "Line";
        }
    }

}
