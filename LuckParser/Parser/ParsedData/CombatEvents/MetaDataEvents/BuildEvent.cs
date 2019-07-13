namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class BuildEvent : AbstractMetaDataEvent
    {
        public ulong Build { get; }

        public BuildEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            Build = evtcItem.SrcAgent;
        }

    }
}
