using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericIconDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public string Image { get; }
        public uint PixelSize { get; }
        public uint WorldSize { get; }

        internal GenericIconDecorationCombatReplayDescription(ParsedEvtcLog log, GenericIconDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Image = decoration.Image;
            PixelSize = decoration.PixelSize;
            WorldSize = decoration.WorldSize;
            if (WorldSize == 0 && PixelSize == 0)
            {
                throw new InvalidDataException("Icon Decoration must have at least one size strictly positive");
            }
        }
    }

}
