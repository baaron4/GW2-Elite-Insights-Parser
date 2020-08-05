namespace GW2EIEvtcParser.ParsedData
{
    internal interface IStateable
    {
        (long start, double value) ToState();

    }
}
