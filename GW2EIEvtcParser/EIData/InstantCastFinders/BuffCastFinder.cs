using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffCastFinder<Event> : CheckedCastFinder<Event> where Event: AbstractBuffEvent
    {
        protected long BuffID { get; }
        protected BuffCastFinder(long skillID, long buffID) : base(skillID)
        {
            BuffID = buffID;
        }
    }
}
