using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractCombatReplayDecorationMetadataDescription : AbstractCombatReplayDescription
    {
        public string Signature { get; private set; }
        internal AbstractCombatReplayDecorationMetadataDescription(GenericDecorationMetadata decoration) 
        {
            Signature = decoration.GetSignature();
        }
    }
}
