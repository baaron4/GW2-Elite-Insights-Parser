using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Decoration;

namespace GW2EIEvtcParser.EIData;

internal class CombatReplayDecorationContainer
{
    private readonly Dictionary<string, _DecorationMetadata> DecorationCache;
    private readonly List<(_DecorationMetadata metadata, _DecorationRenderingData renderingData)> Decorations;

    internal CombatReplayDecorationContainer(Dictionary<string, _DecorationMetadata> cache, int capacity = 0)
    {
        DecorationCache = cache;
        Decorations = new(capacity);
    }

    public void Add(Decoration decoration)
    {
        if (decoration.Lifespan.end <= decoration.Lifespan.start)
        {
            return;
        }

        _DecorationMetadata constantPart = decoration.DecorationMetadata;
        var id = constantPart.GetSignature();
        if (!DecorationCache.TryGetValue(id, out _DecorationMetadata cached))
        {
            cached = constantPart;
            DecorationCache[id] = constantPart;
        }
        Decorations.Add((cached, decoration.DecorationRenderingData));
    }

    public void ReserveAdditionalCapacity(int additionalCapacity)
    {
        if(Decorations.Capacity >= Decorations.Count + additionalCapacity) { return; }

        Decorations.Capacity = (int)(Decorations.Capacity * 1.4f);
    }

    public List<DecorationRenderingDescription> GetCombatReplayRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var result = new List<DecorationRenderingDescription>(Decorations.Count);
        foreach (var (constant, renderingData) in Decorations)
        {
            result.Add(renderingData.GetCombatReplayRenderingDescription(map, log, usedSkills, usedBuffs, constant.GetSignature()));
        }
        return result;
    }
}

