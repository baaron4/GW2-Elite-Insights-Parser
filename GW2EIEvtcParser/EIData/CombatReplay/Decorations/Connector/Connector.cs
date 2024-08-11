namespace GW2EIEvtcParser.EIData
{
    internal abstract class Connector
    {

        internal enum InterpolationMethod
        {
            Linear = 0
        }
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log);
    }
}
