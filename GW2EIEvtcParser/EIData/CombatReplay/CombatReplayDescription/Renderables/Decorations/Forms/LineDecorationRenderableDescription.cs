using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class LineDecorationRenderableDescription : FormDecorationRenderableDescription
    {
        public object ConnectedFrom { get; }

        internal LineDecorationRenderableDescription(ParsedEvtcLog log, LineDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }

}
