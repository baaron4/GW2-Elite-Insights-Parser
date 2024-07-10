using System;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationMetadataDescription : AbstractCombatReplayDecorationMetadataDescription
    {

        public string Signature { get; private set; }
        internal GenericDecorationMetadataDescription(GenericDecorationMetadata decoration) : base()
        {
            Signature = decoration.GetSignature();
        }
    }
}
