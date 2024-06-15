using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplayDecorationContainer
    {
        private Dictionary<string, ConstantGenericDecoration> DecorationCache { get; }
        private List<(ConstantGenericDecoration constant, VariableGenericDecoration variable)> Decorations { get; }

        internal CombatReplayDecorationContainer(Dictionary<string, ConstantGenericDecoration> cache)
        {
            DecorationCache = cache;
            Decorations = new List<(ConstantGenericDecoration constant, VariableGenericDecoration variable)>();
        }

        internal void Add(GenericDecoration decoration)
        {
            if (decoration.Lifespan.end <= decoration.Lifespan.start)
            {
                return;
            }
            ConstantGenericDecoration constantPart = decoration.ConstantDecoration;
            var id = constantPart.GetSignature();
            if (!DecorationCache.TryGetValue(id, out ConstantGenericDecoration cached))
            {
                cached = constantPart;
                DecorationCache[id] = constantPart;
            }
            Decorations.Add((cached, decoration.VariableDecoration));
        }

        internal IReadOnlyList<GenericDecoration> ToList()
        {
            var result = new List<GenericDecoration>();
            foreach ((ConstantGenericDecoration constant, VariableGenericDecoration variable) in Decorations)
            {
                result.Add(constant.GetDecorationFromVariable(variable));
            }
            return result;
        }
    }
}

