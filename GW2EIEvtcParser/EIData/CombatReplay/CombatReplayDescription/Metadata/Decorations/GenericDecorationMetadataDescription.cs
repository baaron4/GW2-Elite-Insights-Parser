using System;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationMetadataDescription : AbstractMetadataDescription
    {

        internal GenericDecorationMetadataDescription(GenericDecorationMetadata decoration) : base(decoration)
        {
        }
    }
}
