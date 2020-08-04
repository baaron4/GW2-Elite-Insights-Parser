namespace GW2EIEvtcParser.ParsedData
{
    public interface Stateable
    {
        (long start, double value) ToState();

    }
}
