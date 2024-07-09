using System;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationMetadataDescription : AbstractCombatReplayDecorationMetadataDescription
    {

        internal GenericDecorationMetadataDescription(GenericDecorationMetadata decoration) : base(decoration)
        {
        }
    }
}
