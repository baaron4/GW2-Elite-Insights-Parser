using static GW2EIEvtcParser.EIData.Decoration;

namespace GW2EIEvtcParser.EIData;

public abstract class DecorationMetadataDescription : CombatReplayMetadataDescription
{

    public string Signature { get; private set; }
    internal DecorationMetadataDescription(_DecorationMetadata decoration) : base()
    {
        Signature = decoration.GetSignature();
    }

    public virtual void ColorToToken(Dictionary<string, string> colorMap)
    {
    }

    protected string GetToken(string color, Dictionary<string, string> colorMap)
    {
        string? token = null;
        foreach (var colorEntry in colorMap)
        {
            if (colorEntry.Value == color)
            {
                token = colorEntry.Key;
                break;
            }
        }
        if (token == null)
        {
            token = "c" + (colorMap.Count + 1);
            colorMap[token] = color;
        }
        Signature = Signature.Replace(color, token, StringComparison.Ordinal);
        return token;
    }
}
