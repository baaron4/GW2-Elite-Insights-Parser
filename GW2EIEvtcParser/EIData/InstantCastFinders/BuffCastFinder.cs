namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffCastFinder : InstantCastFinder
    {
        protected long BuffID { get; }
        protected BuffCastFinder(long skillID, long buffID) : base(skillID)
        {
            BuffID = buffID;
        }
    }
}
