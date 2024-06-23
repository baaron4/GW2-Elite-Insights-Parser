using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplayDecorationContainer
    {
        private Dictionary<string, GenericDecorationMetadata> DecorationCache { get; }
        private List<(GenericDecorationMetadata constant, VariableGenericDecoration variable)> Decorations { get; }

        internal CombatReplayDecorationContainer(Dictionary<string, GenericDecorationMetadata> cache)
        {
            DecorationCache = cache;
            Decorations = new List<(GenericDecorationMetadata constant, VariableGenericDecoration variable)>();
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
            Decorations.Add((cached, decoration.VariableDecoration));
        }

        internal IReadOnlyList<GenericDecoration> ToList()
        {
            var result = new List<GenericDecoration>();
            foreach ((GenericDecorationMetadata constant, VariableGenericDecoration variable) in Decorations)
            {
                result.Add(constant.GetDecorationFromVariable(variable));
            }
            return result;
        }
    }
}

