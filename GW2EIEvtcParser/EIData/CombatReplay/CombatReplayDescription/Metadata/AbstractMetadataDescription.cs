using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractMetadataDescription : AbstractCombatReplayDescription
    {
        public string Signature { get; protected set; }
        internal AbstractMetadataDescription(GenericDecorationMetadata decoration) 
        {
            Signature = decoration.GetSignature();
        }
    }
}
