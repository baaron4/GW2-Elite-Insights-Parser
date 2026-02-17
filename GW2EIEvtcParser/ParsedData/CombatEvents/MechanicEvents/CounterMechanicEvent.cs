using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class CounterMechanicEvent : MechanicEvent
{

    internal CounterMechanicEvent(long time, Mechanic mech, SingleActor actor) : base(time, mech, actor)
    {
    }

    internal override MechanicEvent CopyForSubMechanic(SubMechanic subMechanic)
    {
        return new CounterMechanicEvent(Time, subMechanic, Actor);
    }

    public override double GetWeight()
    {
        return 1.0;
    }
}
