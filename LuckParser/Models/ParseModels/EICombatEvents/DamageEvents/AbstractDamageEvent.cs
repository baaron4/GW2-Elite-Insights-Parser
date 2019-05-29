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

        public long SkillID { get; }
        public ParseEnum.IFF IFF { get; }

        public int Damage { get; protected set; }
        public int ShieldDamage { get; }
        public bool IsNinety { get; }
        public bool IsFifty { get; }
        public bool IsMoving { get; }
        public bool IsFlanking { get; }
        public bool IsIndirectDamage { get; protected set; }
        public bool IsCondi { get; protected set; }

        public AbstractDamageEvent(CombatItem evtcItem, AgentData agentData, long offset) : base(evtcItem.Time, offset)
        {
            From = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.Time);
            MasterFrom = evtcItem.SrcMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.SrcMasterInstid, evtcItem.Time) : null;
            To = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.Time);
            MasterTo = evtcItem.DstMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.DstMasterInstid, evtcItem.Time) : null;
            SkillID = evtcItem.SkillID;
            ShieldDamage = evtcItem.IsShields > 0 ? evtcItem.OverstackValue > 0 ? (int)evtcItem.OverstackValue : evtcItem.Value : 0;
            IsNinety = evtcItem.IsNinety > 0;
            IsFifty = evtcItem.IsFifty > 0;
            IsMoving = evtcItem.IsMoving > 0;
            IsFlanking = evtcItem.IsFlanking > 0;
            IFF = evtcItem.IFF;
        }

        public abstract bool IsHit();
        public abstract bool IsCrit();
        public abstract bool IsGlance();
        public abstract bool IsBlind();
        public abstract bool IsAbsorb();
        public abstract bool IsInterrupt();
        public abstract bool IsDowned();
        public abstract bool IsKilled();
        public abstract bool IsBlock();
        public abstract bool IsEvade();
    }
}
