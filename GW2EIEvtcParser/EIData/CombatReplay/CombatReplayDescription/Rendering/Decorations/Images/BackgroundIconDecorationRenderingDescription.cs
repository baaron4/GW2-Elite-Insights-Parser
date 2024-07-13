using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.BackgroundIconDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class BackgroundIconDecorationRenderingDescription : GenericIconDecorationRenderingDescription
    {

        public IReadOnlyList<float> Opacities { get; private set; }
        public IReadOnlyList<float> Heights { get; private set; }
        internal BackgroundIconDecorationRenderingDescription(ParsedEvtcLog log, BackgroundIconDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
        {
            Type = "BackgroundIconDecoration";
            IsMechanicOrSkill = false;
            var opacities = new List<float>();
            var heights = new List<float>();
            foreach (ParametricPoint1D opacity in decoration.Opacities)
            {
                opacities.Add(opacity.X);
                opacities.Add(opacity.Time);
            }
            foreach (ParametricPoint1D height in decoration.Heights)
            {
                heights.Add(height.X);
                heights.Add(height.Time);
            }
            Opacities = opacities;
            Heights = heights;
        }
    }

}
