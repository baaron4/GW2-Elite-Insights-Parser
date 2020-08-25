namespace GW2EIEvtcParser.Interfaces
{
    internal interface IStateable
    {
        (long start, double value) ToState();

    }
}
