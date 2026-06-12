using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.CustomPolygonDecoration;

namespace GW2EIEvtcParser.EIData;

public class CustomPolygonDecorationRenderingDescription : FormDecorationRenderingDescription
{
    public readonly IReadOnlyList<float[]>? Points;

    internal CustomPolygonDecorationRenderingDescription(ParsedEvtcLog log, CustomPolygonDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
    {
        Type = Types.CustomPolygon;
        var points = new List<float[]>(decoration.Points.Count);
        foreach(var point in decoration.Points)
        {
            points.Add(new float[] { point.X, -point.Y });
        }
        Points = points;
    }
}
