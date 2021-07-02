using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.Extensions
{
    public abstract class AbstractExtensionMetaDataEvent : AbstractMetaDataEvent
    {
        internal AbstractExtensionMetaDataEvent(CombatItem evtcItem) : base(evtcItem)
        {
        }

        internal AbstractExtensionMetaDataEvent() : base()
        {

        }

    }
}
