namespace GW2EIEvtcParser.ParsedData
{
    internal interface IVersionable
    {
        bool Available(ulong gw2Build);
    }
}
