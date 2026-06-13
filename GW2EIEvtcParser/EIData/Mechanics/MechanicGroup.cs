namespace GW2EIEvtcParser.EIData;

public class MechanicGroup : MechanicContainer
{
    private readonly IReadOnlyList<MechanicContainer> Mechanics;

    internal MechanicGroup(List<MechanicContainer> mechanics)
    {
        Mechanics = mechanics;
    }

    public override IReadOnlyList<Mechanic> GetMechanics()
    {
        var mechanics = new List<Mechanic>();
        foreach (var container in Mechanics)
        {
            mechanics.AddRange(container.GetMechanics());
        }
        return mechanics;
    }
}
