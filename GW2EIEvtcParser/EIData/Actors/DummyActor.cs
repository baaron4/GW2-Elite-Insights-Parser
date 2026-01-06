using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class DummyActor : SingleActor
{
    // Constructors
    internal DummyActor(AgentItem agent) : base(agent)
    {
    }

    public override SingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
    {
        throw new InvalidOperationException();
    }

    public override int GetCurrentBarrier(ParsedEvtcLog log, double currentBarrierPercent, long time)
    {
        throw new InvalidOperationException();
    }

    public override int GetCurrentHealth(ParsedEvtcLog log, double currentHealthPercent)
    {
        throw new InvalidOperationException();
    }

    public override string GetIcon(bool forceLowResolutionIfApplicable = false)
    {
        throw new InvalidOperationException();
    }

    internal override void OverrideName(string name)
    {
        throw new InvalidOperationException();
    }

    internal override void SetManualHealth(int health, IReadOnlyList<(long hpValue, double percent)>? hpDistribution = null)
    {
        throw new InvalidOperationException();
    }
}
