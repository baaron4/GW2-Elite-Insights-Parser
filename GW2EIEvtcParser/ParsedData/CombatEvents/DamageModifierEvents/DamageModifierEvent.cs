using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    internal class DamageModifierEvent : AbstractTimeCombatEvent
    {
        public readonly DamageModifier DamageModifier;
        private readonly AbstractHealthDamageEvent _evt;
        public AgentItem Src => _evt.From;
        public AgentItem Dst => _evt.To;
        public double DamageGain { get; }

        internal DamageModifierEvent(AbstractHealthDamageEvent evt, DamageModifier damageModifier, double damageGain) : base(evt.Time)
        {
            _evt = evt;
            DamageGain = damageGain;
            DamageModifier = damageModifier;
        }
    }
}
