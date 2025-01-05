using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.TextDecoration;

namespace GW2EIEvtcParser.EIData;

public class TextDecorationRenderingDescription : DecorationRenderingDescription
{
    public readonly string Text;
    public readonly bool Bold;
    public readonly string? FontType;
    public readonly uint FontSize;
    public readonly ConnectorDescription ConnectedTo;

    internal TextDecorationRenderingDescription(ParsedEvtcLog log, TextDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(decoration, metadataSignature)
    {
        Type = "Text";
        Text = decoration.Text;
        FontType = decoration.FontType;
        FontSize = decoration.FontSize;
        Bold = decoration.Bold;
        ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
    }
}
