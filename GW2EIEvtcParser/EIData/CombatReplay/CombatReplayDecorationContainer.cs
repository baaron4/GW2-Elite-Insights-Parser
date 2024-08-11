using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class CombatReplayDecorationContainer
    {
        private Dictionary<string, GenericDecorationMetadata> DecorationCache { get; }
        private List<(GenericDecorationMetadata metadata, GenericDecorationRenderingData renderingData)> Decorations { get; }

        internal CombatReplayDecorationContainer(Dictionary<string, GenericDecorationMetadata> cache)
        {
            DecorationCache = cache;
            Decorations = new List<(GenericDecorationMetadata metadata, GenericDecorationRenderingData renderingData)>();
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

        public List<GenericDecorationRenderingDescription> GetCombatReplayRenderableDescriptions(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var result = new List<GenericDecorationRenderingDescription>();
            foreach ((GenericDecorationMetadata constant, GenericDecorationRenderingData renderingData) in Decorations)
            {
                result.Add(renderingData.GetCombatReplayRenderingDescription(map, log, usedSkills, usedBuffs, constant.GetSignature()));
            }
            return result;
        }
    }
}

