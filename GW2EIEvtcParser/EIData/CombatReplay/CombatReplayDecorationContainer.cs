using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData;

internal class CombatReplayDecorationContainer
{
    private readonly Dictionary<string, GenericDecorationMetadata> DecorationCache;
    private readonly List<(GenericDecorationMetadata metadata, GenericDecorationRenderingData renderingData)> Decorations;

    internal CombatReplayDecorationContainer(Dictionary<string, GenericDecorationMetadata> cache, int capacity = 0)
    {
        DecorationCache = cache;
        Decorations = new(capacity);
    }

    public void Add(GenericDecoration decoration)
    {
        if (decoration.Lifespan.end <= decoration.Lifespan.start)
        {
            return;
        }

        GenericDecorationMetadata constantPart = decoration.DecorationMetadata;
        var id = constantPart.GetSignature();
        if (!DecorationCache.TryGetValue(id, out GenericDecorationMetadata cached))
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

    public List<GenericDecorationRenderingDescription> GetCombatReplayRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
    {
        var result = new List<GenericDecorationRenderingDescription>(Decorations.Count);
        foreach (var (constant, renderingData) in Decorations)
        {
            result.Add(renderingData.GetCombatReplayRenderingDescription(map, log, usedSkills, usedBuffs, constant.GetSignature()));
        }
        return result;
    }
}

