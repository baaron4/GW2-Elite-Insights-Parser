using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractDamageEvent : AbstractCombatEvent
    {
        public AgentItem From { get; }
        public AgentItem MasterFrom { get; }
        public AgentItem To { get; }
        public AgentItem MasterTo { get; }

        public long SkillId { get; }
        public ParseEnum.IFF IFF { get; }

        public int Damage { get; protected set; }
        public int ShieldDamage { get; }
        public bool IsNinety { get; }
        public bool IsFifty { get; }
        public bool IsMoving { get; }
        public bool IsFlanking { get; }
        public bool IsHit { get; protected set; }
        public bool IsCrit { get; protected set; }
        public bool IsGlance { get; protected set; }
        public bool IsBlind { get; protected set; }
        public bool IsAbsorbed { get; protected set; }
        public bool HasInterrupted { get; protected set; }
        public bool HasDowned { get; protected set; }
        public bool HasKilled { get; protected set; }
        public bool IsBlocked { get; protected set; }
        public bool IsEvaded { get; protected set; }

        public AbstractDamageEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem.LogTime, offset)
        {
            From = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.LogTime);
            MasterFrom = evtcItem.SrcMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.SrcMasterInstid, evtcItem.LogTime) : null;
            To = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.LogTime);
            MasterTo = evtcItem.DstMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.DstMasterInstid, evtcItem.LogTime) : null;
            SkillId = evtcItem.SkillID;
            ShieldDamage = evtcItem.IsShields > 0 ? evtcItem.OverstackValue > 0 ? (int)evtcItem.OverstackValue : evtcItem.Value : 0;
            IsNinety = evtcItem.IsNinety > 0;
            IsFifty = evtcItem.IsFifty > 0;
            IsMoving = evtcItem.IsMoving > 0;
            IsFlanking = evtcItem.IsFlanking > 0;
            IFF = evtcItem.IFF;
        }

        public abstract bool IsCondi(ParsedLog log);
    }
}
