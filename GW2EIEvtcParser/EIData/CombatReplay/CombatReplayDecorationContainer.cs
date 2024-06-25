using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplayDecorationContainer
    {
        private Dictionary<string, GenericDecorationMetadata> DecorationCache { get; }
        private List<(GenericDecorationMetadata metadata, GenericDecorationRenderingData renderingData)> Decorations { get; }

        internal CombatReplayDecorationContainer(Dictionary<string, GenericDecorationMetadata> cache)
        {
            DecorationCache = cache;
            Decorations = new List<(GenericDecorationMetadata metadata, GenericDecorationRenderingData renderingData)>();
        }

        internal void Add(GenericDecoration decoration)
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

        internal IReadOnlyList<GenericDecoration> ToList()
        {
            var result = new List<GenericDecoration>();
            foreach ((GenericDecorationMetadata constant, GenericDecorationRenderingData renderingData) in Decorations)
            {
                result.Add(constant.GetDecorationFromVariable(renderingData));
            }
            return result;
        }
    }
}

