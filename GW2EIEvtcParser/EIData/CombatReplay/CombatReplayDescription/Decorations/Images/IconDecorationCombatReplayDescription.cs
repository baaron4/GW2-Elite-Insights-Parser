using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class IconDecorationCombatReplayDescription : GenericIconDecorationCombatReplayDescription
    {
        public float Opacity { get; }

        internal IconDecorationCombatReplayDescription(ParsedEvtcLog log, IconDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "IconDecoration";
            if (decoration.IsSquadMarker)
            {
                Type = "SquadMarkerDecoration";
            }
            Opacity = decoration.Opacity;
            if (WorldSize == 0 && PixelSize == 0)
            {
                throw new InvalidDataException("Icon Decoration must have at least one size strictly positive");
            }
        }
    }

}
