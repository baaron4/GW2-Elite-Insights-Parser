namespace GW2EIEvtcParser.ParsedData
{
    public class BuildEvent : AbstractMetaDataEvent
    {
        public ulong Build { get; }

        public BuildEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Build = evtcItem.SrcAgent;
        }

    }
}
