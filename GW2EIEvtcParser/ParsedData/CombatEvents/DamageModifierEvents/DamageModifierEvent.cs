using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

internal class DamageModifierEvent : TimeCombatEvent
{
    public readonly DamageModifier DamageModifier;
    private readonly HealthDamageEvent _evt;
    public AgentItem Src => _evt.From;
    public AgentItem Dst => _evt.To;
    public readonly double DamageGain;

    internal DamageModifierEvent(HealthDamageEvent evt, DamageModifier damageModifier, double damageGain) : base(evt.Time)
    {
        _evt = evt;
        DamageGain = damageGain;
        DamageModifier = damageModifier;
    }
}
