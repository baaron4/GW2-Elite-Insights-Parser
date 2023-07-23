namespace GW2EIEvtcParser.ParsedData
{
    public class FractalScaleEvent : AbstractMetaDataEvent
    {
        public byte Scale { get; }

        internal FractalScaleEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Scale = (byte)evtcItem.SrcAgent;
        }

    }
}
