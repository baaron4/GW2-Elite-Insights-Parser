namespace GW2EIEvtcParser.EIData;

public abstract class MechanicContainer
{
    protected MechanicContainer()
    {
    }

    public abstract IReadOnlyList<Mechanic> GetMechanics();
}
