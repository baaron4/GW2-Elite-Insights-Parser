namespace GW2EIEvtcParser.ParsedData
{
    public class GW2BuildEvent : AbstractMetaDataEvent
    {
        public ulong Build { get; }

        internal GW2BuildEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Build = GetBuild(evtcItem);
        }

        internal static ulong GetBuild(CombatItem evtcItem)
        {
            return evtcItem.SrcAgent;
        }

    }
}
