using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;

namespace GW2EIEvtcParser.EIData;

public class BackgroundIconDecorationRenderingDescription : ImageDecorationRenderingDescription
{

    public readonly IReadOnlyList<float> Opacities;
    public readonly IReadOnlyList<float> Heights;
    internal BackgroundIconDecorationRenderingDescription(ParsedEvtcLog log, BackgroundIconDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = "BackgroundIcon"; //TODO(Rennorb) @cleanup: enum
        IsMechanicOrSkill = false;
        var opacities = new List<float>(decoration.Opacities.Count * 2);
        foreach (ParametricPoint1D opacity in decoration.Opacities)
        {
            opacities.Add(opacity.X);
            opacities.Add(opacity.Time);
        }
        Opacities = opacities;

        var heights = new List<float>(decoration.Heights.Count * 2);
        foreach (ParametricPoint1D height in decoration.Heights)
        {
            heights.Add(height.X);
            heights.Add(height.Time);
        }
        Heights = heights;
    }
}
