using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class WeightedMechanicEvent : MechanicEvent
{
    private readonly double _weight;
    internal WeightedMechanicEvent(long time, Mechanic mech, SingleActor actor, double weight) : base(time, mech, actor)
    {
        _weight = weight;
    }

    internal override MechanicEvent CopyForSubMechanic(SubMechanic subMechanic)
    {
        return new WeightedMechanicEvent(Time, subMechanic, Actor, _weight);
    }

    public override double GetWeight()
    {
        return _weight;
    }
}
