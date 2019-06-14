using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractCastEvent : AbstractCombatEvent
    {
        // start item
        public SkillItem Skill { get; protected set; }
        public long SkillId => Skill.ID;
        public AgentItem Caster { get; }
        public AgentItem MasterCaster { get; }
        public int ExpectedDuration { get; protected set; }
        public bool UnderQuickness { get; protected set; }

        // end item
        public bool Interrupted { get; protected set; }
        public bool FullAnimation { get; protected set; }
        public bool ReducedAnimation { get; protected set; }
        public int ActualDuration { get; protected set; }

        // Swaps
        public int SwappedTo { get; protected set; }

        public AbstractCastEvent(CombatItem startEvtcItem, AgentData agentData, SkillData skillData, long offset) : base(startEvtcItem.LogTime, offset)
        {
            Skill = skillData.Get(startEvtcItem.SkillID);
            Caster = agentData.GetAgentByInstID(startEvtcItem.SrcInstid, startEvtcItem.LogTime);
            UnderQuickness = startEvtcItem.IsActivation == ParseEnum.Activation.Quickness;
            ExpectedDuration = startEvtcItem.Value;
            MasterCaster = startEvtcItem.SrcMasterInstid > 0 ? agentData.GetAgentByInstID(startEvtcItem.SrcMasterInstid, startEvtcItem.LogTime) : null;
        }

        public AbstractCastEvent(long time, SkillItem skill, AgentItem caster) : base(time, 0)
        {
            Skill = skill;
            Caster = caster;
            MasterCaster = null;
        }
    }
}
