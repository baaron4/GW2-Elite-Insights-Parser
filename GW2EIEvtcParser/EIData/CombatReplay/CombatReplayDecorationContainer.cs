using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplayDecorationContainer
    {
        private Dictionary<string, ConstantGenericDecoration> DecorationCache { get; }
        private Dictionary<ConstantGenericDecoration, List<VariableGenericDecoration>> Decorations { get; }

        internal CombatReplayDecorationContainer(Dictionary<string, ConstantGenericDecoration> cache)
        {
            DecorationCache = cache;
            Decorations = new Dictionary<ConstantGenericDecoration, List<VariableGenericDecoration>>();
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
            if (!Decorations.TryGetValue(cached, out List<VariableGenericDecoration> list))
            {
                Decorations[cached] = new List<VariableGenericDecoration> { decoration.VariableDecoration };
            }
            else
            {
                list.Add(decoration.VariableDecoration);
            }
        }

        internal IReadOnlyList<GenericDecoration> ToList()
        {
            var result = new List<GenericDecoration>();
            foreach (KeyValuePair<ConstantGenericDecoration, List<VariableGenericDecoration>> pair in Decorations)
            {
                foreach (VariableGenericDecoration decoration in pair.Value)
                {
                    result.Add(pair.Key.GetDecorationFromVariable(decoration));
                }
            }
            return result;
        }
    }
}

