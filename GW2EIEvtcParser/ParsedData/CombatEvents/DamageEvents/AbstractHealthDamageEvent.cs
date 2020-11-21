using System;

namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractHealthDamageEvent : AbstractDamageEvent
    {
        //
        public int HealthDamage { get; protected set; }
        public int ShieldDamage { get; protected set; }
        public bool HasHit { get; protected set; }
        public bool DoubleProcHit { get; protected set; }
        public bool HasCrit { get; protected set; }
        public bool HasGlanced { get; protected set; }
        public bool IsBlind { get; protected set; }
        public bool IsAbsorbed { get; protected set; }
        public bool HasInterrupted { get; protected set; }
        public bool HasDowned { get; protected set; }
        public bool HasKilled { get; protected set; }
        public bool IsBlocked { get; protected set; }
        public bool IsEvaded { get; protected set; }

        internal AbstractHealthDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData) : base(evtcItem, agentData, skillData)
        {
        }

        internal void NegateShieldDamage()
        {
            //_damage = Damage;
            HealthDamage = Math.Max(HealthDamage - ShieldDamage, 0);
        }
    }
}
