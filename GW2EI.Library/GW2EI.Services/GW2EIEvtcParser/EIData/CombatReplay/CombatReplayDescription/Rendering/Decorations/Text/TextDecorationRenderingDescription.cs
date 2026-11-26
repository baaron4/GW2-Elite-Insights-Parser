using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.TextDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class TextDecorationRenderingDescription : DecorationRenderingDescription
{
    public readonly string Text;
    public readonly bool Bold;
    public readonly string? FontType;
    public readonly uint FontSize;
    public readonly ConnectorDescription ConnectedTo;
    public readonly ConnectorDescription? RotationConnectedTo;
    public readonly SkillModeDescription? SkillMode = null;

    internal TextDecorationRenderingDescription(ParsedEvtcLog log, TextDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(decoration, metadataSignature, stringToUlongHandler)
    {
        Type = Types.Text;
        Text = decoration.Text;
        FontType = decoration.FontType;
        FontSize = decoration.FontSize;
        Bold = decoration.Bold;
        ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        RotationConnectedTo = decoration.RotationConnectedTo?.GetConnectedTo(map, log);
        IsMechanicOrSkill = true;
        if (decoration.SkillMode != null)
        {
            SkillMode = decoration.SkillMode.GetDescription(map, log, usedSkills, usedBuffs);
        }
    }
}
