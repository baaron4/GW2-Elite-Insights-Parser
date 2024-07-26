namespace GW2EIEvtcParser.ParsedData
{
    public class InstanceStartEvent : AbstractMetaDataEvent
    {
        public long TimeOffsetFromInstanceCreation { get; }

        public string InstanceIP { get; } = null;

        internal InstanceStartEvent(CombatItem evtcItem) : base(evtcItem)
        {
            TimeOffsetFromInstanceCreation = (long)evtcItem.SrcAgent;
            if (evtcItem.Value != 0)
            {
                // TODO: verify format
                long item1 = (evtcItem.Value & 0xFF000000) >> 24;
                long item2 = (evtcItem.Value & 0x00FF0000) >> 16;
                long item3 = (evtcItem.Value & 0x0000FF00) >> 8;
                long item4 = (evtcItem.Value & 0x000000FF);
                InstanceIP = $"{item1}.{item2}.{item3}.{item4}";
            }
        }

    }
}
