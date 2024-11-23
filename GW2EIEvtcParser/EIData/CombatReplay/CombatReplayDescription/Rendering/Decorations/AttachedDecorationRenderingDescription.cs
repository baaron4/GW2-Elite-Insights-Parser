using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.AttachedDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class AttachedDecorationRenderingDescription : DecorationRenderingDescription
{
    public readonly ConnectorDescription ConnectedTo;
    public readonly ConnectorDescription? RotationConnectedTo;
    public readonly SkillModeDescription? SkillMode = null;

    internal AttachedDecorationRenderingDescription(ParsedEvtcLog log, AttachedDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(decoration, metadataSignature)
    {
        ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        RotationConnectedTo = decoration.RotationConnectedTo?.GetConnectedTo(map, log);
        IsMechanicOrSkill = true;
        if (decoration.SkillMode != null)
        {
            SkillMode = decoration.SkillMode.GetDescription(map, log, usedSkills, usedBuffs);
        }
    }
}
