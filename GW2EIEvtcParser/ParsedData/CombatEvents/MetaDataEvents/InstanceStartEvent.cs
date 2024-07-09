namespace GW2EIEvtcParser.ParsedData
{
    public class InstanceStartEvent : AbstractMetaDataEvent
    {
        public long OffsetFromInstanceCreation { get; }

        internal InstanceStartEvent(CombatItem evtcItem) : base(evtcItem)
        {
            OffsetFromInstanceCreation = evtcItem.Time - (long)evtcItem.SrcAgent;
        }

    }
}
