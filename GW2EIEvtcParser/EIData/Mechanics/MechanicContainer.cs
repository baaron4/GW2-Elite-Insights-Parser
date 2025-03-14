using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData;

public abstract class MechanicContainer
{
    protected MechanicContainer()
    {
    }

    public abstract IReadOnlyList<Mechanic> GetMechanics();
}
