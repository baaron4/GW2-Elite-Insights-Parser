
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.GenericAttachedDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class GenericAttachedDecorationMetadataDescription : GenericDecorationMetadataDescription
{

    internal GenericAttachedDecorationMetadataDescription(GenericAttachedDecorationMetadata decoration) : base(decoration)
    {
    }
}
