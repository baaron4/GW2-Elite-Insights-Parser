namespace GW2EIEvtcParser.ParsedData
{
    public class BuildEvent : AbstractMetaDataEvent
    {
        public ulong Build { get; }

        internal BuildEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Build = GetBuild(evtcItem);
        }

        internal static ulong GetBuild(CombatItem evtcItem)
        {
            return evtcItem.SrcAgent;
        }

    }
}
