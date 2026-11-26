using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.CustomPolygonDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class CustomPolygonDecorationRenderingDescription : FormDecorationRenderingDescription
{
    public readonly IReadOnlyList<float[]>? Points;

    internal CustomPolygonDecorationRenderingDescription(ParsedEvtcLog log, CustomPolygonDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler)
    {
        Type = Types.CustomPolygon;
        var points = new List<float[]>(decoration.Points.Count);
        foreach (var point in decoration.Points)
        {
            points.Add(new float[] { point.X, -point.Y });
        }
        Points = points;
    }
}
