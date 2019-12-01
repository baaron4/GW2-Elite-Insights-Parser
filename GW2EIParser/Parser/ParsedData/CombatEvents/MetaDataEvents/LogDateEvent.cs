namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class LogDateEvent : AbstractMetaDataEvent
    {
        public uint ServerUnixTimeStamp { get; }
        public uint LocalUnixTimeStamp { get; }

        public LogDateEvent(CombatItem evtcItem) : base(evtcItem)
        {
            ServerUnixTimeStamp = (uint)evtcItem.Value;
            LocalUnixTimeStamp = (uint)evtcItem.BuffDmg;
        }

    }
}
