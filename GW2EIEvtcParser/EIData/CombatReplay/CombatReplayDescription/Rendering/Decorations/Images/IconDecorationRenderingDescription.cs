using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconDecoration;

namespace GW2EIEvtcParser.EIData;

public class IconDecorationRenderingDescription : ImageDecorationRenderingDescription
{
    public readonly string? Text;
    internal IconDecorationRenderingDescription(ParsedEvtcLog log, IconDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.Icon;
        if (decoration.IsSquadMarker)
        {
            Type = Types.SquadMarker;
        }
        if (decoration.Text != null)
        {
            Text = decoration.Text;
        }
    }
}
