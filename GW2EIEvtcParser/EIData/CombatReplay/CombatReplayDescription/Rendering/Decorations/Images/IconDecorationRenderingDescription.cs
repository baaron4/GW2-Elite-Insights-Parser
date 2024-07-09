using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class IconDecorationRenderingDescription : GenericIconDecorationRenderingDescription
    {
        internal IconDecorationRenderingDescription(ParsedEvtcLog log, IconDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
        {
            Type = "IconDecoration";
            if (decoration.IsSquadMarker)
            {
                Type = "SquadMarkerDecoration";
            }
        }
    }

}
