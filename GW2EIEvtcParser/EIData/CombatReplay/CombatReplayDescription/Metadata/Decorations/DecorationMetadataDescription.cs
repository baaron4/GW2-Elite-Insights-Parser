using static GW2EIEvtcParser.EIData.Decoration;

namespace GW2EIEvtcParser.EIData;

public abstract class DecorationMetadataDescription : CombatReplayMetadataDescription
{

    public string Signature { get; private set; }
    internal DecorationMetadataDescription(_DecorationMetadata decoration) : base()
    {
        Signature = decoration.GetSignature();
    }
}
