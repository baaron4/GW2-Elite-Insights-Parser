using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecorationRenderingDescription : GenericAttachedDecorationRenderingDescription
    {
        public bool Fill { get; }
        public int GrowingEnd { get; }
        public bool GrowingReverse { get; }

        internal FormDecorationRenderingDescription(ParsedEvtcLog log, FormDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
        {
            Fill = decoration.Filled;
            GrowingEnd = decoration.GrowingEnd;
            GrowingReverse = decoration.GrowingReverse;
        }

    }

}
