using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.PieDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class PieDecorationRenderingDescription : CircleDecorationRenderingDescription
    {
        internal PieDecorationRenderingDescription(ParsedEvtcLog log, PieDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
        {
            Type = "Pie";
        }

    }
}
