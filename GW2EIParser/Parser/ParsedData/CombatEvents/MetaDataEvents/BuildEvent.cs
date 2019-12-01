namespace GW2EIParser.Parser.ParsedData.CombatEvents
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
