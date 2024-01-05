using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class BackgroundIconDecorationCombatReplayDescription : GenericIconDecorationCombatReplayDescription
    {

        public IReadOnlyList<float> Opacities { get; private set; }
        public IReadOnlyList<float> Heights { get; private set; }
        internal BackgroundIconDecorationCombatReplayDescription(ParsedEvtcLog log, BackgroundIconDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
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
